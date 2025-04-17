using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class TahunPajak
    {
        public int Year { get; set; }
    }
    public partial class TahunPajak : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            Year = DBUtil.GetIntField(dr, "Year");

            IsValid = true;
            return IsValid;
        }
    }
}
