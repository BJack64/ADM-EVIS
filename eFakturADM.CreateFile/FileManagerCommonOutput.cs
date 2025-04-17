namespace eFakturADM.FileManager
{
    public class FileManagerCommonOutput
    {
        public CommonOutputType InfoType { get; set; }
        public string MessageInfo { get; set; }
        public string FilePath { get; set; }
        public string IdNo { get; set; }
    }

    public enum CommonOutputType
    {
        Success = 1,
        Error = 2,
        ErrorWithFileDownload = 3
    }

}
