using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eFakturADM.Web.Models
{
    public class MasaPajakInfoModel
    {
        public long FakturPajakId { get; set; }
        public int MasaPajak { get; set; }
        public int TahunPajak { get; set; }
    }
}