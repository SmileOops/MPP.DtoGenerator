using System.Collections.Generic;
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
                jsonClasses = new JsonClassesInfo {ClassesInfo = new List<JsonClassInfo>()};
                return false;
            }

            var undefinedNamesCounter = 0;
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
                    for (int i = 0; i < jsonClass.Properties.Count; i++)
                    {
                        var property = jsonClass.Properties[i];
                        if (!IsPropertyParsedNormally(property))
                        { 
                            jsonClass.Properties.RemoveAt(i);

                            result = false;
                        }
                    }
                }
                else
                {
                    jsonClass.Properties = new List<JsonClassPropertyInfo>();
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