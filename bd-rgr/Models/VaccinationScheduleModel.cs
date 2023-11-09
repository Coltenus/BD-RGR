using System;
using System.Collections.Generic;

namespace bd_rgr
{
    public class VaccinationScheduleModel : BaseModel
    {
        public VaccinationScheduleModel(Connection connection) : base(connection)
        {
            TableName = "vaccination_schedule";
            TableFields = new List<string>()
            {
                "schedule_id", "worker_id", "vaccination_id", "date"
            };
        }
        
        public override void GenerateSeries(uint count)
        {
            int max_id = GetMaxId();

            var command = $"INSERT INTO vaccination_schedule\n" +
                          $"SELECT DISTINCT * FROM (SELECT generate_series AS schedule_id," +
                          $"TRUNC(RANDOM()*(SELECT MAX(worker_id) FROM healthcare_workers))::int + 1 AS worker_id," +
                          $"TRUNC(RANDOM()*(SELECT MAX(vaccination_id) FROM vaccinations))::int + 1 AS vaccination_id," +
                          $"DATE('2023-01-01') + TRUNC(RANDOM()*(DATE('2023-12-31') - DATE('2023-01-01')))::int AS date " +
                          $"FROM generate_series({max_id + 1}, {max_id + count})) AS t1\n" +
                          $"GROUP BY t1.schedule_id, t1.worker_id, t1.vaccination_id, t1.date";
            
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