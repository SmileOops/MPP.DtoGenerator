using System.IO;
using DtoGeneratorLibrary.ClassMetadata;
using Newtonsoft.Json;

namespace DtoGenerator.Classes
{
    internal static class JsonClassesParser
    {
        public static JsonClassesInfo GetJsonClassesInfo(string path)
        {
            return JsonConvert.DeserializeObject<JsonClassesInfo>(GetJsonStringFromFile(path));
        }

        public static bool IsJsonFileCorrect(string path)
        {
            return File.Exists(path) && Path.GetExtension(path) == ".json";
        }

        private static string GetJsonStringFromFile(string path)
        {
            using (var stream = new StreamReader(path))
            {
                return stream.ReadToEnd();
            }
        }
    }
}