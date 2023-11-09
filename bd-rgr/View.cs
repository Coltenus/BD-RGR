using System;
using System.Collections.Generic;

namespace bd_rgr
{
    public class View
    {
        public static void PrintDict(Dictionary<string, object> dict, List<string> columns)
        {
            foreach (var column in columns)
            {
                Console.Write($"{column}: {dict[column]}, ");
            }
            Console.WriteLine();
        }
        public static void PrintDictList(List<Dictionary<string, object>> list, List<string> columns)
        {
            foreach (var column in columns)
            {
                Console.Write($"{column}\t\t");
            }
            Console.WriteLine();
            foreach (var dict in list)
            {
                foreach (var column in columns)
                {
                        Console.Write($"{dict[column]}\t\t");
                }
                Console.WriteLine();
            }
        }
        
    }
}