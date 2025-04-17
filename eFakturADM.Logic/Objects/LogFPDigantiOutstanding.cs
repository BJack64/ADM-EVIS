using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class LogFPDigantiOutstanding
    {
        public long Id { get; set; }
        public string FormatedNoFaktur { get; set; }
        public DateTime ProcessDate { get; set; }
    }

    public partial class LogFPDigantiOutstanding : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            Id = DBUtil.GetIntField(dr, "Id");
            FormatedNoFaktur = DBUtil.GetCharField(dr, "FormatedNoFaktur");
            ProcessDate = DBUtil.GetDateTimeField(dr, "ProcessDate");

            IsValid = true;
            return IsValid;
        }
    }
}
