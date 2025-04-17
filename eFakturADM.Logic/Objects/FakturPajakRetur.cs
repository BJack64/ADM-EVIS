using eFakturADM.Logic.Core;
using eFakturADM.Logic.Utilities;
using System;
using System.Data;

namespace eFakturADM.Logic.Objects
{
    public partial class FakturPajakRetur
    {
        public int TotalItems { get; set; }
        public string TglFakturString { get; set; }
        public string DPPString { get; set; }
        public string PPNString { get; set; }
        public string PPNBMString { get; set; }
        public string TglReturString { get; set; }

    }
    public partial class FakturPajakRetur
    {
        public long FakturPajakReturId { get; set; }
        public string FCode { get; set; }
        public long? FakturPajakId { get; set; }
        public string NoDocRetur { get; set; }
        public DateTime? TglRetur { get; set; }
        public int MasaPajakLapor { get; set; }
        public int TahunPajakLapor { get; set; }
        public decimal? JumlahDPP { get; set; }
        public decimal? JumlahPPN { get; set; }
        public decimal? JumlahPPNBM { get; set; }
        public string Pesan { get; set; }
        
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        //informasi no faktur yang diretur
        public string NoFakturPajak { get; set; }
        public string NPWPPenjual { get; set; }
        public string NamaPenjual { get; set; }
        public string AlamatPenjual { get; set; }

        public string FormatedNoFakturPajak { get; set; }
        public string FormatedNpwpPenjual { get; set; }
        public string KdJenisTransaksi { get; set; }
        public string FgPengganti { get; set; }
        
        public DateTime? TglFaktur { get; set; }
        public bool? Dikreditkan { get; set; }
        
    }

    /// <summary>
    /// Provides a wrapper on single item in the Location database table. The properties of this class mapped on appropriate database fields and methods provide saving and loading into/from database.
    /// An instance of this class can be created by new word or loaded from a database using Location class which returns collection of Location by different condition.  
    /// </summary>     
    public partial class FakturPajakRetur : ApplicationObject, IApplicationObject
    {

        /// <summary>
        /// Loads result set field values and saves into properties of class.
        /// </summary>
        /// <param name="dr">DataReader object which represents current row in the resultset.</param>
        /// <returns>Returns true if it was successfully loaded.</returns>
        public virtual bool Load(IDataReader dr)
        {
            IsValid = false;

            FakturPajakReturId = DBUtil.GetLongField(dr, "FakturPajakReturId");
            FCode = DBUtil.GetCharField(dr, "FCode");
            FakturPajakId = DBUtil.GetLongNullField(dr, "FakturPajakId");
            NoDocRetur = DBUtil.GetCharField(dr, "NoDocRetur");
            TglRetur = DBUtil.GetDateTimeField(dr, "TglRetur");
            MasaPajakLapor = DBUtil.GetIntField(dr, "MasaPajakLapor");
            TahunPajakLapor = DBUtil.GetIntField(dr, "TahunPajakLapor");
            JumlahDPP = DBUtil.GetDecimalNullField(dr, "JumlahDPP");
            JumlahPPN = DBUtil.GetDecimalNullField(dr, "JumlahPPN");
            JumlahPPNBM = DBUtil.GetDecimalNullField(dr, "JumlahPPNBM");
            Pesan = DBUtil.GetCharField(dr, "Pesan");
            IsDeleted = DBUtil.GetBoolField(dr, "IsDeleted");
            Created = DBUtil.GetDateTimeField(dr, "Created");
            Modified = DBUtil.GetDateTimeNullField(dr, "Modified");
            CreatedBy = DBUtil.GetCharField(dr, "CreatedBy");
            ModifiedBy = DBUtil.GetCharField(dr, "ModifiedBy");

            TotalItems = DBUtil.GetIntField(dr, "TotalItems");

            NoFakturPajak = DBUtil.GetCharField(dr, "NoFakturPajak");
            TglFaktur = DBUtil.GetDateTimeNullField(dr, "TglFaktur");
            NPWPPenjual = DBUtil.GetCharField(dr, "NPWPPenjual");
            NamaPenjual = DBUtil.GetCharField(dr, "NamaPenjual");
            AlamatPenjual = DBUtil.GetCharField(dr, "AlamatPenjual");
            FormatedNoFakturPajak = DBUtil.GetCharField(dr, "FormatedNoFakturPajak");
            FormatedNpwpPenjual = DBUtil.GetCharField(dr, "FormatedNpwpPenjual");
            KdJenisTransaksi = DBUtil.GetCharField(dr, "KdJenisTransaksi");
            FgPengganti = DBUtil.GetCharField(dr, "FgPengganti");
            Dikreditkan = DBUtil.GetBoolNullField(dr, "Dikreditkan");

            TglFakturString = ConvertHelper.DateTimeConverter.ToShortDateString(TglFaktur);
            TglReturString = ConvertHelper.DateTimeConverter.ToShortDateString(TglRetur);

            DPPString = ConvertHelper.DecimalConverter.ToString(JumlahDPP, 2, true);
            PPNString = ConvertHelper.DecimalConverter.ToString(JumlahPPN, 2, true);
            PPNBMString = ConvertHelper.DecimalConverter.ToString(JumlahPPNBM, 2, true);

            IsValid = true;
            return IsValid;
        }

    }
}
