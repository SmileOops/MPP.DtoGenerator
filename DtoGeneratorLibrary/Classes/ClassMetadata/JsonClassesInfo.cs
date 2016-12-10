using System.Collections.Generic;
using Newtonsoft.Json;

namespace DtoGeneratorLibrary.ClassMetadata
{
    public class JsonClassesInfo
    {
        [JsonProperty(PropertyName = "classDescriptions")]
        public List<JsonClassInfo> ClassesInfo { get; set; }
    }
}