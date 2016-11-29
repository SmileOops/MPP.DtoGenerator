using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DtoGenerator.Classes
{
    internal static class CsFilesGenerator
    {
        public static void WriteClassStringsToFiles(Dictionary<string, string> classStrings, string filesDirectory)
        {
            foreach (var classString in classStrings)
            {
                WriteClassStringToFile(classString.Key, classString.Value, filesDirectory);
            }
        }

        private static void WriteClassStringToFile(string className, string classString, string fileDirectory)
        {
            var filePath = $"{fileDirectory}\\{className}.cs";

            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.Write(classString);
            }
        }
    }
}