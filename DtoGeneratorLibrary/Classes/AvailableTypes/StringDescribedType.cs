namespace DtoGeneratorLibrary.AvailableTypes
{
    public struct StringDescribedType
    {
        public StringDescribedType(string type, string format)
        {
            _type = type;
            _format = format;
        }

        private readonly string _type;
        private readonly string _format;
    }
}