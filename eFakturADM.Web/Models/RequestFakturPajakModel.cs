using System;

namespace eFakturADM.Web.Models
{
    public class RequestFakturPajakResultModel
    {
        public string Message { get; set; }
        public string FillingIndex { get; set; }
        public Exception ExceptionDetails { get; set; }
    }
}