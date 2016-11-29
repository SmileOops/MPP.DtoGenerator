using System.Collections.Generic;
using System.IO;
using DtoGeneratorLibrary.Classes.ClassMetadata;

namespace DtoGenerator.Classes
{
    internal static class CsFilesGenerator
    {
        public static void WriteClassStringsToFiles(List<WriteableClass> writeableClasses, string directory)
        {
            foreach (var writeableClass in writeableClasses)
            {
                WriteClassStringToFile(writeableClass.Name, writeableClass.Code, directory);
            }
        }

        private static void WriteClassStringToFile(string className, string classString, string fileDirectory)
        {
            var filePath = $"{fileDirectory}\\{className}.cs";

            using (var sw = File.CreateText(filePath))
            {
                sw.Write(classString);
            }
        }
    }
}