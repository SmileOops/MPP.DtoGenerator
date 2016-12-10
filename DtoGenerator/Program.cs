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
                if (JsonClassesParser.IsJsonFileExists(args[0]))
                {
                    JsonClassesInfo jsonClasses;
                    if (!JsonClassesParser.TryGetJsonClassesInfo(args[0], out jsonClasses))
                    {
                        if (jsonClasses.ClassesInfo.Length == 0)
                        {
                            Console.WriteLine("Your json file has incorrect format.");
                            Console.WriteLine("I can't parse anything from it((((");
                            Console.ReadLine();

                            return;
                        }

                        Console.WriteLine("Some classes in your json file has incorrect data.");
                        Console.WriteLine(
                            "Please, check those classes which names or properties are named as undefined in generated files.");
                    }

                    var generator = new MultithreadCsCodeGenerator(classesNamespace, tasksNumber);
                    var writeableClasses = generator.GetWriteableClasses(jsonClasses, classesNamespace);

                    CsFilesWriter.WriteClassStringsToFiles(writeableClasses, args[1]);

                    Console.WriteLine("Done!");
                    Console.WriteLine($"{writeableClasses.Count} classes have been generated!");
                }
                else
                {
                    Console.WriteLine("Json file doesn't exist.");
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