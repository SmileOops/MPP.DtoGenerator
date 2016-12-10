using System.Collections.Generic;
using System.IO;
using DtoGeneratorLibrary.ClassMetadata;

namespace DtoGenerator.Classes
{
    internal static class CsFilesWriter
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
            using (var sw = File.CreateText(Path.ChangeExtension(Path.Combine(fileDirectory, className), "cs")))
            {
                sw.Write(classString);
            }
        }
    }
}