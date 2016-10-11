using Newtonsoft.Json;

namespace DtoGenerator.Classes.ClassMetadata
{
    internal sealed class JsonClassInfo
    {
        [JsonProperty(PropertyName = "className")]
        public string ClassName { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public JsonClassPropertyInfo[] Properties { get; set; }
    }
}