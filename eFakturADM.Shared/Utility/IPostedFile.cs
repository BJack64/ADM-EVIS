namespace eFakturADM.Shared.Utility
{
    public interface IPostedFile
    {
        string FileName { get; }
        string ContentType { get; }
        int ContentLength { get; }
        void SaveAs(string filename);
    }
}
