using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eFakturADM.WebApi.Models
{
    public class FakturPajakUpdateModel
    {
        public string NoFakturPajak { get; set; }
        public long? ObjectId { get; set; }
        public string CertificateID { get; set; }
        public string StatusPayment { get; set; }
        public string Remark { get; set; }
    }
}