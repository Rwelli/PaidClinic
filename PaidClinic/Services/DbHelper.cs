using System;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace PaidClinic.Services;

public static class DbHelper
{
    private static string GetExeLocation()
    {
        using var process = Process.GetCurrentProcess();
        return process.MainModule?.FileName ?? 
               Assembly.GetEntryAssembly()?.Location ??
               Environment.ProcessPath ??
               AppContext.BaseDirectory;
    }
    
    private static string GetDatabasePath()
    {
        var basePath = Path.GetDirectoryName(GetExeLocation())!;
        return Path.Combine(basePath, "Data", "Clinic.mdb");
    }

    public static OleDbConnection CreateConnection()
    {
        File.Create("log.txt").Close();
        TextWriter textWriter = new StreamWriter("log.txt");
        textWriter.WriteLine(GetDatabasePath());
        textWriter.Close();
        Console.WriteLine(GetDatabasePath());
        string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={GetDatabasePath()};" +
                                  "Persist Security Info=False;Jet OLEDB:Engine Type=5;";

        return new OleDbConnection(connectionString);
    }
    
    public static DateTime SafeGetDateTime(OleDbDataReader reader, string columnName)
    {
        try
        {
            int ordinal = reader.GetOrdinal(columnName);
        
            if (reader.IsDBNull(ordinal))
                return DateTime.MinValue;
            
            object value = reader[ordinal];
        
            if (value is DateTime dateTime)
                return dateTime;
            
            if (value is string dateString && DateTime.TryParse(dateString, out DateTime parsedDate))
                return parsedDate;
            
            if (value is double oaDate)
                return DateTime.FromOADate(oaDate);
            
            return Convert.ToDateTime(value);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка чтения даты из колонки '{columnName}': {ex.Message}");
            return DateTime.MinValue;
        }
    }
}