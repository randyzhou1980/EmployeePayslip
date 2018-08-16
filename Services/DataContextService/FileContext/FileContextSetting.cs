namespace DataContextService.FileContext
{
    public class FileContextSetting
    {
        public DataFileSetting DataFile { get; set; }
        public DataFileSetting OutputFile { get; set; }
    }

    public class DataFileSetting
    {
        public string Directory { get; set; }
        public string Extension { get; set; }
    }
}
