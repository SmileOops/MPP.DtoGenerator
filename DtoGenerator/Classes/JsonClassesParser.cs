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
                jsonClasses = new JsonClassesInfo {ClassesInfo = new JsonClassInfo[0]};
                return false;
            }

            var undefinedNamesCounter = 0;
            var undefinedPropertiesCounter = 0;
            var result = true;

            foreach (var jsonClass in jsonClassesToTry.ClassesInfo)
            {
                if (!IsClassNameParsedNormally(jsonClass))
                {
                    undefinedNamesCounter++;
                    jsonClass.ClassName = $"UndefinedName{undefinedNamesCounter}";

                    result = false;
                }

                if (IsPropertiesArrayParsedNormally(jsonClass))
                {
                    foreach (var property in jsonClass.Properties)
                    {
                        if (!IsPropertyParsedNormally(property))
                        {
                            undefinedPropertiesCounter++;
                            property.Type = "integer";
                            property.Format = "int32";
                            property.Name = $"UndefinedProperty{undefinedPropertiesCounter}";

                            result = false;
                        }
                    }

                    undefinedPropertiesCounter = 0;
                }
                else
                {
                    jsonClass.Properties = new JsonClassPropertyInfo[0];
                }
            }

            jsonClasses = jsonClassesToTry;

            return result;
        }

        private static string GetJsonStringFromFile(string path)
        {
            using (var stream = new StreamReader(path))
            {
                return stream.ReadToEnd();
            }
        }
        
        private static bool IsClassNameParsedNormally(JsonClassInfo classInfo)
        {
            return !string.IsNullOrEmpty(classInfo.ClassName);
        }

        private static bool IsPropertiesArrayParsedNormally(JsonClassInfo classInfo)
        {
            return classInfo.Properties != null;
        }

        private static bool IsPropertyParsedNormally(JsonClassPropertyInfo propertyInfo)
        {
            var typesTable = new TypesTable();

            if (string.IsNullOrEmpty(propertyInfo.Name)) return false;
            if (!typesTable.AvailableTypes.ContainsKey(new StringDescribedType(propertyInfo.Type, propertyInfo.Format)))
                return false;

            return true;
        }
    }
}