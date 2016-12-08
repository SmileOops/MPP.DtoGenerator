using System.Collections.Generic;

namespace DtoGeneratorLibrary.AvailableTypes
{
    public sealed class TypesTable
    {
        public TypesTable()
        {
            AvailableTypes = new Dictionary<StringDescribedType, string>
            {
                {new StringDescribedType("integer", "int32"), "int"},
                {new StringDescribedType("integer", "int64"), "long"},
                {new StringDescribedType("number", "float"), "float"},
                {new StringDescribedType("number", "double"), "double"},
                {new StringDescribedType("string", "byte"), "byte"},
                {new StringDescribedType("boolean", ""), "bool"},
                {new StringDescribedType("string", "date"), "DateTime"},
                {new StringDescribedType("string", "string"), "string"}
            };
        }

        public Dictionary<StringDescribedType, string> AvailableTypes { get; }

        public string GetNetType(StringDescribedType stringDescribedType)
        {
            string netType;

            AvailableTypes.TryGetValue(stringDescribedType, out netType);

            return netType;
        }
    }
}