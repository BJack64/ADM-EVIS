using System;
using System.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Objects
{
    public partial class FakturPajakFromCoretax
    {
        public Guid Id { get; set; }
        public string NPWP_Penjual { get; set; }
        public string Nama_Penjual { get; set; }
        public string NPWP_Pembeli { get; set; }
        public string Nama_Pembeli { get; set; }
        public string Nomor_Faktur_Pajak { get; set; }
        public DateTime Tanggal_Faktur_Pajak { get; set; }
        public int Masa_Pajak { get; set; }
        public int Tahun { get; set; }
        public int Masa_Pajak_Pengkreditan { get; set; }
        public int Tahun_Pengkreditan { get; set; }
        public decimal Harga_Jual { get; set; }
        public decimal DPP_Nilai_Lain { get; set; }
        public decimal PPN { get; set; }
        public decimal PPnBM { get; set; }
        public string Status_Faktur { get; set; }
        public string Perekam { get; set; }
        public string Nomor_SP2D { get; set; }
        public bool Valid { get; set; }
        public bool Dikreditkan { get; set; }
        public bool Dilaporkan { get; set; }
        public bool Dilaporkan_oleh_Penjual { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    public partial class FakturPajakFromCoretax : ApplicationObject, IApplicationObject
    {
        public bool Load(IDataReader dr)
        {
            IsValid = false;
            Id = DBUtil.GetGuidField(dr, "Id") != Guid.Empty ? DBUtil.GetGuidField(dr, "Id") : Guid.Empty;
            NPWP_Penjual = DBUtil.GetCharField(dr, "NPWP_Penjual");
            Nama_Penjual = DBUtil.GetCharField(dr, "Nama_Penjual");
            NPWP_Pembeli = DBUtil.GetCharField(dr, "NPWP_Pembeli");
            Nama_Pembeli = DBUtil.GetCharField(dr, "Nama_Pembeli");
            Dikreditkan = DBUtil.GetBoolField(dr, "Dikreditkan");
            Nomor_Faktur_Pajak = DBUtil.GetCharField(dr, "Nomor_Faktur_Pajak");
            Tanggal_Faktur_Pajak = DBUtil.GetDateTimeField(dr, "Tanggal_Faktur_Pajak");
            Masa_Pajak = DBUtil.GetIntField(dr, "Masa_Pajak");
            Tahun = DBUtil.GetIntField(dr, "Tahun");
            Masa_Pajak_Pengkreditan = DBUtil.GetIntField(dr, "Masa_Pajak_Pengkreditan");
            Tahun_Pengkreditan = DBUtil.GetIntField(dr, "Tahun_Pengkreditan");
            Harga_Jual = DBUtil.GetDecimalField(dr, "Harga_Jual");
            DPP_Nilai_Lain = DBUtil.GetDecimalField(dr, "DPP_Nilai_Lain");
            PPN = DBUtil.GetDecimalField(dr, "PPN");
            PPnBM = DBUtil.GetDecimalField(dr, "PPnBM");
            Status_Faktur = DBUtil.GetCharField(dr, "Status_Faktur");
            Perekam = DBUtil.GetCharField(dr, "Perekam");
            Nomor_SP2D = DBUtil.GetCharField(dr, "Nomor_SP2D");
            Valid = DBUtil.GetBoolField(dr, "Valid");
            Dilaporkan = DBUtil.GetBoolField(dr, "Dilaporkan");
            Dilaporkan_oleh_Penjual = DBUtil.GetBoolField(dr, "Dilaporkan_oleh_Penjual");
            CreatedOn = DBUtil.GetDateTimeField(dr, "CreatedOn");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            UpdatedOn = DBUtil.GetDateTimeField(dr, "UpdatedOn");
            UpdatedBy = DBUtil.GetCharField(dr, "UpdatedBy");
            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");

            IsValid = true;
            return IsValid;
        }
    }

}