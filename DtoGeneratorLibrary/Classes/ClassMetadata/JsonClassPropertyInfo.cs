using Newtonsoft.Json;

namespace DtoGeneratorLibrary.ClassMetadata
{
    public sealed class JsonClassPropertyInfo
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "format")]
        public string Format { get; set; }
    }
}