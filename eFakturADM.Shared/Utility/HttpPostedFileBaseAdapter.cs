using System.Web;

namespace eFakturADM.Shared.Utility
{
    public class HttpPostedFileBaseAdapter : IPostedFile
    {
        public HttpPostedFileBase FileBase { get; private set; }
        public HttpPostedFileBaseAdapter(HttpPostedFileBase fileBase)
        {
            FileBase = fileBase;
        }

        public string FileName { get { return FileBase.FileName; } }
        public string ContentType { get { return FileBase.ContentType; } }
        public int ContentLength { get { return FileBase.ContentLength; } }

        public void SaveAs(string filename)
        {
            FileBase.SaveAs(filename);
        }
    }
}
