using Newtonsoft.Json;

namespace DtoGeneratorLibrary.ClassMetadata
{
    public sealed class JsonClassInfo
    {
        [JsonProperty(PropertyName = "className")]
        public string ClassName { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public JsonClassPropertyInfo[] Properties { get; set; }
    }
}