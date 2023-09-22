using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Internship.Models; // Replace `YourAppName` with the actual namespace of your application


public class ExportController : Controller
{
    private readonly IConfiguration _configuration;

    public ExportController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [Route("Export/Modify")]
    public IActionResult Modify()
    {
        var databases = GetDatabaseNames();
        var model = new ExportModel
        {
            Databases = databases
        };

        return View(model);
    }


    public IActionResult Export()
    {
        var databases = GetDatabaseNames();
        var model = new ExportModel
        {
            Databases = databases
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult GetTables(string database)
    {
        var tables = GetTableNames(database);
        return Json(tables);
    }

    [HttpPost]
    public IActionResult ConfirmExport(string database, string table)
    {
        var data = GetTableData(database, table);
        return View("Download", data);
    }

    private List<string> GetDatabaseNames()
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

    private List<string> GetTableNames(string database)
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

    private List<List<string>> GetTableData(string database, string table)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var data = new List<List<string>>();

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand($"USE [{database}]; SELECT * FROM [{table}];", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    // Get column names
                    var columnNames = new List<string>();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames.Add(reader.GetName(i));
                    }
                    data.Add(columnNames);

                    // Get data rows
                    while (reader.Read())
                    {
                        var row = new List<string>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.IsDBNull(i) ? string.Empty : reader.GetValue(i).ToString());
                        }
                        data.Add(row);
                    }
                }
            }
        }

        return data;
    }
}
