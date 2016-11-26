using System;
using DtoGenerator.Classes;
using DtoGeneratorLibrary;

namespace DtoGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                if (JsonClassesParser.IsJsonFileCorrect(args[0]))
                {
                    var jsonClasses = JsonClassesParser.GetJsonClassesInfo(args[0]);
                    var generator = new CsCodeGenerator();
                    Console.WriteLine(generator.GetClassDeclaration(jsonClasses.ClassesInfo[1]));
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