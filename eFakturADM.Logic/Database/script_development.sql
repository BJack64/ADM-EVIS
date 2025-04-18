USE [master]
GO
/****** Object:  Database [EVIS]    Script Date: 4/3/2017 11:48:59 AM ******/
CREATE DATABASE [EVIS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'EVIS', FILENAME = N'F:\Microsoft SQL Server\MSSQL11.SQLSERVER2012\MSSQL\DATA\EVIS.mdf' , SIZE = 48128KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'EVIS_log', FILENAME = N'F:\Microsoft SQL Server\MSSQL11.SQLSERVER2012\MSSQL\DATA\EVIS_log.ldf' , SIZE = 625792KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [EVIS] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [EVIS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [EVIS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [EVIS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [EVIS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [EVIS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [EVIS] SET ARITHABORT OFF 
GO
ALTER DATABASE [EVIS] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [EVIS] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [EVIS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [EVIS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [EVIS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [EVIS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [EVIS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [EVIS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [EVIS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [EVIS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [EVIS] SET  DISABLE_BROKER 
GO
ALTER DATABASE [EVIS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [EVIS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [EVIS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [EVIS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [EVIS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [EVIS] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [EVIS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [EVIS] SET RECOVERY FULL 
GO
ALTER DATABASE [EVIS] SET  MULTI_USER 
GO
ALTER DATABASE [EVIS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [EVIS] SET DB_CHAINING OFF 
GO
ALTER DATABASE [EVIS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [EVIS] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'EVIS', N'ON'
GO
USE [EVIS]
GO
/****** Object:  UserDefinedTableType [dbo].[CompEvisIwsParamTable]    Script Date: 4/3/2017 11:49:00 AM ******/
CREATE TYPE [dbo].[CompEvisIwsParamTable] AS TABLE(
	[ReceivedDate] [date] NOT NULL,
	[VendorCode] [nvarchar](max) NULL,
	[VendorName] [nvarchar](max) NULL,
	[TaxInvoiceNumberEVIS] [nvarchar](max) NULL,
	[TaxInvoiceNumberIWS] [nvarchar](max) NULL,
	[InvoiceNumber] [nvarchar](max) NULL,
	[VATAmountScanned] [decimal](18, 2) NULL,
	[VATAmountIWS] [decimal](18, 2) NULL,
	[VATAmountDiff] [decimal](18, 2) NULL,
	[StatusDJP] [nvarchar](max) NULL,
	[StatusCompare] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](100) NOT NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[SAP_MTDownloadPPN_ParamTable]    Script Date: 4/3/2017 11:49:00 AM ******/
CREATE TYPE [dbo].[SAP_MTDownloadPPN_ParamTable] AS TABLE(
	[CompanyCode] [nvarchar](max) NULL,
	[AccountingDocNo] [nvarchar](max) NULL,
	[FiscalYear] [nvarchar](max) NULL,
	[DocType] [nvarchar](max) NULL,
	[PostingDate] [nvarchar](max) NULL,
	[AmountLocal] [nvarchar](max) NULL,
	[LineItem] [nvarchar](max) NULL,
	[Reference] [nvarchar](max) NULL,
	[HeaderText] [nvarchar](max) NULL,
	[UserName] [nvarchar](max) NULL,
	[ItemText] [nvarchar](max) NULL,
	[AssignmentNo] [nvarchar](max) NULL,
	[BusinessArea] [nvarchar](max) NULL,
	[GLAccount] [nvarchar](max) NULL,
	[ReferenceLineItem] [nvarchar](max) NULL,
	[DocDate] [nvarchar](max) NULL,
	[Currency] [nvarchar](max) NULL,
	[SalesTaxCode] [nvarchar](max) NULL,
	[AmountDocCurrency] [nvarchar](max) NULL,
	[PostingKey] [nvarchar](max) NULL,
	[ClearingDoc] [nvarchar](max) NULL,
	[ClearingDate] [nvarchar](max) NULL,
	[ReverseDocNo] [nvarchar](max) NULL,
	[ReverseDocFiscalYear] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[ReferenceKey1] [nvarchar](max) NULL,
	[ReferenceKey2] [nvarchar](max) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[Vendor_ParamTable]    Script Date: 4/3/2017 11:49:00 AM ******/
CREATE TYPE [dbo].[Vendor_ParamTable] AS TABLE(
	[NPWP] [nvarchar](max) NOT NULL,
	[Nama] [nvarchar](max) NOT NULL,
	[Alamat] [nvarchar](max) NULL,
	[PkpDicabut] [nvarchar](max) NULL,
	[TglPkpDicabut] [nvarchar](max) NULL,
	[KeteranganTambahan] [nvarchar](255) NULL,
	[UserNameLogin] [nvarchar](100) NULL
)
GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsIwsGenerateByReceivingDate]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CompEvisVsIwsGenerateByReceivingDate]
	-- Add the parameters for the stored procedure here
	@receivingDate date,
	@scandate date,
	@filterUser nvarchar(100),
	@userNameLogin nvarchar(100)
AS
BEGIN
	DECLARE @diffToleran decimal = (SELECT CAST(ConfigValue AS decimal) AS configToleran FROM GeneralConfig WHERE ConfigKey = 'CompareEvisVsIwsToleransiDiff')
MERGE INTO dbo.COMP_EVIS_IWS dest USING
	(
	SELECT		v.ReceivedDate
				,v.VendorCode
				,v.VendorName
				,v.TaxInvoiceNumberEVIS
				,v.TaxInvoiceNumberIWS
				,v.InvoiceNumber
				,v.VATAmountScanned
				,v.VATAmountIWS
				,v.VATAmountDiff
				,v.StatusDJP
				,v.StatusCompare					
				,v.CreatedBy AS [ScanUserName]
				,v.LastUpdatedOn
				,v.ScanDate
	FROM		(
				SELECT	 CAST(CASE WHEN evis.ReceivingDate IS NULL THEN iws.RECEIVINGDATE ELSE evis.ReceivingDate END AS date) AS ReceivedDate
					,iws.VENDORID AS VendorCode
					,iws.VENDORNAME AS VendorName
					,evis.FormatedNoFaktur AS TaxInvoiceNumberEvis
					,iws.TAXVOUCHERNO AS TaxInvoiceNumberIws
					,iws.INVOICENO AS InvoiceNumber
					,evis.PPN AS VatAmountScanned
					,iws.PPN AS VatAmountIws
					,ABS(ISNULL(evis.PPN, 0) - ISNULL(iws.PPN, 0)) AS VatAmountDiff
					,evis.StatusDjp AS StatusDjp
					,dbo.fnCompEvisVsIwsGetStatusCompare(evis.NoFakturPajak, iws.TAXVOUCHERNO, evis.PPN, iws.PPN, @diffToleran) AS StatusCompare						
					,evis.CreatedBy AS CreatedBy
					,evis.Created AS ScanDate
					,iws.MODIFIEDON AS LastUpdatedOn
			FROM	(
			SELECT	fp.NoFakturPajak, fp.ReceivingDate, fp.JumlahPPN AS PPN, fp.StatusApproval AS StatusDjp, fp.FormatedNoFaktur, fp.CreatedBy, fp.Created
			FROM	dbo.FakturPajak fp
			WHERE	fp.IsDeleted = 0 AND fp.[Status] = 2
					AND fp.FPType = 1 AND CONVERT(date, @receivingDate) = CONVERT(date, fp.ReceivingDate)
					AND (@scandate IS NULL OR (@scandate IS NOT NULL AND CONVERT(date, @scandate) = CONVERT(date, fp.Created)))
					AND (@filterUser IS NULL OR (@filterUser IS NOT NULL AND fp.CreatedBy LIKE REPLACE(@filterUser, '*', '%')))
					) evis FULL OUTER JOIN
					(
					SELECT	v1.RECEIVINGDATE, v1.VENDORID, v1.VENDORNAME, v1.TAXVOUCHERNO, v1.INVOICENO, v1.PPN, v1.MODIFIEDON
					FROM	vw_DataIWSReqEfis v1
					WHERE	v1.[STATUS] = 'Received' 
							AND v1.RECEIVINGDATE IS NOT NULL 		
							AND CONVERT(date, v1.RECEIVINGDATE) = CONVERT(date,@receivingDate)
					) iws ON CAST(iws.RECEIVINGDATE as date) = CAST(evis.ReceivingDate as date) AND iws.TAXVOUCHERNO = evis.FormatedNoFaktur
			) v
	) src ON dest.ReceivedDate = src.ReceivedDate AND (dest.TaxInvoiceNumberEVIS = src.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT OR dest.TaxInvoiceNumberIWS = src.TaxInvoiceNumberIWS COLLATE DATABASE_DEFAULT)
WHEN MATCHED AND ((dest.LastUpdatedOnIws IS NULL AND src.LastUpdatedOn IS NOT NULL) OR (dest.LastUpdatedOnIws IS NOT NULL AND src.LastUpdatedOn IS NOT NULL AND src.LastUpdatedOn <> dest.LastUpdatedOnIws))  THEN
		UPDATE 
		   SET [VendorCode] = src.VendorCode
			  ,[VendorName] = src.VendorName
			  ,[TaxInvoiceNumberEVIS] = src.TaxInvoiceNumberEVIS
			  ,[TaxInvoiceNumberIWS] = src.TaxInvoiceNumberIWS
			  ,[InvoiceNumber] = src.InvoiceNumber
			  ,[VATAmountScanned] = src.VATAmountScanned
			  ,[VATAmountIWS] = src.VATAmountIWS
			  ,[VATAmountDiff] = src.VATAmountDiff
			  ,[StatusDJP] = src.StatusDJP
			  ,[StatusCompare] = src.StatusCompare
			  ,[Modified] = GETDATE()      
			  ,[ModifiedBy] = @userNameLogin
			  ,[LastUpdatedOnIws] = src.LastUpdatedOn
			  ,[ScanDate] = src.ScanDate
			  ,[ScanUserName] = src.[ScanUserName]
	WHEN NOT MATCHED THEN
	INSERT ([ReceivedDate]
			   ,[VendorCode]
			   ,[VendorName]
			   ,[TaxInvoiceNumberEVIS]
			   ,[TaxInvoiceNumberIWS]
			   ,[InvoiceNumber]
			   ,[VATAmountScanned]
			   ,[VATAmountIWS]
			   ,[VATAmountDiff]
			   ,[StatusDJP]
			   ,[StatusCompare]
			   ,[CreatedBy]
			   ,[LastUpdatedOnIws]
			   ,[ScanUserName]
			   ,[ScanDate])
	VALUES (src.[ReceivedDate]
			   ,src.[VendorCode]
			   ,src.[VendorName]
			   ,src.[TaxInvoiceNumberEVIS]
			   ,src.[TaxInvoiceNumberIWS]
			   ,src.[InvoiceNumber]
			   ,src.[VATAmountScanned]
			   ,src.[VATAmountIWS]
			   ,src.[VATAmountDiff]
			   ,src.[StatusDJP]
			   ,src.[StatusCompare]
			   ,@userNameLogin
			   ,src.LastUpdatedOn
			   ,src.[ScanUserName]
			   ,src.[ScanDate]);

END





GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerate]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
---SP ALTER PROC
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerate]
	-- Add the parameters for the stored procedure here
	@postingDateStart date
	,@postingDateEnd date
	,@tglFakturStart date
	,@tglFakturEnd date
	,@noFakturStart nvarchar(100) 
	,@noFakturEnd nvarchar(100) 
	,@scanDate date 
	,@userName nvarchar(100) 
	,@masaPajak int 
	,@tahunPajak int 
	,@CurrentPage int 
	,@ItemPerPage int 
AS
BEGIN

	DECLARE @diffToleran decimal = (SELECT CAST(ConfigValue AS decimal) AS configToleran FROM GeneralConfig WHERE ConfigKey = 'CompareEvisVsSapToleransiDiff')
	
	SELECT	comp_result.*, CAST(0 AS int) AS Pembetulan
	FROM	(
		SELECT	 CAST(ROW_NUMBER() OVER(ORDER BY sap.AccountingDocNo ASC) as int) AS vSequence
				,CAST(sap.PostingDate as date) AS PostingDate
				,sap.AccountingDocNo
				,sap.ItemNo
				,CAST(efis.TglFaktur as date) AS TglFaktur
				,efis.NamaPenjual AS NamaVendor
				,efis.ScanDate
				,efis.FormatedNoFaktur AS TaxInvoiceNumberEvis
				,sap.TaxInvoiceNumber AS TaxInvoiceNumberSap
				,sap.DocumentHeaderText
				,sap.NPWP
				,efis.AmountEvis AS AmountEvis
				,sap.AmountLocal AS AmountSap
				,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
				,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
				,'' AS Notes
				,COUNT(sap.PostingDate) OVER() AS TotalItems
				,efis.CreatedBy AS UserNameCreator
				,sap.ItemText
				,efis.MasaPajak
				,efis.TahunPajak
				,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
				,sap.GLAccount
				,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
		FROM	(
				SELECT	 fp.NoFakturPajak		
						,fp.NPWPPenjual
						,fp.JumlahPPN AS AmountEvis
						,fp.TglFaktur AS EvisTanggalFaktur
						,fp.FormatedNoFaktur
						,fp.Created AS ScanDate
						,fp.NamaPenjual
						,fp.CreatedBy
						,fp.MasaPajak
						,fp.TahunPajak
						,fp.TglFaktur
				FROM	dbo.FakturPajak fp
				WHERE	fp.IsDeleted = 0 
						AND (fp.StatusReconcile IS NULL OR fp.StatusReconcile = 0)
						AND [Status] = 2
						AND (@scanDate IS NULL OR (@scanDate IS NOT NULL AND CAST(fp.Created as date) = CAST(@scanDate as date)))
						AND (@userName IS NULL OR (@userName IS NOT NULL AND LOWER(fp.CreatedBy) LIKE REPLACE(LOWER(@userName), '*', '%')))
						AND (@masaPajak IS NULL OR (@masaPajak IS NOT NULL AND fp.MasaPajak = @masaPajak))
						AND (@tahunPajak IS NULL OR (@tahunPajak IS NOT NULL AND fp.TahunPajak = @tahunPajak))
						AND (
							(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
							OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL 
								AND CAST(fp.TglFaktur as date) = CAST(@tglFakturStart as date))
							OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL 
								AND CAST(fp.TglFaktur as date) = CAST(@tglFakturEnd as date))
							OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL 
								AND CAST(fp.TglFaktur as date) >= CAST(@tglFakturStart as date) 
								AND CAST(fp.TglFaktur as date) <= CAST(@tglFakturEnd as date)
								)
						)
						AND (
							(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
							OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL 
								AND fp.FormatedNoFaktur = @noFakturStart)
							OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL 
								AND fp.FormatedNoFaktur = @noFakturEnd)
							OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL 
								AND (fp.FormatedNoFaktur BETWEEN @noFakturStart AND @noFakturEnd)
								)
						)
			) efis FULL OUTER JOIN
			(
				SELECT	a.*
				FROM	(
					SELECT	row_number() over(PARTITION BY v1.ItemText ORDER BY v1.Id DESC) vSequence,
							v1.PostingDate,
							v1.AccountingDocNo,
							v1.LineItem AS ItemNo,
							v1.ItemText,
							SUBSTRING(RTRIM(LTRIM(v1.ItemText)), 10, 19) AS TaxInvoiceNumber,
							dbo.fnConvertSapStringToDate(SUBSTRING(LTRIM(RTRIM(V1.ItemText)), 1, 8)) AS TglFaktur,				
							v1.HeaderText AS DocumentHeaderText,
							v1.AssignmentNo AS NPWP,
							v1.Id,
							CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
							v1.StatusReconcile,
							v1.FiscalYear AS FiscalYearDebet,
							v1.GLAccount
					FROM	SAP_MTDownloadPPN v1
					WHERE	v1.IsDeleted = 0
				) a
				WHERE	a.vSequence = 1
						AND a.TglFaktur IS NOT NULL
						AND (a.TaxInvoiceNumber IS NOT NULL OR a.TaxInvoiceNumber = '')
						AND (a.StatusReconcile = 0 OR a.StatusReconcile IS NULL)
						AND (
							(@postingDateStart IS NULL AND @postingDateEnd IS NULL)
							OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NULL 
								AND CAST(a.PostingDate as date) = CAST(@postingDateStart as date))
							OR (@postingDateStart IS NULL AND @postingDateEnd IS NOT NULL 
								AND CAST(a.PostingDate as date) = CAST(@postingDateEnd as date))
							OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NOT NULL 
								AND CAST(a.PostingDate as date) >= CAST(@postingDateStart as date) 
								AND CAST(a.PostingDate as date) <= CAST(@postingDateEnd as date)
								)
						)
						--AND (
						--	(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
						--	OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL 
						--		AND CAST(a.TglFaktur as date) = CAST(@tglFakturStart as date))
						--	OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL 
						--		AND CAST(a.TglFaktur as date) = CAST(@tglFakturEnd as date))
						--	OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL 
						--		AND CAST(a.TglFaktur as date) >= CAST(@tglFakturStart as date) 
						--		AND CAST(a.TglFaktur as date) <= CAST(@tglFakturEnd as date)
						--		)
						--)
						AND (
							(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
							OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL 
								AND a.TaxInvoiceNumber = @noFakturStart)
							OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL 
								AND a.TaxInvoiceNumber = @noFakturEnd)
							OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL 
								AND (a.TaxInvoiceNumber BETWEEN @noFakturStart AND @noFakturEnd)
								)
						)
			) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
		) comp_result
	ORDER BY AccountingDocNo ASC
	OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
	FETCH NEXT @ItemPerPage ROWS ONLY
	OPTION (OPTIMIZE FOR UNKNOWN)

END





GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerateToDownload]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerateToDownload]
	-- Add the parameters for the stored procedure here
	@postingDateStart date
	,@postingDateEnd date
	,@tglFakturStart date
	,@tglFakturEnd date
	,@noFakturStart nvarchar(100) 
	,@noFakturEnd nvarchar(100) 
	,@scanDate date 
	,@userName nvarchar(100) 
	,@masaPajak int 
	,@tahunPajak int 
AS
BEGIN
	DECLARE @diffToleran decimal = (SELECT CAST(ConfigValue AS decimal) AS configToleran FROM GeneralConfig WHERE ConfigKey = 'CompareEvisVsSapToleransiDiff')
	
	SELECT	comp_result.*, CAST(0 AS int) AS Pembetulan
	FROM	(
		SELECT	 CAST(ROW_NUMBER() OVER(ORDER BY sap.AccountingDocNo ASC) as int) AS vSequence
				,CAST(sap.PostingDate as date) AS PostingDate
				,sap.AccountingDocNo
				,sap.ItemNo
				,CAST(sap.TglFaktur as date) AS TglFaktur
				,efis.NamaPenjual AS NamaVendor
				,efis.ScanDate
				,efis.FormatedNoFaktur AS TaxInvoiceNumberEvis
				,sap.TaxInvoiceNumber AS TaxInvoiceNumberSap
				,sap.DocumentHeaderText
				,sap.NPWP
				,efis.AmountEvis AS AmountEvis
				,sap.AmountLocal AS AmountSap
				,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
				,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
				,'' AS Notes
				,COUNT(sap.PostingDate) OVER() AS TotalItems
				,efis.CreatedBy AS UserNameCreator
				,sap.ItemText
				,efis.MasaPajak
				,efis.TahunPajak
				,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
				,sap.GLAccount
				,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus						
		FROM	(
				SELECT	 fp.NoFakturPajak		
						,fp.NPWPPenjual
						,fp.JumlahPPN AS AmountEvis
						,fp.TglFaktur AS EvisTanggalFaktur
						,fp.FormatedNoFaktur
						,fp.Created AS ScanDate
						,fp.NamaPenjual
						,fp.CreatedBy
						,fp.MasaPajak
						,fp.TahunPajak
				FROM	dbo.FakturPajak fp
				WHERE	fp.IsDeleted = 0 
						AND (fp.StatusReconcile IS NULL OR fp.StatusReconcile = 0)
						AND (@scanDate IS NULL OR (@scanDate IS NOT NULL AND CAST(fp.Created as date) = CAST(@scanDate as date)))
						AND (@userName IS NULL OR (@userName IS NOT NULL AND LOWER(fp.CreatedBy) LIKE REPLACE(LOWER(@userName), '*', '%')))
						AND (@masaPajak IS NULL OR (@masaPajak IS NOT NULL AND fp.MasaPajak = @masaPajak))
						AND (@tahunPajak IS NULL OR (@tahunPajak IS NOT NULL AND fp.TahunPajak = @tahunPajak))
						AND (
							(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
							OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL 
								AND CAST(fp.TglFaktur as date) = CAST(@tglFakturStart as date))
							OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL 
								AND CAST(fp.TglFaktur as date) = CAST(@tglFakturEnd as date))
							OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL 
								AND CAST(fp.TglFaktur as date) >= CAST(@tglFakturStart as date) 
								AND CAST(fp.TglFaktur as date) <= CAST(@tglFakturEnd as date)
								)
						)
						AND (
							(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
							OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL 
								AND fp.FormatedNoFaktur = @noFakturStart)
							OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL 
								AND fp.FormatedNoFaktur = @noFakturEnd)
							OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL 
								AND (fp.FormatedNoFaktur BETWEEN @noFakturStart AND @noFakturEnd)
								)
						)
			) efis FULL OUTER JOIN
			(
				SELECT	a.*
				FROM	(
					SELECT	row_number() over(PARTITION BY v1.ItemText ORDER BY v1.Id DESC) vSequence,
							v1.PostingDate,
							v1.AccountingDocNo,
							v1.LineItem AS ItemNo,
							v1.ItemText,
							SUBSTRING(RTRIM(LTRIM(v1.ItemText)), 10, 19) AS TaxInvoiceNumber,
							dbo.fnConvertSapStringToDate(SUBSTRING(LTRIM(RTRIM(V1.ItemText)), 1, 8)) AS TglFaktur,				
							v1.HeaderText AS DocumentHeaderText,
							v1.AssignmentNo AS NPWP,
							v1.Id,
							CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
							v1.StatusReconcile,
							v1.FiscalYear AS FiscalYearDebet,
							v1.GLAccount
					FROM	SAP_MTDownloadPPN v1
					WHERE	v1.IsDeleted = 0
				) a
				WHERE	a.vSequence = 1
						AND a.TglFaktur IS NOT NULL
						AND (a.TaxInvoiceNumber IS NOT NULL OR a.TaxInvoiceNumber = '')
						AND (a.StatusReconcile = 0 OR a.StatusReconcile IS NULL)
						AND (
							(@postingDateStart IS NULL AND @postingDateEnd IS NULL)
							OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NULL 
								AND CAST(a.PostingDate as date) = CAST(@postingDateStart as date))
							OR (@postingDateStart IS NULL AND @postingDateEnd IS NOT NULL 
								AND CAST(a.PostingDate as date) = CAST(@postingDateEnd as date))
							OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NOT NULL 
								AND CAST(a.PostingDate as date) >= CAST(@postingDateStart as date) 
								AND CAST(a.PostingDate as date) <= CAST(@postingDateEnd as date)
								)
						)
						AND (
							(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
							OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL 
								AND CAST(a.TglFaktur as date) = CAST(@tglFakturStart as date))
							OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL 
								AND CAST(a.TglFaktur as date) = CAST(@tglFakturEnd as date))
							OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL 
								AND CAST(a.TglFaktur as date) >= CAST(@tglFakturStart as date) 
								AND CAST(a.TglFaktur as date) <= CAST(@tglFakturEnd as date)
								)
						)
						AND (
							(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
							OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL 
								AND a.TaxInvoiceNumber = @noFakturStart)
							OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL 
								AND a.TaxInvoiceNumber = @noFakturEnd)
							OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL 
								AND (a.TaxInvoiceNumber BETWEEN @noFakturStart AND @noFakturEnd)
								)
						)
			) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
		) comp_result
	ORDER BY AccountingDocNo ASC
	OPTION (OPTIMIZE FOR UNKNOWN)
END





GO
/****** Object:  StoredProcedure [dbo].[sp_CreateFillingIndex]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CreateFillingIndex]
	-- Add the parameters for the stored procedure here
	@fakturPajakIds nvarchar(max),
	@userModify nvarchar(50)
AS
BEGIN

	DECLARE @tempTable TABLE
	(
		RowIdx int identity(1,1),
		FakturPajakId bigint,
		TahunPajak int,
		MasaPajak int,
		CreatedBy nvarchar(50),
		UserInitial nvarchar(20)
	)
	
	INSERT INTO @tempTable(FakturPajakId, TahunPajak, MasaPajak, CreatedBy, UserInitial)
	SELECT	fp.FakturPajakId, fp.TahunPajak, fp.MasaPajak, fp.CreatedBy, u.UserInitial
	FROM	dbo.FakturPajak fp INNER JOIN
			dbo.[User] u ON fp.CreatedBy = u.UserName
	WHERE	fp.IsDeleted = 0 AND u.IsDeleted = 0 AND fp.FillingIndex IS NULL AND FakturPajakId IN (
			SELECT	CAST(RTRIM(LTRIM(Data)) as bigint) AS FPId
			FROM	dbo.Split(@fakturPajakIds)
	)

	DECLARE @currIdx int = 1
	DECLARE @maxIdx int = 0
	DECLARE @generatedFillingIndex nvarchar(20) = ''

	DECLARE @currTahunPajak int
	DECLARE @currMasaPajak int
	DECLARE @currUserInitial nvarchar(20)
	DECLARE @currFakturPajakId bigint

	SET @maxIdx = (SELECT MAX(RowIdx) FROM @tempTable)
	WHILE @currIdx <= @maxIdx
	BEGIN
		
		SELECT	@currMasaPajak = MasaPajak, 
				@currTahunPajak = TahunPajak, 
				@currUserInitial = UserInitial,
				@currFakturPajakId = FakturPajakId
		FROM	@tempTable
		WHERE	RowIdx = @currIdx

		SET @generatedFillingIndex = dbo.fnGenerateFillingIndex(@currTahunPajak, @currMasaPajak, @currUserInitial)

		UPDATE	dbo.FakturPajak
		SET		FillingIndex = @generatedFillingIndex
				, Modified = GETDATE()
				, ModifiedBy = @userModify
		WHERE	FakturPajakId = @currFakturPajakId

		SET @currIdx = @currIdx + 1
	END
	
END




GO
/****** Object:  StoredProcedure [dbo].[sp_MTDownloadPPN_ProcessXML]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_MTDownloadPPN_ProcessXML]
	-- Add the parameters for the stored procedure here
	@paramTable SAP_MTDownloadPPN_ParamTable READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	BEGIN TRY

		MERGE INTO dbo.SAP_MTDownloadPPN dest USING
		(
			SELECT	[CompanyCode]
				   ,[AccountingDocNo]
				   ,[FiscalYear]
				   ,[DocType]
				   ,[PostingDate]
				   ,[AmountLocal]
				   ,[LineItem]
				   ,[Reference]
				   ,[HeaderText]
				   ,[UserName]
				   ,[ItemText]
				   ,[AssignmentNo]
				   ,[BusinessArea]
				   ,[GLAccount]
				   ,[ReferenceLineItem]
				   ,[DocDate]
				   ,[Currency]
				   ,[SalesTaxCode]
				   ,[AmountDocCurrency]
				   ,[PostingKey]
				   ,[ClearingDoc]
				   ,[ClearingDate]
				   ,[ReverseDocNo]
				   ,[ReverseDocFiscalYear]
				   ,[CreatedBy]
				   ,[ModifiedBy]
				   ,[ReferenceKey1]
				   ,[ReferenceKey2]
			FROM	@paramTable
		) src ON CAST(src.PostingDate as date) = CAST(dest.PostingDate as date) 
				AND src.LineItem = dest.LineItem AND src.AccountingDocNo = dest.AccountingDocNo
		WHEN MATCHED AND ((dest.StatusReconcile IS NULL OR dest.StatusReconcile = 0) OR dbo.fnIsReconcileByItemText(src.ItemText) = 0) THEN
			UPDATE
			SET	   [CompanyCode] = RTRIM(LTRIM(src.CompanyCode))
				  ,[AccountingDocNo] = RTRIM(LTRIM(src.AccountingDocNo))
				  ,[FiscalYear] = RTRIM(LTRIM(src.FiscalYear))
				  ,[DocType] = RTRIM(LTRIM(src.DocType))
				  ,[PostingDate] = RTRIM(LTRIM(src.PostingDate))
				  ,[AmountLocal] = RTRIM(LTRIM(src.AmountLocal))
				  ,[LineItem] = RTRIM(LTRIM(src.LineItem))
				  ,[Reference] = RTRIM(LTRIM(src.Reference))
				  ,[HeaderText] = RTRIM(LTRIM(src.HeaderText))
				  ,[UserName] = RTRIM(LTRIM(src.UserName))
				  ,[ItemText] = RTRIM(LTRIM(src.ItemText))
				  ,[AssignmentNo] = RTRIM(LTRIM(src.AssignmentNo))
				  ,[BusinessArea] = RTRIM(LTRIM(src.BusinessArea))
				  ,[GLAccount] = RTRIM(LTRIM(src.GLAccount))
				  ,[ReferenceLineItem] = RTRIM(LTRIM(src.ReferenceLineItem))
				  ,[DocDate] = RTRIM(LTRIM(src.DocDate))
				  ,[Currency] = RTRIM(LTRIM(src.Currency))
				  ,[SalesTaxCode] = RTRIM(LTRIM(src.SalesTaxCode))
				  ,[AmountDocCurrency] = RTRIM(LTRIM(src.AmountDocCurrency))
				  ,[PostingKey] = RTRIM(LTRIM(src.PostingKey))
				  ,[ClearingDoc] = RTRIM(LTRIM(src.ClearingDoc))
				  ,[ClearingDate] = RTRIM(LTRIM(src.ClearingDate))
				  ,[ReverseDocNo] = RTRIM(LTRIM(src.ReverseDocNo))
				  ,[ReverseDocFiscalYear] = RTRIM(LTRIM(src.ReverseDocFiscalYear))
				  ,[Modified] = GETDATE()
				  ,[ModifiedBy] = RTRIM(LTRIM(src.ModifiedBy))
				  ,[ReferenceKey1] = RTRIM(LTRIM(src.ReferenceKey1))
				  ,[ReferenceKey2] = RTRIM(LTRIM(src.ReferenceKey2))
		WHEN NOT MATCHED THEN
			INSERT ([CompanyCode]
				   ,[AccountingDocNo]
				   ,[FiscalYear]
				   ,[DocType]
				   ,[PostingDate]
				   ,[AmountLocal]
				   ,[LineItem]
				   ,[Reference]
				   ,[HeaderText]
				   ,[UserName]
				   ,[ItemText]
				   ,[AssignmentNo]
				   ,[BusinessArea]
				   ,[GLAccount]
				   ,[ReferenceLineItem]
				   ,[DocDate]
				   ,[Currency]
				   ,[SalesTaxCode]
				   ,[AmountDocCurrency]
				   ,[PostingKey]
				   ,[ClearingDoc]
				   ,[ClearingDate]
				   ,[ReverseDocNo]
				   ,[ReverseDocFiscalYear]
				   ,[CreatedBy]
				   ,[ReferenceKey1]
				   ,[ReferenceKey2])
			VALUES (
					RTRIM(LTRIM(src.[CompanyCode]))
				   ,RTRIM(LTRIM(src.[AccountingDocNo]))
				   ,RTRIM(LTRIM(src.[FiscalYear]))
				   ,RTRIM(LTRIM(src.[DocType]))
				   ,RTRIM(LTRIM(src.[PostingDate]))
				   ,RTRIM(LTRIM(src.[AmountLocal]))
				   ,RTRIM(LTRIM(src.[LineItem]))
				   ,RTRIM(LTRIM(src.[Reference]))
				   ,RTRIM(LTRIM(src.[HeaderText]))
				   ,RTRIM(LTRIM(src.[UserName]))
				   ,RTRIM(LTRIM(src.[ItemText]))
				   ,RTRIM(LTRIM(src.[AssignmentNo]))
				   ,RTRIM(LTRIM(src.[BusinessArea]))
				   ,RTRIM(LTRIM(src.[GLAccount]))
				   ,RTRIM(LTRIM(src.[ReferenceLineItem]))
				   ,RTRIM(LTRIM(src.[DocDate]))
				   ,RTRIM(LTRIM(src.[Currency]))
				   ,RTRIM(LTRIM(src.[SalesTaxCode]))
				   ,RTRIM(LTRIM(src.[AmountDocCurrency]))
				   ,RTRIM(LTRIM(src.[PostingKey]))
				   ,RTRIM(LTRIM(src.[ClearingDoc]))
				   ,RTRIM(LTRIM(src.[ClearingDate]))
				   ,RTRIM(LTRIM(src.[ReverseDocNo]))
				   ,RTRIM(LTRIM(src.[ReverseDocFiscalYear]))
				   ,RTRIM(LTRIM(src.[CreatedBy]))
				   ,RTRIM(LTRIM(src.[ReferenceKey1]))
				   ,RTRIM(LTRIM(src.[ReferenceKey2]))
					);

	END TRY

	BEGIN CATCH

		exec usp_InsertErrorLog

	END CATCH
END








GO
/****** Object:  StoredProcedure [dbo].[sp_PajakMasukanExportCsv]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_PajakMasukanExportCsv]
	-- Add the parameters for the stored procedure here
	@noFaktur1 nvarchar(100)
	,@noFaktur2 nvarchar(100)
	,@tglFakturStart date
	,@tglFakturEnd date
	,@npwpVendor nvarchar(100)
	,@namaVendor nvarchar(100)
	,@masaPajak int
	,@tahunPajak int
	,@status nvarchar(100)
	,@sFormatedNpwpPenjual nvarchar(100)
	,@sNamaPenjual nvarchar(100)
	,@sFormatedNoFaktur nvarchar(100)
	,@sTglFakturString nvarchar(100)
	,@sMasaPajakName nvarchar(100)
	,@sTahunPajak nvarchar(100)
	,@sDppString nvarchar(100)
	,@sPpnString nvarchar(100)
	,@sPpnBmString nvarchar(100)
	,@sStatusFaktur nvarchar(100)
	,@scanDateAwal date
	,@scanDateAkhir date
	,@fillingIndex int 
	,@dataType int
	,@sFillingIndex nvarchar(100)
	,@sUserName nvarchar(100)
AS
BEGIN

	DECLARE
		@headerTemplate nvarchar(max)

	DECLARE
	@tempTable TABLE (
		FakturPajakId varchar(100),
		KD_JENIS_TRANSAKSI varchar(2),
		FG_PENGGANTI varchar(1),
		NOMOR_FAKTUR varchar(100),
		MASA_PAJAK varchar(1),
		TAHUN_PAJAK varchar(4),
		TANGGAL_FAKTUR varchar(25),
		NPWP varchar(255),
		NAMA varchar(255),
		ALAMAT_LENGKAP varchar(255),
		JUMLAH_DPP varchar(100),
		JUMLAH_PPN varchar(100),
		JUMLAH_PPNBM varchar(100),
		IS_CREDITABLE varchar(10),
		Created datetime
	)

	DECLARE
	@tempTableKhusus TABLE (
		DK_DM varchar(2),
		FakturPajakId varchar(100),
		JENIS_TRANSAKSI varchar(2),
		JENIS_DOKUMEN varchar(2),
		KD_JNS_TRANSAKSI varchar(2),
		FG_PENGGANTI varchar(1),
		NOMOR_DOK_LAIN_GANTI varchar(255),
		NOMOR_DOK_LAIN varchar(255),
		TANGGAL_DOK_LAIN varchar(25),
		MASA_PAJAK varchar(2),
		TAHUN_PAJAK varchar(4),
		NPWP varchar(255),
		NAMA varchar(255),
		ALAMAT_LENGKAP varchar(255),
		JUMLAH_DPP varchar(255),
		JUMLAH_PPN varchar(255),
		JUMLAH_PPNBM varchar(255),
		KETERANGAN varchar(255),
		CreatedDate date
	)

	IF @dataType = 1 OR @dataType = 2
	BEGIN
		SET @headerTemplate = 'RM,KD_JENIS_TRANSAKSI,FG_PENGGANTI,NOMOR_FAKTUR,MASA_PAJAK,TAHUN_PAJAK,TANGGAL_FAKTUR,NPWP,NAMA,ALAMAT_LENGKAP,JUMLAH_DPP,JUMLAH_PPN,JUMLAH_PPNBM,IS_CREDITABLE'

		
		INSERT INTO @tempTable(FakturPajakId, KD_JENIS_TRANSAKSI, FG_PENGGANTI, NOMOR_FAKTUR, MASA_PAJAK, TAHUN_PAJAK, TANGGAL_FAKTUR, NPWP, NAMA, ALAMAT_LENGKAP, JUMLAH_DPP, JUMLAH_PPN, JUMLAH_PPNBM, IS_CREDITABLE, Created)
		SELECT	CONVERT(VARCHAR(100),fp.FakturPajakId) as FakturPajakId,
					CONVERT(VARCHAR(2),fp.KdJenisTransaksi) AS KD_JENIS_TRANSAKSI,
					CONVERT(VARCHAR(1), fp.FgPengganti) AS FG_PENGGANTI,
					CONVERT(VARCHAR(100), LEFT(fp.NoFakturPajak, 13)) AS NOMOR_FAKTUR,
					CONVERT(VARCHAR(1),fp.MasaPajak) AS MASA_PAJAK,
					CONVERT(VARCHAR(4),fp.TahunPajak) AS TAHUN_PAJAK,
					CONVERT(VARCHAR(25),fp.TglFaktur,103) AS TANGGAL_FAKTUR,
					CONVERT(VARCHAR(255),fp.NPWPPenjual) AS NPWP,		
					REPLACE(CONVERT(VARCHAR(255),fp.NamaPenjual), ',', ' ') AS NAMA,
					REPLACE(CONVERT(VARCHAR(255),fp.AlamatPenjual), ',', ' ') AS ALAMAT_LENGKAP,
					CONVERT(VARCHAR(100), CONVERT(bigint,FLOOR(ISNULL(fp.JumlahDPP,0)))) AS JUMLAH_DPP,
					CONVERT(VARCHAR(100), CONVERT(bigint,FLOOR(fp.JumlahPPN)))  AS JUMLAH_PPN,
					CONVERT(VARCHAR(100), CONVERT(bigint,FLOOR(fp.JumlahPPNBM))) AS JUMLAH_PPNBM,
					CONVERT(VARCHAR(10),fp.Dikreditkan) AS IS_CREDITABLE,
					fp.Created
		FROM	FakturPajak fp INNER JOIN
				dbo.GetMonth() m ON fp.MasaPajak = m.MonthNumber
		WHERE fp.[IsDeleted] = 0
				AND [Status] = 2
				AND [FPType] <> 3
				AND [FPType] =  @dataType
				AND (
					(@scanDateAwal IS NULL AND @scanDateAkhir IS NULL)
					OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NULL AND CAST([Created] AS date) = CAST(@scanDateAwal AS date))
					OR (@scanDateAwal IS NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) = CAST(@scanDateAkhir AS date))
					OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) >= CAST(@scanDateAwal AS date) AND CAST([Created] AS date) <= CAST(@scanDateAkhir AS date))
				)
				AND ( @Status IS NULL
						OR (@status IS NOT NULL AND @status = 0 AND LOWER(StatusApproval) <> 'faktur valid, sudah diapprove oleh djp')
						OR (@status IS NOT NULL AND @status = 1 AND LOWER(StatusApproval) = 'faktur valid, sudah diapprove oleh djp')
					)
				AND (@fillingIndex IS NULL OR (@fillingIndex IS NOT NULL AND ((@fillingIndex = 0 AND FillingIndex IS NULL) OR (@fillingIndex = 1 AND FillingIndex IS NOT NULL))))
				AND (
					(@noFaktur1 IS NULL AND @noFaktur2 IS NULL)
					OR (@noFaktur1 IS NULL AND @noFaktur2 IS NOT NULL 
					AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@noFaktur2, '-',''),'.',''), '*','%'))
					OR (@noFaktur1 IS NOT NULL AND @noFaktur2 IS NULL AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@noFaktur1, '-',''),'.',''), '*', '%'))
					OR (@noFaktur1 IS NOT NULL AND @noFaktur2 IS NOT NULL 
					AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') >= REPLACE(REPLACE(@noFaktur1, '-',''),'.','') 
					AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') <= REPLACE(REPLACE(@noFaktur2, '-',''),'.',''))
				)
				AND (@NpwpVendor IS NULL OR
					(@NpwpVendor IS NOT NULL AND LOWER(REPLACE(REPLACE([FormatedNpwpPenjual], '.', ''), '-', '')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NpwpVendor, '.',''),'-','')), '*', '%'))
					)
				AND (
					(@NamaVendor IS NOT NULL AND LOWER([NamaPenjual]) LIKE REPLACE(LOWER(@NamaVendor), '*', '%'))
					OR @NamaVendor IS NULL
					)
				AND ((@MasaPajak IS NOT NULL AND [MasaPajak] = @MasaPajak) OR @MasaPajak IS NULL)
				AND ((@TahunPajak IS NOT NULL AND [TahunPajak] = @TahunPajak) OR @TahunPajak IS NULL)
				AND ((
						CAST([TglFaktur] AS DATE) BETWEEN CAST(ISNULL(@TglFakturStart, [TglFaktur]) AS DATE) AND CAST(ISNULL(@TglFakturEnd, [TglFaktur]) AS DATE)
					) OR @TglFakturStart IS NULL OR @TglFakturEnd IS NULL)
				AND (@sFormatedNpwpPenjual IS NULL OR (@sFormatedNpwpPenjual IS NOT NULL AND FormatedNpwpPenjual LIKE REPLACE(@sFormatedNpwpPenjual,'*','%')))
					AND (@sNamaPenjual IS NULL OR (@sNamaPenjual IS NOT NULL AND NamaPenjual LIKE REPLACE(@sNamaPenjual,'*','%')))
					AND (@sFormatedNoFaktur IS NULL OR (@sFormatedNoFaktur IS NOT NULL AND FormatedNoFaktur LIKE REPLACE(@sFormatedNoFaktur,'*','%')))
					AND (@sTglFakturString IS NULL OR (@sTglFakturString IS NOT NULL AND CONVERT(VARCHAR,TglFaktur, 103) LIKE REPLACE(@sTglFakturString,'*','%')))
					AND (@sMasaPajakName IS NULL OR (@sMasaPajakName IS NOT NULL AND m.MonthName LIKE REPLACE(@sMasaPajakName, '*', '%')))
					AND (@sTahunPajak IS NULL OR (@sTahunPajak IS NOT NULL AND CAST(TahunPajak AS nvarchar) LIKE REPLACE(@sTahunPajak, '*', '%')))
					AND (@sDPPString IS NULL OR (@sDPPString IS NOT NULL AND CAST(JumlahDPP AS nvarchar) LIKE REPLACE(@sDPPString, '*','%')))
					AND (@sPPNString IS NULL OR (@sPPNString IS NOT NULL AND CAST(JumlahPPN AS nvarchar) LIKE REPLACE(@sPPNString, '*', '%')))
					AND (@sPPNBMString IS NULL OR (@sPPNBMString IS NOT NULL AND CAST(JumlahPPNBM AS nvarchar) LIKE REPLACE(@sPPNBMString, '*', '%')))
					AND (@sStatusFaktur IS NULL OR (@sStatusFaktur IS NOT NULL AND StatusFaktur LIKE REPLACE(@sStatusFaktur,'*','%')))
					AND (@sFillingIndex IS NULL OR (@sFillingIndex IS NOT NULL AND FillingIndex LIKE REPLACE(@sFillingIndex,'*','%')))
					AND (@sUserName IS NULL OR (@sUserName IS NOT NULL AND CreatedBy LIKE REPLACE(@sUserName,'*','%')))
		OPTION (OPTIMIZE FOR UNKNOWN)

		--untuk penamaan file di code
		--SET @scanDateStart = (SELECT TOP 1 Created
		--FROM	@tempTable
		--ORDER BY Created ASC)

		----untuk penamaan file di code
		--SET @scanDateEnd =  (SELECT TOP 1 Created
		--FROM	@tempTable
		--ORDER BY Created DESC)

	END
	ELSE
	BEGIN
		IF @dataType = 3
		BEGIN
			SET @headerTemplate = 'DK_DM,JENIS_TRANSAKSI,JENIS_DOKUMEN,KD_JNS_TRANSAKSI,FG_PENGGANTI,NOMOR_DOK_LAIN_GANTI,NOMOR_DOK_LAIN,TANGGAL_DOK_LAIN,MASA_PAJAK,TAHUN_PAJAK,NPWP,NAMA,ALAMAT_LENGKAP,JUMLAH_DPP,JUMLAH_PPN,JUMLAH_PPNBM,KETERANGAN'
			INSERT INTO @tempTableKhusus(DK_DM, 
					FakturPajakId,
					JENIS_TRANSAKSI,
					JENIS_DOKUMEN,
					KD_JNS_TRANSAKSI,
					FG_PENGGANTI,
					NOMOR_DOK_LAIN_GANTI,
					NOMOR_DOK_LAIN,
					TANGGAL_DOK_LAIN,
					MASA_PAJAK,
					TAHUN_PAJAK,
					NPWP,
					NAMA,
					ALAMAT_LENGKAP,
					JUMLAH_DPP,
					JUMLAH_PPN,
					JUMLAH_PPNBM,
					KETERANGAN,
					CreatedDate)
			SELECT	'DM' AS DK_DM,
					fp.FakturPajakId,
					fp.JenisTransaksi AS JENIS_TRANSAKSI,
					fp.JenisDokumen AS JENIS_DOKUMEN,
					fp.KdJenisTransaksi AS KD_JNS_TRANSAKSI,
					fp.FgPengganti AS FG_PENGGANTI,
					CASE WHEN fp.NoFakturYangDiganti IS NULL THEN '' ELSE fp.NoFakturYangDiganti END AS NOMOR_DOK_LAIN_GANTI,
					REPLACE(REPLACE(fp.FormatedNoFaktur, '-',''), '.','') AS NOMOR_DOK_LAIN,
					CONVERT(VARCHAR(25),fp.TglFaktur,103) AS TANGGAL_DOK_LAIN,
					CONVERT(VARCHAR(1),fp.MasaPajak) AS MASA_PAJAK,
					CONVERT(VARCHAR(4),fp.TahunPajak) AS TAHUN_PAJAK,
	 				CASE WHEN fp.JenisTransaksi = '1' THEN '' ELSE fp.NPWPPenjual END AS NPWP,
					CASE WHEN fp.JenisTransaksi = '1' THEN '' ELSE REPLACE(CONVERT(VARCHAR(255),fp.NamaPenjual), ',', ' ') END AS NAMA,
					CASE WHEN fp.JenisTransaksi = '1' THEN '' ELSE REPLACE(CONVERT(VARCHAR(255),fp.AlamatPenjual), ',', ' ') END AS ALAMAT_LENGKAP,
					CONVERT(VARCHAR(100), CONVERT(bigint,FLOOR(ISNULL(fp.JumlahDPP,0)))) AS JUMLAH_DPP,
					CONVERT(VARCHAR(100), CONVERT(bigint,FLOOR(fp.JumlahPPN)))  AS JUMLAH_PPN,
					CONVERT(VARCHAR(100), CONVERT(bigint,FLOOR(fp.JumlahPPNBM))) AS JUMLAH_PPNBM,
					CASE WHEN fp.Pesan IS NULL THEN '' ELSE fp.Pesan END AS KETERANGAN,
					fp.Created
			FROM	FakturPajak fp INNER JOIN
					dbo.GetMonth() m ON fp.MasaPajak = m.MonthNumber
			WHERE	fp.[IsDeleted] = 0
					AND [Status] = 2
					AND [FPType] = 3
					AND (
						(@scanDateAwal IS NULL AND @scanDateAkhir IS NULL)
						OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NULL AND CAST([Created] AS date) = CAST(@scanDateAwal AS date))
						OR (@scanDateAwal IS NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) = CAST(@scanDateAkhir AS date))
						OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) >= CAST(@scanDateAwal AS date) AND CAST([Created] AS date) <= CAST(@scanDateAkhir AS date))
					)
					AND (@fillingIndex IS NULL OR (@fillingIndex IS NOT NULL AND ((@fillingIndex = 0 AND FillingIndex IS NULL) OR (@fillingIndex = 1 AND FillingIndex IS NOT NULL))))
					AND (
						(@noFaktur1 IS NULL AND @noFaktur2 IS NULL)
						OR (@noFaktur1 IS NULL AND @noFaktur2 IS NOT NULL 
						AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@noFaktur2, '-',''),'.',''), '*','%'))
						OR (@noFaktur1 IS NOT NULL AND @noFaktur2 IS NULL AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@noFaktur1, '-',''),'.',''), '*', '%'))
						OR (@noFaktur1 IS NOT NULL AND @noFaktur2 IS NOT NULL 
						AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') >= REPLACE(REPLACE(@noFaktur1, '-',''),'.','') 
						AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') <= REPLACE(REPLACE(@noFaktur2, '-',''),'.',''))
					)
					AND (@NpwpVendor IS NULL OR
						(@NpwpVendor IS NOT NULL AND LOWER(REPLACE(REPLACE([FormatedNpwpPenjual], '.', ''), '-', '')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NpwpVendor, '.',''),'-','')), '*', '%'))
						)
					AND (
						(@NamaVendor IS NOT NULL AND LOWER([NamaPenjual]) LIKE REPLACE(LOWER(@NamaVendor), '*', '%'))
						OR @NamaVendor IS NULL
						)
					AND ((@MasaPajak IS NOT NULL AND [MasaPajak] = @MasaPajak) OR @MasaPajak IS NULL)
					AND ((@TahunPajak IS NOT NULL AND [TahunPajak] = @TahunPajak) OR @TahunPajak IS NULL)
					AND ((
							CAST([TglFaktur] AS DATE) BETWEEN CAST(ISNULL(@TglFakturStart, [TglFaktur]) AS DATE) AND CAST(ISNULL(@TglFakturEnd, [TglFaktur]) AS DATE)
						) OR @TglFakturStart IS NULL OR @TglFakturEnd IS NULL)
					AND (@sFormatedNpwpPenjual IS NULL OR (@sFormatedNpwpPenjual IS NOT NULL AND FormatedNpwpPenjual LIKE REPLACE(@sFormatedNpwpPenjual,'*','%')))
					AND (@sNamaPenjual IS NULL OR (@sNamaPenjual IS NOT NULL AND NamaPenjual LIKE REPLACE(@sNamaPenjual,'*','%')))
					AND (@sFormatedNoFaktur IS NULL OR (@sFormatedNoFaktur IS NOT NULL AND FormatedNoFaktur LIKE REPLACE(@sFormatedNoFaktur,'*','%')))
					AND (@sTglFakturString IS NULL OR (@sTglFakturString IS NOT NULL AND CONVERT(VARCHAR,TglFaktur, 103) LIKE REPLACE(@sTglFakturString,'*','%')))
					AND (@sMasaPajakName IS NULL OR (@sMasaPajakName IS NOT NULL AND m.MonthName LIKE REPLACE(@sMasaPajakName, '*', '%')))
					AND (@sTahunPajak IS NULL OR (@sTahunPajak IS NOT NULL AND CAST(TahunPajak AS nvarchar) LIKE REPLACE(@sTahunPajak, '*', '%')))
					AND (@sDPPString IS NULL OR (@sDPPString IS NOT NULL AND CAST(JumlahDPP AS nvarchar) LIKE REPLACE(@sDPPString, '*','%')))
					AND (@sPPNString IS NULL OR (@sPPNString IS NOT NULL AND CAST(JumlahPPN AS nvarchar) LIKE REPLACE(@sPPNString, '*', '%')))
					AND (@sPPNBMString IS NULL OR (@sPPNBMString IS NOT NULL AND CAST(JumlahPPNBM AS nvarchar) LIKE REPLACE(@sPPNBMString, '*', '%')))
					AND (@sStatusFaktur IS NULL OR (@sStatusFaktur IS NOT NULL AND StatusFaktur LIKE REPLACE(@sStatusFaktur,'*','%')))
					AND (@sFillingIndex IS NULL OR (@sFillingIndex IS NOT NULL AND FillingIndex LIKE REPLACE(@sFillingIndex,'*','%')))
					AND (@sUserName IS NULL OR (@sUserName IS NOT NULL AND CreatedBy LIKE REPLACE(@sUserName,'*','%')))
			OPTION (OPTIMIZE FOR UNKNOWN)

			--untuk penamaan file di code
			--SET @scanDateStart = (SELECT TOP 1 CreatedDate
			--FROM	@tempTableKhusus
			--ORDER BY CreatedDate ASC)

			----untuk penamaan file di code
			--SET @scanDateEnd =  (SELECT TOP 1 CreatedDate
			--FROM	@tempTableKhusus
			--ORDER BY CreatedDate DESC)
		END
	END

	IF @dataType = 1 OR @dataType = 2
	BEGIN
		-- FP K2 & NON-K2
		SELECT @headerTemplate AS RowData, CAST(@scanDateAwal as nvarchar(100)) + ';' +  CAST(@scanDateAkhir as nvarchar(100)) AS RowData2, '' AS RowData3, 'A' AS Marker
		UNION ALL
		SELECT	('FM' 
				+','+ KD_JENIS_TRANSAKSI
				+','+ FG_PENGGANTI
				+','+ NOMOR_FAKTUR
				+','+ MASA_PAJAK
				+','+ TAHUN_PAJAK
				+','+ TANGGAL_FAKTUR
				+','+ NPWP
				+','+ NAMA
				+','+ ALAMAT_LENGKAP
				+','+ JUMLAH_DPP
				+','+ JUMLAH_PPN
				+','+ JUMLAH_PPNBM
				+','+ IS_CREDITABLE)  as RowData,
				('') as RowData2,(FakturPajakId) as RowData3, 
				'B' as Marker
		FROM	@tempTable
	END
	ELSE
	BEGIN
		IF @dataType = 3
		BEGIN
			SELECT @headerTemplate AS RowData, CAST(@scanDateAwal as nvarchar(100)) + ';' +  CAST(@scanDateAkhir as nvarchar(100)) AS RowData2, '' AS RowData3, 'A' AS Marker
			UNION ALL
			SELECT (DK_DM + ',' +
				JENIS_TRANSAKSI + ',' +
				JENIS_DOKUMEN + ',' +
				KD_JNS_TRANSAKSI + ',' +
				FG_PENGGANTI + ',' +
				NOMOR_DOK_LAIN_GANTI + ',' +
				NOMOR_DOK_LAIN + ',' +
				TANGGAL_DOK_LAIN + ',' +
				MASA_PAJAK + ',' +
				TAHUN_PAJAK + ',' +
				NPWP + ',' +
				NAMA + ',' +
				ALAMAT_LENGKAP + ',' +
				JUMLAH_DPP + ',' +
				JUMLAH_PPN + ',' +
				JUMLAH_PPNBM + ',' +
				KETERANGAN) AS RowData,
				('') as RowData2,(FakturPajakId) as RowData3, 
				'B' as Marker
			FROM @tempTableKhusus
		END
	END


END



GO
/****** Object:  StoredProcedure [dbo].[sp_ReportDetailFakturPajak_GetList]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportDetailFakturPajak_GetList]
	-- Add the parameters for the stored procedure here
	@noFaktur1 varchar(max)
	,@noFaktur2 varchar(max)
	,@Search varchar(max)
	,@CurrentPage int
	,@ItemPerPage int
	,@SortColumnName varchar(max)
	,@sortOrder varchar(5) -- ASC or DESC

AS
BEGIN
	with n_dat as (
			SELECT v.*, COUNT(v.FakturPajakDetailId) OVER() AS TotalItems
			FROM	(
				SELECT	CAST(CASE WHEN LOWER(@sortOrder) = 'asc' THEN
								ROW_NUMBER() OVER(ORDER BY 
								CASE WHEN @SortColumnName = 'FormatedNoFaktur' THEN inDat.FormatedNoFaktur
								WHEN @SortColumnName = 'FakturPajakId' THEN cast(inDat.FakturPajakId as sql_variant)
								WHEN @SortColumnName = 'HargaSatuan' THEN cast(inDat.HargaSatuan as sql_variant)
								WHEN @SortColumnName = 'Nama' THEN cast(cast(Nama as varchar) as sql_variant)
								WHEN @SortColumnName = 'Diskon' THEN cast(Diskon as sql_variant)
								WHEN @SortColumnName = 'JumlahDPP' THEN cast(JumlahDPP as sql_variant)
								WHEN @SortColumnName = 'JumlahPPN' THEN cast(JumlahPPN as sql_variant)
								WHEN @SortColumnName = 'JumlahPPNBM' THEN cast(JumlahPPNBM as sql_variant)
								WHEN @SortColumnName = 'Dpp' THEN cast(Dpp as sql_variant)
								WHEN @SortColumnName = 'Ppn' THEN cast(Ppn as sql_variant)
								WHEN @SortColumnName = 'Ppnbm' THEN cast(Ppnbm as sql_variant)
								WHEN @SortColumnName = 'TarifPpnbm' THEN cast(TarifPpnbm as sql_variant)
								WHEN @SortColumnName = 'FormatedNpwpPenjual' THEN cast(cast(FormatedNpwpPenjual as varchar) as sql_variant)
								WHEN @SortColumnName = 'NamaPenjual' THEN cast(cast(FormatedNpwpPenjual as varchar) as sql_variant)
								WHEN @SortColumnName = '' THEN cast(cast(FormatedNoFaktur as varchar) as sql_variant)
								END ASC)
							ELSE
								ROW_NUMBER() OVER(ORDER BY 
								CASE WHEN @SortColumnName = 'FormatedNoFaktur' THEN inDat.FormatedNoFaktur
								WHEN @SortColumnName = 'FakturPajakId' THEN cast(inDat.FakturPajakId as sql_variant)
								WHEN @SortColumnName = 'Nama' THEN cast(cast(Nama as varchar) as sql_variant)
								WHEN @SortColumnName = 'HargaSatuan' THEN cast(inDat.HargaSatuan as sql_variant)
								WHEN @SortColumnName = 'Diskon' THEN cast(Diskon as sql_variant)
								WHEN @SortColumnName = 'JumlahDPP' THEN cast(JumlahDPP as sql_variant)
								WHEN @SortColumnName = 'JumlahPPN' THEN cast(JumlahPPN as sql_variant)
								WHEN @SortColumnName = 'JumlahPPNBM' THEN cast(JumlahPPNBM as sql_variant)
								WHEN @SortColumnName = 'Dpp' THEN cast(Dpp as sql_variant)
								WHEN @SortColumnName = 'Ppn' THEN cast(Ppn as sql_variant)
								WHEN @SortColumnName = 'Ppnbm' THEN cast(Ppnbm as sql_variant)
								WHEN @SortColumnName = 'TarifPpnbm' THEN cast(TarifPpnbm as sql_variant)
								WHEN @SortColumnName = 'FormatedNpwpPenjual' THEN cast(cast(FormatedNpwpPenjual as varchar) as sql_variant)
								WHEN @SortColumnName = 'NamaPenjual' THEN cast(cast(FormatedNpwpPenjual as varchar) as sql_variant)
								WHEN @SortColumnName = '' THEN cast(cast(FormatedNoFaktur as varchar) as sql_variant)
								END DESC)
							END
						as int) as VSequenceNumber,
						inDat.*
				FROM	(
					SELECT      ROW_NUMBER() OVER(PARTITION BY FakturPajak.FakturPajakId ORDER BY FakturPajakDetail.FakturPajakDetailId ASC) AS VPartition,
								FakturPajak.*, 
								[State].StateName AS StatusText,
								FakturPajakDetail.Nama, 
								FakturPajakDetail.HargaSatuan, 
								FakturPajakDetail.JumlahBarang, 
								FakturPajakDetail.HargaTotal, 
								FakturPajakDetail.Diskon, 
								FakturPajakDetail.Dpp, 
								FakturPajakDetail.Ppn, 
								FakturPajakDetail.TarifPpnbm, 
								FakturPajakDetail.Ppnbm,
								FakturPajakDetail.FakturPajakDetailId			
					FROM        FakturPajak LEFT JOIN
								FakturPajakDetail ON FakturPajak.FakturPajakId = FakturPajakDetail.FakturPajakId INNER JOIN
								[State] ON [State].StateId = FakturPajak.Status
					WHERE		FakturPajak.IsDeleted = 0 
								AND [State].IsDeleted = 0 
								AND FakturPajak.Status = 2
								AND (FakturPajakDetail.IsDeleted = 0 OR FakturPajakDetail.IsDeleted IS NULL)
				) inDat
				WHERE ((@noFaktur1 IS NOT NULL AND @noFaktur2 IS NOT NULL AND (REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') BETWEEN REPLACE(REPLACE(@noFaktur1, '-',''),'.','') AND REPLACE(REPLACE(@noFaktur2, '-',''),'.','')))
							OR (@noFaktur1 IS NOT NULL AND @noFaktur2 IS NULL AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@noFaktur1, '-',''),'.',''), '*','%'))
							OR (@noFaktur1 IS NULL AND @noFaktur2 IS NOT NULL AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@noFaktur2, '-',''),'.',''),'*', '%'))
							OR (@noFaktur1 IS NULL AND @noFaktur2 IS NULL)
					  )
					  AND (
						@Search IS NULL
						OR FormatedNoFaktur LIKE REPLACE(@Search,'*', '%')
						OR Nama LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, HargaSatuan, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, Diskon, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, JumlahDPP, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, JumlahPPN, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, JumlahPPNBM, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, Dpp, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, Ppn, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, Ppnbm, 103) LIKE REPLACE(@Search,'*', '%')
						OR FormatedNpwpPenjual LIKE REPLACE(@Search,'*', '%')
						OR NamaPenjual LIKE REPLACE(@Search,'*', '%')
						OR FillingIndex LIKE REPLACE(@Search,'*', '%')
					  )
			) v
		)
		SELECT	*
		FROM	n_dat
		ORDER BY VSequenceNumber ASC
		OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
		FETCH NEXT @ItemPerPage ROWS ONLY
	OPTION (OPTIMIZE FOR UNKNOWN)
END




GO
/****** Object:  StoredProcedure [dbo].[sp_ReportDetailFakturPajak_GetListWithouPaging]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportDetailFakturPajak_GetListWithouPaging]
	-- Add the parameters for the stored procedure here
	@noFaktur1 varchar(max)
	,@noFaktur2 varchar(max)
	,@Search varchar(max)
AS
BEGIN
	
	SELECT v.*, COUNT(v.FakturPajakDetailId) OVER() AS TotalItems
	FROM	(
		SELECT      
					CAST(ROW_NUMBER() OVER(ORDER BY FakturPajak.FakturPajakId ASC) as int) as VSequenceNumber,
					ROW_NUMBER() OVER(PARTITION BY FakturPajak.FakturPajakId ORDER BY FakturPajakDetail.FakturPajakDetailId ASC) AS VPartition,
					FakturPajak.*, 
					[State].StateName AS StatusText,
					FakturPajakDetail.Nama, 
					FakturPajakDetail.HargaSatuan, 
					FakturPajakDetail.JumlahBarang, 
					FakturPajakDetail.HargaTotal, 
					FakturPajakDetail.Diskon, 
					FakturPajakDetail.Dpp, 
					FakturPajakDetail.Ppn, 
					FakturPajakDetail.TarifPpnbm, 
					FakturPajakDetail.Ppnbm,
					FakturPajakDetail.FakturPajakDetailId			
		FROM        FakturPajak LEFT JOIN
					FakturPajakDetail ON FakturPajak.FakturPajakId = FakturPajakDetail.FakturPajakId INNER JOIN
					[State] ON [State].StateId = FakturPajak.Status
		WHERE		FakturPajak.IsDeleted = 0 
					AND [State].IsDeleted = 0 
					AND FakturPajak.Status = 2
					AND (FakturPajakDetail.IsDeleted = 0 OR FakturPajakDetail.IsDeleted IS NULL)
	) v
	WHERE ((@noFaktur1 IS NOT NULL AND @noFaktur2 IS NOT NULL AND (REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') BETWEEN REPLACE(REPLACE(@noFaktur1, '-',''),'.','') AND REPLACE(REPLACE(@noFaktur2, '-',''),'.','')))
							OR (@noFaktur1 IS NOT NULL AND @noFaktur2 IS NULL AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@noFaktur1, '-',''),'.',''), '*','%'))
							OR (@noFaktur1 IS NULL AND @noFaktur2 IS NOT NULL AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@noFaktur2, '-',''),'.',''),'*', '%'))
							OR (@noFaktur1 IS NULL AND @noFaktur2 IS NULL)
					  )
					  AND (
						@Search IS NULL
						OR FormatedNoFaktur LIKE REPLACE(@Search,'*', '%')
						OR Nama LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, HargaSatuan, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, Diskon, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, JumlahDPP, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, JumlahPPN, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, JumlahPPNBM, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, Dpp, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, Ppn, 103) LIKE REPLACE(@Search,'*', '%')
						OR CONVERT(varchar, Ppnbm, 103) LIKE REPLACE(@Search,'*', '%')
						OR FormatedNpwpPenjual LIKE REPLACE(@Search,'*', '%')
						OR NamaPenjual LIKE REPLACE(@Search,'*', '%')
						OR FillingIndex LIKE REPLACE(@Search,'*', '%')
					  )
	OPTION (OPTIMIZE FOR UNKNOWN)

END




GO
/****** Object:  StoredProcedure [dbo].[sp_ReportFakturPajakBelumDiJurnals_GetList]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportFakturPajakBelumDiJurnals_GetList]
	-- Add the parameters for the stored procedure here
	@tglFakturStart date
	,@tglFakturEnd date
	,@noFakturStart nvarchar(max)
	,@noFakturEnd nvarchar(max)
	,@Search nvarchar(max)
	,@CurrentPage int
	,@ItemPerPage int
	,@SortColumnName varchar(max)
	,@sortOrder varchar(5) -- ASC or DESC
AS
BEGIN
	with n_dat as (
		SELECT	CAST(CASE WHEN LOWER(@sortOrder) = 'asc' THEN
					ROW_NUMBER() OVER(ORDER BY 
					CASE WHEN @SortColumnName = 'FormatedNpwpPenjual' THEN fp.FormatedNpwpPenjual
					WHEN @SortColumnName = 'NamaPenjual' THEN cast(cast(fp.NamaPenjual as varchar) as sql_variant)
					WHEN @SortColumnName = 'FormatedNoFaktur' THEN cast(cast(fp.FormatedNoFaktur as varchar) as sql_variant)
					WHEN @SortColumnName = 'CreatedBy' THEN cast(cast(fp.CreatedBy as varchar) as sql_variant)
					WHEN @SortColumnName = 'FillingIndex' THEN cast(cast(fp.FillingIndex as varchar) as sql_variant)
					WHEN @SortColumnName = 'TglFaktur' THEN cast(fp.TglFaktur as sql_variant)
					WHEN @SortColumnName = 'JumlahPPN' THEN cast(fp.JumlahPPN as sql_variant)					
					WHEN @SortColumnName = '' THEN cast(cast(fp.FormatedNpwpPenjual as varchar) as sql_variant)
					END ASC)
				ELSE
					ROW_NUMBER() OVER(ORDER BY 
					CASE WHEN @SortColumnName = 'FormatedNpwpPenjual' THEN fp.FormatedNpwpPenjual
					WHEN @SortColumnName = 'NamaPenjual' THEN cast(cast(fp.NamaPenjual as varchar) as sql_variant)
					WHEN @SortColumnName = 'FormatedNoFaktur' THEN cast(cast(fp.FormatedNoFaktur as varchar) as sql_variant)
					WHEN @SortColumnName = 'CreatedBy' THEN cast(cast(fp.CreatedBy as varchar) as sql_variant)
					WHEN @SortColumnName = 'FillingIndex' THEN cast(cast(fp.FillingIndex as varchar) as sql_variant)
					WHEN @SortColumnName = 'TglFaktur' THEN cast(fp.TglFaktur as sql_variant)
					WHEN @SortColumnName = 'JumlahPPN' THEN cast(fp.JumlahPPN as sql_variant)					
					WHEN @SortColumnName = '' THEN cast(cast(fp.FormatedNpwpPenjual as varchar) as sql_variant)
					END DESC)
				END
			as int) as VSequenceNumber
				,fp.*
				,COUNT(fp.FakturPajakId) OVER() AS TotalItems
				,spm.MasaPajak AS MasaPajakSpm
				,spm.TahunPajak AS TahunPajakSpm
				,spmdet.FormatedNoFaktur  AS FpNoSpm
		FROM	View_FakturPajak fp LEFT JOIN
				ReportSPMDetail spmdet ON fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT = spmdet.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
				ReportSPM spm ON spmdet.ReportSPMId = spm.Id
		WHERE	fp.IsDeleted = 0 AND (spmdet.IsDeleted = 0 OR spmdet.IsDeleted IS NULL) AND (spm.IsDeleted = 0 OR spm.IsDeleted IS NULL)
				AND fp.[Status] = 2
				AND (fp.[StatusReconcile] IS NULL OR fp.[StatusReconcile] = 0)
				AND (
					(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
					OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL AND CAST(fp.TglFaktur AS date) = CAST(@tglFakturStart AS date))
					OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL AND CAST(fp.TglFaktur AS date) = CAST(@tglFakturEnd AS date))
					OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL AND CAST(fp.TglFaktur AS date) BETWEEN CAST(@tglFakturStart AS date) AND CAST(@tglFakturEnd AS date))
				)
				AND (
					(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
					OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL AND fp.FormatedNoFaktur LIKE REPLACE(@noFakturStart, '*', '%'))
					OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL AND fp.FormatedNoFaktur LIKE REPLACE(@noFakturEnd, '*', '%'))
					OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL AND fp.FormatedNoFaktur BETWEEN @noFakturStart AND @noFakturEnd)
				)
		AND (
					@Search IS NULL
					OR fp.FormatedNpwpPenjual LIKE REPLACE(@Search, '*','%')
					OR fp.NamaPenjual LIKE REPLACE(@Search, '*','%')
					OR fp.FormatedNoFaktur LIKE REPLACE(@Search, '*','%')
					OR fp.CreatedBy LIKE REPLACE(@Search, '*','%')
					OR fp.FillingIndex LIKE REPLACE(@Search, '*','%')
					OR CONVERT(varchar, fp.TglFaktur, 103) LIKE REPLACE(@Search, '*','%')
					OR CONVERT(varchar, fp.JumlahPPN, 103) LIKE REPLACE(@Search, '*','%')
				)
	)
	SELECT	*
	FROM	n_dat
	ORDER BY VSequenceNumber ASC
	OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
	FETCH NEXT @ItemPerPage ROWS ONLY
	OPTION (OPTIMIZE FOR UNKNOWN)
END




GO
/****** Object:  StoredProcedure [dbo].[sp_ReportFakturPajakBelumDiJurnals_GetListWithoutPaging]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportFakturPajakBelumDiJurnals_GetListWithoutPaging]
	-- Add the parameters for the stored procedure here
	@tglFakturStart date
	,@tglFakturEnd date
	,@noFakturStart nvarchar(max)
	,@noFakturEnd nvarchar(max)
	,@Search nvarchar(max)
AS
BEGIN
	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY fp.FakturPajakId ASC) as int) AS VSequenceNumber
			,fp.*
			,COUNT(fp.FakturPajakId) OVER() AS TotalItems
			,spm.MasaPajak AS MasaPajakSpm
			,spm.TahunPajak AS TahunPajakSpm
			,spmdet.FormatedNoFaktur  AS FpNoSpm
	FROM	View_FakturPajak fp LEFT JOIN
			ReportSPMDetail spmdet ON fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT = spmdet.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
			ReportSPM spm ON spmdet.ReportSPMId = spm.Id
	WHERE	fp.IsDeleted = 0 AND (spmdet.IsDeleted = 0 OR spmdet.IsDeleted IS NULL) AND (spm.IsDeleted = 0 OR spm.IsDeleted IS NULL)
			AND fp.[Status] = 2
			AND (fp.[StatusReconcile] IS NULL OR fp.[StatusReconcile] = 0)
			AND (
				(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
				OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL AND CAST(fp.TglFaktur AS date) = CAST(@tglFakturStart AS date))
				OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL AND CAST(fp.TglFaktur AS date) = CAST(@tglFakturEnd AS date))
				OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL AND CAST(fp.TglFaktur AS date) BETWEEN CAST(@tglFakturStart AS date) AND CAST(@tglFakturEnd AS date))
			)
			AND (
					(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
					OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL AND fp.FormatedNoFaktur LIKE REPLACE(@noFakturStart, '*', '%'))
					OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL AND fp.FormatedNoFaktur LIKE REPLACE(@noFakturEnd, '*', '%'))
					OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL AND fp.FormatedNoFaktur BETWEEN @noFakturStart AND @noFakturEnd)
				)
	AND (
				@Search IS NULL
				OR fp.FormatedNpwpPenjual LIKE REPLACE(@Search, '*','%')
				OR fp.NamaPenjual LIKE REPLACE(@Search, '*','%')
				OR fp.FormatedNoFaktur LIKE REPLACE(@Search, '*','%')
				OR fp.CreatedBy LIKE REPLACE(@Search, '*','%')
				OR fp.FillingIndex LIKE REPLACE(@Search, '*','%')
				OR CONVERT(varchar, fp.TglFaktur, 103) LIKE REPLACE(@Search, '*','%')
				OR CONVERT(varchar, fp.JumlahPPN, 103) LIKE REPLACE(@Search, '*','%')
			)

OPTION (OPTIMIZE FOR UNKNOWN)
END




GO
/****** Object:  StoredProcedure [dbo].[sp_ReportFakturPajakMasukans_GetList]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportFakturPajakMasukans_GetList]
	-- Add the parameters for the stored procedure here
	@dTglFakturStart date
	,@dTglFakturEnd date
	,@picEntry nvarchar(max)
	,@Search nvarchar(max)
	,@CurrentPage int
	,@ItemPerPage int
	,@SortColumnName varchar(max)
	,@sortOrder varchar(5) -- ASC or DESC
AS
BEGIN
	with n_dat as (
		SELECT	inDat.*,COUNT(inDat.FakturPajakId) OVER() AS TotalItems
		FROM	(
				SELECT	CAST(CASE WHEN LOWER(@sortOrder) = 'asc' THEN
							ROW_NUMBER() OVER(ORDER BY 
							CASE WHEN @SortColumnName = 'NamaPenjual' THEN fp.NamaPenjual
							WHEN @SortColumnName = 'FormatedNpwpPenjual' THEN cast(cast(fp.FormatedNpwpPenjual as varchar) as sql_variant)
							WHEN @SortColumnName = 'FormatedNoFaktur' THEN cast(cast(fp.FormatedNoFaktur as varchar) as sql_variant)
							WHEN @SortColumnName = 'TglFaktur' THEN cast(fp.TglFaktur as sql_variant)
							WHEN @SortColumnName = 'JumlahPPN' THEN cast(fp.JumlahPPN as sql_variant)
							WHEN @SortColumnName = 'MasaPajak' THEN cast(fp.MasaPajak as sql_variant)
							WHEN @SortColumnName = 'CreatedBy' THEN cast(cast(fp.CreatedBy as varchar) as sql_variant)
							WHEN @SortColumnName = 'FillingIndex' THEN cast(fp.FillingIndex as sql_variant)
							WHEN @SortColumnName = 'Dikreditkan' THEN cast(fp.Dikreditkan as sql_variant)
							WHEN @SortColumnName = '' THEN cast(cast(fp.FormatedNpwpPenjual as varchar) as sql_variant)
							END ASC)
						ELSE
							ROW_NUMBER() OVER(ORDER BY 
							CASE WHEN @SortColumnName = 'NamaPenjual' THEN fp.NamaPenjual
							WHEN @SortColumnName = 'FormatedNpwpPenjual' THEN cast(cast(fp.FormatedNpwpPenjual as varchar) as sql_variant)
							WHEN @SortColumnName = 'FormatedNoFaktur' THEN cast(cast(fp.FormatedNoFaktur as varchar) as sql_variant)
							WHEN @SortColumnName = 'TglFaktur' THEN cast(fp.TglFaktur as sql_variant)
							WHEN @SortColumnName = 'JumlahPPN' THEN cast(fp.JumlahPPN as sql_variant)
							WHEN @SortColumnName = 'MasaPajak' THEN cast(fp.MasaPajak as sql_variant)
							WHEN @SortColumnName = 'CreatedBy' THEN cast(cast(fp.CreatedBy as varchar) as sql_variant)
							WHEN @SortColumnName = 'FillingIndex' THEN cast(fp.FillingIndex as sql_variant)
							WHEN @SortColumnName = 'Dikreditkan' THEN cast(fp.Dikreditkan as sql_variant)
							WHEN @SortColumnName = '' THEN cast(cast(fp.FormatedNpwpPenjual as varchar) as sql_variant)
							END DESC)
						END
						AS int) AS VSequenceNumber,
						fp.*
				FROM	View_FakturPajak fp
				WHERE	fp.IsDeleted = 0
						AND [Status] = 2
						AND (
							(@dTglFakturStart IS NULL AND @dTglFakturEnd IS NULL)
							OR (@dTglFakturStart IS NOT NULL AND @dTglFakturEnd IS NULL AND CAST(fp.TglFaktur as date) = CAST(@dTglFakturStart as date))
							OR (@dTglFakturStart IS NULL AND @dTglFakturEnd IS NOT NULL AND CAST(fp.TglFaktur as date) = CAST(@dTglFakturEnd as date))
							OR (@dTglFakturStart IS NOT NULL AND @dTglFakturEnd IS NOT NULL AND CAST(fp.TglFaktur as date) BETWEEN CAST(@dTglFakturStart as date) AND CAST(@dTglFakturEnd as date))
						)
						AND (@picEntry IS NULL OR (@picEntry IS NOT NULL AND LOWER(FP.CreatedBy) LIKE REPLACE(LOWER(@picEntry),'*','%')))
						AND (
							@Search IS NULL
							OR LOWER(fp.NamaPenjual) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR LOWER(fp.FormatedNpwpPenjual) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR LOWER(fp.FormatedNoFaktur) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR CONVERT(varchar, fp.TglFaktur, 103) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR CONVERT(varchar, fp.JumlahPPN, 103) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR CONVERT(varchar, fp.MasaPajak, 103) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR LOWER(fp.CreatedBy) LIKE REPLACE(LOWER(@Search), '*', '%')
						)
			) inDat
	)
	SELECT	*
	FROM	n_dat
	ORDER BY VSequenceNumber ASC
	OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
	FETCH NEXT @ItemPerPage ROWS ONLY
	OPTION (OPTIMIZE FOR UNKNOWN)
END




GO
/****** Object:  StoredProcedure [dbo].[sp_ReportFakturPajakMasukans_GetListWithouPaging]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportFakturPajakMasukans_GetListWithouPaging]
	-- Add the parameters for the stored procedure here
	@dTglFakturStart date
	,@dTglFakturEnd date
	,@picEntry nvarchar(max)
	,@Search nvarchar(max)
AS
BEGIN
	SELECT	inDat.*,COUNT(inDat.FakturPajakId) OVER() AS TotalItems
	FROM	(
			SELECT	CAST(ROW_NUMBER() OVER(ORDER BY fp.FormatedNoFaktur ASC) AS int) AS VSequenceNumber,
					fp.*
			FROM	View_FakturPajak fp
			WHERE	fp.IsDeleted = 0
					AND [Status] = 2
					AND (
						(@dTglFakturStart IS NULL AND @dTglFakturEnd IS NULL)
						OR (@dTglFakturStart IS NOT NULL AND @dTglFakturEnd IS NULL AND CAST(fp.TglFaktur as date) = CAST(@dTglFakturStart as date))
						OR (@dTglFakturStart IS NULL AND @dTglFakturEnd IS NOT NULL AND CAST(fp.TglFaktur as date) = CAST(@dTglFakturEnd as date))
						OR (@dTglFakturStart IS NOT NULL AND @dTglFakturEnd IS NOT NULL AND CAST(fp.TglFaktur as date) BETWEEN CAST(@dTglFakturStart as date) AND CAST(@dTglFakturEnd as date))
					)
					AND (@picEntry IS NULL OR (@picEntry IS NOT NULL AND LOWER(FP.CreatedBy) LIKE REPLACE(LOWER(@picEntry),'*','%')))
						AND (
							@Search IS NULL
							OR LOWER(fp.NamaPenjual) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR LOWER(fp.FormatedNpwpPenjual) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR LOWER(fp.FormatedNoFaktur) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR CONVERT(varchar, fp.TglFaktur, 103) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR CONVERT(varchar, fp.JumlahPPN, 103) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR CONVERT(varchar, fp.MasaPajak, 103) LIKE REPLACE(LOWER(@Search), '*', '%')
							OR LOWER(fp.CreatedBy) LIKE REPLACE(LOWER(@Search), '*', '%')
						)
		) inDat
	ORDER BY FormatedNoFaktur ASC
	OPTION (OPTIMIZE FOR UNKNOWN)
END




GO
/****** Object:  StoredProcedure [dbo].[sp_ReportFakturPajakOutstandings_GetList]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportFakturPajakOutstandings_GetList]
	-- Add the parameters for the stored procedure here
	@postingDateStart date
	,@postingDateEnd date
	,@docSapStart nvarchar(max)
	,@docSapEnd nvarchar(max)
	,@Search nvarchar(max)
	,@CurrentPage int
	,@ItemPerPage int
	,@SortColumnName varchar(max)
	,@sortOrder varchar(5) -- ASC or DESC
AS
BEGIN
	with n_dat as (
		SELECT	inDat.*,COUNT(inDat.Id) OVER() AS TotalItems
		FROM	(
			SELECT	CAST(CASE WHEN LOWER(@sortOrder) = 'asc' THEN
							ROW_NUMBER() OVER(ORDER BY 
							CASE WHEN @SortColumnName = 'GLAccount' THEN cast(cast(a.GLAccount as varchar) as sql_variant)
							WHEN @SortColumnName = '' THEN cast(cast(a.GLAccount as varchar) as sql_variant)
							WHEN @SortColumnName = 'AccountingDocNo' THEN cast(cast(a.AccountingDocNo as varchar) as sql_variant)
							WHEN @SortColumnName = 'TaxInvoiceNumber' THEN cast(cast(a.TaxInvoiceNumber as varchar) as sql_variant)
							WHEN @SortColumnName = 'AssignmentNo' THEN cast(cast(a.AssignmentNo as varchar) as sql_variant)
							WHEN @SortColumnName = 'TglFaktur' THEN cast(a.TglFaktur as sql_variant)
							WHEN @SortColumnName = 'PostingDate' THEN cast(a.PostingDate as sql_variant)
							END ASC)
						ELSE
							ROW_NUMBER() OVER(ORDER BY 
							CASE WHEN @SortColumnName = 'GLAccount' THEN cast(cast(a.GLAccount as varchar) as sql_variant)
							WHEN @SortColumnName = '' THEN cast(cast(a.GLAccount as varchar) as sql_variant)
							WHEN @SortColumnName = 'AccountingDocNo' THEN cast(cast(a.AccountingDocNo as varchar) as sql_variant)
							WHEN @SortColumnName = 'TaxInvoiceNumber' THEN cast(cast(a.TaxInvoiceNumber as varchar) as sql_variant)
							WHEN @SortColumnName = 'AssignmentNo' THEN cast(cast(a.AssignmentNo as varchar) as sql_variant)
							WHEN @SortColumnName = 'TglFaktur' THEN cast(a.TglFaktur as sql_variant)
							WHEN @SortColumnName = 'PostingDate' THEN cast(a.PostingDate as sql_variant)
							END DESC)
						END
					as int) as VSequenceNumber,
					a.*
			FROM	(
				SELECT	row_number() over(PARTITION BY v1.ItemText ORDER BY v1.Id DESC) VPartition,
						CAST(v1.PostingDate AS date) AS PostingDate,
						v1.AccountingDocNo,
						v1.LineItem AS ItemNo,
						v1.ItemText,
						SUBSTRING(RTRIM(LTRIM(v1.ItemText)), 10, 19) AS TaxInvoiceNumber,
						dbo.fnConvertSapStringToDate(SUBSTRING(LTRIM(RTRIM(V1.ItemText)), 1, 8)) AS TglFaktur,				
						v1.HeaderText AS DocumentHeaderText,
						dbo.FormatNpwp(v1.AssignmentNo) AS AssignmentNo,
						v1.Id,
						CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
						v1.StatusReconcile,
						CAST(v1.FiscalYear AS int) AS FiscalYearDebet,
						v1.GLAccount
				FROM	SAP_MTDownloadPPN v1
				WHERE	v1.IsDeleted = 0
			) a LEFT JOIN
				dbo.FakturPajak fp ON a.TaxInvoiceNumber COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT
			WHERE	a.VPartition = 1
					AND ((@postingDateStart IS NULL AND @postingDateEnd IS NULL)
						OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NULL AND a.PostingDate IS NOT NULL AND CONVERT(date, a.PostingDate) = CONVERT(date, @postingDateStart))
						OR (@postingDateStart IS NULL AND @postingDateEnd IS NOT NULL AND a.PostingDate IS NOT NULL AND CONVERT(date, a.PostingDate) = CONVERT(date, @postingDateEnd))
						OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NOT NULL AND a.PostingDate IS NOT NULL 
						AND CONVERT(date, a.PostingDate) BETWEEN CONVERT(date, @postingDateStart) AND CONVERT(date, @postingDateEnd))
					)
					AND ((@docSapStart IS NULL AND @docSapEnd IS NULL)
						OR (@docSapStart IS NOT NULL AND @docSapEnd IS NULL AND a.AccountingDocNo LIKE REPLACE(@docSapStart,'*', '%'))
						OR (@docSapStart IS NULL AND @docSapEnd IS NOT NULL AND a.AccountingDocNo LIKE REPLACE(@docSapEnd, '*', '%'))
						OR (@docSapStart IS NOT NULL AND @docSapEnd IS NOT NULL AND a.AccountingDocNo BETWEEN @docSapStart AND @docSapEnd)
					)
					AND fp.FormatedNoFaktur IS NULL	
					AND (
						@Search IS NULL
						OR GLAccount LIKE REPLACE(@Search, '*', '%')
						OR AccountingDocNo LIKE REPLACE(@Search, '*', '%')
						OR CONVERT(varchar, a.PostingDate, 103) LIKE REPLACE(@Search, '*', '%')
						OR CONVERT(varchar, a.AmountLocal, 103) LIKE REPLACE(@Search, '*', '%')
						OR a.TaxInvoiceNumber LIKE REPLACE(@Search, '*', '%')
						OR CONVERT(varchar, a.TglFaktur, 103) LIKE REPLACE(@Search, '*', '%')
						OR AssignmentNo LIKE REPLACE(@Search, '*', '%')
					)
		) inDat
	)
	SELECT	*
	FROM	n_dat
	ORDER BY VSequenceNumber ASC
	OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
	FETCH NEXT @ItemPerPage ROWS ONLY
	OPTION (OPTIMIZE FOR UNKNOWN)
END




GO
/****** Object:  StoredProcedure [dbo].[sp_ReportFakturPajakOutstandings_GetListWithouPaging]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportFakturPajakOutstandings_GetListWithouPaging]
	-- Add the parameters for the stored procedure here
	@postingDateStart date
	,@postingDateEnd date
	,@docSapStart nvarchar(max)
	,@docSapEnd nvarchar(max)
	,@Search nvarchar(max)
	
AS
BEGIN
	SELECT	inDat.*,COUNT(inDat.Id) OVER() AS TotalItems
	FROM	(
		SELECT	cast(row_number() over(order by AccountingDocNo ASC) as int) as VSequenceNumber,a.*
		FROM	(
			SELECT	row_number() over(PARTITION BY v1.ItemText ORDER BY v1.Id DESC) VPartition,
					CAST(v1.PostingDate AS date) AS PostingDate,
					v1.AccountingDocNo,
					v1.LineItem AS ItemNo,
					v1.ItemText,
					SUBSTRING(RTRIM(LTRIM(v1.ItemText)), 10, 19) AS TaxInvoiceNumber,
					dbo.fnConvertSapStringToDate(SUBSTRING(LTRIM(RTRIM(V1.ItemText)), 1, 8)) AS TglFaktur,				
					v1.HeaderText AS DocumentHeaderText,
					dbo.FormatNpwp(v1.AssignmentNo) AS AssignmentNo,
					v1.Id,
					CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
					v1.StatusReconcile,
					CAST(v1.FiscalYear AS int) AS FiscalYearDebet,
					v1.GLAccount
			FROM	SAP_MTDownloadPPN v1
			WHERE	v1.IsDeleted = 0
		) a LEFT JOIN
			dbo.FakturPajak fp ON a.TaxInvoiceNumber COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT
		WHERE	a.VPartition = 1
				AND ((@postingDateStart IS NULL AND @postingDateEnd IS NULL)
					OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NULL AND a.PostingDate IS NOT NULL AND CONVERT(date, a.PostingDate) = CONVERT(date, @postingDateStart))
					OR (@postingDateStart IS NULL AND @postingDateEnd IS NOT NULL AND a.PostingDate IS NOT NULL AND CONVERT(date, a.PostingDate) = CONVERT(date, @postingDateEnd))
					OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NOT NULL AND a.PostingDate IS NOT NULL 
					AND CONVERT(date, a.PostingDate) BETWEEN CONVERT(date, @postingDateStart) AND CONVERT(date, @postingDateEnd))
				)
				AND ((@docSapStart IS NULL AND @docSapEnd IS NULL)
						OR (@docSapStart IS NOT NULL AND @docSapEnd IS NULL AND a.AccountingDocNo LIKE REPLACE(@docSapStart,'*', '%'))
						OR (@docSapStart IS NULL AND @docSapEnd IS NOT NULL AND a.AccountingDocNo LIKE REPLACE(@docSapEnd, '*', '%'))
						OR (@docSapStart IS NOT NULL AND @docSapEnd IS NOT NULL AND a.AccountingDocNo BETWEEN @docSapStart AND @docSapEnd)
					)
					AND fp.FormatedNoFaktur IS NULL	
					AND (
						@Search IS NULL
						OR GLAccount LIKE REPLACE(@Search, '*', '%')
						OR AccountingDocNo LIKE REPLACE(@Search, '*', '%')
						OR CONVERT(varchar, a.PostingDate, 103) LIKE REPLACE(@Search, '*', '%')
						OR CONVERT(varchar, a.AmountLocal, 103) LIKE REPLACE(@Search, '*', '%')
						OR a.TaxInvoiceNumber LIKE REPLACE(@Search, '*', '%')
						OR CONVERT(varchar, a.TglFaktur, 103) LIKE REPLACE(@Search, '*', '%')
						OR AssignmentNo LIKE REPLACE(@Search, '*', '%')
					)
	) inDat
	ORDER BY AccountingDocNo ASC
	OPTION (OPTIMIZE FOR UNKNOWN)

END




GO
/****** Object:  StoredProcedure [dbo].[sp_ReportSPMGenerateInsert]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportSPMGenerateInsert]
	-- Add the parameters for the stored procedure here
	 @masaPajak int 
	,@tahunPajak int 
	,@userNameLogin nvarchar(100)
	,@reportSpmId bigint output
AS
BEGIN
	DECLARE
		@statusReguler bit	
		,@lastSpmVersion int
		,@versi int		
		,@statusSp2 bit

	SET @reportSpmId = 0
	SELECT  @statusReguler = StatusRegular, @statusSp2 = StatusSP2
	FROM	OpenClosePeriod ocp
	WHERE	ocp.IsDeleted = 0 AND ocp.MasaPajak = @masaPajak AND ocp.TahunPajak = @tahunPajak

	IF @statusSp2 = 0 
	BEGIN
		RETURN -- skip process, seharusnya tidak boleh create SPM dikarenakan Status Masa Pajak sudah Close SP2
	END

	IF @statusReguler = 1
	BEGIN
		-- Open Reguler
		SET @lastSpmVersion = (SELECT TOP 1 Versi FROM ReportSPM WHERE IsDeleted = 0 AND MasaPajak = @masaPajak AND TahunPajak = @tahunPajak ORDER BY Versi DESC)
		IF @lastSpmVersion IS NULL
		BEGIN
			-- Menggunakan versi 0, first create
			-- insert data baru ke table ReportSPM
			SET @versi = 0

			INSERT INTO ReportSPM(MasaPajak, TahunPajak, Versi, CreatedBy)
			VALUES (@masaPajak, @tahunPajak, @versi, @userNameLogin);

			SELECT @reportSpmId = @@IDENTITY;

			INSERT INTO [dbo].[ReportSPMDetail]
			   ([ReportSPMId]
			   ,[FCode]
			   ,[KdJenisTransaksi]
			   ,[FgPengganti]
			   ,[NoFakturPajak]
			   ,[TglFaktur]
			   ,[NPWPPenjual]
			   ,[NamaPenjual]
			   ,[AlamatPenjual]
			   ,[NPWPLawanTransaksi]
			   ,[NamaLawanTransaksi]
			   ,[AlamatLawanTransaksi]
			   ,[JumlahDPP]
			   ,[JumlahPPN]
			   ,[JumlahPPNBM]
			   ,[KeteranganTambahan]
			   ,[FgUangMuka]
			   ,[UangMukaDPP]
			   ,[UangMukaPPN]
			   ,[UangMukaPPnBM]
			   ,[Referensi]
			   ,[FillingIndex]
			   ,[FormatedNoFaktur]
			   ,[FormatedNpwpPenjual]
			   ,[FormatedNpwpLawanTransaksi]
			   ,[CreatedBy]
			   ,[FPCreatedDate])
			SELECT	
					@reportSpmId
					,fp.FCode
					,fp.KdJenisTransaksi
					,fp.FgPengganti
					,fp.NoFakturPajak
					,fp.TglFaktur
					,fp.[NPWPPenjual]
					,fp.[NamaPenjual]
					,fp.[AlamatPenjual]
					,fp.[NPWPLawanTransaksi]
					,fp.[NamaLawanTransaksi]
					,fp.[AlamatLawanTransaksi]
					,fp.[JumlahDPP]
					,fp.[JumlahPPN]
					,fp.[JumlahPPNBM]
					,fp.Pesan AS [KeteranganTambahan]
					,CAST(NULL AS decimal(18,0)) AS [FgUangMuka]
					,CAST(NULL AS decimal(18,0)) AS [UangMukaDPP]
					,CAST(NULL AS decimal(18,0)) AS [UangMukaPPN]
					,CAST(NULL AS decimal(18,0)) AS [UangMukaPPnBM]
					,fp.[Referensi]
					,fp.[FillingIndex]
					,fp.[FormatedNoFaktur]
					,fp.[FormatedNpwpPenjual]
					,fp.[FormatedNpwpLawanTransaksi]
					,@userNameLogin
					,fp.Created
			FROM	View_FakturPajak fp
			WHERE	fp.IsDeleted = 0 AND fp.[Status] = 2
					AND fp.MasaPajak = @masaPajak AND fp.TahunPajak = @tahunPajak

		END
		ELSE
		BEGIN
			-- Menggunakan versi terakhir, ikut di SPM sebelumnya
			SET @reportSpmId = (
				SELECT TOP 1 Id FROM ReportSPM WHERE IsDeleted = 0 AND MasaPajak = @masaPajak AND TahunPajak = @tahunPajak AND Versi = @lastSpmVersion 
			)

			INSERT INTO [dbo].[ReportSPMDetail]
			   ([ReportSPMId]
			   ,[FCode]
			   ,[KdJenisTransaksi]
			   ,[FgPengganti]
			   ,[NoFakturPajak]
			   ,[TglFaktur]
			   ,[NPWPPenjual]
			   ,[NamaPenjual]
			   ,[AlamatPenjual]
			   ,[NPWPLawanTransaksi]
			   ,[NamaLawanTransaksi]
			   ,[AlamatLawanTransaksi]
			   ,[JumlahDPP]
			   ,[JumlahPPN]
			   ,[JumlahPPNBM]
			   ,[KeteranganTambahan]
			   ,[FgUangMuka]
			   ,[UangMukaDPP]
			   ,[UangMukaPPN]
			   ,[UangMukaPPnBM]
			   ,[Referensi]
			   ,[FillingIndex]
			   ,[FormatedNoFaktur]
			   ,[FormatedNpwpPenjual]
			   ,[FormatedNpwpLawanTransaksi]
			   ,[CreatedBy]
			   ,[FPCreatedDate])
			SELECT	
					@reportSpmId
					,fp.FCode
					,fp.KdJenisTransaksi
					,fp.FgPengganti
					,fp.NoFakturPajak
					,fp.TglFaktur
					,fp.[NPWPPenjual]
					,fp.[NamaPenjual]
					,fp.[AlamatPenjual]
					,fp.[NPWPLawanTransaksi]
					,fp.[NamaLawanTransaksi]
					,fp.[AlamatLawanTransaksi]
					,fp.[JumlahDPP]
					,fp.[JumlahPPN]
					,fp.[JumlahPPNBM]
					,fp.Pesan AS [KeteranganTambahan]
					,CAST(NULL AS decimal(18,0)) AS [FgUangMuka]
					,CAST(NULL AS decimal(18,0)) AS [UangMukaDPP]
					,CAST(NULL AS decimal(18,0)) AS [UangMukaPPN]
					,CAST(NULL AS decimal(18,0)) AS [UangMukaPPnBM]
					,fp.[Referensi]
					,fp.[FillingIndex]
					,fp.[FormatedNoFaktur]
					,fp.[FormatedNpwpPenjual]
					,fp.[FormatedNpwpLawanTransaksi]
					,@userNameLogin
					,fp.Created
			FROM	View_FakturPajak fp
			WHERE	fp.IsDeleted = 0 AND fp.[Status] = 2
					AND fp.MasaPajak = @masaPajak AND fp.TahunPajak = @tahunPajak
					AND fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT NOT IN (
						SELECT	DISTINCT rpd.FormatedNoFaktur
						FROM	ReportSPM rp INNER JOIN
								ReportSPMDetail rpd ON rp.Id = rpd.ReportSPMId
						WHERE	rp.MasaPajak = @masaPajak 
								AND rp.IsDeleted = 0 
								AND rp.TahunPajak = @tahunPajak
								AND rpd.FPCreatedDate = fp.Created
					)

		END
	END
	ELSE
	BEGIN
		-- Close Reguler
		SET @lastSpmVersion = (SELECT TOP 1 Versi FROM ReportSPM WHERE IsDeleted = 0 AND MasaPajak = @masaPajak AND TahunPajak = @tahunPajak ORDER BY Versi DESC)
		IF @lastSpmVersion IS NULL
		BEGIN
			SET @lastSpmVersion = 0
		END
		ELSE
		BEGIN
			SET @lastSpmVersion = @lastSpmVersion + 1	
		END
		-- Insert new data
		
		INSERT INTO ReportSPM(MasaPajak, TahunPajak, Versi, CreatedBy)
		VALUES (@masaPajak, @tahunPajak, @lastSpmVersion, @userNameLogin);

		SELECT @reportSpmId = @@IDENTITY;

		INSERT INTO [dbo].[ReportSPMDetail]
           ([ReportSPMId]
           ,[FCode]
           ,[KdJenisTransaksi]
           ,[FgPengganti]
           ,[NoFakturPajak]
           ,[TglFaktur]
           ,[NPWPPenjual]
           ,[NamaPenjual]
           ,[AlamatPenjual]
           ,[NPWPLawanTransaksi]
           ,[NamaLawanTransaksi]
           ,[AlamatLawanTransaksi]
           ,[JumlahDPP]
           ,[JumlahPPN]
           ,[JumlahPPNBM]
           ,[KeteranganTambahan]
           ,[FgUangMuka]
           ,[UangMukaDPP]
           ,[UangMukaPPN]
           ,[UangMukaPPnBM]
           ,[Referensi]
           ,[FillingIndex]
           ,[FormatedNoFaktur]
           ,[FormatedNpwpPenjual]
           ,[FormatedNpwpLawanTransaksi]
           ,[CreatedBy]
		   ,[FPCreatedDate])
		SELECT	
				@reportSpmId
				,fp.FCode
				,fp.KdJenisTransaksi
				,fp.FgPengganti
				,fp.NoFakturPajak
				,fp.TglFaktur
				,fp.[NPWPPenjual]
			    ,fp.[NamaPenjual]
			    ,fp.[AlamatPenjual]
			    ,fp.[NPWPLawanTransaksi]
			    ,fp.[NamaLawanTransaksi]
			    ,fp.[AlamatLawanTransaksi]
			    ,fp.[JumlahDPP]
			    ,fp.[JumlahPPN]
			    ,fp.[JumlahPPNBM]
			    ,fp.Pesan AS [KeteranganTambahan]
			    ,CAST(NULL AS decimal(18,0)) AS [FgUangMuka]
			    ,CAST(NULL AS decimal(18,0)) AS [UangMukaDPP]
			    ,CAST(NULL AS decimal(18,0)) AS [UangMukaPPN]
			    ,CAST(NULL AS decimal(18,0)) AS [UangMukaPPnBM]
			    ,fp.[Referensi]
			    ,fp.[FillingIndex]
			    ,fp.[FormatedNoFaktur]
			    ,fp.[FormatedNpwpPenjual]
			    ,fp.[FormatedNpwpLawanTransaksi]
			    ,@userNameLogin
				,fp.Created
		FROM	View_FakturPajak fp
		WHERE	fp.IsDeleted = 0 AND fp.[Status] = 2
				AND fp.MasaPajak = @masaPajak AND fp.TahunPajak = @tahunPajak
				AND fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT NOT IN (
					SELECT	DISTINCT rpd.FormatedNoFaktur
					FROM	ReportSPM rp INNER JOIN
							ReportSPMDetail rpd ON rp.Id = rpd.ReportSPMId
					WHERE	rp.MasaPajak = @masaPajak 
							AND rp.IsDeleted = 0 
							AND rp.TahunPajak = @tahunPajak
							AND rpd.FPCreatedDate = fp.Created
				)

	END



END



GO
/****** Object:  StoredProcedure [dbo].[sp_ReportSPMGet]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportSPMGet]
	-- Add the parameters for the stored procedure here
	 @masaPajak int 
	,@tahunPajak int 
AS
BEGIN
	DECLARE
		@statusSp2 bit	
		,@statusReguler bit
		,@lastSpmVersion int
	
	SELECT  @statusSp2 = StatusSP2, @statusReguler = StatusRegular
	FROM	OpenClosePeriod ocp
	WHERE	ocp.IsDeleted = 0 AND ocp.MasaPajak = @masaPajak AND ocp.TahunPajak = @tahunPajak

	IF @statusSp2 = 0 
	BEGIN
		-- Seharusnya tidak boleh, karena sudah Close SP2
		SELECT	
				CAST(ROW_NUMBER() OVER(ORDER BY fp.FakturPajakId ASC) as int) as VSequenceNumber
				,fp.*
				,CAST(0 AS int) AS Versi
				,CAST(NULL AS decimal(18,0)) AS FgUangMuka
				,CAST(NULL AS decimal(18,0)) AS UangMukaDPP
				,CAST(NULL AS decimal(18,0)) AS UangMukaPPN
				,CAST(NULL AS decimal(18,0)) AS UangMukaPPnBM
				,CAST(0 as bigint) AS Id
				,CAST(0 as bigint) as ReportSPMId
				,CAST(NULL AS nvarchar(255)) AS KeteranganTambahan
				,fp.Created AS [FPCreatedDate]
		FROM	View_FakturPajak fp
		WHERE	fp.IsDeleted = 0 AND fp.[Status] = 2
				AND fp.MasaPajak = -1 AND fp.TahunPajak = -1
		OPTION (OPTIMIZE FOR UNKNOWN)
	END
	ELSE
	BEGIN

		SET @lastSpmVersion = (SELECT TOP 1 Versi FROM ReportSPM WHERE IsDeleted = 0 AND MasaPajak = @masaPajak AND TahunPajak = @tahunPajak ORDER BY Versi DESC)
		IF @lastSpmVersion IS NOT NULL AND @statusReguler = 0
		BEGIN
			SET @lastSpmVersion = @lastSpmVersion + 1
		END
		SELECT	
				CAST(ROW_NUMBER() OVER(ORDER BY fp.FakturPajakId ASC) as int) as VSequenceNumber
				,fp.*
				,CAST(ISNULL(@lastSpmVersion, 0) AS int) AS Versi
				,CAST(NULL AS decimal(18,0)) AS FgUangMuka
				,CAST(NULL AS decimal(18,0)) AS UangMukaDPP
				,CAST(NULL AS decimal(18,0)) AS UangMukaPPN
				,CAST(NULL AS decimal(18,0)) AS UangMukaPPnBM
				,CAST(0 as bigint) AS Id
				,CAST(0 as bigint) as ReportSPMId
				,CAST(NULL AS nvarchar(255)) AS KeteranganTambahan
				,fp.Created AS [FPCreatedDate]
		FROM	View_FakturPajak fp
		WHERE	fp.IsDeleted = 0 AND fp.[Status] = 2
				AND fp.MasaPajak = @masaPajak AND fp.TahunPajak = @tahunPajak
				AND fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT NOT IN (
					SELECT	DISTINCT rpd.FormatedNoFaktur
					FROM	ReportSPM rp INNER JOIN
							ReportSPMDetail rpd ON rp.Id = rpd.ReportSPMId
					WHERE	rp.MasaPajak = @masaPajak 
							AND rp.IsDeleted = 0 
							AND rp.TahunPajak = @tahunPajak
							AND rpd.FPCreatedDate = fp.Created
				)
		OPTION (OPTIMIZE FOR UNKNOWN)
	END

END



GO
/****** Object:  StoredProcedure [dbo].[sp_ReportSPMGetByFormatedNoFaktur]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportSPMGetByFormatedNoFaktur]
	-- Add the parameters for the stored procedure here
	@formatedNoFaktur nvarchar(255)
AS
BEGIN
	SELECT	TOP 1 CAST(ROW_NUMBER() OVER(ORDER BY spmdet.Id DESC) as int) as VSequenceNumber
			,spmdet.*
			,spm.Versi AS Versi
			,spm.MasaPajak
			,spm.TahunPajak
			,m.[MonthName] AS MasaPajakName 
	FROM	ReportSPMDetail spmdet INNER JOIN
			ReportSPM spm ON spmdet.ReportSPMId = spm.Id INNER JOIN
			dbo.GetMonth() m ON m.MonthNumber = spm.MasaPajak
	WHERE	spmdet.IsDeleted = 0 AND spmdet.FormatedNoFaktur = @formatedNoFaktur
	OPTION (OPTIMIZE FOR UNKNOWN)
END



GO
/****** Object:  StoredProcedure [dbo].[sp_ReportSPMGetById]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ReportSPMGetById]
	-- Add the parameters for the stored procedure here
	@Id bigint
AS
BEGIN
	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY spmdet.Id ASC) as int) as VSequenceNumber
			,spmdet.*
			,spm.Versi AS Versi
			,spm.MasaPajak
			,spm.TahunPajak
			,m.[MonthName] AS MasaPajakName 
	FROM	ReportSPMDetail spmdet INNER JOIN
			ReportSPM spm ON spmdet.ReportSPMId = spm.Id INNER JOIN
			dbo.GetMonth() m ON m.MonthNumber = spm.MasaPajak
	WHERE	spmdet.IsDeleted = 0 AND spmdet.ReportSPMId = @Id
	OPTION (OPTIMIZE FOR UNKNOWN)
END



GO
/****** Object:  StoredProcedure [dbo].[sp_ReportSPMGetByMasaAndTahunPajak]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReportSPMGetByMasaAndTahunPajak]
	-- Add the parameters for the stored procedure here
	@masaPajak int
	,@tahunPajak int
AS
BEGIN
	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY spmdet.Id ASC) as int) as VSequenceNumber
			,spmdet.*
			,spm.Versi AS Versi
			,spm.MasaPajak
			,spm.TahunPajak
			,m.[MonthName] AS MasaPajakName 
	FROM	ReportSPMDetail spmdet INNER JOIN
			ReportSPM spm ON spmdet.ReportSPMId = spm.Id INNER JOIN
			dbo.GetMonth() m ON m.MonthNumber = spm.MasaPajak
	WHERE	spmdet.IsDeleted = 0 AND spm.MasaPajak = @masaPajak AND spm.TahunPajak = @tahunPajak
	ORDER  BY spmdet.Id ASC
	OPTION (OPTIMIZE FOR UNKNOWN)
END



GO
/****** Object:  StoredProcedure [dbo].[sp_ReportSPMGetForExcel]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ReportSPMGetForExcel]
	-- Add the parameters for the stored procedure here
	@masaPajak int
	,@tahunPajak int
	,@versi int
AS
BEGIN
	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY spmdet.Id ASC) as int) as VSequenceNumber
			,spmdet.*
			,spm.Versi AS Versi
			,spm.MasaPajak
			,spm.TahunPajak
			,m.[MonthName] AS MasaPajakName 
	FROM	ReportSPMDetail spmdet INNER JOIN
			ReportSPM spm ON spmdet.ReportSPMId = spm.Id INNER JOIN
			dbo.GetMonth() m ON m.MonthNumber = spm.MasaPajak
	WHERE	spmdet.IsDeleted = 0 AND spm.MasaPajak = @masaPajak AND spm.TahunPajak = @tahunPajak AND spm.Versi <= @versi
	OPTION (OPTIMIZE FOR UNKNOWN)
END



GO
/****** Object:  StoredProcedure [dbo].[sp_ReturPajakMasukanExportCsv]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_ReturPajakMasukanExportCsv]
	-- Add the parameters for the stored procedure here
	@NoFakturPajak nvarchar(100)
	,@NoDocRetur nvarchar(100) 
	,@NpwpVendor nvarchar(100) 
	,@NamaVendor nvarchar(100) 
	,@TglFakturReturStart date 
	,@TglFakturReturEnd date 	
	,@MasaPajak int 
	,@TahunPajak int 
	,@fTglRetur nvarchar(100)
	,@fNpwpVendor nvarchar(100)
	,@fNamaVendor nvarchar(100)
	,@fNoFakturDiRetur nvarchar(100)
	,@fTglFaktur nvarchar(100)
	,@fNomorRetur nvarchar(100)
	,@fMasaRetur nvarchar(100)
	,@fTahunRetur nvarchar(100)
	,@fDpp nvarchar(100)
	,@fPpn nvarchar(100)
	,@fPpnBm nvarchar(100)
	,@fUserName nvarchar(100)
AS
BEGIN
	DECLARE
		@headerTemplate nvarchar(max)

	DECLARE
	@tempTable TABLE (
		ReturId varchar(100),
		NPWP varchar(100),
		NAMA varchar(100),
		KD_JENIS_TRANSAKSI varchar(2),
		FG_PENGGANTI varchar(1),
		NOMOR_FAKTUR varchar(100),
		TANGGAL_FAKTUR varchar(25),
		NOMOR_DOKUMEN_RETUR varchar(100),
		IS_CREDITABLE varchar(1),
		TANGGAL_RETUR varchar(25),
		MASA_PAJAK_RETUR varchar(1),
		TAHUN_PAJAK_RETUR varchar(4),
		NILAI_RETUR_DPP varchar(100),
		NILAI_RETUR_PPN varchar(100),
		NILAI_RETUR_PPNBM varchar(100),
		Created datetime
	)

	SET @headerTemplate = 'RM,NPWP,NAMA,KD_JENIS_TRANSAKSI,FG_PENGGANTI,NOMOR_FAKTUR,TANGGAL_FAKTUR,IS_CREDITABLE,NOMOR_DOKUMEN_RETUR,TANGGAL_RETUR,MASA_PAJAK_RETUR,TAHUN_PAJAK_RETUR,NILAI_RETUR_DPP,NILAI_RETUR_PPN,NILAI_RETUR_PPNBM'
	INSERT INTO @tempTable(ReturId,NPWP, NAMA, KD_JENIS_TRANSAKSI,FG_PENGGANTI,NOMOR_FAKTUR,TANGGAL_FAKTUR,NOMOR_DOKUMEN_RETUR,IS_CREDITABLE,TANGGAL_RETUR,MASA_PAJAK_RETUR,TAHUN_PAJAK_RETUR,NILAI_RETUR_DPP,NILAI_RETUR_PPN,NILAI_RETUR_PPNBM,Created)
	SELECT	CONVERT(VARCHAR(100),r.FakturPajakReturId) as ReturId,
			CONVERT(VARCHAR(100),r.NPWPPenjual) AS NPWP,
			REPLACE(CONVERT(VARCHAR(100),r.NamaPenjual), ',', ' ') AS NAMA,
			CONVERT(VARCHAR(2),r.KdJenisTransaksi) AS KD_JENIS_TRANSAKSI,
			CONVERT(VARCHAR(1), r.FgPengganti) AS FG_PENGGANTI,
			CONVERT(VARCHAR(100),r.NoFakturPajak) AS NOMOR_FAKTUR,
			CONVERT(VARCHAR(25),r.TglFaktur,103) AS TANGGAL_FAKTUR,		
			CONVERT(VARCHAR(100), r.NoDocRetur) AS NOMOR_DOKUMEN_RETUR,
			CONVERT(VARCHAR(10),r.Dikreditkan) AS IS_CREDITABLE,
			CONVERT(VARCHAR(25),r.TglRetur,103) AS TANGGAL_RETUR,
			CONVERT(VARCHAR(100),r.MasaPajakLapor) AS MASA_PAJAK_RETUR,
			CONVERT(VARCHAR(4),r.TahunPajakLapor) AS TAHUN_PAJAK_RETUR,
			CONVERT(VARCHAR(100), CONVERT(bigint,FLOOR(ISNULL(r.JumlahDPP,0)))) AS NILAI_RETUR_DPP,
			CONVERT(VARCHAR(100), CONVERT(bigint,FLOOR(r.JumlahPPN)))  AS NILAI_RETUR_PPN,
			CONVERT(VARCHAR(100), CONVERT(bigint,FLOOR(r.JumlahPPNBM))) AS NILAI_RETUR_PPNBM,
			r.Created
	FROM	FakturPajakRetur r 
	WHERE r.[IsDeleted] = 0
		AND (@NoDocRetur IS NULL OR (@NoDocRetur IS NOT NULL AND LOWER(r.[NoDocRetur]) LIKE REPLACE(LOWER(@NoDocRetur), '*', '%')))
		AND (@NoFakturPajak IS NULL OR (@NoFakturPajak IS NOT NULL AND LOWER(REPLACE(REPLACE(r.[FormatedNoFakturPajak],'.',''),'-','')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NoFakturPajak, '.',''),'-','')), '*', '%')))
		AND (@NpwpVendor IS NULL OR
			(@NpwpVendor IS NOT NULL AND LOWER(REPLACE(REPLACE([FormatedNpwpPenjual], '.', ''), '-', '')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NpwpVendor, '.',''),'-','')), '*', '%'))
			)
		AND (
			(@NamaVendor IS NOT NULL AND LOWER([NamaPenjual]) LIKE REPLACE(LOWER(@NamaVendor), '*', '%'))
			OR @NamaVendor IS NULL
			)
		AND (CAST(r.[TglRetur] AS DATE) BETWEEN CAST(isnull(@TglFakturReturStart, r.[TglRetur]) AS DATE) AND CAST(isnull(@TglFakturReturEnd, r.[TglRetur]) AS DATE))
		AND (@MasaPajak IS NULL OR (@MasaPajak IS NOT NULL AND r.MasaPajakLapor = @MasaPajak))
		AND (@TahunPajak IS NULL OR (@TahunPajak IS NOT NULL AND r.TahunPajakLapor = @TahunPajak))
		AND (@fTglRetur IS NULL OR (@fTglRetur IS NOT NULL AND CONVERT(VARCHAR,r.TglRetur, 103) LIKE REPLACE(@fTglRetur, '*', '%')))
		AND (@fNpwpVendor IS NULL OR (@fNpwpVendor IS NOT NULL AND r.FormatedNpwpPenjual LIKE REPLACE(@fNpwpVendor,'*', '%')))
		AND (@fNamaVendor IS NULL OR (@fNamaVendor IS NOT NULL AND r.NamaPenjual LIKE REPLACE(@fNamaVendor, '*', '%')))
		AND (@fNoFakturDiRetur IS NULL OR (@fNoFakturDiRetur IS NOT NULL AND r.FormatedNoFakturPajak LIKE REPLACE(@fNoFakturDiRetur, '*', '%')))
		AND (@fTglFaktur IS NULL OR (@fTglFaktur IS NOT NULL AND CONVERT(varchar, r.TglFaktur, 103) LIKE REPLACE(@fTglFaktur,'*', '%')))
		AND (@fNomorRetur IS NULL OR (@fNomorRetur IS NOT NULL AND r.NoDocRetur LIKE REPLACE(@fNomorRetur,'*', '%')))
		AND (@fMasaRetur IS NULL OR (@fMasaRetur IS NOT NULL AND CAST(r.MasaPajakLapor as varchar(100)) LIKE REPLACE(@fMasaRetur, '*', '%')))
		AND (@fTahunRetur IS NULL OR (@fTahunRetur IS NOT NULL AND CAST(r.TahunPajakLapor as varchar(100)) LIKE REPLACE(@fTahunRetur, '*', '%')))
		AND (@fDpp IS NULL OR (@fDpp IS NOT NULL AND CAST(r.JumlahDPP as varchar(100)) LIKE REPLACE(@fDpp, '*', '%')))
		AND (@fPpn IS NULL OR (@fPpn IS NOT NULL AND CAST(r.JumlahPPN as varchar(100)) LIKE REPLACE(@fPpn, '*', '%')))
		AND (@fPpnBm IS NULL OR (@fPpnBm IS NOT NULL AND CAST(r.JumlahPPNBM as varchar(100)) LIKE REPLACE(@fPpnBm, '*', '%')))
		AND (@fUserName IS NULL OR (@fUserName IS NOT NULL AND r.CreatedBy LIKE REPLACE(@fUserName, '*', '%')))

	SELECT @headerTemplate AS RowData, '' AS RowData2, '' AS RowData3, 'A' AS Marker
	UNION ALL
	SELECT	('RM' 
			+','+ NPWP
			+','+ NAMA
			+','+ KD_JENIS_TRANSAKSI 
			+','+ FG_PENGGANTI
			+','+ NOMOR_FAKTUR		
			+','+ TANGGAL_FAKTUR
			+','+ IS_CREDITABLE
			+','+ NOMOR_DOKUMEN_RETUR 
			+','+ TANGGAL_RETUR		 
			+','+ MASA_PAJAK_RETUR
			+','+ TAHUN_PAJAK_RETUR		 
			+','+ NILAI_RETUR_DPP
			+','+ NILAI_RETUR_PPN 
			+','+ NILAI_RETUR_PPNBM)  as RowData,
			('') as RowData2,(ReturId) as RowData3, 
			'B' as Marker
	FROM	@tempTable

END




GO
/****** Object:  StoredProcedure [dbo].[sp_VendorProcessUpload]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_VendorProcessUpload]
	-- Add the parameters for the stored procedure here
	@paramTable Vendor_ParamTable READONLY	
AS
BEGIN
	INSERT INTO [dbo].[Vendor]
           ([NPWP]
           ,[Nama]
           ,[Alamat]
           ,[CreatedBy]           
           ,[PkpDicabut]
           ,[TglPkpDicabut]
           ,[KeteranganTambahan])
	SELECT	v.[NPWP]
           ,v.[Nama]
           ,v.[Alamat]
		   ,v.[UserNameLogin]
           ,CASE WHEN LOWER(v.[PkpDicabut]) = 'ya' OR LOWER(v.[PkpDicabut]) = 'y' THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [PkpDicabut]
           ,CASE WHEN v.[TglPkpDicabut] IS NULL OR v.[TglPkpDicabut] = '' THEN NULL ELSE CAST(v.[TglPkpDicabut] AS DATE) END AS [TglPkpDicabut]
           ,CASE WHEN v.[KeteranganTambahan] IS NULL OR v.[KeteranganTambahan] = '' THEN NULL ELSE v.[KeteranganTambahan] END AS [KeteranganTambahan]
	FROM	@paramTable v
END





GO
/****** Object:  StoredProcedure [dbo].[usp_InsertErrorLog]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[usp_InsertErrorLog] 
AS
-- Declaration statements

DECLARE @ErrorNumber int
DECLARE @ErrorMessage varchar(4000)
DECLARE @ErrorSeverity int
DECLARE @ErrorState int
DECLARE @ErrorProcedure varchar(200)
DECLARE @ErrorLine int

-- Initialize variables
SELECT @ErrorNumber = isnull(error_number(),0),
@ErrorMessage = isnull(error_message(),'NULL Message'),
@ErrorSeverity = isnull(error_severity(),0),
@ErrorState = isnull(error_state(),1),
@ErrorLine = isnull(error_line(), 0),
@ErrorProcedure = isnull(error_procedure(),'')

-- Insert into the dbo.ErrorHandling table
INSERT INTO dbo.ErrorLog 
(
ErrorNumber, 
ErrorMessage, 
ErrorSeverity, 
ErrorState, 
ErrorProcedure,
ErrorLine
)
VALUES 
(
@ErrorNumber, 
@ErrorMessage, 
@ErrorSeverity, 
@ErrorState, 
@ErrorProcedure, 
@ErrorLine
)




GO
/****** Object:  UserDefinedFunction [dbo].[fnCompEvisVsIwsGetStatusCompare]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnCompEvisVsIwsGetStatusCompare]
(
	@TaxInvoiceNumberEvis nvarchar(25),
	@TaxInvoiceNumberIws nvarchar(25),
	@VatAmountScanned decimal,
	@VatAmountIws decimal,
	@DiffToleranConfig decimal
)
RETURNS nvarchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @amtDiff decimal
	
	SET @amtDiff = ABS(ISNULL(@VatAmountScanned, 0) - ISNULL(@VatAmountIws, 0))
	DECLARE @strRet nvarchar(100) = 'OK'

	-- Add the T-SQL statements to compute the return value here
	IF @TaxInvoiceNumberEvis IS NULL OR @TaxInvoiceNumberIws IS NULL
	BEGIN
		SET @strRet = 'Missed'
	END
	ELSE
	BEGIN
		IF @TaxInvoiceNumberEvis = @TaxInvoiceNumberIws
		BEGIN
			IF @amtDiff >= @DiffToleranConfig
			BEGIN
				SET @strRet = 'NOT OK'
			END
		ELSE
			BEGIN
				SET @strRet = 'NOT OK'
			END
		END	
	END
	
	-- Return the result of the function
	RETURN @strRet

END




GO
/****** Object:  UserDefinedFunction [dbo].[fnCompEvisVsSapGetDocStatus]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnCompEvisVsSapGetDocStatus]
(
	-- Add the parameters for the function here
	@TaxInvoiceNumberEvis nvarchar(100)
	,@TaxInvoiceNumberSap nvarchar(100)
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE
		@toRet int = 1
	
	IF (@TaxInvoiceNumberEvis <> '' AND @TaxInvoiceNumberSap <> '') 
		OR (@TaxInvoiceNumberEvis IS NOT NULL AND @TaxInvoiceNumberSap IS NOT NULL)
	BEGIN
		SET @toRet = 1 -- both exists
	END
	ELSE 
	BEGIN
		IF (@TaxInvoiceNumberEvis = '' AND @TaxInvoiceNumberSap <> '')
			OR (@TaxInvoiceNumberEvis IS NULL AND @TaxInvoiceNumberSap IS NOT NULL)
		BEGIN
			SET @toRet = 2 -- document in problem
		END
		ELSE
		BEGIN
			IF (@TaxInvoiceNumberEvis <> '' AND @TaxInvoiceNumberSap = '')
				OR ((@TaxInvoiceNumberEvis IS NOT NULL AND @TaxInvoiceNumberSap IS NULL))
			BEGIN
				SET @toRet = 3 -- document outstanding
			END
			ELSE
			BEGIN
				SET @toRet = 0 -- both not exists, imposible in select result
			END
		END
	END

	-- Return the result of the function
	RETURN @toRet

END




GO
/****** Object:  UserDefinedFunction [dbo].[fnCompEvisVsSapGetStatusCompare]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnCompEvisVsSapGetStatusCompare]
(
	 @TaxInvoiceNumberEvis nvarchar(25)
	,@TaxInvoiceNumberSap nvarchar(25)
	,@AmountEvis decimal(18,0)
	,@AmountSap decimal(18,0)
	,@DiffToleranConfig decimal(18,0)
	,@itemText nvarchar(100)
)
RETURNS nvarchar(100)
AS
BEGIN
	
	-- Declare the return variable here
	DECLARE @amtDiff decimal
	
	SET @amtDiff = ABS(ISNULL(@AmountEvis, 0) - ISNULL(@AmountSap, 0))
	DECLARE @strRet nvarchar(100) = 'OK'

	-- Add the T-SQL statements to compute the return value here
	IF @TaxInvoiceNumberEvis IS NULL OR @TaxInvoiceNumberSap IS NULL
	BEGIN
		SET @strRet = 'NOT OK'
	END
	ELSE
	BEGIN
		IF @TaxInvoiceNumberEvis = @TaxInvoiceNumberSap
		BEGIN			
			IF @amtDiff <> 0
			BEGIN
				IF @amtDiff > @DiffToleranConfig
				BEGIN
					SET @strRet = 'NOT OK'
				END
			END
		END
		ELSE
		BEGIN
			SET @strRet = 'NOT OK'
		END
	END
	
	IF @strRet = 'OK'
	BEGIN
		DECLARE @isValid bit = dbo.fnSapMtDownloadCheckValiditas(@itemText)
		IF @isValid = 0
		BEGIN
			SET @strRet = 'NOT OK'
		END
	END

	-- Return the result of the function
	RETURN @strRet

END




GO
/****** Object:  UserDefinedFunction [dbo].[fnConvertSapStringToDate]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnConvertSapStringToDate]
(
	@inputString nvarchar(8)
)
RETURNS date
AS
BEGIN
	
	DECLARE @oRet date = NULL
			,@sYear nvarchar(4)
			,@sMonth nvarchar(2)
			,@sDay nvarchar(2)
			,@iYear int
			,@iMonth int
			,@iDay int
			,@cleanInputString nvarchar(10)
	
	SET @cleanInputString = LTRIM(RTRIM(REPLACE(REPLACE(@inputString,'.',''),'-','')))
	IF LEN(@cleanInputString) = 8
	BEGIN
		SET @sYear = LTRIM(RTRIM(SUBSTRING(@cleanInputString, 1, 4)))
		SET @sMonth = LTRIM(RTRIM(SUBSTRING(@cleanInputString, 5, 2)))
		SET @sDay = LTRIM(RTRIM(SUBSTRING(@cleanInputString, 7, 2)))

		IF ISNUMERIC(@sYear) = 1 AND ISNUMERIC(@sMonth) = 1 AND ISNUMERIC(@sDay) = 1
		BEGIN
			SET @iYear = CAST(@sYear as int)
			SET @iMonth = CAST(@sMonth as int)
			SET @iDay = CAST(@sDay as int)
						
			IF @iMonth = 12
			BEGIN
				IF @iMonth <= 12 AND @iMonth > 0 AND DAY(DATEADD(day, -1, DATEFROMPARTS(@iYear - 1, 1, 1))) >= @iDay
				BEGIN
					SET @oRet = DATEFROMPARTS(@sYear, @sMonth, @sDay)						
				END
			END
			ELSE
			BEGIN
				IF @iMonth <= 12 AND @iMonth > 0 AND DAY(DATEADD(day, -1, DATEFROMPARTS(@iYear, @iMonth + 1, 1))) >= @iDay
				BEGIN
					SET @oRet = DATEFROMPARTS(@sYear, @sMonth, @sDay)
				END
			END	

		END

	END
	RETURN @oRet

END




GO
/****** Object:  UserDefinedFunction [dbo].[fnGenerateDocumentNoRetur]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnGenerateDocumentNoRetur] 
(
	-- Add the parameters for the function here
	@tahunRetur int
)
RETURNS nvarchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE 
		@toRet nvarchar(100) = ''
		,@prefix nvarchar(20) = 'ADM/RETUR/' + CAST(@tahunRetur as nvarchar(4)) + '/'
		,@lastDocNo int
		,@newDocNo nvarchar(100)
	
	SET @lastDocNo = (
		SELECT	ISNULL(MAX(v.DocNumber), 0)
		FROM	(
			SELECT	CAST(REPLACE(NoDocRetur, @prefix, '') as int) AS DocNumber
			FROM	FakturPajakRetur
			WHERE	NoDocRetur LIKE @prefix + '%'					
		) v
	)
	
	SET @toRet = @prefix + RIGHT('00000000' + CAST((CAST(REPLACE(@lastDocNo, @prefix, '') AS int) + 1) AS NVARCHAR(20)), 8)

	RETURN @toRet
	
END



GO
/****** Object:  UserDefinedFunction [dbo].[fnGenerateFillingIndex]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnGenerateFillingIndex]
(
	@tahunPajak int,
	@masaPajak int,
	@userInitial nvarchar(20)
)
RETURNS nvarchar(20)
AS
BEGIN
	
	DECLARE @toRet nvarchar(20) = '';

	DECLARE @prefix nvarchar(20) = CAST(@tahunPajak AS nvarchar(4)) + RIGHT('00' + CAST(@masaPajak AS nvarchar(2)), 2) + '-' + @userInitial + '-';
	
	DECLARE @lastFillingIndex int = 1
	
	SET @lastFillingIndex = (SELECT	MAX(CAST(RIGHT(FillingIndex, 5) AS int)) AS MaxOrdner
FROM	FakturPajak
WHERE	FillingIndex IS NOT NULL AND FillingIndex LIKE @prefix + '%'								
							)
	IF @lastFillingIndex IS NULL
	BEGIN
		SET @lastFillingIndex = 1	
	END	
	ELSE
	BEGIN
		SET @lastFillingIndex = @lastFillingIndex + 1
	END

	SET @toRet = @prefix + RIGHT('0000' + CAST(@lastFillingIndex AS nvarchar(20)), 5)

	RETURN @toRet;

END




GO
/****** Object:  UserDefinedFunction [dbo].[fnGenerateFillingIndexByCreatedBy]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnGenerateFillingIndexByCreatedBy]
(
	@scanType int,
	@StatusApproval nvarchar(100),
	@tahunPajak int,
	@masaPajak int,
	@createdBy nvarchar(100) -- Created User (Username at table User)
)
RETURNS varchar(20)
AS
BEGIN
	
	--Scan Type Satuan, dengan status approval DJP valid
	IF @scanType = 1 AND LOWER(@StatusApproval) <> 'faktur valid, sudah diapprove oleh djp'
	BEGIN
		RETURN NULL;
	END

	DECLARE @toRet varchar(20) = '';
	DECLARE @userInitial varchar(5) = (SELECT TOP(1) UserInitial FROM dbo.[User] WHERE UserName = @createdBy);

	DECLARE @prefix varchar(13) = CAST(@tahunPajak AS varchar(4)) + RIGHT('00' + CAST(@masaPajak AS nvarchar(2)), 2) + '-' + @userInitial + '-';
	
	DECLARE @lastFillingIndex int = 1
	
	SET @lastFillingIndex = (SELECT	MAX(CAST(RIGHT(FillingIndex, 5) AS int)) AS MaxOrdner
		FROM	FakturPajak
		WHERE	FillingIndex IS NOT NULL AND FillingIndex LIKE @prefix + '%'								
							)
	IF @lastFillingIndex IS NULL
	BEGIN
		SET @lastFillingIndex = 1	
	END	
	ELSE
	BEGIN
		SET @lastFillingIndex = @lastFillingIndex + 1
	END

	SET @toRet = @prefix + RIGHT('0000' + CAST(@lastFillingIndex AS varchar(20)), 5)

	RETURN @toRet;

END



GO
/****** Object:  UserDefinedFunction [dbo].[fnIsReconcileByItemText]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnIsReconcileByItemText]
(
	-- Add the parameters for the function here
	@ItemText nvarchar(100)
)
RETURNS bit
AS
BEGIN
	DECLARE
		@toRet bit = 0
		,@noFaktur nvarchar(20)
		,@countCheck int = 0

	SET @noFaktur = LTRIM(RTRIM(SUBSTRING(@ItemText, 10, LEN(@ItemText))))

	SET @countCheck = (
		SELECT	COUNT(FakturPajakId)
		FROM	FakturPajak 
		WHERE	IsDeleted = 0 AND FormatedNoFaktur = @noFaktur AND StatusReconcile = 1
		)

	IF @countCheck > 0 
	BEGIN
		SET @toRet = 1
	END

	RETURN @toRet;
	
END




GO
/****** Object:  UserDefinedFunction [dbo].[fnSapMtDownloadCheckStatusCompareByKey]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnSapMtDownloadCheckStatusCompareByKey]
(
	-- Add the parameters for the function here
	@postingDate date,
	@accountingDocNo nvarchar(255),
	@itemNo nvarchar(255)
)
RETURNS nvarchar(100)
AS
BEGIN
	DECLARE @toRet nvarchar(100) = ''

	DECLARE @checkCountData int = (	SELECT COUNT(Id) FROM COMP_EVIS_SAP 
									WHERE CAST(PostingDate as date) = CAST(@postingDate as date)
									AND AccountingDocNo = @accountingDocNo AND ItemNo = @itemNo)

	IF @checkCountData > 0
	BEGIN
		DECLARE @currentStatusCompare nvarchar(100) = (SELECT TOP 1 StatusCompare FROM COMP_EVIS_SAP
													   WHERE CAST(PostingDate as date) = CAST(@postingDate as date)
														AND AccountingDocNo = @accountingDocNo AND ItemNo = @itemNo
														ORDER BY ID DESC)
		SET @toRet = @currentStatusCompare
	END

	RETURN @toRet
END




GO
/****** Object:  UserDefinedFunction [dbo].[fnSapMtDownloadCheckValiditas]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnSapMtDownloadCheckValiditas]
(
	@itemText nvarchar(225)
)
RETURNS bit
AS
BEGIN
	
	DECLARE @tempTable TABLE
	(
		RowIdx int identity(1,1) NOT NULL,
		DataId bigint,
		PostingDate date,
		AmountLocal decimal(18,0)
	)

	INSERT INTO @tempTable(DataId, PostingDate, AmountLocal)
	SELECT	Id, PostingDate, CASE WHEN RTRIM(LTRIM(AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(AmountLocal)) AS decimal(18,0)) END AS AmountLocal
	FROM	SAP_MTDownloadPPN
	WHERE	ItemText = @itemText
	
	DECLARE @dataCount int = (SELECT COUNT(RowIdx) FROM @tempTable)

	DECLARE @toRet bit = 1
	DECLARE @amtFirst decimal(18,0)
	DECLARE @amtSecond decimal(18,0)
	DECLARE @amtThird decimal(18,0)

	IF @dataCount = 1
	BEGIN
		
		SET @amtFirst = (SELECT AmountLocal FROM @tempTable WHERE RowIdx = 1)
		-- only plus that valid
		IF @amtFirst < 0 
		BEGIN
			SET @toRet = 0
		END
	ELSE IF @dataCount = 2
		BEGIN
			 SET @amtFirst = (SELECT AmountLocal FROM @tempTable WHERE RowIdx = 1) 
			 SET @amtSecond = (SELECT AmountLocal FROM @tempTable WHERE RowIdx = 2)

			-- minus, plus : not valid
			IF @amtFirst < 0 AND @amtSecond > 0
			BEGIN
				SET @toRet = 0
			END
		END
	ELSE IF @dataCount = 3
		BEGIN
			SET @amtFirst = (SELECT AmountLocal FROM @tempTable WHERE RowIdx = 1) 
			SET @amtSecond = (SELECT AmountLocal FROM @tempTable WHERE RowIdx = 2)
			SET @amtThird  = (SELECT AmountLocal FROM @tempTable WHERE RowIdx = 3)

			-- plus, minus, plus : valid
			IF NOT (@amtFirst > 0 AND @amtSecond < 0 AND @amtThird > 0 AND ABS(@amtFirst) = ABS(@amtSecond)) 
			BEGIN
				SET @toRet = 0
			END
		END
	ELSE IF @dataCount > 3
		BEGIN
			SET @amtFirst = (SELECT AmountLocal AS DecAmtLocal FROM @tempTable WHERE RowIdx = 1) 
			IF @amtFirst < 0 
			BEGIN
				SET @toRet = 0
			END
		END
	END

	RETURN @toRet

END




GO
/****** Object:  UserDefinedFunction [dbo].[FormatNoFaktur]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[FormatNoFaktur]
(
	@fpType int,
	@NoFakturOrigin nvarchar(100),
	@KdJenisTransaksi nvarchar(2),
	@FgPengganti nvarchar(2)
)
RETURNS nvarchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @toRet nvarchar(100);
	IF @fpType = 3 
	BEGIN
		RETURN @NoFakturOrigin;
	END

	SET @toRet = @KdJenisTransaksi + @FgPengganti + '.' + left(@NoFakturOrigin, 3) + '-' 
	+ substring(@NoFakturOrigin, 4, 2) + '.' + right(@NoFakturOrigin, 8)


	RETURN @toRet;
END




GO
/****** Object:  UserDefinedFunction [dbo].[FormatNpwp]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[FormatNpwp]
(
	@NpwpOrigin nvarchar(100)
)
RETURNS nvarchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @toRet nvarchar(100);

	IF @NpwpOrigin = ''
	BEGIN
		SET @toRet = NULL
	END
	ELSE
	BEGIN
		SET @toRet = left(@NpwpOrigin, 2) + '.' + substring(@NpwpOrigin, 3,3) 
+ '.' + SUBSTRING(@NpwpOrigin, 6, 3) + '.' + SUBSTRING(@NpwpOrigin, 9,1) + '-' 
+ SUBSTRING(@NpwpOrigin, 10, 3) + '.' + RIGHT(@NpwpOrigin, 3);
	END
	


	RETURN @toRet;
END




GO
/****** Object:  UserDefinedFunction [dbo].[GetMonth]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[GetMonth]
(	
	
)
RETURNS @ReturnValue TABLE ( [MonthNumber] int, [MonthName] NVARCHAR(100) ) 
AS
BEGIN
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (1, 'Januari');
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (2, 'Februari');
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (3, 'Maret');
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (4, 'April');
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (5, 'Mei');
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (6, 'Juni');
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (7, 'Juli');
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (8, 'Agustus');
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (9, 'September');
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (10, 'Oktober');
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (11, 'November');
	INSERT  INTO @ReturnValue([MonthNumber],[MonthName])
	VALUES (12, 'Desember');
    RETURN  
END;



GO
/****** Object:  UserDefinedFunction [dbo].[Split]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[Split]
    (
      @RowData NVARCHAR(MAX) 
      
    )
RETURNS @ReturnValue TABLE ( Data NVARCHAR(MAX) )
AS 
    BEGIN
		DECLARE @SplitOn NVARCHAR(5) = ','
        DECLARE @Counter INT
        SET @Counter = 1 
        WHILE ( CHARINDEX(@SplitOn, @RowData) > 0 ) 
            BEGIN  
                INSERT  INTO @ReturnValue
                        ( data
                        )
                        SELECT  Data = LTRIM(RTRIM(SUBSTRING(@RowData, 1,
                                                             CHARINDEX(@SplitOn,
                                                              @RowData) - 1)))
                SET @RowData = SUBSTRING(@RowData,
                                         CHARINDEX(@SplitOn, @RowData) + 1,
                                         LEN(@RowData)) 
                SET @Counter = @Counter + 1  
            END 
        INSERT  INTO @ReturnValue
                ( data )
                SELECT  Data = LTRIM(RTRIM(@RowData))  
        RETURN  
    END;




GO
/****** Object:  UserDefinedFunction [dbo].[UnFormatNpwp]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[UnFormatNpwp]
(
	@formattedNpwp nvarchar(100)
)
RETURNS nvarchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @toRet nvarchar(100);

	SET @toRet = REPLACE(REPLACE(@formattedNpwp, '.',''),'-','')


	RETURN @toRet;
END





GO
/****** Object:  Table [dbo].[Activity]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Activity](
	[ActivityId] [int] IDENTITY(1,1) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[ActivityName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED 
(
	[ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[COMP_EVIS_IWS]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[COMP_EVIS_IWS](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ReceivedDate] [date] NOT NULL,
	[VendorCode] [nvarchar](max) NULL,
	[VendorName] [nvarchar](max) NULL,
	[TaxInvoiceNumberEVIS] [nvarchar](max) NULL,
	[TaxInvoiceNumberIWS] [nvarchar](max) NULL,
	[InvoiceNumber] [nvarchar](max) NULL,
	[VATAmountScanned] [decimal](18, 2) NULL,
	[VATAmountIWS] [decimal](18, 2) NULL,
	[VATAmountDiff] [decimal](18, 2) NULL,
	[StatusDJP] [nvarchar](max) NULL,
	[StatusCompare] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[LastUpdatedOnIws] [datetime] NULL,
	[ScanDate] [datetime] NULL,
	[ScanUserName] [nvarchar](100) NULL,
 CONSTRAINT [PK_COMP_EVIS_IWS] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[COMP_EVIS_SAP]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[COMP_EVIS_SAP](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PostingDate] [date] NULL,
	[AccountingDocNo] [nvarchar](max) NULL,
	[ItemNo] [nvarchar](max) NULL,
	[TglFaktur] [date] NULL,
	[TaxInvoiceNumberEVIS] [nvarchar](100) NULL,
	[TaxInvoiceNumberSAP] [nvarchar](100) NULL,
	[DocumentHeaderText] [nvarchar](max) NULL,
	[NPWP] [nvarchar](100) NULL,
	[AmountEVIS] [decimal](18, 2) NULL,
	[AmountSAP] [decimal](18, 2) NULL,
	[AmountDiff] [decimal](18, 2) NULL,
	[StatusCompare] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[IdNo] [nvarchar](max) NOT NULL,
	[Pembetulan] [int] NOT NULL,
	[MasaPajak] [int] NULL,
	[TahunPajak] [int] NULL,
	[ItemText] [nvarchar](100) NULL,
	[FiscalYearDebet] [int] NULL,
	[GLAccount] [nvarchar](100) NULL,
 CONSTRAINT [PK_COMP_EVIS_SAP] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ErrorLog]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ErrorLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ErrorNumber] [int] NOT NULL,
	[ErrorMessage] [varchar](4000) NULL,
	[ErrorSeverity] [smallint] NOT NULL,
	[ErrorState] [smallint] NOT NULL,
	[ErrorProcedure] [varchar](200) NOT NULL,
	[ErrorLine] [int] NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FakturPajak]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FakturPajak](
	[FakturPajakId] [bigint] IDENTITY(1,1) NOT NULL,
	[FCode] [nvarchar](5) NULL,
	[UrlScan] [nvarchar](max) NULL,
	[KdJenisTransaksi] [nvarchar](2) NULL,
	[FgPengganti] [nvarchar](2) NULL,
	[NoFakturPajak] [nvarchar](255) NULL,
	[TglFaktur] [date] NULL,
	[NPWPPenjual] [nvarchar](15) NULL,
	[NamaPenjual] [nvarchar](100) NULL,
	[AlamatPenjual] [nvarchar](max) NULL,
	[VendorId] [int] NULL,
	[NPWPLawanTransaksi] [nvarchar](15) NULL,
	[NamaLawanTransaksi] [nvarchar](100) NULL,
	[AlamatLawanTransaksi] [nvarchar](max) NULL,
	[JumlahDPP] [decimal](18, 2) NULL,
	[JumlahPPN] [decimal](18, 2) NULL,
	[JumlahPPNBM] [decimal](18, 2) NULL,
	[StatusApproval] [nvarchar](100) NULL,
	[StatusFaktur] [nvarchar](50) NULL,
	[Dikreditkan] [bit] NULL,
	[MasaPajak] [int] NOT NULL,
	[TahunPajak] [int] NOT NULL,
	[ReceivingDate] [date] NULL,
	[Pesan] [nvarchar](max) NULL,
	[FPType] [int] NOT NULL,
	[FillingIndex] [nvarchar](20) NULL,
	[ScanType] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[FormatedNoFaktur] [nvarchar](255) NULL,
	[FormatedNpwpPenjual] [nvarchar](100) NULL,
	[FormatedNpwpLawanTransaksi] [nvarchar](100) NULL,
	[Status] [int] NOT NULL,
	[StatusReconcile] [bit] NULL,
	[Referensi] [nvarchar](255) NULL,
	[JenisTransaksi] [nvarchar](255) NULL,
	[JenisDokumen] [nvarchar](255) NULL,
	[NoFakturYangDiganti] [nvarchar](255) NULL,
 CONSTRAINT [PK_FakturPajak] PRIMARY KEY CLUSTERED 
(
	[FakturPajakId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FakturPajakDetail]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FakturPajakDetail](
	[FakturPajakDetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[FakturPajakId] [bigint] NOT NULL,
	[Nama] [nvarchar](max) NOT NULL,
	[HargaSatuan] [decimal](18, 2) NOT NULL,
	[JumlahBarang] [decimal](18, 2) NOT NULL,
	[HargaTotal] [decimal](18, 2) NOT NULL,
	[Diskon] [decimal](18, 2) NOT NULL,
	[Dpp] [decimal](18, 2) NOT NULL,
	[Ppn] [decimal](18, 2) NOT NULL,
	[TarifPpnbm] [decimal](18, 2) NOT NULL,
	[Ppnbm] [decimal](18, 2) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_FakturPajakDetail] PRIMARY KEY CLUSTERED 
(
	[FakturPajakDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FakturPajakRetur]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FakturPajakRetur](
	[FakturPajakReturId] [bigint] IDENTITY(1,1) NOT NULL,
	[FCode] [nvarchar](5) NOT NULL,
	[FakturPajakId] [bigint] NOT NULL,
	[NoDocRetur] [nvarchar](50) NOT NULL,
	[TglRetur] [date] NOT NULL,
	[MasaPajakLapor] [int] NOT NULL,
	[TahunPajakLapor] [int] NOT NULL,
	[JumlahDPP] [decimal](18, 2) NULL,
	[JumlahPPN] [decimal](18, 2) NULL,
	[JumlahPPNBM] [decimal](18, 2) NULL,
	[Pesan] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[NPWPPenjual] [nvarchar](15) NULL,
	[NamaPenjual] [nvarchar](100) NULL,
	[AlamatPenjual] [nvarchar](max) NULL,
	[FormatedNoFakturPajak] [varchar](100) NULL,
	[FormatedNpwpPenjual] [nvarchar](100) NULL,
	[KdJenisTransaksi] [nvarchar](2) NULL,
	[FgPengganti] [nvarchar](2) NULL,
	[NoFakturPajak] [nvarchar](25) NULL,
	[TglFaktur] [date] NULL,
	[Dikreditkan] [bit] NULL,
 CONSTRAINT [PK_FakturPajakRetur] PRIMARY KEY CLUSTERED 
(
	[FakturPajakReturId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FPJenisDokumen]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FPJenisDokumen](
	[Id] [nvarchar](2) NOT NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_FPJenisDokumen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FPJenisTransaksi]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FPJenisTransaksi](
	[Id] [nvarchar](2) NOT NULL,
	[FCode] [nvarchar](2) NOT NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_FPJenisTransaksi] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FPKdJenisTransaksi]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FPKdJenisTransaksi](
	[Id] [nvarchar](2) NOT NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_FPKdJenisTransaksi] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GeneralConfig]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GeneralConfig](
	[GeneralConfigId] [int] NOT NULL,
	[ConfigKey] [nvarchar](255) NOT NULL,
	[ConfigValue] [nvarchar](255) NOT NULL,
	[ConfigExtra] [nvarchar](255) NULL,
	[ConfigDesc] [nvarchar](255) NULL,
 CONSTRAINT [PK_GeneralConfig] PRIMARY KEY CLUSTERED 
(
	[GeneralConfigId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogDownload]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogDownload](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Requestor] [nvarchar](100) NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[FilePath] [nvarchar](max) NOT NULL,
	[RequestDate] [datetime] NOT NULL,
	[FileType] [nvarchar](255) NOT NULL,
	[ClientIp] [nvarchar](50) NULL,
 CONSTRAINT [PK_LogDownload] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogPrintFakturPajak]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogPrintFakturPajak](
	[LogPrintId] [bigint] IDENTITY(1,1) NOT NULL,
	[FakturPajakId] [bigint] NOT NULL,
	[PrintType] [nvarchar](50) NOT NULL,
	[PrintDate] [datetime] NOT NULL,
	[PrintBy] [nvarchar](100) NOT NULL,
	[Reason] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[HeaderGuid] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_LogPrintFakturPajak] PRIMARY KEY CLUSTERED 
(
	[LogPrintId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogProcessSap]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogProcessSap](
	[LogProcessSapId] [bigint] IDENTITY(1,1) NOT NULL,
	[IdNo] [nvarchar](max) NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[FilePath] [nvarchar](max) NOT NULL,
	[Note] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[XmlFileType] [nvarchar](100) NULL,
 CONSTRAINT [PK_LogProcessSap] PRIMARY KEY CLUSTERED 
(
	[LogProcessSapId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogRequestFakturPajak]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogRequestFakturPajak](
	[LogRequestFakturPajakId] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestDate] [datetime] NOT NULL,
	[RequestUrl] [nvarchar](max) NOT NULL,
	[FakturPajakId] [bigint] NOT NULL,
	[Status] [int] NOT NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_LogRequestFakturPajak] PRIMARY KEY CLUSTERED 
(
	[LogRequestFakturPajakId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogSap]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogSap](
	[LogSapId] [bigint] IDENTITY(1,1) NOT NULL,
	[TransactionType] [nvarchar](50) NOT NULL,
	[IdNo] [nvarchar](max) NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[LocalExecution] [datetime] NULL,
	[LocalPath] [nvarchar](max) NOT NULL,
	[TransferDate] [datetime] NULL,
	[SapPath] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[Note] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[AccountingDocNoCredit] [nvarchar](max) NULL,
	[FiscalYearCredit] [int] NULL,
 CONSTRAINT [PK_LogSap] PRIMARY KEY CLUSTERED 
(
	[LogSapId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MapJnsTransaksiKdJnsTransaksi]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MapJnsTransaksiKdJnsTransaksi](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FCode] [nvarchar](2) NOT NULL,
	[KdJenisTransaksi] [nvarchar](2) NOT NULL,
	[JenisTransaksi] [nvarchar](2) NOT NULL,
 CONSTRAINT [PK_MapJnsTransaksiKdJnsTransaksi] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MapJnsTransJnsDok]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MapJnsTransJnsDok](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FCode] [nvarchar](2) NOT NULL,
	[JenisDokumen] [nvarchar](2) NOT NULL,
	[JenisTransaksi] [nvarchar](2) NOT NULL,
 CONSTRAINT [PK_MapJnsTransJnsDok] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Modules]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Modules](
	[ModuleId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ModuleIdParent] [int] NULL,
	[IconUrl] [nvarchar](512) NULL,
	[IconHoverUrl] [nvarchar](512) NULL,
	[Url] [nvarchar](512) NULL,
 CONSTRAINT [PK_Modules] PRIMARY KEY CLUSTERED 
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OpenClosePeriod]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OpenClosePeriod](
	[OpenClosePeriodId] [int] IDENTITY(1,1) NOT NULL,
	[MasaPajak] [int] NOT NULL,
	[TahunPajak] [int] NOT NULL,
	[StatusRegular] [bit] NOT NULL,
	[StatusSP2] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[DocumentSP2] [nvarchar](400) NULL,
 CONSTRAINT [PK_OpenClosePeriod] PRIMARY KEY CLUSTERED 
(
	[OpenClosePeriodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportSPM]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportSPM](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[MasaPajak] [int] NOT NULL,
	[TahunPajak] [int] NOT NULL,
	[Versi] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_SuratPemberitahuanMasa] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportSPMDetail]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportSPMDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ReportSPMId] [bigint] NOT NULL,
	[FCode] [nvarchar](2) NOT NULL,
	[KdJenisTransaksi] [nvarchar](2) NOT NULL,
	[FgPengganti] [nvarchar](2) NOT NULL,
	[NoFakturPajak] [nvarchar](25) NOT NULL,
	[TglFaktur] [date] NOT NULL,
	[NPWPPenjual] [nvarchar](15) NULL,
	[NamaPenjual] [nvarchar](100) NULL,
	[AlamatPenjual] [nvarchar](max) NULL,
	[NPWPLawanTransaksi] [nvarchar](15) NULL,
	[NamaLawanTransaksi] [nvarchar](100) NULL,
	[AlamatLawanTransaksi] [nvarchar](max) NULL,
	[JumlahDPP] [decimal](18, 2) NOT NULL,
	[JumlahPPN] [decimal](18, 2) NOT NULL,
	[JumlahPPNBM] [decimal](18, 2) NOT NULL,
	[KeteranganTambahan] [nvarchar](max) NULL,
	[FgUangMuka] [decimal](18, 2) NULL,
	[UangMukaDPP] [decimal](18, 2) NULL,
	[UangMukaPPN] [decimal](18, 2) NULL,
	[UangMukaPPnBM] [decimal](18, 2) NULL,
	[Referensi] [nvarchar](255) NULL,
	[FillingIndex] [nvarchar](20) NULL,
	[FormatedNoFaktur] [nvarchar](100) NULL,
	[FormatedNpwpPenjual] [nvarchar](100) NULL,
	[FormatedNpwpLawanTransaksi] [nvarchar](100) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[FPCreatedDate] [datetime] NULL,
 CONSTRAINT [PK_ReportSPMDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Role]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RoleActivity]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleActivity](
	[RoleActivityId] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[ActivityId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_RoleActivity] PRIMARY KEY CLUSTERED 
(
	[RoleActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SAP_MTDownloadPPN]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SAP_MTDownloadPPN](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyCode] [nvarchar](max) NULL,
	[AccountingDocNo] [nvarchar](max) NULL,
	[FiscalYear] [nvarchar](max) NULL,
	[DocType] [nvarchar](max) NULL,
	[PostingDate] [nvarchar](max) NULL,
	[AmountLocal] [nvarchar](max) NULL,
	[LineItem] [nvarchar](max) NULL,
	[Reference] [nvarchar](max) NULL,
	[HeaderText] [nvarchar](max) NULL,
	[UserName] [nvarchar](max) NULL,
	[ItemText] [nvarchar](max) NULL,
	[AssignmentNo] [nvarchar](max) NULL,
	[BusinessArea] [nvarchar](max) NULL,
	[GLAccount] [nvarchar](max) NULL,
	[ReferenceLineItem] [nvarchar](max) NULL,
	[DocDate] [nvarchar](max) NULL,
	[Currency] [nvarchar](max) NULL,
	[SalesTaxCode] [nvarchar](max) NULL,
	[AmountDocCurrency] [nvarchar](max) NULL,
	[PostingKey] [nvarchar](max) NULL,
	[ClearingDoc] [nvarchar](max) NULL,
	[ClearingDate] [nvarchar](max) NULL,
	[ReverseDocNo] [nvarchar](max) NULL,
	[ReverseDocFiscalYear] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[ReferenceKey1] [nvarchar](max) NULL,
	[ReferenceKey2] [nvarchar](max) NULL,
	[StatusReconcile] [bit] NULL,
 CONSTRAINT [PK_SAP_MTDownloadPPN] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[State]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[State](
	[StateId] [int] NOT NULL,
	[StateName] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED 
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[ResetPassword] [bit] NULL,
	[BadPasswordCount] [int] NULL,
	[UserInitial] [nvarchar](20) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserAuthentication]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAuthentication](
	[UserAuthenticationId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[IP] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](255) NULL,
	[Token] [nvarchar](255) NOT NULL,
	[TimeStart] [datetime] NOT NULL,
	[TimeEnd] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_UserAuthentication] PRIMARY KEY CLUSTERED 
(
	[UserAuthenticationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[UserRoleId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[UserRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Vendor]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendor](
	[VendorId] [int] IDENTITY(1,1) NOT NULL,
	[NPWP] [nvarchar](15) NOT NULL,
	[Nama] [nvarchar](100) NOT NULL,
	[Alamat] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[PkpDicabut] [bit] NULL,
	[TglPkpDicabut] [date] NULL,
	[KeteranganTambahan] [nvarchar](255) NULL,
	[FormatedNpwp] [nvarchar](100) NULL,
 CONSTRAINT [PK_Vendor] PRIMARY KEY CLUSTERED 
(
	[VendorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  View [dbo].[View_FakturPajak]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-------
------- UPDATE 2017-03-27
CREATE VIEW [dbo].[View_FakturPajak]
AS
SELECT fp.[FakturPajakId]
      ,fp.[FCode]
      ,fp.[UrlScan]
      ,fp.[KdJenisTransaksi]
      ,fp.[FgPengganti]
      ,fp.[NoFakturPajak]
      ,fp.[TglFaktur]
      ,fp.[NPWPPenjual]
      ,fp.[NamaPenjual]
      ,fp.[AlamatPenjual]
      ,fp.[VendorId]
      ,fp.[NPWPLawanTransaksi]
      ,fp.[NamaLawanTransaksi]
      ,fp.[AlamatLawanTransaksi]
      ,fp.[JumlahDPP]
      ,fp.[JumlahPPN]
      ,fp.[JumlahPPNBM]
      ,fp.[StatusApproval]
      ,CASE WHEN fp.FPType = 3 THEN (CASE WHEN fp.FgPengganti = 1 THEN 'Faktur Pajak Normal-Pengganti' ELSE 'Faktur Pajak Normal' END) ELSE fp.[StatusFaktur] END AS [StatusFaktur]
      ,fp.[Dikreditkan]
      ,fp.[MasaPajak]
	  ,m.MonthName AS MasaPajakName
      ,fp.[TahunPajak]
      ,fp.[ReceivingDate]
      ,fp.[Pesan]      
      ,fp.[FPType]
      ,fp.[FillingIndex]
      ,fp.[ScanType]
      ,fp.[IsDeleted]
      ,fp.[Created]
      ,fp.[Modified]
      ,fp.[CreatedBy]
      ,fp.[ModifiedBy]	  
	  ,fp.[FormatedNoFaktur] AS FormatedNoFaktur
	  ,fp.[FormatedNpwpPenjual] AS FormatedNpwpPenjual
	  ,fp.[FormatedNpwpLawanTransaksi] AS FormatedNPWPLawanTransaksi	  
	  ,logreq.ErrorMessage
	  ,fp.[Status]
	  ,s.StateName AS StatusText
	  ,fp.StatusReconcile
	  ,fp.Referensi
	  ,fp.JenisTransaksi
	  ,fp.JenisDokumen
	  ,fp.NoFakturYangDiganti
  FROM [dbo].[FakturPajak] fp 
		INNER JOIN [dbo].GetMonth() m ON fp.MasaPajak = m.MonthNumber 
		LEFT JOIN
		(
			SELECT	r.*, ROW_NUMBER() OVER(PARTITION BY r.FakturPajakId ORDER BY r.RequestDate DESC) AS vSeq
			FROM	dbo.LogRequestFakturPajak r			
		) logreq ON logreq.FakturPajakId = fp.FakturPajakId AND logreq.vSeq = 1 INNER JOIN
		dbo.[State] s ON fp.[Status] = s.StateId






GO
/****** Object:  View [dbo].[vw_DataIWSReqEfis]    Script Date: 4/3/2017 11:49:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_DataIWSReqEfis]
AS
SELECT [INVOICEID]
      ,[RECEIVINGDATE]
      ,[STATUS]
      ,[ACTIVITY]
      ,[INVOICEDATE]
      ,[VENDORID]
      ,[VENDORNAME]
      ,[TAXVOUCHERNO]
      ,[INVOICENO]
      ,[PPN]
	  ,[MODIFIEDON]
  FROM [ADM_IWSTEST].[ADM_IWS].[dbo].[vw_DataIWSReqEfis]
  WHERE [STATUS] = 'Received'






GO
SET IDENTITY_INSERT [dbo].[Activity] ON 

GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (1, 3, N'Input Scan Satuan')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (2, 4, N'Input Scan Bulk')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (3, 5, N'Input Pembetulan')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (4, 7, N'Input Scan Satuan')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (5, 8, N'Input Scan Bulk')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (6, 9, N'Input Pembetulan')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (7, 11, N'Input Faktur Pajak Khusus')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (8, 12, N'Input Pembetulan')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (9, 13, N'View Daftar Faktur Pajak')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (10, 13, N'Create CSV')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (11, 13, N'Download Excel')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (12, 13, N'Delete Faktur Pajak')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (13, 14, N'View Request Detail Transaksi')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (14, 14, N'Send Request Baris Pertama')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (15, 14, N'Send Request All')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (16, 14, N'Retry')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (17, 16, N'View Daftar Faktur Pajak Retur')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (18, 16, N'Create CSV')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (19, 16, N'Download Excel')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (20, 17, N'Input Retur Faktur Pajak')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (21, 19, N'View Compare')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (22, 19, N'Submit Compare')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (23, 19, N'Export To Excel')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (24, 20, N'View Compare')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (25, 20, N'Submit Compare')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (26, 20, N'Force Submit Compare')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (27, 20, N'Export to Excel')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (28, 21, N'View List Ordner')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (29, 21, N'Print')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (30, 21, N'Re-Print')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (31, 22, N'View List Open Period')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (32, 22, N'Add Open Period')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (33, 22, N'Close SP2')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (34, 22, N'Open Regular')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (35, 22, N'Close Regular')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (36, 23, N'View Log')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (37, 23, N'Retry')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (38, 25, N'View List Role')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (39, 25, N'Add New Role')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (40, 25, N'Edit Role')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (41, 25, N'Delete Role')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (42, 26, N'View List User')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (43, 26, N'Add New User')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (44, 26, N'Edit User')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (45, 26, N'Delete User')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (46, 26, N'Reset Password')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (47, 28, N'View List Vendor')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (48, 28, N'Add New Vendor')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (49, 28, N'Upload Vendor')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (50, 28, N'Edit Vendor')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (51, 28, N'Delete Vendor')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (52, 30, N'View')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (53, 30, N'Download Excel')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (54, 31, N'View')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (55, 31, N'Download Excel')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (56, 32, N'View')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (57, 32, N'Download Excel')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (58, 33, N'View')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (59, 33, N'Download Excel')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (60, 34, N'View')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (61, 34, N'Download Excel')
GO
INSERT [dbo].[Activity] ([ActivityId], [ModuleId], [ActivityName]) VALUES (62, 35, N'Create SPM')
GO
SET IDENTITY_INSERT [dbo].[Activity] OFF
GO
INSERT [dbo].[FPJenisDokumen] ([Id], [Description]) VALUES (N'2', N'PIB dan SSP')
GO
INSERT [dbo].[FPJenisDokumen] ([Id], [Description]) VALUES (N'3', N'Surat Setoran Pajak')
GO
INSERT [dbo].[FPJenisDokumen] ([Id], [Description]) VALUES (N'5', N'Dokumen yang Dipersamakan Dengan Faktur Pajak')
GO
INSERT [dbo].[FPJenisDokumen] ([Id], [Description]) VALUES (N'6', N'Dokumen Ekspor (PEB)')
GO
INSERT [dbo].[FPJenisDokumen] ([Id], [Description]) VALUES (N'8', N'PIB')
GO
INSERT [dbo].[FPJenisTransaksi] ([Id], [FCode], [Description]) VALUES (N'1', N'DM', N'Impor BKP dan Pemanfaatan BKP Tidak Berwujud dari Luar Daerah Pabean serta Pemanfaatan JKP dari Luar Daerah Pabean')
GO
INSERT [dbo].[FPJenisTransaksi] ([Id], [FCode], [Description]) VALUES (N'2', N'DM', N'Perolehan BKP/JKP Dalam Negeri')
GO
INSERT [dbo].[FPJenisTransaksi] ([Id], [FCode], [Description]) VALUES (N'3', N'DM', N'Pajak Masukan yang tidak dapat dikreditkan dan/atau Pajak Masukan dan PPn BM yang atas Impor atau Perolehannya mendapat Fasilitas')
GO
INSERT [dbo].[FPJenisTransaksi] ([Id], [FCode], [Description]) VALUES (N'4', N'DK', N'Penyerahan Luar Negeri/Ekspor')
GO
INSERT [dbo].[FPJenisTransaksi] ([Id], [FCode], [Description]) VALUES (N'5', N'DK', N'Penyerahan Dalam Negeri Dengan Dokumen yang Dipersamakan dengan Faktur Pajak')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'01', N'Kepada Pihak yang Bukan Pemungut PPN')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'02', N'Kepada Pemungut Bendaharawan')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'03', N'Kepada Pemungut Selain Bendaharawan')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'04', N'DPP Nilai Lain')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'06', N'Penyerahan Lainnya, termasuk penyerahan kepada turis asing dalam rangka VAT refund')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'07', N'Penyerahan yang PPN-nya Tidak Dipungut')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'08', N'Penyerahan yang PPN-nya Dibebaskan')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'09', N'Penyerahan Aktiva (Pasal 16D UU PPN)')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'10', N'Impor BKP Berwujud')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'11', N'Impor BKP Tidak Berwujud')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'12', N'Impor JKP')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'13', N'Ekspor BKP Berwujud')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'14', N'Ekspor BKP Tidak Berwujud')
GO
INSERT [dbo].[FPKdJenisTransaksi] ([Id], [Description]) VALUES (N'15', N'Ekspor JKP')
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (1, N'TimeSchedulerWatcherInboxService', N'0:00:00', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (2, N'DataFolderWatcherInboxService', N'D:\ADM_EFIS\XML_IN\Inbox', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (3, N'ErrorInboxWatcherInboxService', N'D:\ADM_EFIS\XML_IN\ErrorInbox', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (4, N'LogFolderWatcherInboxService', N'D:\ADM_EFIS\XML_IN\Log', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (5, N'FileExtWatcherInboxService', N'xml', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (6, N'DelimiterWatcherInboxService', N'|', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (7, N'ClientSettingsProviderServiceUriWatcherInboxService', N'', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (8, N'TimeSleepWatcherInboxService', N'100', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (9, N'DataFolder2WatcherInboxService', N'\\10.59.233.21\admevisin\', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (10, N'USERNAMEWatcherInboxService', N'admevis', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (11, N'PASSWORDWatcherInboxService', N'P1ssw0rd', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (12, N'SERVER_ADDRESSWatcherInboxService', N'10.59.233.21', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (13, N'waktujedaWatcherOutboxService', N'10', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (14, N'DestinationFolderWatcherOutboxService', N'D:\ADM_EFIS\XML_OUT\Outbox', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (15, N'LogFolderWatcherOutboxService', N'D:\ADM_EFIS\XML_OUT\Log', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (16, N'FileExtWatcherOutboxService', N'xml', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (17, N'DelimiterWatcherOutboxService', N'|', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (18, N'ClientSettingsProviderServiceUriWatcherOutboxService', N'', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (19, N'TimeSleepWatcherOutboxService', N'100', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (20, N'MaxCopyWatcherOutboxService', N'10', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (21, N'DataFolderWatcherOutboxService', N'\\10.59.233.21\admevisout\', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (22, N'USERNAMEWatcherOutboxService', N'admevis', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (23, N'PASSWORDWatcherOutboxService', N'P1ssw0rd', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (24, N'SERVER_ADDRESSWatcherOutboxService', N'10.59.233.21', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (25, N'TimeSchedulerWatcherService', N'0:00:00', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (26, N'DataFolderWatcherService', N'D:\ADM_EFIS\XML_OUT\Outbox', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (27, N'ResultFolderWatcherService', N'D:\ADM_EFIS\XML_OUT\BackupOutbox', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (28, N'LogFolderWatcherService', N'D:\ADM_EFIS\XML_OUT\Log', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (29, N'FileExtWatcherService', N'xml', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (30, N'DelimiterWatcherService', N'|', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (31, N'ClientSettingsProviderServiceUriWatcherService', N'', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (32, N'TimeSleepWatcherService', N'10', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (33, N'TokenDuration', N'10', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (34, N'MaxBadPasswordCount', N'3', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (35, N'MailHelperSMTPServer', N'', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (36, N'MailHelperPort', N'25', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (37, N'MailHelperEnableSSL', N'FALSE', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (38, N'MailHelperUseDefaultCredential', N'TRUE', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (39, N'MailHelperFromUser', N'', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (40, N'MailHelperPasswd', N'', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (41, N'MailHelperActivateEmail', N'FALSE', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (42, N'DJPRequestTimeOutSetting', N'300000', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (43, N'CompareEvisVsIwsToleransiDiff', N'10000', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (44, N'CompareEvisVsSapToleransiDiff', N'1', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (45, N'CompanyCode', N'adm', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (46, N'EvisShareFolderRootPath', N'\\10.59.240.247\EvisData', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (47, N'EvisShareFolderDomain', N'SV-DB-MSPRO', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (48, N'EvisShareFolderUser', N'admingm32', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (49, N'EvisShareFolderPassword', N'P@ssw0rd', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (50, N'NpwpAdm', N'01.000.571.8-092.000', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (51, N'NamaNpwpAdm', N'Astra Daihatsu Motor', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (52, N'GLAccountForceSubmitSAP', N'3630102010', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (53, N'DefaultPrintOrdner', N'2', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (54, N'PelaporanTglFaktur', N'[-3]:[0]', NULL, N'Format : [Min]:[Max]')
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (55, N'EvisShareFolderIsSameDomain', N'true', NULL, N'true/false digunakan apakah shared folder dalam 1 domain atau tidak')
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (56, N'FpKhususJenisTransaksiEmptyNpwp', N'1', NULL, N'Jenis Transaksi yang mengharuskan NPWP dan Nama Penjual Kosong untuk Faktur Pajak Khusus')
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (57, N'BackupFolderWatcherInboxService', N'D:\ADM_EFIS\XML_IN\BackupInbox', NULL, N'Backup Folder untuk Watcher Outbox saat berhasil / gagal Transfer ke shared folder SAP')
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (58, N'MaxCopyWatcherInboxService', N'10', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ON 

GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (1, N'DK', N'13', N'4')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (2, N'DK', N'14', N'4')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (3, N'DK', N'15', N'4')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (4, N'DK', N'01', N'5')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (5, N'DK', N'02', N'5')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (6, N'DK', N'03', N'5')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (7, N'DK', N'04', N'5')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (8, N'DK', N'06', N'5')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (9, N'DK', N'07', N'5')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (10, N'DK', N'08', N'5')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (11, N'DK', N'09', N'5')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (12, N'DM', N'10', N'1')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (13, N'DM', N'11', N'1')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (14, N'DM', N'12', N'1')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (15, N'DM', N'01', N'2')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (16, N'DM', N'03', N'2')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (17, N'DM', N'04', N'2')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (18, N'DM', N'06', N'2')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (19, N'DM', N'09', N'2')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (20, N'DM', N'01', N'3')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (21, N'DM', N'02', N'3')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (22, N'DM', N'03', N'3')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (23, N'DM', N'04', N'3')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (24, N'DM', N'06', N'3')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (25, N'DM', N'07', N'3')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (26, N'DM', N'08', N'3')
GO
INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] ([Id], [FCode], [KdJenisTransaksi], [JenisTransaksi]) VALUES (27, N'DM', N'09', N'3')
GO
SET IDENTITY_INSERT [dbo].[MapJnsTransaksiKdJnsTransaksi] OFF
GO
SET IDENTITY_INSERT [dbo].[MapJnsTransJnsDok] ON 

GO
INSERT [dbo].[MapJnsTransJnsDok] ([Id], [FCode], [JenisDokumen], [JenisTransaksi]) VALUES (1, N'DK', N'6', N'4')
GO
INSERT [dbo].[MapJnsTransJnsDok] ([Id], [FCode], [JenisDokumen], [JenisTransaksi]) VALUES (2, N'DK', N'3', N'5')
GO
INSERT [dbo].[MapJnsTransJnsDok] ([Id], [FCode], [JenisDokumen], [JenisTransaksi]) VALUES (3, N'DM', N'2', N'1')
GO
INSERT [dbo].[MapJnsTransJnsDok] ([Id], [FCode], [JenisDokumen], [JenisTransaksi]) VALUES (4, N'DM', N'3', N'1')
GO
INSERT [dbo].[MapJnsTransJnsDok] ([Id], [FCode], [JenisDokumen], [JenisTransaksi]) VALUES (5, N'DM', N'5', N'2')
GO
INSERT [dbo].[MapJnsTransJnsDok] ([Id], [FCode], [JenisDokumen], [JenisTransaksi]) VALUES (6, N'DM', N'2', N'3')
GO
INSERT [dbo].[MapJnsTransJnsDok] ([Id], [FCode], [JenisDokumen], [JenisTransaksi]) VALUES (7, N'DM', N'3', N'3')
GO
INSERT [dbo].[MapJnsTransJnsDok] ([Id], [FCode], [JenisDokumen], [JenisTransaksi]) VALUES (8, N'DM', N'5', N'3')
GO
INSERT [dbo].[MapJnsTransJnsDok] ([Id], [FCode], [JenisDokumen], [JenisTransaksi]) VALUES (9, N'DM', N'8', N'3')
GO
SET IDENTITY_INSERT [dbo].[MapJnsTransJnsDok] OFF
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (1, N'Home', NULL, N'/Content/images/icon/home.png', NULL, N'/')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (2, N'Scan IWS', NULL, N'/Content/images/icon/fuel-data-entry.png', NULL, NULL)
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (3, N'Satuan', 2, NULL, NULL, N'/ScanQRCode/ScanQRCodeSatuanIWS')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (4, N'Bulk', 2, NULL, NULL, N'/ScanQRCode/ScanQRCodeBulkIWS')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (5, N'Pembetulan', 2, NULL, NULL, N'/ScanQRCode/PembetulanQRCodeSatuanIWS')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (6, N'Scan Non IWS', NULL, N'/Content/images/icon/fuel-data-entry.png', NULL, NULL)
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (7, N'Satuan', 6, NULL, NULL, N'/ScanQRCode/ScanQRCodeSatuanNonIWS')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (8, N'Bulk', 6, NULL, NULL, N'/ScanQRCode/ScanQRCodeBulkNonIWS')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (9, N'Pembetulan', 6, NULL, NULL, N'/ScanQRCode/PembetulanQRCodeSatuanNonIWS')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (10, N'Faktur Pajak Khusus', NULL, N'/Content/images/icon/fuel-data-entry.png', NULL, NULL)
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (11, N'Input FP Khusus', 10, N'/Content/images/icon/fuel-data-entry.png', NULL, N'/ScanQRCode/ScanManual')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (12, N'Input Pembetulan', 10, N'/Content/images/icon/fuel-data-entry.png', NULL, N'/ScanQRCode/PembetulanScanManual')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (13, N'Daftar Faktur Pajak', NULL, N'/Content/images/icon/fuel-data-entry.png', NULL, N'/ScanQRCode/ListFakturPajak')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (14, N'Request Faktur Pajak', NULL, N'/Content/images/icon/activity-entry.png', NULL, N'/RequestFakturPajak/Index')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (15, N'Retur', NULL, N'/Content/images/icon/fuel-data-entry.png', NULL, NULL)
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (16, N'Daftar Faktur Pajak Retur', 15, NULL, NULL, N'/Retur/ListReturFakturPajak')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (17, N'Input Retur', 15, NULL, NULL, N'/Retur/InputRetur')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (18, N'Data Compare', NULL, N'/Content/images/icon/fuel-data-entry.png', NULL, NULL)
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (19, N'EVIS VS IWS', 18, NULL, NULL, N'/DataCompare/EvisVsIws')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (20, N'EVIS VS SAP', 18, NULL, NULL, N'/DataCompare/EvisVsSap')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (21, N'List Ordner', NULL, N'/Content/images/icon/activity-entry.png', NULL, N'/Ordner/ListOrdner')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (22, N'Open Close Period', NULL, N'/Content/images/icon/activity-entry.png', NULL, N'/OpenClosePeriod/ListOpenClosePeriod')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (23, N'Log SAP', NULL, N'/Content/images/icon/activity-entry.png', NULL, N'/LogMonitoring/LogSap')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (24, N'Setting', NULL, N'/Content/images/icon/app-setting.png', NULL, NULL)
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (25, N'Role Management', 24, NULL, NULL, N'/Setting/RoleManagement')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (26, N'User Management', 24, NULL, NULL, N'/Setting/UserManagement')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (27, N'Master', NULL, N'/Content/images/icon/audit-trail.png', NULL, NULL)
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (28, N'Vendor', 27, NULL, NULL, N'/Master/Vendor')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (29, N'Report', NULL, N'/Content/images/icon/fuel-data-entry.png', NULL, NULL)
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (30, N'SPM', 29, N'/Content/images/icon/fuel-data-entry.png', NULL, N'/Report/SuratPemberitahuanMasa')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (31, N'Detail Faktur Pajak', 29, N'/Content/images/icon/fuel-data-entry.png', NULL, N'/Report/DetailFakturPajak')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (32, N'List Filing Index Faktur Pajak Masukan', 29, N'/Content/images/icon/fuel-data-entry.png', NULL, N'/Report/FakturPajakMasukan')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (33, N'Faktur Pajak Outstanding', 29, N'/Content/images/icon/fuel-data-entry.png', NULL, N'/Report/FakturPajakOutstanding')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (34, N'Faktur Pajak Belum Di Jurnal', 29, N'/Content/images/icon/fuel-data-entry.png', NULL, N'/Report/FakturPajakBelumDiJurnal')
GO
INSERT [dbo].[Modules] ([ModuleId], [Name], [ModuleIdParent], [IconUrl], [IconHoverUrl], [Url]) VALUES (35, N'Create SPM', NULL, N'/Content/images/icon/fuel-data-entry.png', NULL, N'/Spm/Create')
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

GO
INSERT [dbo].[Role] ([RoleId], [Name], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (1, N'administrator', 0, CAST(0x0000A72000000000 AS DateTime), CAST(0x0000A74301085037 AS DateTime), N'SYS', N'System')
GO
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[RoleActivity] ON 

GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (1, 1, 1, 0, CAST(0x0000A721009EB00D AS DateTime), CAST(0x0000A74301085038 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (2, 1, 2, 0, CAST(0x0000A721009EB00E AS DateTime), CAST(0x0000A74301085039 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (3, 1, 3, 0, CAST(0x0000A721009EB00E AS DateTime), CAST(0x0000A74301085039 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (4, 1, 4, 0, CAST(0x0000A721009EB00E AS DateTime), CAST(0x0000A7430108503A AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (5, 1, 5, 0, CAST(0x0000A721009EB00E AS DateTime), CAST(0x0000A7430108503A AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (6, 1, 6, 0, CAST(0x0000A721009EB00E AS DateTime), CAST(0x0000A7430108503A AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (7, 1, 7, 0, CAST(0x0000A721009EB00F AS DateTime), CAST(0x0000A7430108503B AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (8, 1, 8, 0, CAST(0x0000A721009EB00F AS DateTime), CAST(0x0000A7430108503B AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (9, 1, 9, 0, CAST(0x0000A721009EB00F AS DateTime), CAST(0x0000A7430108503B AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (10, 1, 10, 0, CAST(0x0000A721009EB00F AS DateTime), CAST(0x0000A7430108503B AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (11, 1, 11, 0, CAST(0x0000A721009EB00F AS DateTime), CAST(0x0000A7430108503B AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (12, 1, 12, 0, CAST(0x0000A721009EB00F AS DateTime), CAST(0x0000A7430108503C AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (13, 1, 13, 0, CAST(0x0000A721009EB010 AS DateTime), CAST(0x0000A7430108503C AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (14, 1, 14, 0, CAST(0x0000A721009EB010 AS DateTime), CAST(0x0000A7430108503C AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (15, 1, 15, 0, CAST(0x0000A721009EB010 AS DateTime), CAST(0x0000A7430108503C AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (16, 1, 16, 0, CAST(0x0000A721009EB010 AS DateTime), CAST(0x0000A7430108503C AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (17, 1, 17, 0, CAST(0x0000A721009EB011 AS DateTime), CAST(0x0000A7430108503D AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (18, 1, 18, 0, CAST(0x0000A721009EB011 AS DateTime), CAST(0x0000A7430108503D AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (19, 1, 19, 0, CAST(0x0000A721009EB011 AS DateTime), CAST(0x0000A7430108503D AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (20, 1, 20, 0, CAST(0x0000A721009EB011 AS DateTime), CAST(0x0000A7430108503D AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (21, 1, 21, 0, CAST(0x0000A721009EB011 AS DateTime), CAST(0x0000A7430108503D AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (22, 1, 22, 0, CAST(0x0000A721009EB011 AS DateTime), CAST(0x0000A7430108503E AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (23, 1, 23, 0, CAST(0x0000A721009EB012 AS DateTime), CAST(0x0000A7430108503E AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (24, 1, 24, 0, CAST(0x0000A721009EB012 AS DateTime), CAST(0x0000A7430108503E AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (25, 1, 25, 0, CAST(0x0000A721009EB012 AS DateTime), CAST(0x0000A7430108503E AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (26, 1, 26, 0, CAST(0x0000A721009EB012 AS DateTime), CAST(0x0000A7430108503E AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (27, 1, 27, 0, CAST(0x0000A721009EB012 AS DateTime), CAST(0x0000A7430108503E AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (28, 1, 28, 0, CAST(0x0000A721009EB012 AS DateTime), CAST(0x0000A7430108503F AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (29, 1, 29, 0, CAST(0x0000A721009EB012 AS DateTime), CAST(0x0000A7430108503F AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (30, 1, 30, 0, CAST(0x0000A721009EB012 AS DateTime), CAST(0x0000A7430108503F AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (31, 1, 31, 0, CAST(0x0000A721009EB012 AS DateTime), CAST(0x0000A7430108503F AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (32, 1, 32, 0, CAST(0x0000A721009EB012 AS DateTime), CAST(0x0000A7430108503F AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (33, 1, 33, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A7430108503F AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (34, 1, 34, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085040 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (35, 1, 35, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085040 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (36, 1, 36, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085040 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (37, 1, 37, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085040 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (38, 1, 38, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085041 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (39, 1, 39, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085041 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (40, 1, 40, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085041 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (41, 1, 41, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085041 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (42, 1, 42, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085041 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (43, 1, 43, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085041 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (44, 1, 44, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085041 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (45, 1, 45, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085041 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (46, 1, 46, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085041 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (47, 1, 47, 0, CAST(0x0000A721009EB013 AS DateTime), CAST(0x0000A74301085042 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (48, 1, 48, 0, CAST(0x0000A721009EB014 AS DateTime), CAST(0x0000A74301085042 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (49, 1, 49, 0, CAST(0x0000A721009EB014 AS DateTime), CAST(0x0000A74301085042 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (50, 1, 50, 0, CAST(0x0000A721009EB014 AS DateTime), CAST(0x0000A74301085042 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (51, 1, 51, 0, CAST(0x0000A721009EB014 AS DateTime), CAST(0x0000A74301085042 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (59, 1, 52, 0, CAST(0x0000A72E00FBC1E8 AS DateTime), CAST(0x0000A74301085042 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (60, 1, 53, 0, CAST(0x0000A72E00FBC1E8 AS DateTime), CAST(0x0000A74301085042 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (61, 1, 54, 0, CAST(0x0000A72E00FBC1E9 AS DateTime), CAST(0x0000A74301085042 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (62, 1, 55, 0, CAST(0x0000A72E00FBC1E9 AS DateTime), CAST(0x0000A74301085042 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (63, 1, 56, 0, CAST(0x0000A72E00FBC1E9 AS DateTime), CAST(0x0000A74301085042 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (64, 1, 57, 0, CAST(0x0000A72E00FBC1E9 AS DateTime), CAST(0x0000A74301085043 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (65, 1, 58, 0, CAST(0x0000A72E00FBC1E9 AS DateTime), CAST(0x0000A74301085043 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (66, 1, 59, 0, CAST(0x0000A72E00FBC1EA AS DateTime), CAST(0x0000A74301085043 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (67, 1, 60, 0, CAST(0x0000A72E00FBC1EA AS DateTime), CAST(0x0000A74301085043 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (68, 1, 61, 0, CAST(0x0000A72E00FBC1EA AS DateTime), CAST(0x0000A74301085043 AS DateTime), N'System', N'System')
GO
INSERT [dbo].[RoleActivity] ([RoleActivityId], [RoleId], [ActivityId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (69, 1, 62, 0, CAST(0x0000A74301085043 AS DateTime), NULL, N'System', N'System')
GO
SET IDENTITY_INSERT [dbo].[RoleActivity] OFF
GO
INSERT [dbo].[State] ([StateId], [StateName], [IsDeleted]) VALUES (1, N'Scanned', 0)
GO
INSERT [dbo].[State] ([StateId], [StateName], [IsDeleted]) VALUES (2, N'Success', 0)
GO
INSERT [dbo].[State] ([StateId], [StateName], [IsDeleted]) VALUES (3, N'Error Request', 0)
GO
INSERT [dbo].[State] ([StateId], [StateName], [IsDeleted]) VALUES (4, N'Error Validation', 0)
GO
SET IDENTITY_INSERT [dbo].[User] ON 

GO
INSERT [dbo].[User] ([UserId], [UserName], [Password], [Email], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy], [ResetPassword], [BadPasswordCount], [UserInitial]) VALUES (1, N'Admin', N'YtJw13LbdkVgaDC1pnE8pA==', N'irman@infinite.web.id', 0, CAST(0x0000A6C0012FF072 AS DateTime), CAST(0x0000A720015628FB AS DateTime), N'System', N'Admin', NULL, 0, N'ADM')
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO
SET IDENTITY_INSERT [dbo].[UserRole] ON 

GO
INSERT [dbo].[UserRole] ([UserRoleId], [UserId], [RoleId], [IsDeleted], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (1, 1, 1, 0, CAST(0x0000A720015628FC AS DateTime), NULL, N'Admin', N'Admin')
GO
SET IDENTITY_INSERT [dbo].[UserRole] OFF
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [_dta_index_FakturPajak_31_837578022__K29_K22_K21_K7_K36_K37_K28_K33]    Script Date: 4/3/2017 11:49:00 AM ******/
CREATE NONCLUSTERED INDEX [_dta_index_FakturPajak_31_837578022__K29_K22_K21_K7_K36_K37_K28_K33] ON [dbo].[FakturPajak]
(
	[Created] ASC,
	[TahunPajak] ASC,
	[MasaPajak] ASC,
	[TglFaktur] ASC,
	[Status] ASC,
	[StatusReconcile] ASC,
	[IsDeleted] ASC,
	[FormatedNoFaktur] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [_dta_index_FakturPajak_36_837578022__K21_K36_K28_K22_K29_K33_K1_7_37]    Script Date: 4/3/2017 11:49:00 AM ******/
CREATE NONCLUSTERED INDEX [_dta_index_FakturPajak_36_837578022__K21_K36_K28_K22_K29_K33_K1_7_37] ON [dbo].[FakturPajak]
(
	[MasaPajak] ASC,
	[Status] ASC,
	[IsDeleted] ASC,
	[TahunPajak] ASC,
	[Created] ASC,
	[FormatedNoFaktur] ASC,
	[FakturPajakId] ASC
)
INCLUDE ( 	[TglFaktur],
	[StatusReconcile]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [_dta_index_LogRequestFakturPajak_36_1061578820__K4_K2D_6]    Script Date: 4/3/2017 11:49:00 AM ******/
CREATE NONCLUSTERED INDEX [_dta_index_LogRequestFakturPajak_36_1061578820__K4_K2D_6] ON [dbo].[LogRequestFakturPajak]
(
	[FakturPajakId] ASC,
	[RequestDate] DESC
)
INCLUDE ( 	[ErrorMessage]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [_dta_index_SAP_MTDownloadPPN_31_1253579504__K33_K26_K1_3_4_6_7_8_10_12_13_15]    Script Date: 4/3/2017 11:49:00 AM ******/
CREATE NONCLUSTERED INDEX [_dta_index_SAP_MTDownloadPPN_31_1253579504__K33_K26_K1_3_4_6_7_8_10_12_13_15] ON [dbo].[SAP_MTDownloadPPN]
(
	[StatusReconcile] ASC,
	[IsDeleted] ASC,
	[Id] ASC
)
INCLUDE ( 	[AccountingDocNo],
	[FiscalYear],
	[PostingDate],
	[AmountLocal],
	[LineItem],
	[HeaderText],
	[ItemText],
	[AssignmentNo],
	[GLAccount]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[COMP_EVIS_IWS] ADD  CONSTRAINT [DF_COMP_EVIS_IWS_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[COMP_EVIS_IWS] ADD  CONSTRAINT [DF_COMP_EVIS_IWS_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[COMP_EVIS_SAP] ADD  CONSTRAINT [DF_COMP_EVIS_SAP_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[COMP_EVIS_SAP] ADD  CONSTRAINT [DF_COMP_EVIS_SAP_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[ErrorLog] ADD  DEFAULT ((1)) FOR [ErrorState]
GO
ALTER TABLE [dbo].[ErrorLog] ADD  DEFAULT ((0)) FOR [ErrorLine]
GO
ALTER TABLE [dbo].[ErrorLog] ADD  DEFAULT (getdate()) FOR [CreatedTime]
GO
ALTER TABLE [dbo].[FakturPajak] ADD  CONSTRAINT [DF_FakturPajak_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[FakturPajak] ADD  CONSTRAINT [DF_FakturPajak_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[FakturPajak] ADD  CONSTRAINT [DF_FakturPajak_StatusReconcile]  DEFAULT ((0)) FOR [StatusReconcile]
GO
ALTER TABLE [dbo].[FakturPajakDetail] ADD  CONSTRAINT [DF_FakturPajakDetail_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[FakturPajakDetail] ADD  CONSTRAINT [DF_FakturPajakDetail_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[FakturPajakRetur] ADD  CONSTRAINT [DF_FakturPajakRetur_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[FakturPajakRetur] ADD  CONSTRAINT [DF_FakturPajakRetur_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[LogDownload] ADD  CONSTRAINT [DF_LogDownload_RequestDate]  DEFAULT (getdate()) FOR [RequestDate]
GO
ALTER TABLE [dbo].[LogPrintFakturPajak] ADD  CONSTRAINT [DF_LogPrintFakturPajak_PrintDate]  DEFAULT (getdate()) FOR [PrintDate]
GO
ALTER TABLE [dbo].[LogPrintFakturPajak] ADD  CONSTRAINT [DF_LogPrintFakturPajak_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[LogPrintFakturPajak] ADD  CONSTRAINT [DF_LogPrintFakturPajak_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[LogProcessSap] ADD  CONSTRAINT [DF_LogProcessSap_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[LogProcessSap] ADD  CONSTRAINT [DF_LogProcessSap_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[LogRequestFakturPajak] ADD  CONSTRAINT [DF_LogRequestFakturPajak_RequestDate]  DEFAULT (getdate()) FOR [RequestDate]
GO
ALTER TABLE [dbo].[LogRequestFakturPajak] ADD  CONSTRAINT [DF_LogRequestFakturPajak_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[LogRequestFakturPajak] ADD  CONSTRAINT [DF_LogRequestFakturPajak_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[LogSap] ADD  CONSTRAINT [DF_LogSap_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[LogSap] ADD  CONSTRAINT [DF_LogSap_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[OpenClosePeriod] ADD  CONSTRAINT [DF_OpenClosePeriod_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[OpenClosePeriod] ADD  CONSTRAINT [DF_OpenClosePeriod_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[ReportSPM] ADD  CONSTRAINT [DF_SuratPemberitahuanMasa_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ReportSPM] ADD  CONSTRAINT [DF_SuratPemberitahuanMasa_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[ReportSPMDetail] ADD  CONSTRAINT [DF_ReportSPMDetail_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ReportSPMDetail] ADD  CONSTRAINT [DF_ReportSPMDetail_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[RoleActivity] ADD  CONSTRAINT [DF_RoleActivity_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[RoleActivity] ADD  CONSTRAINT [DF_RoleActivity_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[SAP_MTDownloadPPN] ADD  CONSTRAINT [DF_SAP_MTDownloadPPN_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[SAP_MTDownloadPPN] ADD  CONSTRAINT [DF_SAP_MTDownloadPPN_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[SAP_MTDownloadPPN] ADD  CONSTRAINT [DF_SAP_MTDownloadPPN_StatusReconcile]  DEFAULT ((0)) FOR [StatusReconcile]
GO
ALTER TABLE [dbo].[State] ADD  CONSTRAINT [DF_State_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[UserRole] ADD  CONSTRAINT [DF_UserRole_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserRole] ADD  CONSTRAINT [DF_UserRole_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Vendor] ADD  CONSTRAINT [DF_Vendor_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Vendor] ADD  CONSTRAINT [DF_Vendor_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_Modules] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Modules] ([ModuleId])
GO
ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_Modules]
GO
ALTER TABLE [dbo].[FakturPajak]  WITH CHECK ADD  CONSTRAINT [FK_FakturPajak_State] FOREIGN KEY([Status])
REFERENCES [dbo].[State] ([StateId])
GO
ALTER TABLE [dbo].[FakturPajak] CHECK CONSTRAINT [FK_FakturPajak_State]
GO
ALTER TABLE [dbo].[FakturPajakDetail]  WITH CHECK ADD  CONSTRAINT [FK_FakturPajakDetail_FakturPajak] FOREIGN KEY([FakturPajakId])
REFERENCES [dbo].[FakturPajak] ([FakturPajakId])
GO
ALTER TABLE [dbo].[FakturPajakDetail] CHECK CONSTRAINT [FK_FakturPajakDetail_FakturPajak]
GO
ALTER TABLE [dbo].[LogRequestFakturPajak]  WITH CHECK ADD  CONSTRAINT [FK_LogRequestFakturPajak_FakturPajak] FOREIGN KEY([FakturPajakId])
REFERENCES [dbo].[FakturPajak] ([FakturPajakId])
GO
ALTER TABLE [dbo].[LogRequestFakturPajak] CHECK CONSTRAINT [FK_LogRequestFakturPajak_FakturPajak]
GO
ALTER TABLE [dbo].[MapJnsTransaksiKdJnsTransaksi]  WITH CHECK ADD  CONSTRAINT [FK_MapJnsTransaksiKdJnsTransaksi_FPJenisTransaksi] FOREIGN KEY([JenisTransaksi])
REFERENCES [dbo].[FPJenisTransaksi] ([Id])
GO
ALTER TABLE [dbo].[MapJnsTransaksiKdJnsTransaksi] CHECK CONSTRAINT [FK_MapJnsTransaksiKdJnsTransaksi_FPJenisTransaksi]
GO
ALTER TABLE [dbo].[MapJnsTransaksiKdJnsTransaksi]  WITH CHECK ADD  CONSTRAINT [FK_MapJnsTransaksiKdJnsTransaksi_FPKdJenisTransaksi] FOREIGN KEY([KdJenisTransaksi])
REFERENCES [dbo].[FPKdJenisTransaksi] ([Id])
GO
ALTER TABLE [dbo].[MapJnsTransaksiKdJnsTransaksi] CHECK CONSTRAINT [FK_MapJnsTransaksiKdJnsTransaksi_FPKdJenisTransaksi]
GO
ALTER TABLE [dbo].[MapJnsTransJnsDok]  WITH CHECK ADD  CONSTRAINT [FK_MapJnsTransJnsDok_FPJenisDokumen] FOREIGN KEY([JenisDokumen])
REFERENCES [dbo].[FPJenisDokumen] ([Id])
GO
ALTER TABLE [dbo].[MapJnsTransJnsDok] CHECK CONSTRAINT [FK_MapJnsTransJnsDok_FPJenisDokumen]
GO
ALTER TABLE [dbo].[MapJnsTransJnsDok]  WITH CHECK ADD  CONSTRAINT [FK_MapJnsTransJnsDok_FPJenisTransaksi] FOREIGN KEY([JenisTransaksi])
REFERENCES [dbo].[FPJenisTransaksi] ([Id])
GO
ALTER TABLE [dbo].[MapJnsTransJnsDok] CHECK CONSTRAINT [FK_MapJnsTransJnsDok_FPJenisTransaksi]
GO
ALTER TABLE [dbo].[ReportSPMDetail]  WITH CHECK ADD  CONSTRAINT [FK_ReportSPMDetail_ReportSPM] FOREIGN KEY([ReportSPMId])
REFERENCES [dbo].[ReportSPM] ([Id])
GO
ALTER TABLE [dbo].[ReportSPMDetail] CHECK CONSTRAINT [FK_ReportSPMDetail_ReportSPM]
GO
ALTER TABLE [dbo].[RoleActivity]  WITH CHECK ADD  CONSTRAINT [FK_RoleActivity_Activity] FOREIGN KEY([ActivityId])
REFERENCES [dbo].[Activity] ([ActivityId])
GO
ALTER TABLE [dbo].[RoleActivity] CHECK CONSTRAINT [FK_RoleActivity_Activity]
GO
ALTER TABLE [dbo].[RoleActivity]  WITH CHECK ADD  CONSTRAINT [FK_RoleActivity_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([RoleId])
GO
ALTER TABLE [dbo].[RoleActivity] CHECK CONSTRAINT [FK_RoleActivity_Role]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([RoleId])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'DK or DM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FPJenisTransaksi', @level2type=N'COLUMN',@level2name=N'FCode'
GO
USE [master]
GO
ALTER DATABASE [EVIS] SET  READ_WRITE 
GO
