using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using System;
using System.Data;

namespace eFakturADM.Logic.Objects
{
    public partial class FakturPajakPenampungDetail
    {
        public long FakturPajakPenampungDetailId { get; set; }
        public long FakturPajakPenampungId { get; set; }
        public string Nama { get; set; }
        public decimal HargaSatuan { get; set; }
        public decimal JumlahBarang { get; set; }
        public decimal HargaTotal { get; set; }
        public decimal Diskon { get; set; }
        public decimal Dpp { get; set; }
        public decimal Ppn { get; set; }
        public decimal TarifPpnbm { get; set; }
        public decimal Ppnbm { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        public int TotalItems { get; set; }

    }

    public partial class FakturPajakPenampungDetail : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;

            FakturPajakPenampungDetailId = DBUtil.GetLongField(dr, "FakturPajakPenampungDetailId");
            FakturPajakPenampungId = DBUtil.GetLongField(dr, "FakturPajakPenampungId");
            Nama = DBUtil.GetCharField(dr, "Nama");
            HargaSatuan = DBUtil.GetDecimalField(dr, "HargaSatuan");
            JumlahBarang = DBUtil.GetDecimalField(dr, "JumlahBarang");
            HargaTotal = DBUtil.GetDecimalField(dr, "HargaTotal");
            Diskon = DBUtil.GetDecimalField(dr, "Diskon");
            Dpp = DBUtil.GetDecimalField(dr, "Dpp");
            Ppn = DBUtil.GetDecimalField(dr, "Ppn");
            TarifPpnbm = DBUtil.GetDecimalField(dr, "TarifPpnbm");
            Ppnbm = DBUtil.GetDecimalField(dr, "Ppnbm");

            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");
            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");

            TotalItems = DBUtil.GetIntField(dr, "TotalItems");

            IsValid = true;
            return IsValid;
        }
    }
}
