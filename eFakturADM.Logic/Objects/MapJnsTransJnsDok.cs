using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class MapJnsTransJnsDok
    {
        public int Id { get; set; }
        public string FCode { get; set; }
        public string JenisDokumen { get; set; }
        public string JenisTransaksi { get; set; }
        public string JenisDokumenDescription { get; set; }
    }
    public partial class MapJnsTransJnsDok : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            Id = DBUtil.GetIntField(dr, "Id");
            FCode = DBUtil.GetCharField(dr, "FCode");
            JenisDokumen = DBUtil.GetCharField(dr, "JenisDokumen");
            JenisTransaksi = DBUtil.GetCharField(dr, "JenisTransaksi");
            JenisDokumenDescription = DBUtil.GetCharField(dr, "JenisDokumenDescription");
            IsValid = true;
            return IsValid;
        }
    }
}
