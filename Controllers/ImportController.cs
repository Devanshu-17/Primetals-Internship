using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

public class ImportController : Controller
{
    private readonly IConfiguration _configuration;

    public ImportController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Upload()
    {
        var databases = GetDatabases();
        var model = new ImportModel
        {
            Databases = databases
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult GetTables(string database)
    {
        var tables = GetTablesForDatabase(database);
        return Json(tables);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(string database, string tableName, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["ImportMessage"] = "No file selected";
            return RedirectToAction("Upload");
        }

        var columnMappings = await GetColumnMappings(database, tableName);
        if (columnMappings.Count == 0)
        {
            TempData["ImportMessage"] = "Invalid table selected";
            return RedirectToAction("Upload");
        }

        var dataTable = await GetDataTableFromFile(file, columnMappings.Keys.ToList());
        if (dataTable == null)
        {
            TempData["ImportMessage"] = "Invalid data format";
            return RedirectToAction("Upload");
        }

        // Compare the header of the uploaded file with the database header columns
        var headerFromUploadedFile = GetHeaderFromUploadedFile(file);
        var databaseHeaders = columnMappings.Keys;

        var missingHeaders = CompareHeaders(headerFromUploadedFile, databaseHeaders);

        if (missingHeaders.Any())
        {
            var errorMessage = "File header does not match database column names: " + string.Join(", ", missingHeaders);
            TempData["HeaderMessage"] = errorMessage;
            return RedirectToAction("Upload");
        }



        await SaveDataToDatabase(dataTable, tableName, database);

        TempData["ImportMessage"] = "Data imported successfully";

        return RedirectToAction("Upload");
    }

    private DataTable ConvertJsonToDataTable(string json)
    {
        var dataTable = JsonConvert.DeserializeObject<DataTable>(json);
        return dataTable;
    }

    private List<string> GetDatabases()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var databases = new List<string>();

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT name FROM sys.databases;", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        databases.Add(reader.GetString(0));
                    }
                }
            }
        }

        return databases;
    }

    private List<string> GetTablesForDatabase(string database)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var tables = new List<string>();

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand($"USE [{database}]; SELECT name FROM sys.tables;", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
            }
        }

        return tables;
    }

    private async Task<Dictionary<string, string>> GetColumnMappings(string database, string tableName)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var columnMappings = new Dictionary<string, string>();

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand($"USE [{database}]; SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName;", connection))
            {
                command.Parameters.AddWithValue("@TableName", tableName);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var columnName = reader.GetString(0);
                        var dataType = reader.GetString(1);
                        columnMappings[columnName] = dataType;
                    }
                }
            }
        }

        return columnMappings;
    }

    private async Task<DataTable> GetDataTableFromFile(IFormFile file, List<string> columnNames)
    {
        try
        {
            using (var streamReader = new System.IO.StreamReader(file.OpenReadStream()))
            {
                var dataTable = new DataTable();

                // Set up columns
                foreach (var columnName in columnNames)
                {
                    dataTable.Columns.Add(columnName);
                }

                // Read the header line
                var headerLine = await streamReader.ReadLineAsync();
                var headers = headerLine.Split(',');

                while (!streamReader.EndOfStream)
                {
                    var line = await streamReader.ReadLineAsync();
                    var values = line.Split(',');

                    if (values.Length != columnNames.Count)
                    {
                        return null; // Invalid data format
                    }

                    var dataRow = dataTable.NewRow();

                    for (var i = 0; i < values.Length; i++)
                    {
                        var columnName = columnNames[i];
                        var value = values[i];

                        dataRow[i] = value;
                    }

                    dataTable.Rows.Add(dataRow);
                }

                return dataTable;
            }
        }
        catch
        {
            return null; // Error reading the file
        }
    }



    private List<string> GetHeaderFromUploadedFile(IFormFile file)
    {
        try
        {
            using (var streamReader = new System.IO.StreamReader(file.OpenReadStream()))
            {
                var headerLine = streamReader.ReadLine();
                var headers = headerLine.Split(',');

                return headers.ToList();
            }
        }
        catch
        {
            return new List<string>();
        }
    }

    private List<string> CompareHeaders(List<string> fileHeaders, IEnumerable<string> databaseHeaders)
    {
        var missingHeaders = databaseHeaders.Except(fileHeaders, StringComparer.OrdinalIgnoreCase).ToList();

        return missingHeaders;
    }


    private async Task SaveDataToDatabase(DataTable dataTable, string tableName, string database)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = $"[{database}].[dbo].[{tableName}]";
                bulkCopy.BulkCopyTimeout = 300; // Timeout value in seconds
                await bulkCopy.WriteToServerAsync(dataTable);
            }
        }
    }

}
