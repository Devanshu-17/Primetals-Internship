# Idaten: A Web-based MVC Application for Efficient Data Import and Export in ASP.NET Core and SQL Server

## Introduction

This project introduces a web-based MVC application designed to simplify the process of importing and exporting data between CSV files and SQL Server databases. The application offers a user-friendly interface with three main pages: Home, Import, and Export. On the Import page, users can select the target database and table, upload a CSV file, and view the data in an editable table format. Before saving the edited data to the database, the application performs column mapping to ensure header compatibility. In case of a mismatch, users are promptly notified. The Export page enables users to select a database and table, view the data in a table format, and download it as a CSV file. By automating data management tasks, this MVC application enhances efficiency and accuracy in data handling processes.

## Technical Specifications

**1. Technical Specifications**
- Framework:ASP.NETMVC
- Programming Language: C#
- UserInterface:HTML5,CSS3,JavaScript d. Frontend Framework: Bootstrap
- Client-sideScripting:jQuery
  
**2. Backend:**
- Programming Language: C#
- Framework:ASP.NET
- Database:SQLServer
- ORM (Object-Relational Mapping): Entity Framework
- Server-sideScripting:RazorViewEngine
- API Development: ASP.NET Web API
  
**3. Additional Technical Components:**
- CSVParsing:.NETCSVHelperLibrary
- Data Manipulation: DataTables in C#
- DatabaseConnectivity:ADO.NET
- Database Query Language: Transact-SQL (T-SQL)
- FileUploadHandling:ASP.NETCoreFileUpload
- Integrated Development Environment (IDE): Visual Studio

## System Architecture
  
<img width="393" alt="Architecture" src="https://github.com/Devanshu-17/Primetals-Internship/assets/93381397/367dca91-a1e4-4c6b-8c99-d82115b0bca1">

## References

1. Stack Overflow. (2021). Dynamically map columns from SQL table to use for SqlBulkCopy. [Online].
Available: https://stackoverflow.com/questions/74306259/dynamically-map-columns-from-sql-table-to-use-for-sqlbulkcopy
2. Learn More Seek More. (2022). EF Core 7 JsonColumns - Mapping, Querying & Updating Example. [Online].
Available: https://www.learmoreseekmore.com/2022/12/efcore7-jsoncolumns-mapping-querying-updating-example.html
3. Stack Overflow. (2013). Mapping columns in a DataTable to a SQL table with SqlBulkCopy. [Online].
Available: https://stackoverflow.com/questions/17469349/mapping-columns-in-a-datatable-to-a-sql-table-with-sqlbulkcopy?noredirect=1&lq=1
4. YouTube. (Published on 2017, April 25). How to list all databases in SQL Server using C# .NET [Video].
Available: https://www.youtube.com/watch?v=UuM0mNOzMKM&t=72s
5. YouTube. (Published on 2019, March 21). Get List of all Database From SQL
Server to Combobox Using C#.Net [Video].
Available: https://www.youtube.com/watch?v=Aw66h9kibDI
6. YouTube. (Published on 2016, October 21). How To Insert Data In SQL Using C# Windows Application [Video].
Available: https://www.youtube.com/watch?v=LFJIysSBtS8
7. YouTube. (Published on 2017, February 9). How to Connect SQL Server
Database Using C# [Video].
Available: https://www.youtube.com/watch?v=WJ-CdeTGxp8&t=526s
