using System.Collections.Generic;
using Newtonsoft.Json;

namespace DtoGeneratorLibrary.ClassMetadata
{
    public sealed class JsonClassInfo
    {
        [JsonProperty(PropertyName = "className")]
        public string ClassName { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public List<JsonClassPropertyInfo> Properties { get; set; }
    }
}