using System.Collections.Generic;
using eFakturADM.Logic.Core;

namespace eFakturADM.Web.Models
{
    public class FpDigantiOutstandingModel
    {
        public long ID { get; set; }
        public string FakturPajakDigantiOutstandingIDs { get; set; }
        public string OutstandingfakturPajakIDs { get; internal set; }
    }
}