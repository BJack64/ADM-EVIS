using System;
using System.Collections.Generic;

namespace eFakturADM.WebApi.Models
{

    public class DocumentResponseModel
    {
        public List<DocResponseModel> SuccessDocument { get; set; } = new List<DocResponseModel>();
        public List<DocResponseModel> FailedDocument { get; set; } = new List<DocResponseModel>();

    }
    public class DocResponseModel
    {
        public string DocumentNo { get; set; }
        public string Message { get; set; }
    }
}