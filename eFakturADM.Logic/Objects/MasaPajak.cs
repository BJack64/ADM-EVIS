using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class MasaPajak
    {
        public int MonthNumber { get; set; }
        public string MonthName { get; set; }
    }
    public partial class MasaPajak : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            MonthNumber = DBUtil.GetIntField(dr, "MonthNumber");
            MonthName = DBUtil.GetCharField(dr, "MonthName");

            IsValid = true;
            return IsValid;
        }
    }
}
