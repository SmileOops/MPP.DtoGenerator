using System;
using System.IO;
using DtoGeneratorLibrary.ClassMetadata;
using Newtonsoft.Json;

namespace DtoGenerator.Classes
{
    internal static class JsonClassesParser
    {
        public static bool TryGetJsonClassesInfo(string path, out JsonClassesInfo jsonClasses)
        {
            JsonClassesInfo jsonClassesToTry;
            try
            {
                jsonClassesToTry = JsonConvert.DeserializeObject<JsonClassesInfo>(GetJsonStringFromFile(path));
            }
            catch (JsonReaderException)
            {
                jsonClasses = new JsonClassesInfo();

                return false;
            }

            foreach (var jsonClass in jsonClassesToTry.ClassesInfo)
            {
                jsonClasses = new JsonClassesInfo();
                if (!IsClassParsedNormally(jsonClass)) return false;
            }

            jsonClasses = jsonClassesToTry;

            return true;
        }

        private static bool IsClassParsedNormally(JsonClassInfo classInfo)
        {
            if (string.IsNullOrEmpty(classInfo.ClassName)) return false;

            if (classInfo.Properties == null) return false;

            foreach (var property in classInfo.Properties)
            {
                if (string.IsNullOrEmpty(property.Type)) return false;
                if (property.Format == null) return false;
                if (string.IsNullOrEmpty(property.Name)) return false;
            }

            return true;
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