using System;
using System.Collections.Generic;

namespace bd_rgr
{
    public class HealthcareWorkersModel : BaseModel
    {
        public HealthcareWorkersModel(Connection connection) : base(connection)
        {
            TableName = "healthcare_workers";
            TableFields = new List<string>()
            {
                "worker_id", "name", "specialization", "medical_license_number"
            };
        }
        
        public override void GenerateSeries(uint count, bool debug)
        {
            int max_id = GetMaxId();

            var command = $"INSERT INTO healthcare_workers\n" +
                          $"SELECT DISTINCT * FROM (SELECT generate_series AS worker_id, CHR(TRUNC(65+RANDOM()*25)::int)" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"AS name, CHR(TRUNC(65+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"AS spec, TRUNC(RANDOM()*99999999999999)::int8 AS license " +
                          $"FROM generate_series({max_id + 1}, {max_id + count})) AS t1\n" +
                          $"GROUP BY t1.worker_id, t1.name, t1.spec, t1.license";
            
            if(debug)
                Console.WriteLine(command);
            
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

        public override void Remove<T>(string column, T value)
        {
            var list = Find(column, new List<T>() { value });
            var model = new VaccinationScheduleModel(Connection);
            foreach (var item in list)
            {
                model.Remove(TableFields[0], item[TableFields[0]]);
            }
            base.Remove(column, value);
        }

        public override void Remove<T>(string column, T value, bool greater)
        {
            var list = Find(column, value, greater);
            var model = new VaccinationScheduleModel(Connection);
            foreach (var item in list)
            {
                model.Remove(TableFields[0], item[TableFields[0]]);
            }
            base.Remove(column, value, greater);
        }
    }
}