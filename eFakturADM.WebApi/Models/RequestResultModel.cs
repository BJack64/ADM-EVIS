using System.Collections.Generic;

namespace eFakturADM.WebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestResultModel
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class RequestResultModelWithDocInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DocumentResponseModel DocumentInfo { get; set; } = new DocumentResponseModel();

    }

    /// <summary>
    /// 
    /// </summary>
    public class InvoiceCreateResponseModel
    {

        /// <summary>
        /// 
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DocumentNo { get; set; }
    }
}