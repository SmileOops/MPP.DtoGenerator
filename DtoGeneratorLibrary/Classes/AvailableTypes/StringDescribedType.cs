namespace DtoGeneratorLibrary.AvailableTypes
{
    internal struct StringDescribedType
    {
        public StringDescribedType(string type, string format)
        {
            Type = type;
            Format = format;
        }

        private readonly string Type;
        private readonly string Format;

        //private bool Equals(StringDescribedType other)
        //{
        //    return string.Equals(Type, other.Type) && string.Equals(Format, other.Format);
        //}

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        return ((Type?.GetHashCode() ?? 0)*397) ^ (Format?.GetHashCode() ?? 0);
        //    }
        //}
    }
}