using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


public class ImportModel
{
    [Required(ErrorMessage = "Please select a file.")]
    [Display(Name = "File")]
    public IFormFile File { get; set; }

    [Display(Name = "File Name")]
    public string FileName { get; set; }

    public string FileContent { get; set; }

    [Display(Name = "Database")]
    public string Database { get; set; }

    [Display(Name = "Table")]
    public string TableName { get; set; }

    public List<string> Databases { get; set; }
    public List<string> Tables { get; set; }

    // Additional properties if needed

    public ImportModel()
    {
        Databases = new List<string>();
        Tables = new List<string>();
    }
}
