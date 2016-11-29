using System;
using DtoGenerator.Classes;
using DtoGeneratorLibrary;

namespace DtoGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int tasksNumber;
            string classesNamespace;

            if (!ConfigManager.TryGetTasksNumber(out tasksNumber) ||
                !ConfigManager.TryGetNamespace(out classesNamespace))
            {
                Console.WriteLine("Config file error!");
                Console.WriteLine("Check \"tasksNumber\" and \"namespace\" fields.");

                return;
            }

            if (args.Length == 2)
            {
                if (JsonClassesParser.IsJsonFileCorrect(args[0]))
                {
                    var jsonClasses = JsonClassesParser.GetJsonClassesInfo(args[0]);
                    var generator = new CsCodeGenerator();
                    var classStrings = generator.GetClassStrings(jsonClasses);

                    CsFilesGenerator.WriteClassStringsToFiles(classStrings, args[1]);
                }
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("This program works with two parameters:");
                Console.WriteLine("1) JSON file path");
                Console.WriteLine("2) Output directory path");
                Console.ReadLine();
            }
        }
    }
}