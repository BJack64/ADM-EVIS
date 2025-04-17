using System;
using System.Collections.Generic;

namespace eFakturADM.Logic.Objects
{
    public class AttachmentViewModel
    {
        public Guid ID { get; set; }
        public Guid ParentID { get; set; }
        public string ParentType { get; set; }
        public string Category { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string RelativeUrl { get; set; }
        public string DownloadLink { get; set; }
    }

    public class ListAttachmentViewModel {
        public List<AttachmentViewModel> ListAttachment { get; set; }
    }
}
