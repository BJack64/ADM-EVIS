using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class MapJnsTransaksiKdJnsTransaksi
    {
        public int Id { get; set; }
        public string FCode { get; set; }
        public string KdJenisTransaksi { get; set; }
        public string JenisTransaksi { get; set; }
        public string KdJenisDescription { get; set; }
    }
    public partial class MapJnsTransaksiKdJnsTransaksi : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            Id = DBUtil.GetIntField(dr, "Id");
            FCode = DBUtil.GetCharField(dr, "FCode");
            KdJenisTransaksi = DBUtil.GetCharField(dr, "KdJenisTransaksi");
            JenisTransaksi = DBUtil.GetCharField(dr, "JenisTransaksi");
            KdJenisDescription = DBUtil.GetCharField(dr, "KdJenisDescription");

            IsValid = true;
            return IsValid;
        }
    }
}
