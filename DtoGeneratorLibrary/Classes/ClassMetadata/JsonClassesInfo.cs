using Newtonsoft.Json;

namespace DtoGeneratorLibrary.ClassMetadata
{
    public class JsonClassesInfo
    {
        [JsonProperty(PropertyName = "classDescriptions")]
        public JsonClassInfo[] ClassesInfo { get; set; }
    }
}