using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class FPJenisDokumen
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }
    public partial class FPJenisDokumen : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            Id = DBUtil.GetCharField(dr, "Id");
            Description = DBUtil.GetCharField(dr, "Description");

            IsValid = true;
            return IsValid;
        }
    }
}
