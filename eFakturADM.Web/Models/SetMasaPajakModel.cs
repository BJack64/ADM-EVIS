using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eFakturADM.Web.Models
{
    public class SetMasaPajakModel
    {
        public long FakturPajakId { get; set; }
        public int MasaPajak { get; set; }
        public int TahunPajak { get; set; }
    }
}