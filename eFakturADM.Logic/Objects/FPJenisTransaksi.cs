using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class FPJenisTransaksi
    {
        public string Id { get; set; }
        public string FCode { get; set; }
        public string Description { get; set; }
    }
    public partial class FPJenisTransaksi : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            Id = DBUtil.GetCharField(dr, "Id");
            FCode = DBUtil.GetCharField(dr, "FCode");
            Description = DBUtil.GetCharField(dr, "Description");

            IsValid = true;
            return IsValid;
        }
    }
}
