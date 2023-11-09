using System;
using System.Collections.Generic;

namespace bd_rgr
{
    public class VaccinationsModel : BaseModel
    {
        public VaccinationsModel(Connection connection) : base(connection)
        {
            TableName = "vaccinations";
            TableFields = new List<string>()
            {
                "vaccination_id", "patient_id", "vaccine_id", "given_dose", "notes"
            };
        }
        
        public override void GenerateSeries(uint count)
        {
            int max_id = GetMaxId();

            var command = $"INSERT INTO vaccinations\n" +
                          $"SELECT DISTINCT * FROM (SELECT generate_series AS vaccination_id," +
                          $"TRUNC(RANDOM()*(SELECT MAX(patient_id) FROM patients))::int + 1 AS patient_id," +
                          $"TRUNC(RANDOM()*(SELECT MAX(vaccine_id) FROM vaccines))::int + 1 AS vaccine_id," +
                          $"float4(RANDOM()*0.99 + 0.01) AS dosage " +
                          $"FROM GENERATE_SERIES({max_id + 1}, {max_id + count})) AS t1\n" +
                          $"GROUP BY t1.vaccination_id, t1.patient_id, t1.vaccine_id, t1.dosage";
            
            try
            {
                Connection.Cmd.Connection.Open();
                Connection.Cmd.CommandText = command;
                Connection.Cmd.ExecuteNonQuery();
            }
            catch (Npgsql.PostgresException er)
            {
                Console.WriteLine($"{er.MessageText}");
                Console.WriteLine($"{er.Hint}");
            }
            finally
            {
                Connection.Cmd.Connection.Close();
            }
        }
    }
}