using Newtonsoft.Json;

namespace DtoGenerator.Classes.ClassMetadata
{
    internal class JsonClassesInfo
    {
        [JsonProperty(PropertyName = "classDescriptions")]
        public JsonClassInfo[] ClassesInfo { get; set; }
    }
}