using System.Web;

namespace eFakturADM.Shared.Utility
{
    public class HttpPostedFileAdapter : IPostedFile
    {
        public HttpPostedFile FileBase { get; private set; }
        public HttpPostedFileAdapter(HttpPostedFile fileBase)
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
