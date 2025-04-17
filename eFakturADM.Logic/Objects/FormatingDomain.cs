using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class FormatingDomain
    {
        public string OriginalField { get; set; }
        public string FormattedField { get; set; }
    }
    public partial class FormatingDomain : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            OriginalField = DBUtil.GetCharField(dr, "OriginalField");
            FormattedField = DBUtil.GetCharField(dr, "FormattedField");

            IsValid = true;
            return IsValid;
        }
    }
}
