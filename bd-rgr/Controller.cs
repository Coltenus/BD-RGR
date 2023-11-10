#nullable enable
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace bd_rgr
{
    public class Controller
    {
        private BaseModel? _model;
        private Connection _connection;
        
        public Controller()
        {
            _connection = new Connection("localhost", "lab1", "postgres", "1111");
            _model = null;
        }

        public void Run()
        {
            bool active = true;
            while(active)
            {
                try
                {
                    active = ChooseAction();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private bool ChooseAction()
        {
            bool active = true;
            Console.Write("Choose action: ");
            int action = Convert.ToInt32(Console.ReadLine());
            try
            {
                switch (action)
                {
                    case 0:
                        active = false;
                        break;
                    case 1:
                        ChooseModel();
                        break;
                    case 2:
                    {
                        Console.Write("Need debug(true, false): ");
                        var debug = Convert.ToBoolean(Console.ReadLine());
                        var data = new List<Tuple<string, string, string>>();
                        Regex regexData = new Regex(@"\w+");
                        while (true)
                        {
                            Console.Write("Enter data(table name, needed column, column for join)\n: ");
                            var dt = Console.ReadLine();
                            if (dt == string.Empty) break;
                            var _data = regexData.Matches(dt);
                            if (_data.Count < 3) continue;
                            data.Add(new Tuple<string, string, string>(_data[0].Value, _data[1].Value, _data[2].Value));
                        }

                        var columns = new List<string>();
                        foreach (var item in data)
                        {
                            columns.Add(item.Item2);
                        }

                        Tuple<int, string, List<object>> where;
                        {
                            Console.Write("Enter count of table: ");
                            var table = Convert.ToInt32(Console.ReadLine());
                            Console.Write("Enter column: ");
                            var column = Console.ReadLine();
                            List<object> values = new List<object>();
                            while (true)
                            {
                                Console.Write("Enter value: ");
                                var value = Console.ReadLine();
                                if (value == string.Empty)
                                    break;
                                values.Add(value);
                            }

                            where = new Tuple<int, string, List<object>>(table, column, values);
                        }
                        View.PrintDictList(_model.FindInTables(data, where, debug), columns);
                    }
                        break;
                    case 3:
                    {
                        Console.Write("Enter column: ");
                        var column = Console.ReadLine();
                        Console.Write("Enter value: ");
                        var value = Console.ReadLine();
                        View.PrintDict(_model.Find(column, value), _model.TableFields);
                    }
                        break;
                    case 4:
                    {
                        Console.Write("Enter column: ");
                        var column = Console.ReadLine();
                        List<object> values = new List<object>();
                        while (true)
                        {
                            Console.Write("Enter value: ");
                            var value = Console.ReadLine();
                            if (value == string.Empty)
                                break;
                            values.Add(value);
                        }

                        View.PrintDictList(_model.Find(column, values), _model.TableFields);
                    }
                        break;
                    case 5:
                        View.PrintDictList(_model.Find(), _model.TableFields);
                        break;
                    case 6:
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var field in _model.TableFields)
                        {
                            Console.Write($"Enter {field}: ");
                            var value = Console.ReadLine();
                            dict.Add(field, value);
                        }

                        _model.Add(dict);
                    }
                        break;
                    case 7:
                    {
                        var list = new List<Dictionary<string, object>>();
                        while(true)
                        {
                            bool end = false;
                            var dict = new Dictionary<string, object>();
                            foreach (var field in _model.TableFields)
                            {
                                Console.Write($"Enter {field}: ");
                                var value = Console.ReadLine();
                                if (value == string.Empty)
                                {
                                    end = true;
                                    break;
                                }
                                dict.Add(field, value);
                            }
                            if(end) break;
                            list.Add(dict);
                        }

                        _model.Add(list);
                    }
                        break;
                    case 8:
                    {
                        Console.Write("Enter column: ");
                        var column = Console.ReadLine();
                        Console.Write("Enter value: ");
                        var value = Console.ReadLine();
                        _model.Remove(column, value);
                    }
                        break;
                    case 9:
                    {
                        Console.Write("Enter column: ");
                        var column = Console.ReadLine();
                        Console.Write("Enter value: ");
                        var value = Console.ReadLine();
                        Console.Write("Greater or less(true, false): ");
                        var greater = Convert.ToBoolean(Console.ReadLine());
                        _model.Remove(column, value, greater);
                    }
                        break;
                    case 10:
                    {
                        Console.Write("Enter column to find: ");
                        var columnEdit = Console.ReadLine();
                        Console.Write("Enter its value: ");
                        var valueEdit = Console.ReadLine();
                        var columns = new List<string>();
                        var values = new List<object>();
                        Console.WriteLine("Enter values to edit");
                        {
                            Console.Write("Enter column: ");
                            var column = Console.ReadLine();
                            if(column == string.Empty) break;
                            Console.Write("Enter value: ");
                            var value = Console.ReadLine();
                            columns.Add(column);
                            values.Add(value);
                        }
                        _model.EditOne(columnEdit, valueEdit, columns, values);
                    }
                        break;
                    case 11:
                    {
                        Console.Write("Enter column to find: ");
                        var columnEdit = Console.ReadLine();
                        Console.Write("Enter its value: ");
                        var valueEdit = Console.ReadLine();
                        var columns = new List<string>();
                        var values = new List<object>();
                        Console.WriteLine("Enter values to edit");
                        {
                            Console.Write("Enter column: ");
                            var column = Console.ReadLine();
                            if(column == string.Empty) break;
                            Console.Write("Enter value: ");
                            var value = Console.ReadLine();
                            columns.Add(column);
                            values.Add(value);
                        }
                        _model.EditSome(columnEdit, valueEdit, columns, values);
                    }
                        break;
                    case 12:
                    {
                        Console.Write("Enter count of elements: ");
                        var count = Convert.ToUInt32(Console.ReadLine());
                        Console.Write("Need debug(true, false): ");
                        var debug = Convert.ToBoolean(Console.ReadLine());
                        _model.GenerateSeries(count, debug);
                    }
                        break;
                }
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("You need to choose model to use it.");
            }
            
            System.Threading.Thread.Sleep(100);
            return active;
        }

        private void ChooseModel()
        {
            Console.Write("Choose model: ");
            int action = Convert.ToInt32(Console.ReadLine());
            switch (action)
            {
                case 1:
                    _model = new PatientsModel(_connection);
                    break;
                case 2:
                    _model = new VaccinesModel(_connection);  
                    break;
                case 3:
                    _model = new VaccinationsModel(_connection);
                    break;
                case 4:
                    _model = new HealthcareWorkersModel(_connection);
                    break;
                case 5:
                    _model = new VaccinationScheduleModel(_connection);
                    break;
            }
        }
    }
}