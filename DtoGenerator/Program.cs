using System;
using DtoGenerator.Classes;
using DtoGeneratorLibrary;
using DtoGeneratorLibrary.ClassMetadata;

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

                Console.ReadLine();
                return;
            }

            if (args.Length == 2)
            {
                if (JsonClassesParser.IsJsonFileCorrect(args[0]))
                {
                    JsonClassesInfo jsonClasses;
                    if (!JsonClassesParser.TryGetJsonClassesInfo(args[0], out jsonClasses))
                    {
                        Console.WriteLine("Your json file has incorrect data.");
                        Console.ReadLine();

                        return;
                    }

                    var generator = new CsCodeGenerator(classesNamespace, tasksNumber);
                    var writeableClasses = generator.GetClassStrings(jsonClasses, classesNamespace);

                    CsFilesGenerator.WriteClassStringsToFiles(writeableClasses, args[1]);
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