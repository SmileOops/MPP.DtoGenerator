using System.IO;
using DtoGeneratorLibrary.AvailableTypes;
using DtoGeneratorLibrary.ClassMetadata;
using Newtonsoft.Json;

namespace DtoGenerator.Classes
{
    internal static class JsonClassesParser
    {
        public static bool IsJsonFileExists(string path)
        {
            return File.Exists(path) && Path.GetExtension(path) == ".json";
        }

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

        private static string GetJsonStringFromFile(string path)
        {
            using (var stream = new StreamReader(path))
            {
                return stream.ReadToEnd();
            }
        }

        private static bool IsClassParsedNormally(JsonClassInfo classInfo)
        {
            var typesTable = new TypesTable();

            if (string.IsNullOrEmpty(classInfo.ClassName)) return false;

            if (classInfo.Properties == null) return false;

            foreach (var property in classInfo.Properties)
            {
                if (string.IsNullOrEmpty(property.Name)) return false;
                if (!typesTable.AvailableTypes.ContainsKey(new StringDescribedType(property.Type, property.Format)))
                    return false;
            }

            return true;
        }
    }
}