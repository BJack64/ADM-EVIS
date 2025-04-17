using System;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System.Collections.Generic;
using eFakturADM.Logic.Utilities;
using System.Web;
using System.IO;
using eFakturADM.Shared.Utility;
using System.Reflection;
using System.Globalization;

namespace eFakturADM.Logic.Collections
{
    public class FakturPajakTerlaporCollections : ApplicationCollection<FakturPajakTerlapor, SpBase>
    {
        public static List<FakturPajakTerlapor> GetList(Filter filter, out int totalItems, string MasaPajak, string TahunPajak)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(@"EXEC	[dbo].[sp_FakturPajakTerlapor_GetList]
		    @MasaPajak = @MasaPajakParam,
		    @TahunPajak = @TahunPajakParam,
		    @CurrentPage = @CurrentPageParam,
		    @ItemPerPage = @ItemPerPageParam,
		    @SortColumnName = @SortColumnNameParam,
		    @sortOrder = @sortOrderParam
		");



            sp.AddParameter("CurrentPageParam", filter.CurrentPage);
            sp.AddParameter("ItemPerPageParam", filter.ItemsPerPage);
            sp.AddParameter("SortColumnNameParam", filter.SortColumnName);
            sp.AddParameter("sortOrderParam", sortOrder);

            sp.AddParameter("MasaPajakParam", string.IsNullOrEmpty(MasaPajak) ? "0" : MasaPajak);
            sp.AddParameter("TahunPajakParam", string.IsNullOrEmpty(TahunPajak) ? "0" : TahunPajak);

            var data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems, MasaPajak, TahunPajak);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, MasaPajak, TahunPajak);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, MasaPajak, TahunPajak);
            }

            return data;
        }

        public static FakturPajakTerlapor GetById(long FakturPajakTerlaporId)
        {
            var sp = new SpBase(@"SELECT fp.*,m.MonthName AS MasaPajakName, COUNT(FakturPajakTerlaporID) OVER() AS TotalItems FROM FakturPajakTerlapor fp LEFT JOIN [dbo].TMonth (NOLOCK) m ON fp.MasaPajak = m.MonthNumber WHERE fp.FakturPajakTerlaporID = @FakturPajakTerlaporID");
            sp.AddParameter("FakturPajakTerlaporID", FakturPajakTerlaporId);
            var dbData = GetApplicationObject(sp);

            return dbData == null || dbData.FakturPajakTerlaporID == 0 ? null : dbData;

        }

        public static int GetTotalRecord(int masaPajak, int tahunPajak)
        {

            var sp = new SpBase(@"
                SELECT COUNT(fp.FakturPajakId) AS TotalRow FROM  FakturPajak fp 
                WHERE fp.MasaPajak = @MasaPajak AND fp.TahunPajak = @TahunPajak AND fp.IsDeleted = 0 AND fp.FakturPajakTerlaporID IS NULL
                AND (StatusFaktur IS NULL OR (StatusFaktur IS NOT NULL AND StatusFaktur <> 'Faktur Diganti' AND StatusFaktur <> 'Faktur Dibatalkan'))
            ");
            sp.AddParameter("MasaPajak", masaPajak, ParameterDirection.Input, DbType.Int32);
            sp.AddParameter("TahunPajak", tahunPajak, ParameterDirection.Input, DbType.Int32);
            var retScalar = sp.ExecuteScalar();
            if (retScalar != null)
            {
                return (int)retScalar;
            }

            return 0;

        }

        public static decimal GetTotalPPN(int masaPajak, int tahunPajak)
        {

            var sp = new SpBase(@"
                SELECT  SUM(fp.JumlahPPN) AS TotalPPN  FROM  FakturPajak fp 
                WHERE fp.MasaPajak = @MasaPajak AND fp.TahunPajak = @TahunPajak AND fp.IsDeleted = 0 AND fp.FakturPajakTerlaporID IS NULL
                AND (StatusFaktur IS NULL OR (StatusFaktur IS NOT NULL AND StatusFaktur <> 'Faktur Diganti' AND StatusFaktur <> 'Faktur Dibatalkan'))
            ");
            sp.AddParameter("MasaPajak", masaPajak, ParameterDirection.Input, DbType.Int32);
            sp.AddParameter("TahunPajak", tahunPajak, ParameterDirection.Input, DbType.Int32);
            var retScalar = sp.ExecuteScalar();
            try
            {
                if (retScalar != null)
                {
                    return (decimal)retScalar;
                }
            }
            catch (Exception)
            {

                return 0;
            }

            return 0;

        }


        public static int GetPembetulan(int masaPajak, int tahunPajak)
        {

            var sp = new SpBase(@"
                SELECT COUNT(1) FROM FakturPajakTerlapor WHERE MasaPajak = @MasaPajak AND TahunPajak = @TahunPajak
            ");
            sp.AddParameter("MasaPajak", masaPajak, ParameterDirection.Input, DbType.Int32);
            sp.AddParameter("TahunPajak", tahunPajak, ParameterDirection.Input, DbType.Int32);
            var retScalar = sp.ExecuteScalar();
            try
            {
                if (retScalar != null)
                {
                    return (int)retScalar;
                }
            }
            catch (Exception)
            {

                return 0;
            }

            return 0;

        }
        public static bool Submit(FakturPajakTerlapor data, string userNameLogin, out string MsgError, out string logKey)
        {
            bool result = true;
            MsgError = "";
            logKey = "";
            var destfile = "";
            try
            {
                var isfilemoved = true;

                if (data.AttachmentPath != null)
                {
                    FileInfo info = new FileInfo(data.AttachmentPath);
                    destfile = string.Format(@"{0}\{1}\{2}"
                            , GeneralConfigs.GetByConfigKey(EnumHelper.GetDescription(ApplicationEnums.GeneralConfig.EvisShareFolderRootPath)).ConfigValue
                            , "FakturPajakTerlapor"
                            , info.Name);
                    try
                    {
                        var dir = Path.GetDirectoryName(destfile);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        File.Copy(data.AttachmentPath, destfile, true);
                    }
                    catch (Exception e)
                    {
                        MsgError = "[Faktur Pajak Terlapor Attachment] File Move failed";
                        Logger.WriteLog(out logKey, LogLevel.Error, "[Faktur Pajak Terlapor Attachment]" + e.Message, MethodBase.GetCurrentMethod(), e);
                        isfilemoved = false;
                    }

                }
                if (isfilemoved)
                {

                    SpBase sp;
                    //Update
                    sp = new SpBase(@"
                        EXEC [dbo].[sp_FakturPajakTerlapor_Save]
	                    @Nama
                        ,@TanggalLapor
                        ,@MasaPajak
                        ,@TahunPajak
                        ,@AttachmentPath
                        ,@UserName
	                    ,@IsValid OUT
	                    ,@MsgError OUT
                        SELECT @IsValid AS IsValid,@MsgError AS MsgError
                        ");

                    sp.AddParameter("Nama", data.Nama, ParameterDirection.Input, DbType.String, 255);
                    var dt = DateTime.ParseExact(data.sTanggalLapor, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    data.TanggalLapor = Convert.ToDateTime(string.Concat(dt.Year, "-", dt.Month, "-", dt.Day));
                    sp.AddParameter("TanggalLapor", data.TanggalLapor, ParameterDirection.Input, DbType.Date);
                    sp.AddParameter("MasaPajak", data.MasaPajak, ParameterDirection.Input);
                    sp.AddParameter("TahunPajak", data.TahunPajak, ParameterDirection.Input);
                    sp.AddParameter("AttachmentPath", string.IsNullOrEmpty(destfile) ? SqlString.Null : destfile, ParameterDirection.Input, DbType.String, 500);
                    sp.AddParameter("UserName", string.IsNullOrEmpty(userNameLogin) ? SqlString.Null : userNameLogin, ParameterDirection.Input, DbType.String, 100);
                    sp.AddParameter("IsValid", result, ParameterDirection.Output, DbType.Boolean);
                    sp.AddParameter("MsgError", MsgError, ParameterDirection.Output, DbType.String, 500);

                    if (sp.ExecuteNonQuery() == 0)
                    {
                        result = (bool)sp.GetParameter("IsValid");
                        MsgError = (string)sp.GetParameter("MsgError");
                    }
                    else result = false;
                }
            }
            catch (Exception exception)
            {
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                MsgError = exception.Message;
                result = false;
            }
            return result;
        }


        public static bool UpdateFile(int ID, string AttachmentFile, string userNameLogin, out string MsgError, out string logKey)
        {
            bool result = true;
            MsgError = "";
            logKey = "";
            try
            {

                SpBase sp;
                //Update
                sp = new SpBase(@"
                        EXEC [dbo].[sp_FakturPajakTerlapor_UpdateFile]
                        @FakturPajakTerlaporID
                        ,@AttachmentPath
                        ,@UserName
	                    ,@IsValid OUT
	                    ,@MsgError OUT
                        SELECT @IsValid AS IsValid,@MsgError AS MsgError
                        ");

                sp.AddParameter("FakturPajakTerlaporID", ID, ParameterDirection.Input);
                sp.AddParameter("AttachmentPath", string.IsNullOrEmpty(AttachmentFile) ? SqlString.Null : AttachmentFile, ParameterDirection.Input, DbType.String, 500);
                sp.AddParameter("UserName", string.IsNullOrEmpty(userNameLogin) ? SqlString.Null : userNameLogin, ParameterDirection.Input, DbType.String, 100);
                sp.AddParameter("IsValid", result, ParameterDirection.Output, DbType.Boolean);
                sp.AddParameter("MsgError", MsgError, ParameterDirection.Output, DbType.String, 500);

                if (sp.ExecuteNonQuery() == 0)
                {
                    result = (bool)sp.GetParameter("IsValid");
                    MsgError = (string)sp.GetParameter("MsgError");
                }
                else result = false;
            }
            catch (Exception exception)
            {
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                MsgError = exception.Message;
                result = false;
            }
            return result;
        }


    }
}
