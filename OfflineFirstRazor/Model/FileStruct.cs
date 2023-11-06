namespace Model
{
    public struct FileStruct
    {
        public string Directory { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }


        public FileStruct(string directory, string name, string extension)
        {
            Directory = directory;
            Name = name;
            Extension = extension;
        }
    }
}
