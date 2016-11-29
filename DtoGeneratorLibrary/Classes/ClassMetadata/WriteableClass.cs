namespace DtoGeneratorLibrary.Classes.ClassMetadata
{
    public sealed class WriteableClass
    {
        public WriteableClass(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public string Name { get; }
        public string Code { get; }
    }
}