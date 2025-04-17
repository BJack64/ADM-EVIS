using System;
using System.Data;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System.Collections.Generic;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class OpenClosePeriods : ApplicationCollection<OpenClosePeriod, SpBase>
    {

        public static List<OpenClosePeriod> Get()
        {
            var sp = new SpBase(@"SELECT [OpenClosePeriodId]
                    ,[MasaPajak]
                    ,m.MonthName AS MonthName
                    ,[TahunPajak]
                    ,[StatusRegular]
                    ,[StatusSP2]
	                ,CASE WHEN [StatusRegular] = 0 THEN 'Close' ELSE 'Open' END AS StatusRegularText
                    ,CASE WHEN [StatusSP2] = 0 THEN 'Close' ELSE 'Open' END AS StatusSp2Text
                    ,[DocumentSP2]
                    ,[IsDeleted]
                    ,[Created]
                    ,[Modified]
                    ,[CreatedBy]
                    ,[ModifiedBy]
	                ,COUNT([OpenClosePeriodId]) OVER() AS TotalItems
            FROM [dbo].[OpenClosePeriod] ocp INNER JOIN
		            dbo.GetMonth() m ON ocp.MasaPajak = m.MonthNumber
                WHERE ocp.[IsDeleted] = 0");
            return GetApplicationCollection(sp);
        }

        public static List<OpenClosePeriod> GetOpenReguler()
        {
            var sp = new SpBase(@"SELECT [OpenClosePeriodId]
                    ,[MasaPajak]
                    ,m.MonthName AS MonthName
                    ,[TahunPajak]
                    ,[StatusRegular]
                    ,[StatusSP2]
	                ,CASE WHEN [StatusRegular] = 0 THEN 'Close' ELSE 'Open' END AS StatusRegularText
                    ,CASE WHEN [StatusSP2] = 0 THEN 'Close' ELSE 'Open' END AS StatusSp2Text
                    ,[DocumentSP2]
                    ,[IsDeleted]
                    ,[Created]
                    ,[Modified]
                    ,[CreatedBy]
                    ,[ModifiedBy]
	                ,COUNT([OpenClosePeriodId]) OVER() AS TotalItems
            FROM [dbo].[OpenClosePeriod] ocp INNER JOIN
		            dbo.GetMonth() m ON ocp.MasaPajak = m.MonthNumber
                WHERE ocp.[IsDeleted] = 0 AND ocp.[StatusRegular] = 1");
            return GetApplicationCollection(sp);
        }

        public static OpenClosePeriod GetById(int OpenClosePeriodId)
        {
            var sp = new SpBase(@"SELECT [OpenClosePeriodId]
                            ,[MasaPajak]
                            ,m.MonthName AS MonthName
                            ,[TahunPajak]
                            ,[StatusRegular]
                            ,[StatusSP2]
	                        ,CASE WHEN [StatusRegular] = 0 THEN 'Close' ELSE 'Open' END AS StatusRegularText
                            ,CASE WHEN [StatusSP2] = 0 THEN 'Close' ELSE 'Open' END AS StatusSp2Text
                            ,[DocumentSP2]
                            ,[IsDeleted]
                            ,[Created]
                            ,[Modified]
                            ,[CreatedBy]
                            ,[ModifiedBy]
	                        ,COUNT([OpenClosePeriodId]) OVER() AS TotalItems
                    FROM [dbo].[OpenClosePeriod] ocp INNER JOIN
		                    dbo.GetMonth() m ON ocp.MasaPajak = m.MonthNumber
                WHERE ocp.[IsDeleted] = 0 AND ocp.[OpenClosePeriodId] = @OpenClosePeriodId");

            sp.AddParameter("OpenClosePeriodId", OpenClosePeriodId);
            return GetApplicationObject(sp);
        }
        
        
        public static OpenClosePeriod GetByMasaPajak(int month, int year)
        {
            var sp = new SpBase(@"SELECT [OpenClosePeriodId]
                    ,[MasaPajak]
                    ,m.MonthName AS MonthName
                    ,[TahunPajak]
                    ,[StatusRegular]
                    ,[StatusSP2]
	                ,CASE WHEN [StatusRegular] = 0 THEN 'Close' ELSE 'Open' END AS StatusRegularText
                    ,CASE WHEN [StatusSP2] = 0 THEN 'Close' ELSE 'Open' END AS StatusSp2Text
                    ,[DocumentSP2]
                    ,[IsDeleted]
                    ,[Created]
                    ,[Modified]
                    ,[CreatedBy]
                    ,[ModifiedBy]
	                ,COUNT([OpenClosePeriodId]) OVER() AS TotalItems
            FROM [dbo].[OpenClosePeriod] ocp INNER JOIN
		            dbo.GetMonth() m ON ocp.MasaPajak = m.MonthNumber
            WHERE [IsDeleted] = 0 AND [MasaPajak] = @MasaPajak AND [TahunPajak] = @TahunPajak");

            sp.AddParameter("MasaPajak", month);
            sp.AddParameter("TahunPajak", year);

            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.OpenClosePeriodId == 0 ? null : dbData;
        }


        public static List<OpenClosePeriod> GetListOpenClosePeriod(Filter filter, out int totalItems, int? masaPajakStart, int? masaPajakEnd, int? tahunPajakStart, int? tahunPajakEnd)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;
            var sp = new SpBase(string.Format(@"SELECT [OpenClosePeriodId]
                                        ,[MasaPajak]
                                        ,m.MonthName AS MonthName
                                        ,[TahunPajak]
                                        ,[StatusRegular]
                                        ,[StatusSP2]
	                                    ,CASE WHEN [StatusRegular] = 0 THEN 'Close' ELSE 'Open' END AS StatusRegularText
                                        ,CASE WHEN [StatusSP2] = 0 THEN 'Close' ELSE 'Open' END AS StatusSp2Text
                                        ,[DocumentSP2]
                                        ,[IsDeleted]
                                        ,[Created]
                                        ,[Modified]
                                        ,[CreatedBy]
                                        ,[ModifiedBy]
	                                    ,COUNT([OpenClosePeriodId]) OVER() AS TotalItems
                                FROM [dbo].[OpenClosePeriod] ocp INNER JOIN
		                                dbo.GetMonth() m ON ocp.MasaPajak = m.MonthNumber
                        WHERE (@MasaPajakStart IS NOT NULL AND @MasaPajakEnd IS NOT NULL AND ([MasaPajak] BETWEEN ISNULL(@MasaPajakStart, [MasaPajak]) AND ISNULL(@MasaPajakEnd, [MasaPajak])) 
						OR @MasaPajakStart IS NULL AND @MasaPajakEnd IS NOT NULL AND [MasaPajak] = @MasaPajakEnd
						OR @MasaPajakStart IS NOT NULL AND @MasaPajakEnd IS NULL AND [MasaPajak] = @MasaPajakStart
						OR @MasaPajakStart IS NULL AND @MasaPajakEnd IS NULL)
                            AND 
							(@TahunPajakStart IS NOT NULL AND @TahunPajakEnd IS NOT NULL AND ([TahunPajak] BETWEEN ISNULL(@TahunPajakStart, [TahunPajak]) AND ISNULL(@TahunPajakEnd, [TahunPajak])) 
						OR @TahunPajakStart IS NULL AND @TahunPajakEnd IS NOT NULL AND [TahunPajak] = @TahunPajakEnd
						OR @TahunPajakStart IS NOT NULL AND @TahunPajakEnd IS NULL AND [TahunPajak] = @TahunPajakStart
						OR @TahunPajakStart IS NULL AND @TahunPajakEnd IS NULL)
                            AND ((@Search IS NOT NULL AND (
                                [MonthName] LIKE '%' + LOWER(@Search) + '%'
                                OR [TahunPajak] LIKE '%' + LOWER(@Search) + '%'
                                OR CASE WHEN [StatusRegular] = 0 THEN 'Close' ELSE 'Open' END  LIKE '%' + LOWER(@Search) + '%'
                                OR CASE WHEN [StatusSP2] = 0 THEN 'Close' ELSE 'Open' END LIKE '%' + LOWER(@Search) + '%'                          
	                            )OR @Search IS NULL
                            ))
                        ORDER BY {0} {1}
                        OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
                        FETCH NEXT @ItemPerPage ROWS ONLY", filter.SortColumnName, sortOrder));            

            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("MasaPajakStart", masaPajakStart.HasValue ? masaPajakStart.Value : SqlInt32.Null);
            sp.AddParameter("MasaPajakEnd", masaPajakEnd.HasValue ? masaPajakEnd.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajakStart", tahunPajakStart.HasValue ? tahunPajakStart.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajakEnd", tahunPajakEnd.HasValue ? tahunPajakEnd.Value : SqlInt32.Null);
            sp.AddParameter("Search", string.IsNullOrEmpty(filter.Search) ? SqlString.Null : filter.Search);
      

            List<OpenClosePeriod> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetListOpenClosePeriod(filter, out totalItems, masaPajakStart, masaPajakEnd, tahunPajakStart, tahunPajakEnd);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetListOpenClosePeriod(filter, out totalItems, masaPajakStart, masaPajakEnd, tahunPajakStart, tahunPajakEnd);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetListOpenClosePeriod(filter, out totalItems, masaPajakStart, masaPajakEnd, tahunPajakStart, tahunPajakEnd);
            }

            return data;

        }

        public static OpenClosePeriod Save(OpenClosePeriod data)
        {
            data.WasSaved = false;
            SpBase sp;

            if (data.OpenClosePeriodId > 0)
            {
                sp = new SpBase(@"UPDATE [dbo].[OpenClosePeriod]
                                   SET 
                                      [MasaPajak] = @MasaPajak
                                      ,[TahunPajak] = @TahunPajak
                                      ,[StatusRegular] = @StatusRegular
                                      ,[StatusSP2] = @StatusSP2
                                      ,[DocumentSP2] = @DocumentSP2
                                      ,[Modified] = GETDATE()
                                      ,[ModifiedBy] = @ModifiedBy
                                 WHERE [OpenClosePeriodId] = @OpenClosePeriodId");

                sp.AddParameter("OpenClosePeriodId", data.OpenClosePeriodId);
                sp.AddParameter("ModifiedBy", data.ModifiedBy);
            }
            else
            {
                sp = new SpBase(@"INSERT INTO [dbo].[OpenClosePeriod]
                           ([MasaPajak]
                           ,[TahunPajak]
                           ,[StatusRegular]
                           ,[StatusSP2]
                           ,[DocumentSP2]
                           ,[CreatedBy]
                           )
                     VALUES
                           (@MasaPajak
                           ,@TahunPajak
                           ,@StatusRegular
                           ,@StatusSP2
                           ,@DocumentSP2
                           ,@CreatedBy
		   ); SELECT @OpenClosePeriodId = @@IDENTITY");

                sp.AddParameter("OpenClosePeriodId", data.OpenClosePeriodId, ParameterDirection.Output);
                sp.AddParameter("CreatedBy", data.CreatedBy);

            }

            sp.AddParameter("MasaPajak", data.MasaPajak);
            sp.AddParameter("TahunPajak", data.TahunPajak);
            sp.AddParameter("StatusRegular", data.StatusRegular);
            sp.AddParameter("StatusSP2", data.StatusSp2);
            sp.AddParameter("DocumentSP2", !string.IsNullOrEmpty(data.DocumentSP2) ? data.DocumentSP2 : SqlString.Null);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            if (data.OpenClosePeriodId <= 0)
            {
                data.OpenClosePeriodId = (int)sp.GetParameter("OpenClosePeriodId");
            }

            return data;
        }

        public static bool UpdateStatusRegular(int OpenClosePeriodId, bool StatusRegular, string modifiedBy)
        {
            var sp = new SpBase(string.Format(@"UPDATE dbo.OpenClosePeriod SET StatusRegular = @StatusRegular, Modified = GETDATE(), ModifiedBy = @ModifiedBy WHERE OpenClosePeriodId = @OpenClosePeriodId;"));
            sp.AddParameter("OpenClosePeriodId", OpenClosePeriodId);
            sp.AddParameter("StatusRegular", StatusRegular);
            sp.AddParameter("ModifiedBy", modifiedBy);

            return sp.ExecuteNonQuery() == 0;
        }

        public static bool UpdateStatusSP2(int OpenClosePeriodId, bool StatusSP2, string DocumentSP2, string modifiedBy)
        {
            var sp = new SpBase(string.Format(@"UPDATE dbo.OpenClosePeriod SET StatusSP2 = @StatusSP2, DocumentSP2 = @DocumentSP2, Modified = GETDATE(), ModifiedBy = @ModifiedBy WHERE OpenClosePeriodId = @OpenClosePeriodId;"));
            sp.AddParameter("OpenClosePeriodId", OpenClosePeriodId);
            sp.AddParameter("StatusSP2", StatusSP2);
            sp.AddParameter("DocumentSP2", DocumentSP2);
            sp.AddParameter("ModifiedBy", modifiedBy);

            return sp.ExecuteNonQuery() == 0;
        }

        public static List<OpenClosePeriod> GetByRange(DateTime dtstart, DateTime dtend)
        {
            var sp = new SpBase(@"SELECT [OpenClosePeriodId]
        ,[MasaPajak]
        ,m.MonthName AS MonthName
        ,[TahunPajak]
        ,[StatusRegular]
        ,[StatusSP2]
	    ,CASE WHEN [StatusRegular] = 0 THEN 'Close' ELSE 'Open' END AS StatusRegularText
        ,CASE WHEN [StatusSP2] = 0 THEN 'Close' ELSE 'Open' END AS StatusSp2Text
        ,[DocumentSP2]
        ,[IsDeleted]
        ,[Created]
        ,[Modified]
        ,[CreatedBy]
        ,[ModifiedBy]
	    ,COUNT([OpenClosePeriodId]) OVER() AS TotalItems
FROM [dbo].[OpenClosePeriod] ocp INNER JOIN
		dbo.GetMonth() m ON ocp.MasaPajak = m.MonthNumber
WHERE	ocp.IsDeleted = 0
		AND DATEFROMPARTS(ocp.TahunPajak, ocp.MasaPajak, 1) BETWEEN CAST(@min AS date) AND CAST(@max AS date)");
            sp.AddParameter("min", dtstart);
            sp.AddParameter("max", dtend);

            return GetApplicationCollection(sp);

        }

    }
}
