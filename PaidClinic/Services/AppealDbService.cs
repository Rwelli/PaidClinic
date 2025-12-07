using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading.Tasks;
using PaidClinic.Models;

namespace PaidClinic.Services;

public class AppealDbService
{
    private DoctorsDbService _doctorsDbService = new();
    private PatientDbService _patientDbService = new();
    
    public async Task<List<Appeal>> GetAppeals()
    {
        List<Appeal> appeals = new List<Appeal>();
        var doctorAndPatientId = new Dictionary<int, (int, int)>();
        
        await using (var conn = DbHelper.CreateConnection())
        {
            await conn.OpenAsync();
            const string query = "SELECT * FROM [Обращения]";
            await using var command = new OleDbCommand(query, conn);
            await using var reader = (OleDbDataReader) await command.ExecuteReaderAsync();
        
            while (await reader.ReadAsync())
            {
                var appealId = reader["Код_обращения"] != DBNull.Value ? Convert.ToInt32(reader["Код_обращения"]) : 0;
                var appeal = new Appeal(
                    appealId,
                    null,
                    null,
                    DbHelper.SafeGetDateTime(reader, "Дата_обращения"),
                    reader["Диагноз"]?.ToString() ?? string.Empty,
                    reader["Стоимость_лечения"] != DBNull.Value ? Convert.ToUInt32(reader["Стоимость_лечения"]) : 0
                );
                var doctorId = reader["Код_врача"] != DBNull.Value ? Convert.ToInt32(reader["Код_врача"]) : 0;
                var patientId = reader["Код_пациента"] != DBNull.Value ? Convert.ToInt32(reader["Код_пациента"]) : 0;
                doctorAndPatientId.Add(appealId, (doctorId, patientId));
                appeals.Add(appeal);
            }
        }

        foreach (var appeal in appeals)
        {
            var (doctorId, patientId) = doctorAndPatientId[appeal.Id];
            var doctor = await _doctorsDbService.GetDoctor(doctorId);
            var patient = await _patientDbService.GetPatient(patientId);
            appeal.Patient = patient;
            appeal.Doctor = doctor;
        }

        return appeals;
    }

    public async Task AddAppealAsync(Appeal appeal)
    {
        await using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
    
        const string query = @"
        INSERT INTO [Обращения] 
        ([Код_врача], [Код_пациента], [Дата_обращения], [Диагноз], [Стоимость_лечения]) 
        VALUES (@IdDoctor, @IdPatient, @DateAppeal, @Diagnosis, @CostOfTreatment)";
    
        await using var command = new OleDbCommand(query, conn);
    
        command.Parameters.AddWithValue("@IdDoctor", appeal.Doctor?.Id ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@IdPatient", appeal.Patient?.Id ?? (object)DBNull.Value);
        if (appeal.Date.HasValue)
        {
            DateTime dateOnly = appeal.Date.Value.Date;
            command.Parameters.AddWithValue("@DateAppeal", dateOnly);
        }
        else command.Parameters.AddWithValue("@DateAppeal", DBNull.Value);

        command.Parameters.AddWithValue("@Diagnosis", appeal.Diagnosis ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@CostOfTreatment", appeal.CostOfTreatment);
        
        _ = await command.ExecuteNonQueryAsync();
    }
    
    public async Task DeleteAppeal(Appeal appeal)
    {
        using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        const string query =
            """
            DELETE FROM [Обращения]
            WHERE [Код_обращения]=@IdAppeal
            """;
        await using var command = new OleDbCommand(query, conn);
        command.Parameters.AddWithValue("@IdAppeal", appeal.Id);
        _ = await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAppealAsync(Appeal appeal)
    {
        using var conn = DbHelper.CreateConnection();
        await conn.OpenAsync();
        const string query =
            """
            UPDATE [Обращения]
            SET [Код_врача]=@IdDoctor, [Код_пациента]=@IdPatient, [Дата_обращения]=@DateAppeal, [Диагноз]=@Diagnosis, [Стоимость_лечения]=@CostOfTreatment
            WHERE [Код_обращения]=@IdAppeal
            """;
        
        await using var command = new OleDbCommand(query, conn);
    
        command.Parameters.AddWithValue("@IdDoctor", appeal.Doctor!.Id);
        command.Parameters.AddWithValue("@IdPatient", appeal.Patient!.Id);
        
        if (appeal.Date.HasValue)
        {
            DateTime dateOnly = appeal.Date.Value.Date;
            command.Parameters.AddWithValue("@DateAppeal", dateOnly);
        }
        else command.Parameters.AddWithValue("@DateAppeal", DBNull.Value);
        
        command.Parameters.AddWithValue("@Diagnosis", appeal.Diagnosis);
        command.Parameters.AddWithValue("@CostOfTreatment", appeal.CostOfTreatment);
        command.Parameters.AddWithValue("@IdAppeal", appeal.Id);
    
        _ = await command.ExecuteNonQueryAsync();
    }
}