using System;
using System.Data.OleDb;
using System.IO;

namespace PaidClinic.Services;

public static class DbHelper
{
    private static string GetDatabasePath()
    {
        var basePath = AppContext.BaseDirectory;
        return Path.Combine(basePath, "Data", "Clinic.mdb");
    }

    public static OleDbConnection CreateConnection()
    {
        string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={GetDatabasePath()};" +
                                  "Persist Security Info=False;Jet OLEDB:Engine Type=5;";
#if DEBUG
        connectionString = """Provider=Microsoft.ACE.OLEDB.12.0;Data Source="C:\Users\habib\RiderProjects\PaidClinic\PaidClinic\Data\Clinic.mdb";""";
#endif
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
        
            // Access может возвращать разные типы для даты
            if (value is DateTime dateTime)
                return dateTime;
            
            if (value is string dateString && DateTime.TryParse(dateString, out DateTime parsedDate))
                return parsedDate;
            
            // Если это double (OADate)
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