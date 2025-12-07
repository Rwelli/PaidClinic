using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading.Tasks;
using PaidClinic.Models;

namespace PaidClinic.Services;

public class ReportDbService
{
    private readonly DoctorsDbService _doctorsDbService = new();

    public async Task<List<ReportAppeal>> GetReportAppeals(DateTime beginDate, DateTime endDate)
    {
        var doctorsId = new List<int>();
        var reportAppeals = new List<ReportAppeal>();
        using (var conn = DbHelper.CreateConnection())
        {
            await conn.OpenAsync();
            const string query = """
                                 SELECT Код_врача, SUM(Стоимость_лечения) AS Стоимость
                                 FROM Обращения
                                 WHERE Дата_обращения>=@BeginDate AND Дата_обращения<=@EndDate
                                 GROUP BY Код_врача
                                 """;
            await using  var cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("BeginDate", beginDate);
            cmd.Parameters.AddWithValue("EndDate", endDate);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var doctorId = reader.GetInt32(0);
                doctorsId.Add(doctorId);
                var reportAppeal = new ReportAppeal(null, reader["Стоимость"] != DBNull.Value ? Convert.ToInt32(reader["Стоимость"]) : 0);
                reportAppeals.Add(reportAppeal);
            }
        }

        for (int i = 0; i < reportAppeals.Count; i++)
        {
            var doctor = await _doctorsDbService.GetDoctor(doctorsId[i]);
            reportAppeals[i].Doctor = doctor;
        }

        return reportAppeals;
    }
}