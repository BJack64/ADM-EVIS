using System;
using System.Data;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System.Collections.Generic;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class FakturPajakReturs : ApplicationCollection<FakturPajakRetur, SpBase>
    {

        public static List<FakturPajakRetur> Get()
        {
            var sp = new SpBase(@"SELECT fpr.[FakturPajakReturId]
      ,fpr.[FCode]
      ,fpr.[FakturPajakId]
      ,fpr.[NoDocRetur]
      ,fpr.[TglRetur]
      ,fpr.[MasaPajakLapor]
      ,fpr.[TahunPajakLapor]
      ,fpr.[JumlahDPP]
      ,fpr.[JumlahPPN]
      ,fpr.[JumlahPPNBM]
      ,fpr.[Pesan]
      ,fpr.[IsDeleted]
      ,fpr.[Created]
      ,fpr.[Modified]
      ,fpr.[CreatedBy]
      ,fpr.[ModifiedBy]
	  ,fpr.NPWPPenjual
	  ,fpr.NamaPenjual
	  ,fpr.AlamatPenjual
	  ,fpr.FormatedNoFakturPajak
	  ,fpr.FormatedNpwpPenjual
	  ,fpr.TglFaktur
      ,fpr.NoFakturPajak
	  ,fpr.KdJenisTransaksi
	  ,fpr.FgPengganti
	  ,fpr.Dikreditkan
	  ,COUNT(FakturPajakReturId) OVER() AS TotalItems
  FROM [dbo].[FakturPajakRetur] fpr 
WHERE fpr.[IsDeleted] = 0");
            return GetApplicationCollection(sp);
        }

        public static FakturPajakRetur GetById(long FakturPajakReturId)
        {
            var sp = new SpBase(@"SELECT fpr.[FakturPajakReturId]
      ,fpr.[FCode]
      ,fpr.[FakturPajakId]
      ,fpr.[NoDocRetur]
      ,fpr.[TglRetur]
      ,fpr.[MasaPajakLapor]
      ,fpr.[TahunPajakLapor]
      ,fpr.[JumlahDPP]
      ,fpr.[JumlahPPN]
      ,fpr.[JumlahPPNBM]
      ,fpr.[Pesan]
      ,fpr.[IsDeleted]
      ,fpr.[Created]
      ,fpr.[Modified]
      ,fpr.[CreatedBy]
      ,fpr.[ModifiedBy]
	  ,fpr.NPWPPenjual
	  ,fpr.NamaPenjual
	  ,fpr.AlamatPenjual
	  ,fpr.FormatedNoFakturPajak
	  ,fpr.FormatedNpwpPenjual
	  ,fpr.TglFaktur
      ,fpr.NoFakturPajak
	  ,fpr.KdJenisTransaksi
	  ,fpr.FgPengganti
	  ,fpr.Dikreditkan
	  ,COUNT(FakturPajakReturId) OVER() AS TotalItems
  FROM [dbo].[FakturPajakRetur] fpr 
WHERE fpr.FakturPajakReturId = @FakturPajakReturId");
            sp.AddParameter("FakturPajakReturId", FakturPajakReturId);
            return GetApplicationObject(sp);
        }        

        public static List<FakturPajakRetur> GetList(Filter filter, out int totalItems, string noFaktur, string noDocRetur, 
            DateTime? tglFakturReturStart, DateTime? tglFakturReturEnd,
            string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak,
            string fTglRetur, string fNpwpVendor, string fNamaVendor, string fNoFakturDiRetur, string fTglFaktur,
            string fNomorRetur, string fMasaRetur, string fTahunRetur, string fDpp, string fPpn, string fPpnBm, string fUserName)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;
            var sp = new SpBase(string.Format(@"SELECT fpr.[FakturPajakReturId]
                                                      ,fpr.[FCode]
                                                      ,fpr.[FakturPajakId]
                                                      ,fpr.[NoDocRetur]
                                                      ,fpr.[TglRetur]
                                                      ,fpr.[MasaPajakLapor]
                                                      ,fpr.[TahunPajakLapor]
                                                      ,fpr.[JumlahDPP]
                                                      ,fpr.[JumlahPPN]
                                                      ,fpr.[JumlahPPNBM]
                                                      ,fpr.[Pesan]
                                                      ,fpr.[IsDeleted]
                                                      ,fpr.[Created]
                                                      ,fpr.[Modified]
                                                      ,fpr.[CreatedBy]
                                                      ,fpr.[ModifiedBy]
	                                                  ,fpr.NPWPPenjual
	                                                  ,fpr.NamaPenjual
	                                                  ,fpr.AlamatPenjual
	                                                  ,fpr.FormatedNoFakturPajak
	                                                  ,fpr.FormatedNpwpPenjual
	                                                  ,fpr.TglFaktur
                                                      ,fpr.NoFakturPajak
	                                                  ,fpr.KdJenisTransaksi
	                                                  ,fpr.FgPengganti
	                                                  ,fpr.Dikreditkan
	                                                  ,COUNT(FakturPajakReturId) OVER() AS TotalItems
                                                 FROM [dbo].[FakturPajakRetur] fpr 
                                                WHERE fpr.[IsDeleted] = 0
                                                        AND (@NoDocRetur IS NULL OR (@NoDocRetur IS NOT NULL AND LOWER(fpr.[NoDocRetur]) LIKE REPLACE(LOWER(@NoDocRetur), '*','%')))
                                                        AND (@NoFakturPajak IS NULL OR (@NoFakturPajak IS NOT NULL AND LOWER(REPLACE(REPLACE(fpr.[FormatedNoFakturPajak],'.',''),'-','')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NoFakturPajak, '.',''),'-','')),'*','%')))
                                                        AND (@NpwpVendor IS NULL OR
	                                                        (@NpwpVendor IS NOT NULL AND LOWER(REPLACE(REPLACE([FormatedNpwpPenjual], '.', ''), '-', '')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NpwpVendor, '.',''),'-','')),'*','%'))
	                                                        )
                                                        AND (
	                                                        (@NamaVendor IS NOT NULL AND LOWER([NamaPenjual]) LIKE REPLACE(LOWER(@NamaVendor),',*', '%'))
	                                                        OR @NamaVendor IS NULL
	                                                        )
                                                        AND (CAST(fpr.[TglRetur] AS DATE) BETWEEN CAST(isnull(@TglFakturReturStart, fpr.[TglRetur]) AS DATE) AND CAST(isnull(@TglFakturReturEnd, fpr.[TglRetur]) AS DATE))
                                                        AND (@MasaPajak IS NULL OR (@MasaPajak IS NOT NULL AND fpr.MasaPajakLapor = @MasaPajak))
                                                        AND (@TahunPajak IS NULL OR (@TahunPajak IS NOT NULL AND fpr.TahunPajakLapor = @TahunPajak))
                                                        AND (@fTglRetur IS NULL OR (@fTglRetur IS NOT NULL AND CONVERT(VARCHAR,fpr.TglRetur, 103) LIKE REPLACE(@fTglRetur, '*', '%')))
                                                        AND (@fNpwpVendor IS NULL OR (@fNpwpVendor IS NOT NULL AND fpr.FormatedNpwpPenjual LIKE REPLACE(@fNpwpVendor, '*', '%')))
                                                        AND (@fNamaVendor IS NULL OR (@fNamaVendor IS NOT NULL AND fpr.NamaPenjual LIKE REPLACE(@fNamaVendor, '*', '%')))
                                                        AND (@fNoFakturDiRetur IS NULL OR (@fNoFakturDiRetur IS NOT NULL AND fpr.FormatedNoFakturPajak LIKE REPLACE(@fNoFakturDiRetur, '*', '%')))
                                                        AND (@fTglFaktur IS NULL OR (@fTglFaktur IS NOT NULL AND CONVERT(varchar, fpr.TglFaktur, 103) LIKE REPLACE(@fTglFaktur, '*', '')))
                                                        AND (@fNomorRetur IS NULL OR (@fNomorRetur IS NOT NULL AND fpr.NoDocRetur LIKE REPLACE(@fNomorRetur, '*', '%')))
                                                        AND (@fMasaRetur IS NULL OR (@fMasaRetur IS NOT NULL AND CAST(fpr.MasaPajakLapor as varchar(100)) LIKE REPLACE(@fMasaRetur, '*', '%')))
                                                        AND (@fTahunRetur IS NULL OR (@fTahunRetur IS NOT NULL AND CAST(fpr.TahunPajakLapor as varchar(100)) LIKE REPLACE(@fTahunRetur, '*', '%')))
                                                        AND (@fDpp IS NULL OR (@fDpp IS NOT NULL AND CAST(fpr.JumlahDPP as varchar(100)) LIKE REPLACE(@fDpp, '*', '%')))
                                                        AND (@fPpn IS NULL OR (@fPpn IS NOT NULL AND CAST(fpr.JumlahPPN as varchar(100)) LIKE REPLACE(@fPpn, '*', '%')))
                                                        AND (@fPpnBm IS NULL OR (@fPpnBm IS NOT NULL AND CAST(fpr.JumlahPPNBM as varchar(100)) LIKE REPLACE(@fPpnBm, '*', '%')))
                                                        AND (@fUserName IS NULL OR (@fUserName IS NOT NULL AND fpr.CreatedBy LIKE REPLACE(@fUserName, '*', '%')))
                                                        ORDER BY {0} {1}
                                                        OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
                                                        FETCH NEXT @ItemPerPage ROWS ONLY", filter.SortColumnName, sortOrder));
            
            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("NoDocRetur", string.IsNullOrEmpty(noDocRetur) ? SqlString.Null : noDocRetur);
            sp.AddParameter("NoFakturPajak", string.IsNullOrEmpty(noFaktur) ? SqlString.Null : noFaktur);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("TglFakturReturStart", tglFakturReturStart.HasValue ? tglFakturReturStart.Value : SqlDateTime.Null);
            sp.AddParameter("tglFakturReturEnd", tglFakturReturEnd.HasValue ? tglFakturReturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("MasaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("fTglRetur", string.IsNullOrEmpty(fTglRetur) ? SqlString.Null : fTglRetur);
            sp.AddParameter("fNpwpVendor", string.IsNullOrEmpty(fNpwpVendor) ? SqlString.Null : fNpwpVendor);
            sp.AddParameter("fNamaVendor", string.IsNullOrEmpty(fNamaVendor) ? SqlString.Null : fNamaVendor);
            sp.AddParameter("fNoFakturDiRetur", string.IsNullOrEmpty(fNoFakturDiRetur) ? SqlString.Null : fNoFakturDiRetur);
            sp.AddParameter("fTglFaktur", string.IsNullOrEmpty(fTglFaktur) ? SqlString.Null : fTglFaktur);
            sp.AddParameter("fNomorRetur", string.IsNullOrEmpty(fNomorRetur) ? SqlString.Null : fNomorRetur);
            sp.AddParameter("fMasaRetur", string.IsNullOrEmpty(fMasaRetur) ? SqlString.Null : fMasaRetur);
            sp.AddParameter("fTahunRetur", string.IsNullOrEmpty(fTahunRetur) ? SqlString.Null : fTahunRetur);
            sp.AddParameter("fDpp", string.IsNullOrEmpty(fDpp) ? SqlString.Null : fDpp);
            sp.AddParameter("fPpn", string.IsNullOrEmpty(fPpn) ? SqlString.Null : fPpn);
            sp.AddParameter("fPpnBm", string.IsNullOrEmpty(fPpnBm) ? SqlString.Null : fPpnBm);
            sp.AddParameter("fUserName", string.IsNullOrEmpty(fUserName) ? SqlString.Null : fUserName);
            
            List<FakturPajakRetur> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems, noFaktur, noDocRetur, tglFakturReturStart, 
                    tglFakturReturEnd, npwpVendor, namaVendor, masaPajak, tahunPajak, fTglRetur, fNpwpVendor, fNamaVendor, fNoFakturDiRetur
                    ,fTglFaktur,fNomorRetur,fMasaRetur, fTahunRetur, fDpp, fPpn, fPpnBm, fUserName);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, noFaktur, noDocRetur, tglFakturReturStart,
                    tglFakturReturEnd, npwpVendor, namaVendor, masaPajak, tahunPajak, fTglRetur, fNpwpVendor, fNamaVendor, fNoFakturDiRetur
                    , fTglFaktur, fNomorRetur, fMasaRetur, fTahunRetur, fDpp, fPpn, fPpnBm, fUserName);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, noFaktur, noDocRetur, tglFakturReturStart,
                    tglFakturReturEnd, npwpVendor, namaVendor, masaPajak, tahunPajak, fTglRetur, fNpwpVendor, fNamaVendor, fNoFakturDiRetur
                    , fTglFaktur, fNomorRetur, fMasaRetur, fTahunRetur, fDpp, fPpn, fPpnBm, fUserName);
            }

            return data;

        }

        public static List<FakturPajakRetur> GetListToDownloadExcel(string noFaktur, string noDocRetur,
            DateTime? tglFakturReturStart, DateTime? tglFakturReturEnd,
            string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak, string fTglRetur, string fNpwpVendor, string fNamaVendor, string fNoFakturDiRetur, string fTglFaktur,
            string fNomorRetur, string fMasaRetur, string fTahunRetur, string fDpp, string fPpn, string fPpnBm, string fUserName)
        {
            var sp = new SpBase(@"SELECT fpr.[FakturPajakReturId]
      ,fpr.[FCode]
      ,fpr.[FakturPajakId]
      ,fpr.[NoDocRetur]
      ,fpr.[TglRetur]
      ,fpr.[MasaPajakLapor]
      ,fpr.[TahunPajakLapor]
      ,fpr.[JumlahDPP]
      ,fpr.[JumlahPPN]
      ,fpr.[JumlahPPNBM]
      ,fpr.[Pesan]
      ,fpr.[IsDeleted]
      ,fpr.[Created]
      ,fpr.[Modified]
      ,fpr.[CreatedBy]
      ,fpr.[ModifiedBy]
	  ,fpr.NPWPPenjual
	  ,fpr.NamaPenjual
	  ,fpr.AlamatPenjual
	  ,fpr.FormatedNoFakturPajak
	  ,fpr.FormatedNpwpPenjual
	  ,fpr.TglFaktur
      ,fpr.NoFakturPajak
	  ,fpr.KdJenisTransaksi
	  ,fpr.FgPengganti
	  ,fpr.Dikreditkan
	  ,COUNT(FakturPajakReturId) OVER() AS TotalItems
  FROM [dbo].[FakturPajakRetur] fpr 
WHERE fpr.[IsDeleted] = 0
AND (@NoDocRetur IS NULL OR (@NoDocRetur IS NOT NULL AND LOWER(fpr.[NoDocRetur]) LIKE REPLACE(LOWER(@NoDocRetur), '*','%')))
AND (@NoFakturPajak IS NULL OR (@NoFakturPajak IS NOT NULL AND LOWER(REPLACE(REPLACE(fpr.[FormatedNoFakturPajak],'.',''),'-','')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NoFakturPajak, '.',''),'-','')),'*','%')))
AND (@NpwpVendor IS NULL OR
	(@NpwpVendor IS NOT NULL AND LOWER(REPLACE(REPLACE([FormatedNpwpPenjual], '.', ''), '-', '')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NpwpVendor, '.',''),'-','')),'*','%'))
	)
AND (
	(@NamaVendor IS NOT NULL AND LOWER([NamaPenjual]) LIKE REPLACE(LOWER(@NamaVendor),',*', '%'))
	OR @NamaVendor IS NULL
	)
AND (CAST(fpr.[TglRetur] AS DATE) BETWEEN CAST(isnull(@TglFakturReturStart, fpr.[TglRetur]) AS DATE) AND CAST(isnull(@TglFakturReturEnd, fpr.[TglRetur]) AS DATE))
AND (@MasaPajak IS NULL OR (@MasaPajak IS NOT NULL AND fpr.MasaPajakLapor = @MasaPajak))
AND (@TahunPajak IS NULL OR (@TahunPajak IS NOT NULL AND fpr.TahunPajakLapor = @TahunPajak))
AND (@fTglRetur IS NULL OR (@fTglRetur IS NOT NULL AND CONVERT(VARCHAR,fpr.TglRetur, 103) LIKE REPLACE(@fTglRetur, '*', '%')))
AND (@fNpwpVendor IS NULL OR (@fNpwpVendor IS NOT NULL AND fpr.FormatedNpwpPenjual LIKE REPLACE(@fNpwpVendor, '*', '%')))
AND (@fNamaVendor IS NULL OR (@fNamaVendor IS NOT NULL AND fpr.NamaPenjual LIKE REPLACE(@fNamaVendor, '*', '%')))
AND (@fNoFakturDiRetur IS NULL OR (@fNoFakturDiRetur IS NOT NULL AND fpr.FormatedNoFakturPajak LIKE REPLACE(@fNoFakturDiRetur, '*', '%')))
AND (@fTglFaktur IS NULL OR (@fTglFaktur IS NOT NULL AND CONVERT(varchar, fpr.TglFaktur, 103) LIKE REPLACE(@fTglFaktur, '*', '')))
AND (@fNomorRetur IS NULL OR (@fNomorRetur IS NOT NULL AND fpr.NoDocRetur LIKE REPLACE(@fNomorRetur, '*', '%')))
AND (@fMasaRetur IS NULL OR (@fMasaRetur IS NOT NULL AND CAST(fpr.MasaPajakLapor as varchar(100)) LIKE REPLACE(@fMasaRetur, '*', '%')))
AND (@fTahunRetur IS NULL OR (@fTahunRetur IS NOT NULL AND CAST(fpr.TahunPajakLapor as varchar(100)) LIKE REPLACE(@fTahunRetur, '*', '%')))
AND (@fDpp IS NULL OR (@fDpp IS NOT NULL AND CAST(fpr.JumlahDPP as varchar(100)) LIKE REPLACE(@fDpp, '*', '%')))
AND (@fPpn IS NULL OR (@fPpn IS NOT NULL AND CAST(fpr.JumlahPPN as varchar(100)) LIKE REPLACE(@fPpn, '*', '%')))
AND (@fPpnBm IS NULL OR (@fPpnBm IS NOT NULL AND CAST(fpr.JumlahPPNBM as varchar(100)) LIKE REPLACE(@fPpnBm, '*', '%')))
AND (@fUserName IS NULL OR (@fUserName IS NOT NULL AND fpr.CreatedBy LIKE REPLACE(@fUserName, '*', '%')))
ORDER BY fpr.[TglRetur] ASC");

            sp.AddParameter("NoDocRetur", string.IsNullOrEmpty(noDocRetur) ? SqlString.Null : noDocRetur);
            sp.AddParameter("NoFakturPajak", string.IsNullOrEmpty(noFaktur) ? SqlString.Null : noFaktur);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("MasaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("TglFakturReturStart", tglFakturReturStart.HasValue ? tglFakturReturStart.Value : SqlDateTime.Null);
            sp.AddParameter("tglFakturReturEnd", tglFakturReturEnd.HasValue ? tglFakturReturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("fTglRetur", string.IsNullOrEmpty(fTglRetur) ? SqlString.Null : fTglRetur);
            sp.AddParameter("fNpwpVendor", string.IsNullOrEmpty(fNpwpVendor) ? SqlString.Null : fNpwpVendor);
            sp.AddParameter("fNamaVendor", string.IsNullOrEmpty(fNamaVendor) ? SqlString.Null : fNamaVendor);
            sp.AddParameter("fNoFakturDiRetur", string.IsNullOrEmpty(fNoFakturDiRetur) ? SqlString.Null : fNoFakturDiRetur);
            sp.AddParameter("fTglFaktur", string.IsNullOrEmpty(fTglFaktur) ? SqlString.Null : fTglFaktur);
            sp.AddParameter("fNomorRetur", string.IsNullOrEmpty(fNomorRetur) ? SqlString.Null : fNomorRetur);
            sp.AddParameter("fMasaRetur", string.IsNullOrEmpty(fMasaRetur) ? SqlString.Null : fMasaRetur);
            sp.AddParameter("fTahunRetur", string.IsNullOrEmpty(fTahunRetur) ? SqlString.Null : fTahunRetur);
            sp.AddParameter("fDpp", string.IsNullOrEmpty(fDpp) ? SqlString.Null : fDpp);
            sp.AddParameter("fPpn", string.IsNullOrEmpty(fPpn) ? SqlString.Null : fPpn);
            sp.AddParameter("fPpnBm", string.IsNullOrEmpty(fPpnBm) ? SqlString.Null : fPpnBm);
            sp.AddParameter("fUserName", string.IsNullOrEmpty(fUserName) ? SqlString.Null : fUserName);

            var data = GetApplicationCollection(sp);
            return data;

        } 

        public static FakturPajakRetur Save(FakturPajakRetur data)
        {
            data.WasSaved = false;
            SpBase sp;

            if (data.FakturPajakReturId > 0)
            {
                sp = new SpBase(@"UPDATE [dbo].[FakturPajakRetur]
                                   SET 
                                      [FCode] = @FCode
                                      ,[FakturPajakId] = @FakturPajakId
                                      ,[NPWPPenjual] = @NPWPPenjual
                                      ,[NamaPenjual] = @NamaPenjual
                                      ,[AlamatPenjual] = @AlamatPenjual
                                      ,[FormatedNoFakturPajak] = @FormatedNoFakturPajak
                                      ,[FormatedNpwpPenjual] = @FormatedNpwpPenjual
                                      ,[KdJenisTransaksi] = @KdJenisTransaksi
                                      ,[FgPengganti] = @FgPengganti
                                      ,[NoFakturPajak] = @NoFakturPajak
                                      ,[TglFaktur] = @TglFaktur
                                      ,[Dikreditkan] = @Dikreditkan
                                      ,[TglRetur] = @TglRetur
                                      ,[MasaPajakLapor] = @MasaPajakLapor
                                      ,[TahunPajakLapor] = @TahunPajakLapor
                                      ,[JumlahDPP] = @JumlahDPP
                                      ,[JumlahPPN] = @JumlahPPN
                                      ,[JumlahPPNBM] = @JumlahPPNBM
                                      ,[Pesan] = @Pesan
                                      ,[Modified] = GETDATE()
                                      ,[ModifiedBy] = @ModifiedBy
                                 WHERE [FakturPajakReturId] = @FakturPajakReturId");

                sp.AddParameter("FakturPajakReturId", data.FakturPajakReturId);
                sp.AddParameter("ModifiedBy", data.ModifiedBy);
            }
            else
            {
                sp = new SpBase(@"INSERT INTO [dbo].[FakturPajakRetur]
           ([FCode]
           ,[FakturPajakId]
           ,[NPWPPenjual]
           ,[NamaPenjual]
           ,[AlamatPenjual]
           ,[FormatedNoFakturPajak]
           ,[FormatedNpwpPenjual]
           ,[KdJenisTransaksi]
           ,[FgPengganti]
           ,[NoFakturPajak]
           ,[TglFaktur]
           ,[Dikreditkan]
           ,[NoDocRetur]
           ,[TglRetur]
           ,[MasaPajakLapor]
           ,[TahunPajakLapor]
           ,[JumlahDPP]
           ,[JumlahPPN]
           ,[JumlahPPNBM]
           ,[Pesan]
           ,[CreatedBy]
           )
     VALUES
           (@FCode
           ,@FakturPajakId
           ,@NPWPPenjual
           ,@NamaPenjual
           ,@AlamatPenjual
           ,@FormatedNoFakturPajak
           ,@FormatedNpwpPenjual
           ,@KdJenisTransaksi
           ,@FgPengganti
           ,@NoFakturPajak
           ,@TglFaktur
           ,@Dikreditkan
           ,dbo.fnGenerateDocumentNoRetur(@TahunPajakLapor)
           ,@TglRetur
           ,@MasaPajakLapor
           ,@TahunPajakLapor
           ,@JumlahDPP
           ,@JumlahPPN
           ,@JumlahPPNBM
           ,@Pesan
           ,@CreatedBy
		   ); SELECT @FakturPajakReturId = @@IDENTITY");

                sp.AddParameter("FakturPajakReturId", data.FakturPajakReturId, ParameterDirection.Output);
                sp.AddParameter("CreatedBy", data.CreatedBy);

            }

            sp.AddParameter("FCode", data.FCode);
            sp.AddParameter("FakturPajakId", data.FakturPajakId.HasValue ? data.FakturPajakId.Value : SqlInt64.Null);
            sp.AddParameter("TglRetur", data.TglRetur.HasValue ? data.TglRetur.Value : SqlDateTime.Null);
            sp.AddParameter("MasaPajakLapor", data.MasaPajakLapor);
            sp.AddParameter("TahunPajakLapor", data.TahunPajakLapor);
            sp.AddParameter("JumlahDPP", data.JumlahDPP.HasValue ? data.JumlahDPP.Value : SqlDecimal.Null);
            sp.AddParameter("JumlahPPN", data.JumlahPPN.HasValue ? data.JumlahPPN.Value : SqlDecimal.Null);
            sp.AddParameter("JumlahPPNBM", data.JumlahPPNBM.HasValue ? data.JumlahPPNBM.Value : SqlDecimal.Null);
            sp.AddParameter("Pesan", string.IsNullOrEmpty(data.Pesan) ? SqlString.Null : data.Pesan);
            sp.AddParameter("NPWPPenjual", string.IsNullOrEmpty(data.NPWPPenjual) ? SqlString.Null : data.NPWPPenjual);
            sp.AddParameter("NamaPenjual", string.IsNullOrEmpty(data.NamaPenjual) ? SqlString.Null : data.NamaPenjual);
            sp.AddParameter("AlamatPenjual", string.IsNullOrEmpty(data.AlamatPenjual) ? SqlString.Null : data.AlamatPenjual);
            sp.AddParameter("FormatedNoFakturPajak", string.IsNullOrEmpty(data.FormatedNoFakturPajak) ? SqlString.Null : data.FormatedNoFakturPajak);
            sp.AddParameter("FormatedNpwpPenjual", string.IsNullOrEmpty(data.FormatedNpwpPenjual) ? SqlString.Null : data.FormatedNpwpPenjual);
            sp.AddParameter("TglFaktur", data.TglFaktur.HasValue ? data.TglFaktur.Value : SqlDateTime.Null);
            sp.AddParameter("Dikreditkan", data.Dikreditkan.HasValue ? data.Dikreditkan.Value : SqlBoolean.Null);

            sp.AddParameter("KdJenisTransaksi", string.IsNullOrEmpty(data.KdJenisTransaksi) ? SqlString.Null : data.KdJenisTransaksi);
            sp.AddParameter("FgPengganti", string.IsNullOrEmpty(data.FgPengganti) ? SqlString.Null : data.FgPengganti);
            sp.AddParameter("NoFakturPajak", string.IsNullOrEmpty(data.NoFakturPajak) ? SqlString.Null : data.NoFakturPajak);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            if (data.FakturPajakReturId <= 0)
            {
                data.FakturPajakReturId = (long)sp.GetParameter("FakturPajakReturId");
            }

            data = GetById(data.FakturPajakReturId);

            return data;
        }

        public static bool Delete(long FakturPajakReturId, string modifiedBy)
        {
            var sp = new SpBase(string.Format(@"UPDATE dbo.FakturPajakRetur SET IsDeleted = 1, Modified = GETDATE(), ModifiedBy = @ModifiedBy WHERE FakturPajakReturId = @FakturPajakReturId;"));
            sp.AddParameter("FakturPajakReturId", FakturPajakReturId);
            sp.AddParameter("ModifiedBy", modifiedBy);
            return sp.ExecuteNonQuery() == 0;
        }

    }
}
