using System.Collections.Generic;

namespace DtoGeneratorLibrary.AvailableTypes
{
    internal sealed class TypesTable
    {
        private readonly Dictionary<StringDescribedType, string> _availableTypes;

        public TypesTable()
        {
            _availableTypes = new Dictionary<StringDescribedType, string>
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

        public string GetNetType(StringDescribedType stringDescribedType)
        {
            string netType;

            _availableTypes.TryGetValue(stringDescribedType, out netType);

            return netType;
        }
    }
}