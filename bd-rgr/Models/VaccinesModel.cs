using System;
using System.Collections.Generic;

namespace bd_rgr
{
    public class VaccinesModel : BaseModel
    {
        public VaccinesModel(Connection connection) : base(connection)
        {
            TableName = "vaccines";
            TableFields = new List<string>()
            {
                "vaccine_id", "name", "manufacturer", "type", "dosage"
            };
        }
        
        public override void GenerateSeries(uint count, bool debug)
        {
            int max_id = GetMaxId();

            var command = $"INSERT INTO vaccines\n" +
                          $"SELECT DISTINCT * FROM (SELECT generate_series AS vaccine_id, CHR(TRUNC(65+RANDOM()*25)::int)" +
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
                          $"AS manufact, CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"|| CHR(TRUNC(97+RANDOM()*25)::int) || CHR(TRUNC(97+RANDOM()*25)::int)\n" +
                          $"AS type, TRUNC(RANDOM()*99.99 + 0.01)::float4 AS dosage " +
                          $"FROM GENERATE_SERIES({max_id + 1}, {max_id + count})) AS t1\n" +
                          $"GROUP BY t1.vaccine_id, t1.name, t1.manufact, t1.type, t1.dosage";
            
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
            var model = new VaccinationsModel(Connection);
            foreach (var item in list)
            {
                model.Remove(TableFields[0], item[TableFields[0]]);
            }
            base.Remove(column, value);
        }

        public override void Remove<T>(string column, T value, bool greater)
        {
            var list = Find(column, value, greater);
            var model = new VaccinationsModel(Connection);
            foreach (var item in list)
            {
                model.Remove(TableFields[0], item[TableFields[0]]);
            }
            base.Remove(column, value, greater);
        }
    }
}