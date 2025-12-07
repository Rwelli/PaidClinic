using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading.Tasks;
using PaidClinic.Models;
using Path = System.IO.Path;

namespace PaidClinic.Services;

public class DoctorsDbService
{

    public async Task<List<Doctor>> GetDoctors()
    {
        List<Doctor> doctors = new List<Doctor>();
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        const string query = "SELECT * FROM [Врачи]";
        await using var command = new  OleDbCommand(query, conn);
        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            var doctor = new Doctor(
                reader["Код_Врача"] != DBNull.Value ? Convert.ToInt32(reader["Код_Врача"]) : 0,
                reader["Имя"]?.ToString() ?? string.Empty,
                reader["Фамилия"]?.ToString() ?? string.Empty,
                reader["Отчество"]?.ToString() ?? string.Empty,
                reader["Специальность"]?.ToString() ?? string.Empty,
                reader["Категория"]?.ToString() ?? string.Empty
                );
            doctors.Add(doctor);
        }

        return doctors;
    }

    public async Task<Doctor> GetDoctor(int id)
    {
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        const string query = "SELECT * FROM [Врачи] WHERE Код_врача=@Id";
        await using var command = new  OleDbCommand(query, conn);
        command.Parameters.AddWithValue("@Id", id);
        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        var doctor = new Doctor(
            reader["Код_врача"] != DBNull.Value ? Convert.ToInt32(reader["Код_врача"]) : 0,
            reader["Имя"]?.ToString() ?? string.Empty,
            reader["Фамилия"]?.ToString() ?? string.Empty,
            reader["Отчество"]?.ToString() ?? string.Empty,
            reader["Специальность"]?.ToString() ?? string.Empty,
            reader["Категория"]?.ToString() ?? string.Empty
            );
        return doctor;
    }

    public async Task AddDoctorAsync(Doctor doctor)
    {
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
    
        const string query = """
                             INSERT INTO [Врачи] ([Фамилия], [Имя], [Отчество], [Специальность], [Категория]) 
                             VALUES (@LastName, @FirstName, @MiddleName, @Specialty, @Category)
                             """;
    
        await using var command = new OleDbCommand(query, conn);
    
        command.Parameters.AddWithValue("@LastName", doctor.LastName);
        command.Parameters.AddWithValue("@FirstName", doctor.FirstName);
        command.Parameters.AddWithValue("@MiddleName", doctor.MiddleName);
        command.Parameters.AddWithValue("@Specialty", doctor.Speciality);
        command.Parameters.AddWithValue("@Category", doctor.Category);
    
        _ = await command.ExecuteNonQueryAsync();
    }
    
    public async Task DeleteDoctor(Doctor doctor)
    {
        using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        const string query =
            """
            DELETE FROM [Врачи]
            WHERE [Код_врача]=@IdDoctor
            """;
        await using var command = new OleDbCommand(query, conn);
        command.Parameters.AddWithValue("@IdDoctor", doctor.Id);
        _ = await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateDoctorAsync(Doctor doctor)
    {
        using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        const string query =
            """
            UPDATE [Врачи]
            SET [Фамилия]=@LastName, [Имя]=@FirstName, [Отчество]=@MiddleName, [Специальность]=@Speciality, [Категория]=@Category
            WHERE [Код_врача]=@IdDoctor
            """;
        
        await using var command = new OleDbCommand(query, conn);
    
        command.Parameters.AddWithValue("@LastName", doctor.LastName);
        command.Parameters.AddWithValue("@FirstName", doctor.FirstName);
        command.Parameters.AddWithValue("@MiddleName", doctor.MiddleName);
        command.Parameters.AddWithValue("@Specialty", doctor.Speciality);
        command.Parameters.AddWithValue("@Category", doctor.Category);
        command.Parameters.AddWithValue("@IdDoctor", doctor.Id);
    
        _ = await command.ExecuteNonQueryAsync();
    }
}