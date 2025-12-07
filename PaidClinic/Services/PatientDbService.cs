using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading.Tasks;
using PaidClinic.Models;

namespace PaidClinic.Services;

public class PatientDbService
{
    public async Task<List<Patient>> GetPatientsAsync()
    {
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        const string query = "SELECT * FROM [Пациенты]";
        await using var command = new OleDbCommand(query, conn);
        await using var reader = await command.ExecuteReaderAsync();
        
        var patients = new List<Patient>();
        while (await reader.ReadAsync())
        {
            var patient = new Patient(
                reader["Код_пациента"] != DBNull.Value ? Convert.ToInt32(reader["Код_пациента"]) : 0,
                reader["Фамилия"]?.ToString() ?? string.Empty,
                reader["Имя"]?.ToString() ?? string.Empty,
                reader["Отчество"]?.ToString() ?? string.Empty,
                reader["Год_рождения"] != DBNull.Value ? Convert.ToInt32(reader["Год_рождения"]) : 0
            );
            patients.Add(patient);
        }
        return patients;
    }
    
    public async Task<Patient> GetPatient(int id)
    {
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        const string query = "SELECT * FROM [Пациенты] WHERE Код_пациента=@Id";
        await using var command = new  OleDbCommand(query, conn);
        command.Parameters.AddWithValue("@Id", id);
        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        var patient = new Patient(
            reader["Код_пациента"] != DBNull.Value ? Convert.ToInt32(reader["Код_пациента"]) : 0,
            reader["Фамилия"]?.ToString() ?? string.Empty,
            reader["Имя"]?.ToString() ?? string.Empty,
            reader["Отчество"]?.ToString() ?? string.Empty,
            reader["Год_рождения"] != DBNull.Value ? Convert.ToInt32(reader["Год_рождения"]) : 0
        );
        return patient;
    }

    public async Task AddPatientAsync(Patient patient)
    {
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        const string query = """
                             INSERT INTO [Пациенты](Фамилия, Имя, Отчество, Год_рождения)
                             VALUES(@MiddleName, @FirstName, @LastName, @BirthDate);
                             """;
        await using var command = new OleDbCommand(query, conn);
        command.Parameters.AddWithValue("@MiddleName", patient.MiddleName);
        command.Parameters.AddWithValue("@LastName", patient.LastName);
        command.Parameters.AddWithValue("@FirstName", patient.FirstName);
        command.Parameters.AddWithValue("@BirthDate", patient.BirthDate);
        _ = await command.ExecuteNonQueryAsync();
    }

    public async Task UpdatePatientAsync(Patient patient)
    {
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        const string query = """
                             UPDATE [Пациенты]
                             SET Фамилия =@MiddleName, Имя = @FirstName, Отчество = @LastName, Год_рождения = @BirthDate
                             WHERE Код_пациента=@Id;
                             """;
        await using var command = new OleDbCommand(query, conn);
        command.Parameters.AddWithValue("@MiddleName", patient.MiddleName);
        command.Parameters.AddWithValue("@LastName", patient.LastName);
        command.Parameters.AddWithValue("@FirstName", patient.FirstName);
        command.Parameters.AddWithValue("@BirthDate", patient.BirthDate);
        command.Parameters.AddWithValue("@Id", patient.Id);
        _ = await command.ExecuteNonQueryAsync();
    }

    public async Task DeletePatientAsync(Patient patient)
    {
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        const string query = """
                             DELETE FROM [Пациенты]
                             WHERE Код_пациента=@Id;
                             """;
        await using var command = new OleDbCommand(query, conn);
        command.Parameters.AddWithValue("@Id", patient.Id);
        _ = await command.ExecuteNonQueryAsync();
    }
}