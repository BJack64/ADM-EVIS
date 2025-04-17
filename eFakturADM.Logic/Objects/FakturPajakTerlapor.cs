using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class FakturPajakTerlapor
    {
        public int TotalItems { get; set; }
    }
    public partial class FakturPajakTerlapor
    {
        public long FakturPajakTerlaporID { get; set; }
        public string Nama { get; set; }
        public DateTime TanggalLapor { get; set; }
        public int MasaPajak { get; set; }
        public int TahunPajak { get; set; }
        public string AttachmentPath { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public string FileType { get; set; }
        public decimal TotalFakturPajak { get; set; }
        public decimal TotalPPN { get; set; }
        public string sTanggalLapor { get; set; }
        public string sTotalFakturPajak { get; set; }
        public string sTotalPPN { get; set; }
        public string MasaPajakName { get; set; }
    }
    public partial class FakturPajakTerlapor : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            FakturPajakTerlaporID = DBUtil.GetLongField(dr, "FakturPajakTerlaporID");
            Nama = DBUtil.GetCharField(dr, "Nama");
            TanggalLapor = DBUtil.GetDateTimeField(dr, "TanggalLapor");
            MasaPajak = DBUtil.GetIntField(dr, "MasaPajak");
            TahunPajak = DBUtil.GetIntField(dr, "TahunPajak");
            Created = DBUtil.GetDateTimeField(dr, "Created");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            AttachmentPath = DBUtil.GetCharField(dr, "AttachmentPath");
            TotalFakturPajak = DBUtil.GetDecimalField(dr, "TotalFakturPajak");
            TotalPPN = DBUtil.GetDecimalField(dr, "TotalPPN");

            sTotalFakturPajak = ConvertHelper.DecimalConverter.ToString(TotalFakturPajak, 2);
            sTotalPPN = ConvertHelper.DecimalConverter.ToString(TotalPPN, 2);
            MasaPajakName = DBUtil.GetCharField(dr, "MasaPajakName");
            IsValid = true;
            return IsValid;
        }
    }
}
