ALTER TABLE LogSap
ADD AccountingDocNoCredit nvarchar(max) NULL;
GO

ALTER TABLE LogSap
ADD FiscalYearCredit int NULL;
GO


ALTER TABLE FakturPajak
ALTER COLUMN NoFakturPajak nvarchar(255) NULL;
GO

ALTER TABLE FakturPajak
ALTER COLUMN FormatedNoFaktur nvarchar(255) NULL;
GO


ALTER TABLE ReportSPMDetail
ADD FPCreatedDate datetime NULL;
GO

UPDATE ReportSPMDetail
SET FPCreatedDate = (SELECT Created FROM FakturPajak WHERE FakturPajak.FormatedNoFaktur COLLATE DATABASE_DEFAULT = ReportSPMDetail.FormatedNoFaktur COLLATE DATABASE_DEFAULT)

/****** Object:  StoredProcedure [dbo].[sp_ReportSPMGenerateInsert]    Script Date: 4/1/2017 2:21:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_ReportSPMGenerateInsert]
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

/****** Object:  StoredProcedure [dbo].[sp_ReportSPMGet]    Script Date: 4/1/2017 2:22:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_ReportSPMGet]
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

--update 04-04-2017
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (59, N'DjpRequestErrorMailTo', N'irman@infinite.web.id', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (60, N'DjpRequestErrorMailToDisplayName', N'Accounting User', NULL, NULL)
GO
--update 05-04-2017
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (61, N'InternetProxy', N'http://webfilter', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (62, N'InternetProxyPort', N'8080', NULL, NULL)
GO
INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (63, N'UseDefaultCredential', N'true', NULL, NULL)
GO
---update : 05-04-2017

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_VendorProcessUpload]
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
           ,[KeteranganTambahan]
		   ,[FormatedNpwp])
	SELECT	v.[NPWP]
           ,v.[Nama]
           ,v.[Alamat]
		   ,v.[UserNameLogin]
           ,CASE WHEN LOWER(v.[PkpDicabut]) = 'ya' OR LOWER(v.[PkpDicabut]) = 'y' THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [PkpDicabut]
           ,CASE WHEN v.[TglPkpDicabut] IS NULL OR v.[TglPkpDicabut] = '' THEN NULL ELSE CAST(v.[TglPkpDicabut] AS DATE) END AS [TglPkpDicabut]
           ,CASE WHEN v.[KeteranganTambahan] IS NULL OR v.[KeteranganTambahan] = '' THEN NULL ELSE v.[KeteranganTambahan] END AS [KeteranganTambahan]
		   ,dbo.FormatNpwp(v.NPWP)
	FROM	@paramTable v
END





GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_FakturPajak_ValidateScanPengganti]
	-- Add the parameters for the stored procedure here
	@noFaktur nvarchar(100)	
	,@fgPengganti nvarchar(2)
	,@fpType int -- 1 = IWS, 2 = Non-IWS, 3 = Khusus
AS
BEGIN
	
	DECLARE
		@message nvarchar(max) = '' --empty if valid
		,@countOfFpNormal int
		,@cleanFpPengganti nvarchar(2) = RIGHT('0' + @fgPengganti, 2)

	IF @cleanFpPengganti <> '00'
	BEGIN
		IF @cleanFpPengganti = '01'
		BEGIN
			-- validate fp normal
			SET @countOfFpNormal = (SELECT	COUNT(FakturPajakId) AS countDat
			FROM	FakturPajak
			WHERE	IsDeleted = 0 AND NoFakturPajak = @noFaktur AND FgPengganti = 0)

			IF @countOfFpNormal IS NULL OR @countOfFpNormal = 0
			BEGIN
				SET @message = 'No FP Normal [' + dbo.FormatNoFaktur(@fpType, @noFaktur, 'xx', '0') + '] belum discan'
			END
		END
		ELSE
		BEGIN
			SET @message = 'FP Normal - Pengganti hanya diperbolehkan hingga 1'
		END

	END
	
	SELECT @message AS [Message]

END
GO

--- UPDATE : 2017-04-13
/****** Object:  StoredProcedure [dbo].[sp_ReportFakturPajakMasukans_GetListWithouPaging]    Script Date: 4/13/2017 1:46:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_ReportFakturPajakMasukans_GetListWithouPaging]
	-- Add the parameters for the stored procedure here
	@dTglFakturStart date
	,@dTglFakturEnd date
	,@picEntry nvarchar(max)
	,@Search nvarchar(max)
	,@fillingIndexStart varchar(max)
	,@fillingIndexEnd varchar(max)
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
					AND ((@fillingIndexStart IS NOT NULL AND @fillingIndexEnd IS NOT NULL AND FillingIndex BETWEEN @fillingIndexStart AND @fillingIndexEnd)
							OR (@fillingIndexStart IS NOT NULL AND @fillingIndexEnd IS NULL AND FillingIndex LIKE REPLACE(@fillingIndexStart, '*','%'))
							OR (@fillingIndexStart IS NULL AND @fillingIndexEnd IS NOT NULL AND FillingIndex LIKE REPLACE(@fillingIndexEnd,'*', '%'))
							OR (@fillingIndexStart IS NULL AND @fillingIndexEnd IS NULL)
					  )
		) inDat
	ORDER BY FormatedNoFaktur ASC
	OPTION (OPTIMIZE FOR UNKNOWN)
END

GO

/****** Object:  StoredProcedure [dbo].[sp_ReportFakturPajakMasukans_GetList]    Script Date: 4/13/2017 1:42:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_ReportFakturPajakMasukans_GetList]
	-- Add the parameters for the stored procedure here
	@dTglFakturStart date
	,@dTglFakturEnd date
	,@picEntry nvarchar(max)
	,@Search nvarchar(max)
	,@CurrentPage int
	,@ItemPerPage int
	,@SortColumnName varchar(max)
	,@sortOrder varchar(5) -- ASC or DESC
	,@fillingIndexStart varchar(max)
	,@fillingIndexEnd varchar(max)
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
						AND ((@fillingIndexStart IS NOT NULL AND @fillingIndexEnd IS NOT NULL AND FillingIndex BETWEEN @fillingIndexStart AND @fillingIndexEnd)
							OR (@fillingIndexStart IS NOT NULL AND @fillingIndexEnd IS NULL AND FillingIndex LIKE REPLACE(@fillingIndexStart, '*','%'))
							OR (@fillingIndexStart IS NULL AND @fillingIndexEnd IS NOT NULL AND FillingIndex LIKE REPLACE(@fillingIndexEnd,'*', '%'))
							OR (@fillingIndexStart IS NULL AND @fillingIndexEnd IS NULL)
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

INSERT [dbo].[GeneralConfig] ([GeneralConfigId], [ConfigKey], [ConfigValue], [ConfigExtra], [ConfigDesc]) VALUES (64, N'DefaultAutoHideLeftMenu', N'collapse', NULL, N'collapse atau expand')
GO

-- update : 2017-04-20

USE [EVIS]
GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerateToDownload]    Script Date: 4/20/2017 12:23:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerateToDownload]
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
	OPTION (OPTIMIZE FOR UNKNOWN)
END

GO

-- update : 2017-04-25

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerate]    Script Date: 4/25/2017 8:15:21 PM ******/
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
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerate]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,COUNT(*) OVER() AS TotalItems
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
					) efis LEFT JOIN
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,COUNT(*) OVER() AS TotalItems
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
					) efis RIGHT JOIN
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END

END
GO

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerateToDownload]    Script Date: 4/25/2017 8:52:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerateToDownload]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,COUNT(*) OVER() AS TotalItems
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
					) efis LEFT JOIN
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,COUNT(*) OVER() AS TotalItems
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
					) efis RIGHT JOIN
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC			
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
END

GO

/****** Object:  StoredProcedure [dbo].[sp_ReportFakturPajakMasukans_GetList]    Script Date: 4/25/2017 9:21:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_ReportFakturPajakMasukans_GetList]
	-- Add the parameters for the stored procedure here
	@dTglFakturStart date
	,@dTglFakturEnd date
	,@picEntry nvarchar(max)
	,@Search nvarchar(max)
	,@CurrentPage int
	,@ItemPerPage int
	,@SortColumnName varchar(max)
	,@sortOrder varchar(5) -- ASC or DESC
	,@fillingIndexStart varchar(max)
	,@fillingIndexEnd varchar(max)
	,@masaPajak int
	,@tahunPajak int
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
						--AND (@picEntry IS NULL OR (@picEntry IS NOT NULL AND LOWER(FP.CreatedBy) LIKE REPLACE(LOWER(@picEntry),'*','%')))
						AND (@picEntry IS NULL OR (@picEntry IS NOT NULL AND LOWER(FP.CreatedBy) = LOWER(@picEntry)))
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
						AND ((@fillingIndexStart IS NOT NULL AND @fillingIndexEnd IS NOT NULL AND FillingIndex BETWEEN @fillingIndexStart AND @fillingIndexEnd)
							OR (@fillingIndexStart IS NOT NULL AND @fillingIndexEnd IS NULL AND FillingIndex LIKE REPLACE(@fillingIndexStart, '*','%'))
							OR (@fillingIndexStart IS NULL AND @fillingIndexEnd IS NOT NULL AND FillingIndex LIKE REPLACE(@fillingIndexEnd,'*', '%'))
							OR (@fillingIndexStart IS NULL AND @fillingIndexEnd IS NULL)
					  )
						AND (@masaPajak IS NULL OR (@masaPajak IS NOT NULL AND fp.MasaPajak = @masaPajak))
						AND (@tahunPajak IS NULL OR (@tahunPajak IS NOT NULL AND fp.TahunPajak = @tahunPajak))
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

/****** Object:  StoredProcedure [dbo].[sp_ReportFakturPajakMasukans_GetListWithouPaging]    Script Date: 4/25/2017 9:21:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_ReportFakturPajakMasukans_GetListWithouPaging]
	-- Add the parameters for the stored procedure here
	@dTglFakturStart date
	,@dTglFakturEnd date
	,@picEntry nvarchar(max)
	,@Search nvarchar(max)
	,@fillingIndexStart varchar(max)
	,@fillingIndexEnd varchar(max)
	,@masaPajak int
	,@tahunPajak int
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
					--AND (@picEntry IS NULL OR (@picEntry IS NOT NULL AND LOWER(FP.CreatedBy) LIKE REPLACE(LOWER(@picEntry),'*','%')))
						AND (@picEntry IS NULL OR (@picEntry IS NOT NULL AND LOWER(FP.CreatedBy) = LOWER(@picEntry)))
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
					AND ((@fillingIndexStart IS NOT NULL AND @fillingIndexEnd IS NOT NULL AND FillingIndex BETWEEN @fillingIndexStart AND @fillingIndexEnd)
							OR (@fillingIndexStart IS NOT NULL AND @fillingIndexEnd IS NULL AND FillingIndex LIKE REPLACE(@fillingIndexStart, '*','%'))
							OR (@fillingIndexStart IS NULL AND @fillingIndexEnd IS NOT NULL AND FillingIndex LIKE REPLACE(@fillingIndexEnd,'*', '%'))
							OR (@fillingIndexStart IS NULL AND @fillingIndexEnd IS NULL)
					  )
					AND (@masaPajak IS NULL OR (@masaPajak IS NOT NULL AND fp.MasaPajak = @masaPajak))
					AND (@tahunPajak IS NULL OR (@tahunPajak IS NOT NULL AND fp.TahunPajak = @tahunPajak))
		) inDat
	ORDER BY FormatedNoFaktur ASC
	OPTION (OPTIMIZE FOR UNKNOWN)
END

GO

--- update : 2017-04-27
/****** Object:  View [dbo].[View_FakturPajak]    Script Date: 4/27/2017 11:26:55 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-------
------- UPDATE 2017-04-27
ALTER VIEW [dbo].[View_FakturPajak]
AS
SELECT fp.[FakturPajakId]
      ,fp.[FCode]
      ,LTRIM(RTRIM(fp.[UrlScan])) AS UrlScan
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

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerateToDownload]    Script Date: 4/27/2017 9:22:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerateToDownload]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,COUNT(*) OVER() AS TotalItems
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
					) efis LEFT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									SUBSTRING(RTRIM(LTRIM(v1.ItemText)), 10, 20) AS TaxInvoiceNumber,
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
								AND (a.StatusReconcile = 0 OR a.StatusReconcile IS NULL)								
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,COUNT(*) OVER() AS TotalItems
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
					) efis RIGHT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									SUBSTRING(RTRIM(LTRIM(v1.ItemText)), 10, 20) AS TaxInvoiceNumber,
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC			
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
END

GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerate]    Script Date: 4/27/2017 9:22:12 PM ******/
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
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerate]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,COUNT(*) OVER() AS TotalItems
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
					) efis LEFT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									SUBSTRING(RTRIM(LTRIM(v1.ItemText)), 10, 20) AS TaxInvoiceNumber,
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
								AND (a.StatusReconcile = 0 OR a.StatusReconcile IS NULL)								
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,COUNT(*) OVER() AS TotalItems
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
					) efis RIGHT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									SUBSTRING(RTRIM(LTRIM(v1.ItemText)), 10, 20) AS TaxInvoiceNumber,
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END

END
GO

-- update : 2017-05-02
/****** Object:  StoredProcedure [dbo].[sp_ReturPajakMasukanExportCsv]    Script Date: 5/2/2017 8:57:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_ReturPajakMasukanExportCsv]
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

	SET @headerTemplate = 'RM;NPWP;NAMA;KD_JENIS_TRANSAKSI;FG_PENGGANTI;NOMOR_FAKTUR;TANGGAL_FAKTUR;IS_CREDITABLE;NOMOR_DOKUMEN_RETUR;TANGGAL_RETUR;MASA_PAJAK_RETUR;TAHUN_PAJAK_RETUR;NILAI_RETUR_DPP;NILAI_RETUR_PPN;NILAI_RETUR_PPNBM'
	INSERT INTO @tempTable(ReturId,NPWP, NAMA, KD_JENIS_TRANSAKSI,FG_PENGGANTI,NOMOR_FAKTUR,TANGGAL_FAKTUR,NOMOR_DOKUMEN_RETUR,IS_CREDITABLE,TANGGAL_RETUR,MASA_PAJAK_RETUR,TAHUN_PAJAK_RETUR,NILAI_RETUR_DPP,NILAI_RETUR_PPN,NILAI_RETUR_PPNBM,Created)
	SELECT	CONVERT(VARCHAR(100),r.FakturPajakReturId) as ReturId,
			CONVERT(VARCHAR(100),r.NPWPPenjual) AS NPWP,
			REPLACE(CONVERT(VARCHAR(100),r.NamaPenjual), ';', ' ') AS NAMA,
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
			+';'+ NPWP
			+';'+ NAMA
			+';'+ KD_JENIS_TRANSAKSI 
			+';'+ FG_PENGGANTI
			+';'+ NOMOR_FAKTUR		
			+';'+ TANGGAL_FAKTUR
			+';'+ IS_CREDITABLE
			+';'+ NOMOR_DOKUMEN_RETUR 
			+';'+ TANGGAL_RETUR		 
			+';'+ MASA_PAJAK_RETUR
			+';'+ TAHUN_PAJAK_RETUR		 
			+';'+ NILAI_RETUR_DPP
			+';'+ NILAI_RETUR_PPN 
			+';'+ NILAI_RETUR_PPNBM)  as RowData,
			('') as RowData2,(ReturId) as RowData3, 
			'B' as Marker
	FROM	@tempTable

END

GO

/****** Object:  StoredProcedure [dbo].[sp_PajakMasukanExportCsv]    Script Date: 5/2/2017 8:54:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_PajakMasukanExportCsv]
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
		SET @headerTemplate = 'RM;KD_JENIS_TRANSAKSI;FG_PENGGANTI;NOMOR_FAKTUR;MASA_PAJAK;TAHUN_PAJAK;TANGGAL_FAKTUR;NPWP;NAMA;ALAMAT_LENGKAP;JUMLAH_DPP;JUMLAH_PPN;JUMLAH_PPNBM;IS_CREDITABLE'

		
		INSERT INTO @tempTable(FakturPajakId, KD_JENIS_TRANSAKSI, FG_PENGGANTI, NOMOR_FAKTUR, MASA_PAJAK, TAHUN_PAJAK, TANGGAL_FAKTUR, NPWP, NAMA, ALAMAT_LENGKAP, JUMLAH_DPP, JUMLAH_PPN, JUMLAH_PPNBM, IS_CREDITABLE, Created)
		SELECT	CONVERT(VARCHAR(100),fp.FakturPajakId) as FakturPajakId,
					CONVERT(VARCHAR(2),fp.KdJenisTransaksi) AS KD_JENIS_TRANSAKSI,
					CONVERT(VARCHAR(1), fp.FgPengganti) AS FG_PENGGANTI,
					CONVERT(VARCHAR(100), LEFT(fp.NoFakturPajak, 13)) AS NOMOR_FAKTUR,
					CONVERT(VARCHAR(1),fp.MasaPajak) AS MASA_PAJAK,
					CONVERT(VARCHAR(4),fp.TahunPajak) AS TAHUN_PAJAK,
					CONVERT(VARCHAR(25),fp.TglFaktur,103) AS TANGGAL_FAKTUR,
					CONVERT(VARCHAR(255),fp.NPWPPenjual) AS NPWP,		
					REPLACE(CONVERT(VARCHAR(255),fp.NamaPenjual), ';', ' ') AS NAMA,
					REPLACE(CONVERT(VARCHAR(255),fp.AlamatPenjual), ';', ' ') AS ALAMAT_LENGKAP,
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
			SET @headerTemplate = 'DK_DM;JENIS_TRANSAKSI;JENIS_DOKUMEN;KD_JNS_TRANSAKSI;FG_PENGGANTI;NOMOR_DOK_LAIN_GANTI;NOMOR_DOK_LAIN;TANGGAL_DOK_LAIN;MASA_PAJAK;TAHUN_PAJAK;NPWP;NAMA;ALAMAT_LENGKAP;JUMLAH_DPP;JUMLAH_PPN;JUMLAH_PPNBM;KETERANGAN'
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
					CASE WHEN fp.JenisTransaksi = '1' THEN '' ELSE REPLACE(CONVERT(VARCHAR(255),fp.NamaPenjual), ';', ' ') END AS NAMA,
					CASE WHEN fp.JenisTransaksi = '1' THEN '' ELSE REPLACE(CONVERT(VARCHAR(255),fp.AlamatPenjual), ';', ' ') END AS ALAMAT_LENGKAP,
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
				+';'+ KD_JENIS_TRANSAKSI
				+';'+ FG_PENGGANTI
				+';'+ NOMOR_FAKTUR
				+';'+ MASA_PAJAK
				+';'+ TAHUN_PAJAK
				+';'+ TANGGAL_FAKTUR
				+';'+ NPWP
				+';'+ NAMA
				+';'+ ALAMAT_LENGKAP
				+';'+ JUMLAH_DPP
				+';'+ JUMLAH_PPN
				+';'+ JUMLAH_PPNBM
				+';'+ IS_CREDITABLE)  as RowData,
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
			SELECT (DK_DM + ';' +
				JENIS_TRANSAKSI + ';' +
				JENIS_DOKUMEN + ';' +
				KD_JNS_TRANSAKSI + ';' +
				FG_PENGGANTI + ';' +
				NOMOR_DOK_LAIN_GANTI + ';' +
				NOMOR_DOK_LAIN + ';' +
				TANGGAL_DOK_LAIN + ';' +
				MASA_PAJAK + ';' +
				TAHUN_PAJAK + ';' +
				NPWP + ';' +
				NAMA + ';' +
				ALAMAT_LENGKAP + ';' +
				JUMLAH_DPP + ';' +
				JUMLAH_PPN + ';' +
				JUMLAH_PPNBM + ';' +
				KETERANGAN) AS RowData,
				('') as RowData2,(FakturPajakId) as RowData3, 
				'B' as Marker
			FROM @tempTableKhusus
		END
	END


END

GO

-- update : 2017-05-04

ALTER TABLE COMP_EVIS_SAP
ADD NPWPPenjual nvarchar(100) NULL;

/****** Object:  UserDefinedFunction [dbo].[fnGetTglFakturFromItemText]    Script Date: 5/4/2017 11:52:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnGetTglFakturFromItemText]
(
	-- Add the parameters for the function here
	@itemText nvarchar(255)
)
RETURNS nvarchar(8)
AS
BEGIN
	RETURN SUBSTRING(@itemText, 21, 8)

END
GO
/****** Object:  UserDefinedFunction [dbo].[fnGetNoFakturFromItemText]    Script Date: 5/4/2017 11:51:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnGetNoFakturFromItemText]
(
	-- Add the parameters for the function here
	@itemText nvarchar(255)
)
RETURNS nvarchar(20)
AS
BEGIN
	
	RETURN SUBSTRING(@itemText, 1, 19)

END
GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerateToDownload]    Script Date: 5/4/2017 11:51:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerateToDownload]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
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
					) efis LEFT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,
									dbo.fnGetTglFakturFromItemText(v1.ItemText) AS TglFaktur,
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
								--AND a.TglFaktur IS NOT NULL
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
								AND (a.StatusReconcile = 0 OR a.StatusReconcile IS NULL)								
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
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
					) efis RIGHT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,
									dbo.fnGetTglFakturFromItemText(v1.ItemText) AS TglFaktur,
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
								--AND a.TglFaktur IS NOT NULL
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC			
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
END

GO

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerate]    Script Date: 5/4/2017 11:50:51 AM ******/
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
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerate]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
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
					) efis LEFT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,
									dbo.fnConvertSapStringToDate(dbo.fnGetTglFakturFromItemText(v1.ItemText)) AS TglFaktur,
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
								--AND a.TglFaktur IS NOT NULL
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
								AND (a.StatusReconcile = 0 OR a.StatusReconcile IS NULL)								
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
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
					) efis RIGHT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,
									dbo.fnConvertSapStringToDate(dbo.fnGetTglFakturFromItemText(v1.ItemText)) AS TglFaktur,
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
								--AND a.TglFaktur IS NOT NULL
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END

END
GO

-- update : 2017-05-05
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION fnCheckIsNumberOnly
(
	-- Add the parameters for the function here
	@charToCheck varchar(255)
)
RETURNS bit -- 0 = false, 1 = true
AS
BEGIN
	DECLARE @toRet bit = 0

	DECLARE
	@tableToCheck TABLE (
		Value nvarchar(100)
	)

	INSERT INTO @tableToCheck
	VALUES(@charToCheck)

	DECLARE @checkedResult nvarchar(100) = (SELECT * FROM @tableToCheck WHERE Value LIKE '%[^0-9]%')
	
	IF @checkedResult IS NULL
	BEGIN
		SET @toRet = 1
	END

	RETURN @toRet

END
GO

/****** Object:  UserDefinedFunction [dbo].[fnConvertSapStringToDate]    Script Date: 5/5/2017 3:41:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[fnConvertSapStringToDate]
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
	
	--SET @cleanInputString = LTRIM(RTRIM(REPLACE(REPLACE(@inputString,'.',''),'-','')))
	SET @cleanInputString = LTRIM(RTRIM(@inputString))
	IF LEN(@cleanInputString) = 8
	BEGIN
		SET @sYear = LTRIM(RTRIM(SUBSTRING(@cleanInputString, 1, 4)))
		SET @sMonth = LTRIM(RTRIM(SUBSTRING(@cleanInputString, 5, 2)))
		SET @sDay = LTRIM(RTRIM(SUBSTRING(@cleanInputString, 7, 2)))

		IF dbo.fnCheckIsNumberOnly(@sYear) = 1 AND dbo.fnCheckIsNumberOnly(@sMonth) = 1 AND dbo.fnCheckIsNumberOnly(@sDay) = 1
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

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerate]    Script Date: 5/5/2017 4:36:26 PM ******/
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
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerate]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
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
					) efis LEFT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
								AND (a.StatusReconcile = 0 OR a.StatusReconcile IS NULL)
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
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
					) efis RIGHT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END

END
GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerateToDownload]    Script Date: 5/5/2017 4:36:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerateToDownload]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
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
					) efis LEFT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
								AND (a.StatusReconcile = 0 OR a.StatusReconcile IS NULL)								
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
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
					) efis RIGHT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC			
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
		END
	END
END

GO

--- update :2017-05-05
ALTER TABLE COMP_EVIS_SAP
ADD StatusFaktur nvarchar(max) NULL;

GO

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetByIdNo]    Script Date: 5/5/2017 10:05:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CompEvisVsSapGetByIdNo]
	-- Add the parameters for the stored procedure here
	@idNo varchar(255)
AS
BEGIN
	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY d.AccountingDocNo ASC) as int) AS vSequence
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]
			,dbo.fnCompEvisVsSapGetDocStatus(d.TaxInvoiceNumberEvis, d.TaxInvoiceNumberSap) AS DocumentStatus
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
			[dbo].[SAP_MTDownloadPPN] sap ON dbo.fnGetNoFakturFromItemText(sap.ItemText) = d.TaxInvoiceNumberSAP 
			AND sap.AccountingDocNo = d.AccountingDocNo AND CAST(sap.PostingDate as date) = CAST(d.PostingDate as date)
			AND sap.LineItem = d.ItemNo
	WHERE	d.IsDeleted = 0 AND d.IdNo = @idNo
END

GO

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetByIds]    Script Date: 5/5/2017 10:05:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CompEvisVsSapGetByIds]
	-- Add the parameters for the stored procedure here
	@ids varchar(255)
AS
BEGIN
	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY d.AccountingDocNo ASC) as int) AS vSequence
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]
			,dbo.fnCompEvisVsSapGetDocStatus(d.TaxInvoiceNumberEvis, d.TaxInvoiceNumberSap) AS DocumentStatus
			,d.StatusFaktur
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
			[dbo].[SAP_MTDownloadPPN] sap ON dbo.fnGetNoFakturFromItemText(sap.ItemText) = d.TaxInvoiceNumberSAP 
					AND sap.AccountingDocNo = d.AccountingDocNo AND CAST(sap.PostingDate as date) = CAST(d.PostingDate as date)
					AND sap.LineItem = d.ItemNo
	WHERE	d.Id IN (SELECT Data FROM dbo.Split(@ids))
END

GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerate]    Script Date: 5/5/2017 10:06:04 PM ******/
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
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerate]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
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
								,fp.StatusFaktur
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
					) efis LEFT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
								AND (a.StatusReconcile = 0 OR a.StatusReconcile IS NULL)
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
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
								,fp.StatusFaktur
						FROM	dbo.FakturPajak fp
						WHERE	fp.IsDeleted = 0 
								AND (fp.StatusReconcile IS NULL OR fp.StatusReconcile = 0)
								AND [Status] = 2								
					) efis RIGHT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END

END

GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerateToDownload]    Script Date: 5/5/2017 10:06:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerateToDownload]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
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
								,fp.StatusFaktur
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
					) efis LEFT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
								AND (a.StatusReconcile = 0 OR a.StatusReconcile IS NULL)								
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
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
								,fp.StatusFaktur
						FROM	dbo.FakturPajak fp
						WHERE	fp.IsDeleted = 0 
								AND (fp.StatusReconcile IS NULL OR fp.StatusReconcile = 0)
								AND [Status] = 2								
					) efis RIGHT JOIN
					(
						SELECT	a.*
						FROM	(
							SELECT	row_number() over(PARTITION BY v1.ItemText,v1.PostingDate,v1.AccountingDocNo,v1.LineItem ORDER BY v1.Id DESC) vSequence,
									v1.PostingDate,
									v1.AccountingDocNo,
									v1.LineItem AS ItemNo,
									v1.ItemText,
									dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,
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
								AND NOT (a.TaxInvoiceNumber IS NULL OR a.TaxInvoiceNumber = '')
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
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo ASC			
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
END
GO

-- update : 2017-05-10



DROP INDEX _dta_index_FakturPajak_31_837578022__K29_K22_K21_K7_K36_K37_K28_K33 ON FakturPajak;
GO

DROP INDEX _dta_index_FakturPajak_36_837578022__K21_K36_K28_K22_K29_K33_K1_7_37 ON FakturPajak;
GO

DROP INDEX _dta_index_SAP_MTDownloadPPN_31_1253579504__K33_K26_K1_3_4_6_7_8_10_12_13_15 ON SAP_MTDownloadPPN;
GO

ALTER TABLE FakturPajak
DROP CONSTRAINT DF_FakturPajak_StatusReconcile;
GO

ALTER TABLE FakturPajak
ALTER COLUMN StatusReconcile int NULL;
GO

ALTER TABLE SAP_MTDownloadPPN
DROP CONSTRAINT DF_SAP_MTDownloadPPN_StatusReconcile;
GO

ALTER TABLE SAP_MTDownloadPPN
ALTER COLUMN StatusReconcile int NULL;

GO
/****** Object:  UserDefinedFunction [dbo].[fnIsReconcileByItemText]    Script Date: 5/10/2017 4:29:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[fnIsReconcileByItemText]
(
	-- Add the parameters for the function here
	@ItemText nvarchar(100)
)
RETURNS bit
AS
BEGIN
	-- StatusReconcile = NULL / 0 = first data / ready to process, 1 = ON PROGRESS, 2 = SUCCESS, 3 = ERROR
	DECLARE
		@toRet bit = 0
		,@noFaktur nvarchar(20)
		,@countCheck int = 0

	SET @noFaktur = dbo.fnGetNoFakturFromItemText(@ItemText)

	SET @countCheck = (
		SELECT	COUNT(FakturPajakId)
		FROM	FakturPajak 
		WHERE	IsDeleted = 0 AND FormatedNoFaktur = @noFaktur AND (StatusReconcile IS NOT NULL AND (StatusReconcile = 1 OR StatusReconcile = 2))
		)

	IF @countCheck > 0 
	BEGIN
		SET @toRet = 1
	END
	ELSE
	BEGIN
		SET @countCheck = (SELECT COUNT(Id) 
				FROM SAP_MTDownloadPPN 
				WHERE IsDeleted = 0 AND (StatusReconcile IS NOT NULL AND (StatusReconcile = 1 OR StatusReconcile = 2) AND ItemText = @ItemText))
		IF @countCheck > 0
		BEGIN
			SET @toRet = 1
		END
	END

	RETURN @toRet;
	
END
GO

/****** Object:  UserDefinedFunction [dbo].[fnSapMtDownloadCheckValiditas]    Script Date: 5/10/2017 4:31:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[fnSapMtDownloadCheckValiditas]
(
	@itemText nvarchar(225)
)
RETURNS bit
AS
BEGIN
	
	DECLARE
		@tempTable TABLE
		(
			IdSap bigint,
			ItemText nvarchar(255),
			NoFaktur nvarchar(20),
			ReverseDocNo nvarchar(255),
			AmountLocal decimal(18,0),
			CalculateParam int
		)
	DECLARE @noFaktur nvarchar(20) 
			,@sumReversal decimal(18,0)
			,@toRet bit = 0

	SET @noFaktur = dbo.fnGetNoFakturFromItemText(@itemText)

	INSERT @tempTable(IdSap, ItemText, ReverseDocNo, AmountLocal, NoFaktur, CalculateParam)
	SELECT	Id, ItemText, ReverseDocNo, AmountLocal, dbo.fnGetNoFakturFromItemText(ItemText), CASE WHEN PostingKey = '40' THEN -1 ELSE 1 END AS CalculateParam
	FROM	SAP_MTDownloadPPN
	WHERE	IsDeleted = 0 AND ItemText = @itemText AND (ClearingDoc IS NULL OR LTRIM(RTRIM(ClearingDoc)) = '')

	SET @sumReversal = ISNULL((SELECT SUM(AmountLocal) FROM @tempTable WHERE ReverseDocNo IS NOT NULL AND ReverseDocNo <> ''), 0)

	IF @sumReversal = 0
	BEGIN
		SET @toRet = 1
	END

	RETURN @toRet

END
GO

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSap_GetByIdNoAndTaxInvoiceNumberEvis]    Script Date: 5/10/2017 4:31:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSap_GetByIdNoAndTaxInvoiceNumberEvis]
	-- Add the parameters for the stored procedure here
	@IdNo nvarchar(255)
	,@TaxInvoiceNumberEvis nvarchar(100)
AS
BEGIN

	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY d.AccountingDocNo ASC) as int) AS vSequence
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]
			,dbo.fnCompEvisVsSapGetDocStatus(d.TaxInvoiceNumberEvis, d.TaxInvoiceNumberSap) AS DocumentStatus
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
			[dbo].[SAP_MTDownloadPPN] sap ON dbo.fnGetNoFakturFromItemText(sap.ItemText) = d.TaxInvoiceNumberSAP 
			AND sap.AccountingDocNo = d.AccountingDocNo AND CAST(sap.PostingDate as date) = CAST(d.PostingDate as date)
			AND sap.LineItem = d.ItemNo
	WHERE	d.IsDeleted = 0 AND d.IdNo = @IdNo AND d.TaxInvoiceNumberEVIS = @TaxInvoiceNumberEvis
END
GO

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSap_Insert]    Script Date: 5/10/2017 4:31:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSap_Insert]
	-- Add the parameters for the stored procedure here
	@PostingDate date
           ,@AccountingDocNo nvarchar(255)
           ,@ItemNo nvarchar(255)
           ,@TglFaktur date
           ,@TaxInvoiceNumberEVIS nvarchar(100)
           ,@TaxInvoiceNumberSAP nvarchar(100)
           ,@DocumentHeaderText nvarchar(255)
           ,@NPWP nvarchar(100)
           ,@AmountEVIS decimal(18,2)
           ,@AmountSAP decimal(18,2)
           ,@AmountDiff decimal(18,2)
           ,@StatusCompare nvarchar(255)
           ,@Notes nvarchar(255)
           ,@CreatedBy nvarchar(100)
           ,@IdNo nvarchar(255)           
           ,@MasaPajak int
           ,@TahunPajak int
           ,@ItemText nvarchar(100)
           ,@FiscalYearDebet int
           ,@GLAccount nvarchar(100)
           ,@NPWPPenjual nvarchar(100)
           ,@StatusFaktur nvarchar(255)
		   ,@CompEvisSapId bigint out
AS
BEGIN

	DECLARE
		@pembetulanToInsert int
	
	SET @pembetulanToInsert = (SELECT TOP 1 Pembetulan 
		FROM COMP_EVIS_SAP 
		WHERE SUBSTRING(TaxInvoiceNumberEVIS, 4, 19) = SUBSTRING(@TaxInvoiceNumberEVIS, 4, 19) 
				AND IsDeleted = 0 ORDER BY Id DESC)

	IF @pembetulanToInsert IS NULL
	BEGIN
		SET @pembetulanToInsert = 0
	END
	ELSE
	BEGIN
		SET @pembetulanToInsert = @pembetulanToInsert + 1
	END
			
	INSERT INTO [dbo].[COMP_EVIS_SAP]
           ([PostingDate]
           ,[AccountingDocNo]
           ,[ItemNo]
           ,[TglFaktur]
           ,[TaxInvoiceNumberEVIS]
           ,[TaxInvoiceNumberSAP]
           ,[DocumentHeaderText]
           ,[NPWP]
           ,[AmountEVIS]
           ,[AmountSAP]
           ,[AmountDiff]
           ,[StatusCompare]
           ,[Notes]
           ,[CreatedBy]
           ,[IdNo]
           ,[Pembetulan]
           ,[MasaPajak]
           ,[TahunPajak]
           ,[ItemText]
           ,[FiscalYearDebet]
           ,[GLAccount]
           ,[NPWPPenjual]
           ,[StatusFaktur])
     VALUES
           (@PostingDate
           ,@AccountingDocNo
           ,@ItemNo
           ,@TglFaktur
           ,@TaxInvoiceNumberEVIS
           ,@TaxInvoiceNumberSAP
           ,@DocumentHeaderText
           ,@NPWP
           ,@AmountEVIS
           ,@AmountSAP
           ,@AmountDiff
           ,@StatusCompare
           ,@Notes
           ,@CreatedBy
           ,@IdNo
           ,@pembetulanToInsert
           ,@MasaPajak
           ,@TahunPajak
           ,@ItemText
           ,@FiscalYearDebet
           ,@GLAccount
           ,@NPWPPenjual
           ,@StatusFaktur
		   ); SELECT @CompEvisSapId = @@IDENTITY

	DECLARE
		@modifiedDate datetime = GETDATE()

	-- UPDATE StatusReconcile on SAP_MTDownloadPPN Table to OnProgress (1)
	IF @TaxInvoiceNumberSAP IS NOT NULL AND @PostingDate IS NOT NULL
	BEGIN
		UPDATE SAP_MTDownloadPPN
		SET		StatusReconcile = 1 /*1 = OnProgress*/
				,Modified = @modifiedDate
				,ModifiedBy = @CreatedBy
		WHERE	IsDeleted = 0 AND CAST(PostingDate as date) = CAST(@postingDate as date) 
				AND AccountingDocNo = @AccountingDocNo AND LineItem = @itemNo AND ItemText = @itemText	
	END
	
	-- UPDATE StatusReconcile on FakturPajak Table to OnProgress (1)
	IF @TaxInvoiceNumberEVIS IS NOT NULL
	BEGIN
		UPDATE FakturPajak
		SET StatusReconcile = 1
			,Modified = @modifiedDate
			,ModifiedBy = @CreatedBy
		WHERE IsDeleted = 0 AND FormatedNoFaktur = @TaxInvoiceNumberEVIS
	END
END
GO

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetByIdNo]    Script Date: 5/10/2017 4:32:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetByIdNo]
	-- Add the parameters for the stored procedure here
	@idNo varchar(255)
AS
BEGIN
	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY d.AccountingDocNo ASC) as int) AS vSequence
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]
			,dbo.fnCompEvisVsSapGetDocStatus(d.TaxInvoiceNumberEvis, d.TaxInvoiceNumberSap) AS DocumentStatus
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
			[dbo].[SAP_MTDownloadPPN] sap ON dbo.fnGetNoFakturFromItemText(sap.ItemText) = d.TaxInvoiceNumberSAP 
			AND sap.AccountingDocNo = d.AccountingDocNo AND CAST(sap.PostingDate as date) = CAST(d.PostingDate as date)
			AND sap.LineItem = d.ItemNo
	WHERE	d.IsDeleted = 0 AND d.IdNo = @idNo
END
GO

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetByIds]    Script Date: 5/10/2017 4:32:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetByIds]
	-- Add the parameters for the stored procedure here
	@ids varchar(255)
AS
BEGIN
	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY d.AccountingDocNo ASC) as int) AS vSequence
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]
			,dbo.fnCompEvisVsSapGetDocStatus(d.TaxInvoiceNumberEvis, d.TaxInvoiceNumberSap) AS DocumentStatus
			,d.StatusFaktur
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
			[dbo].[SAP_MTDownloadPPN] sap ON dbo.fnGetNoFakturFromItemText(sap.ItemText) = d.TaxInvoiceNumberSAP 
					AND sap.AccountingDocNo = d.AccountingDocNo AND CAST(sap.PostingDate as date) = CAST(d.PostingDate as date)
					AND sap.LineItem = d.ItemNo
	WHERE	d.Id IN (SELECT Data FROM dbo.Split(@ids))
END
GO

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerate]    Script Date: 5/10/2017 4:32:24 PM ******/
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
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerate]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
						) evissrc
						WHERE evissrc.vPartition = 1
								AND (evissrc.StatusReconcile IS NULL OR evissrc.StatusReconcile = 0 OR evissrc.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/
								AND (@scanDate IS NULL OR (@scanDate IS NOT NULL AND CAST(evissrc.Created as date) = CAST(@scanDate as date)))
								AND (@userName IS NULL OR (@userName IS NOT NULL AND LOWER(evissrc.CreatedBy) LIKE REPLACE(LOWER(@userName), '*', '%')))
								AND (@masaPajak IS NULL OR (@masaPajak IS NOT NULL AND evissrc.MasaPajak = @masaPajak))
								AND (@tahunPajak IS NULL OR (@tahunPajak IS NOT NULL AND evissrc.TahunPajak = @tahunPajak))
								AND (
									(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturStart as date))
									OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturEnd as date))
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) >= CAST(@tglFakturStart as date) 
										AND CAST(evissrc.TglFaktur as date) <= CAST(@tglFakturEnd as date)
										)
								)
								AND (
									(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL 
										AND evissrc.FormatedNoFaktur = @noFakturStart)
									OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL 
										AND evissrc.FormatedNoFaktur = @noFakturEnd)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL 
										AND (evissrc.FormatedNoFaktur BETWEEN @noFakturStart AND @noFakturEnd)
										)
								)
					) efis LEFT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR LTRIM(RTRIM(v1.ReverseDocNo)) = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
						) evissrc
						WHERE evissrc.vPartition = 1
								AND (evissrc.StatusReconcile IS NULL OR evissrc.StatusReconcile = 0 OR evissrc.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
					) efis RIGHT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR v1.ReverseDocNo = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
						AND (
								(@postingDateStart IS NULL AND @postingDateEnd IS NULL)
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateStart as date))
								OR (@postingDateStart IS NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateEnd as date))
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) >= CAST(@postingDateStart as date) 
									AND CAST(v1.PostingDate as date) <= CAST(@postingDateEnd as date)
									)
							)							
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END

END
GO

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerateToDownload]    Script Date: 5/10/2017 4:32:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerateToDownload]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
						) evissrc
						WHERE evissrc.vPartition = 1
								AND (evissrc.StatusReconcile IS NULL OR evissrc.StatusReconcile = 0 OR evissrc.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/
								AND (@scanDate IS NULL OR (@scanDate IS NOT NULL AND CAST(evissrc.Created as date) = CAST(@scanDate as date)))
								AND (@userName IS NULL OR (@userName IS NOT NULL AND LOWER(evissrc.CreatedBy) LIKE REPLACE(LOWER(@userName), '*', '%')))
								AND (@masaPajak IS NULL OR (@masaPajak IS NOT NULL AND evissrc.MasaPajak = @masaPajak))
								AND (@tahunPajak IS NULL OR (@tahunPajak IS NOT NULL AND evissrc.TahunPajak = @tahunPajak))
								AND (
									(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturStart as date))
									OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturEnd as date))
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) >= CAST(@tglFakturStart as date) 
										AND CAST(evissrc.TglFaktur as date) <= CAST(@tglFakturEnd as date)
										)
								)
								AND (
									(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL 
										AND evissrc.FormatedNoFaktur = @noFakturStart)
									OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL 
										AND evissrc.FormatedNoFaktur = @noFakturEnd)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL 
										AND (evissrc.FormatedNoFaktur BETWEEN @noFakturStart AND @noFakturEnd)
										)
								)
					) efis LEFT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR LTRIM(RTRIM(v1.ReverseDocNo)) = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
						) evissrc
						WHERE evissrc.vPartition = 1
								AND (evissrc.StatusReconcile IS NULL OR evissrc.StatusReconcile = 0 OR evissrc.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
					) efis RIGHT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR v1.ReverseDocNo = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
						AND (
								(@postingDateStart IS NULL AND @postingDateEnd IS NULL)
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateStart as date))
								OR (@postingDateStart IS NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateEnd as date))
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) >= CAST(@postingDateStart as date) 
									AND CAST(v1.PostingDate as date) <= CAST(@postingDateEnd as date)
									)
							)							
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
END
GO

/****** Object:  StoredProcedure [dbo].[sp_ReportFakturPajakMasukans_GetList]    Script Date: 5/10/2017 4:33:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_ReportFakturPajakMasukans_GetList]
	-- Add the parameters for the stored procedure here
	@dTglFakturStart date
	,@dTglFakturEnd date
	,@picEntry nvarchar(max)
	,@Search nvarchar(max)
	,@CurrentPage int
	,@ItemPerPage int
	,@SortColumnName varchar(max)
	,@sortOrder varchar(5) -- ASC or DESC
	,@fillingIndexStart varchar(max)
	,@fillingIndexEnd varchar(max)
	,@masaPajak int
	,@tahunPajak int
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
						--AND (@picEntry IS NULL OR (@picEntry IS NOT NULL AND LOWER(FP.CreatedBy) LIKE REPLACE(LOWER(@picEntry),'*','%')))
						AND (@picEntry IS NULL OR (@picEntry IS NOT NULL AND LOWER(FP.CreatedBy) = LOWER(@picEntry)))
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
						AND ((@fillingIndexStart IS NOT NULL AND @fillingIndexEnd IS NOT NULL AND FillingIndex BETWEEN @fillingIndexStart AND @fillingIndexEnd)
							OR (@fillingIndexStart IS NOT NULL AND @fillingIndexEnd IS NULL AND FillingIndex LIKE REPLACE(@fillingIndexStart, '*','%'))
							OR (@fillingIndexStart IS NULL AND @fillingIndexEnd IS NOT NULL AND FillingIndex LIKE REPLACE(@fillingIndexEnd,'*', '%'))
							OR (@fillingIndexStart IS NULL AND @fillingIndexEnd IS NULL)
					  )
						AND (@masaPajak IS NULL OR (@masaPajak IS NOT NULL AND fp.MasaPajak = @masaPajak))
						AND (@tahunPajak IS NULL OR (@tahunPajak IS NOT NULL AND fp.TahunPajak = @tahunPajak))
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

-- update : 2017-05-10 (2)
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetByIds]    Script Date: 5/10/2017 8:11:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetByIds]
	-- Add the parameters for the stored procedure here
	@ids varchar(255)
AS
BEGIN
	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY d.AccountingDocNo ASC) as int) AS vSequence
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]
			,dbo.fnCompEvisVsSapGetDocStatus(d.TaxInvoiceNumberEvis, d.TaxInvoiceNumberSap) AS DocumentStatus
			,d.StatusFaktur
			,d.Id
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
			[dbo].[SAP_MTDownloadPPN] sap ON dbo.fnGetNoFakturFromItemText(sap.ItemText) = d.TaxInvoiceNumberSAP 
					AND sap.AccountingDocNo = d.AccountingDocNo AND CAST(sap.PostingDate as date) = CAST(d.PostingDate as date)
					AND sap.LineItem = d.ItemNo
	WHERE	d.Id IN (SELECT Data FROM dbo.Split(@ids))
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerateToDownload]    Script Date: 5/10/2017 8:11:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerateToDownload]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
			SELECT	comp_result.*, CAST(0 AS int) AS Pembetulan, CAST(0 AS bigint) AS Id
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
						) evissrc
						WHERE evissrc.vPartition = 1
								AND (evissrc.StatusReconcile IS NULL OR evissrc.StatusReconcile = 0 OR evissrc.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/
								AND (@scanDate IS NULL OR (@scanDate IS NOT NULL AND CAST(evissrc.Created as date) = CAST(@scanDate as date)))
								AND (@userName IS NULL OR (@userName IS NOT NULL AND LOWER(evissrc.CreatedBy) LIKE REPLACE(LOWER(@userName), '*', '%')))
								AND (@masaPajak IS NULL OR (@masaPajak IS NOT NULL AND evissrc.MasaPajak = @masaPajak))
								AND (@tahunPajak IS NULL OR (@tahunPajak IS NOT NULL AND evissrc.TahunPajak = @tahunPajak))
								AND (
									(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturStart as date))
									OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturEnd as date))
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) >= CAST(@tglFakturStart as date) 
										AND CAST(evissrc.TglFaktur as date) <= CAST(@tglFakturEnd as date)
										)
								)
								AND (
									(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL 
										AND evissrc.FormatedNoFaktur = @noFakturStart)
									OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL 
										AND evissrc.FormatedNoFaktur = @noFakturEnd)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL 
										AND (evissrc.FormatedNoFaktur BETWEEN @noFakturStart AND @noFakturEnd)
										)
								)
					) efis LEFT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR LTRIM(RTRIM(v1.ReverseDocNo)) = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					, CAST(0 AS bigint) AS Id
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
			SELECT	comp_result.*, CAST(0 AS int) AS Pembetulan, CAST(0 AS bigint) AS Id
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
						) evissrc
						WHERE evissrc.vPartition = 1
								AND (evissrc.StatusReconcile IS NULL OR evissrc.StatusReconcile = 0 OR evissrc.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
					) efis RIGHT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR v1.ReverseDocNo = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
						AND (
								(@postingDateStart IS NULL AND @postingDateEnd IS NULL)
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateStart as date))
								OR (@postingDateStart IS NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateEnd as date))
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) >= CAST(@postingDateStart as date) 
									AND CAST(v1.PostingDate as date) <= CAST(@postingDateEnd as date)
									)
							)							
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					, CAST(0 AS bigint) AS Id
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerate]    Script Date: 5/10/2017 8:10:29 PM ******/
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
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerate]
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
	
	IF @scanDate IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
			SELECT	comp_result.*, CAST(0 AS int) AS Pembetulan, CAST(0 AS bigint) AS Id
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
						) evissrc
						WHERE evissrc.vPartition = 1
								AND (evissrc.StatusReconcile IS NULL OR evissrc.StatusReconcile = 0 OR evissrc.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/
								AND (@scanDate IS NULL OR (@scanDate IS NOT NULL AND CAST(evissrc.Created as date) = CAST(@scanDate as date)))
								AND (@userName IS NULL OR (@userName IS NOT NULL AND LOWER(evissrc.CreatedBy) LIKE REPLACE(LOWER(@userName), '*', '%')))
								AND (@masaPajak IS NULL OR (@masaPajak IS NOT NULL AND evissrc.MasaPajak = @masaPajak))
								AND (@tahunPajak IS NULL OR (@tahunPajak IS NOT NULL AND evissrc.TahunPajak = @tahunPajak))
								AND (
									(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturStart as date))
									OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturEnd as date))
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) >= CAST(@tglFakturStart as date) 
										AND CAST(evissrc.TglFaktur as date) <= CAST(@tglFakturEnd as date)
										)
								)
								AND (
									(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL 
										AND evissrc.FormatedNoFaktur = @noFakturStart)
									OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL 
										AND evissrc.FormatedNoFaktur = @noFakturEnd)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL 
										AND (evissrc.FormatedNoFaktur BETWEEN @noFakturStart AND @noFakturEnd)
										)
								)
					) efis LEFT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR LTRIM(RTRIM(v1.ReverseDocNo)) = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 CAST(0 AS bigint) AS Id
					,1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
			SELECT	comp_result.*, CAST(0 AS int) AS Pembetulan, CAST(0 AS bigint) AS Id
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
						) evissrc
						WHERE evissrc.vPartition = 1
								AND (evissrc.StatusReconcile IS NULL OR evissrc.StatusReconcile = 0 OR evissrc.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
					) efis RIGHT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR v1.ReverseDocNo = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
						AND (
								(@postingDateStart IS NULL AND @postingDateEnd IS NULL)
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateStart as date))
								OR (@postingDateStart IS NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateEnd as date))
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) >= CAST(@postingDateStart as date) 
									AND CAST(v1.PostingDate as date) <= CAST(@postingDateEnd as date)
									)
							)							
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					, CAST(0 AS bigint) AS Id
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END

END
GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetByIdNo]    Script Date: 5/10/2017 8:13:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetByIdNo]
	-- Add the parameters for the stored procedure here
	@idNo varchar(255)
AS
BEGIN
	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY d.AccountingDocNo ASC) as int) AS vSequence
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]
			,dbo.fnCompEvisVsSapGetDocStatus(d.TaxInvoiceNumberEvis, d.TaxInvoiceNumberSap) AS DocumentStatus
			,d.Id
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
			[dbo].[SAP_MTDownloadPPN] sap ON dbo.fnGetNoFakturFromItemText(sap.ItemText) = d.TaxInvoiceNumberSAP 
			AND sap.AccountingDocNo = d.AccountingDocNo AND CAST(sap.PostingDate as date) = CAST(d.PostingDate as date)
			AND sap.LineItem = d.ItemNo
	WHERE	d.IsDeleted = 0 AND d.IdNo = @idNo
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSap_GetByIdNoAndTaxInvoiceNumberEvis]    Script Date: 5/10/2017 8:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSap_GetByIdNoAndTaxInvoiceNumberEvis]
	-- Add the parameters for the stored procedure here
	@IdNo nvarchar(255)
	,@TaxInvoiceNumberEvis nvarchar(100)
	,@AccountingDocNo nvarchar(255)
AS
BEGIN

	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY d.AccountingDocNo ASC) as int) AS vSequence
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]
			,dbo.fnCompEvisVsSapGetDocStatus(d.TaxInvoiceNumberEvis, d.TaxInvoiceNumberSap) AS DocumentStatus
			,d.Id
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
			[dbo].[SAP_MTDownloadPPN] sap ON dbo.fnGetNoFakturFromItemText(sap.ItemText) = d.TaxInvoiceNumberSAP 
			AND sap.AccountingDocNo = d.AccountingDocNo AND CAST(sap.PostingDate as date) = CAST(d.PostingDate as date)
			AND sap.LineItem = d.ItemNo
	WHERE	d.IsDeleted = 0 AND d.IdNo = @IdNo AND d.TaxInvoiceNumberEVIS = @TaxInvoiceNumberEvis AND d.AccountingDocNo = @AccountingDocNo
END
GO

-- update : 2017-05-12

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSap_GetByIdNoAndTaxInvoiceNumberEvis]    Script Date: 5/12/2017 11:04:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSap_GetByIdNoAndTaxInvoiceNumberEvis]
	-- Add the parameters for the stored procedure here
	@IdNo nvarchar(255)
	,@TaxInvoiceNumberEvis nvarchar(100)
	,@AccountingDocNo nvarchar(255)
AS
BEGIN

	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY d.AccountingDocNo ASC) as int) AS vSequence
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]
			,dbo.fnCompEvisVsSapGetDocStatus(d.TaxInvoiceNumberEvis, d.TaxInvoiceNumberSap) AS DocumentStatus
			,d.Id
			,d.StatusFaktur
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
			[dbo].[SAP_MTDownloadPPN] sap ON dbo.fnGetNoFakturFromItemText(sap.ItemText) = d.TaxInvoiceNumberSAP 
			AND sap.AccountingDocNo = d.AccountingDocNo AND CAST(sap.PostingDate as date) = CAST(d.PostingDate as date)
			AND sap.LineItem = d.ItemNo
	WHERE	d.IsDeleted = 0 AND d.IdNo = @IdNo AND d.TaxInvoiceNumberEVIS = @TaxInvoiceNumberEvis AND d.AccountingDocNo = @AccountingDocNo
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetByIdNo]    Script Date: 5/12/2017 11:04:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetByIdNo]
	-- Add the parameters for the stored procedure here
	@idNo varchar(255)
AS
BEGIN
	SELECT	CAST(ROW_NUMBER() OVER(ORDER BY d.AccountingDocNo ASC) as int) AS vSequence
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]
			,dbo.fnCompEvisVsSapGetDocStatus(d.TaxInvoiceNumberEvis, d.TaxInvoiceNumberSap) AS DocumentStatus
			,d.Id
			,d.StatusFaktur
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT LEFT JOIN
			[dbo].[SAP_MTDownloadPPN] sap ON dbo.fnGetNoFakturFromItemText(sap.ItemText) = d.TaxInvoiceNumberSAP 
			AND sap.AccountingDocNo = d.AccountingDocNo AND CAST(sap.PostingDate as date) = CAST(d.PostingDate as date)
			AND sap.LineItem = d.ItemNo
	WHERE	d.IsDeleted = 0 AND d.IdNo = @idNo
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSap_UpdateReconcile]    Script Date: 5/12/2017 11:28:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CompEvisVsSap_UpdateReconcile]
	-- Add the parameters for the stored procedure here
	@idNo nvarchar(255)
	,@newStatus int -- 0 = ready to process, 1 = on progress, 2 = success, 3 = error
	,@userModifier nvarchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	DECLARE
		@taxInvoiceNumberEvis nvarchar(255) = NULL
		,@taxInvoiceNumberSap nvarchar(255) = NULL
	DECLARE @tempTable TABLE (
			Idx int identity(1,1)
			,AcountingDocNo nvarchar(max)
			,PostingDate date
			,ItemNo nvarchar(max)
			,TaxInvoiceNumber nvarchar(100)
			,ItemText nvarchar(max)
		)

	SELECT TOP (1) @taxInvoiceNumberEvis = TaxInvoiceNumberEVIS, @taxInvoiceNumberSap = TaxInvoiceNumberSAP
	FROM	COMP_EVIS_SAP
	WHERE	IdNo = @idNo AND IsDeleted = 0

	IF @taxInvoiceNumberEvis IS NOT NULL AND @taxInvoiceNumberEvis <> '' AND (@taxInvoiceNumberSap IS NULL OR @taxInvoiceNumberSap = '')
	BEGIN
		-- force submit
		-- update only to FakturPajak Table
		UPDATE FakturPajak
		SET StatusReconcile = @newStatus
			,Modified = GETDATE()
			,ModifiedBy = @userModifier
		WHERE IsDeleted = 0 AND FormatedNoFaktur IN (
			SELECT	DISTINCT TaxInvoiceNumberEVIS
			FROM	COMP_EVIS_SAP
			WHERE	IsDeleted = 0 AND IdNo = @idNo AND TaxInvoiceNumberEVIS IS NOT NULL AND TaxInvoiceNumberEVIS <> ''
		)
	END
	ELSE
	BEGIN
		IF @taxInvoiceNumberEvis IS NOT NULL AND @taxInvoiceNumberEvis <> '' AND @taxInvoiceNumberSap IS NOT NULL AND @taxInvoiceNumberSap <> ''
			AND @taxInvoiceNumberEvis = @taxInvoiceNumberSap
			BEGIN
				-- submit
				-- update FakturPajak and SAP_MTDownloadPPN table
				UPDATE FakturPajak
				SET StatusReconcile = @newStatus
					,Modified = GETDATE()
					,ModifiedBy = @userModifier
				WHERE IsDeleted = 0 AND FormatedNoFaktur IN (
					SELECT	DISTINCT TaxInvoiceNumberEVIS
					FROM	COMP_EVIS_SAP
					WHERE	IsDeleted = 0 AND IdNo = @idNo AND TaxInvoiceNumberEVIS IS NOT NULL AND TaxInvoiceNumberEVIS <> ''
				)

				DECLARE
					@maxIdx int
					,@currentIdx int
					,@currentAccountingDocNo nvarchar(255)
					,@currentPostingDate date
					,@currentItemNo nvarchar(255)
					,@currentItemText nvarchar(255)
					,@currentTaxInvoiceNumber nvarchar(100)

				INSERT @tempTable(AcountingDocNo, ItemNo, ItemText, PostingDate, TaxInvoiceNumber)
				SELECT	DISTINCT AccountingDocNo, ItemNo, ItemText, PostingDate, TaxInvoiceNumberEVIS
				FROM	COMP_EVIS_SAP
				WHERE	IsDeleted = 0 AND IdNo = @idNo

				SET @maxIdx = (SELECT MAX(Idx) FROM @tempTable)
				SET @currentIdx = 1

				WHILE @currentIdx <= @maxIdx
				BEGIN
					SELECT	@currentAccountingDocNo = AcountingDocNo, 
							@currentItemNo = ItemNo, 
							@currentItemText = ItemText,
							@currentPostingDate = PostingDate,
							@currentTaxInvoiceNumber = TaxInvoiceNumber
					FROM	@tempTable
					WHERE	Idx = @currentIdx

					UPDATE SAP_MTDownloadPPN
					SET StatusReconcile = @newStatus
						,Modified = GETDATE()
						,ModifiedBy = @userModifier
					WHERE IsDeleted = 0 AND AccountingDocNo = @currentAccountingDocNo 
							AND LineItem = @currentItemNo AND ItemText = @currentItemText
							AND CAST(PostingDate AS date) = CAST(@currentPostingDate AS date)							

					SET @currentIdx = @currentIdx + 1
				END
				
			END
	END

END
GO
-----------------------------------------
-- update : 2017-05-24
-- author : irman sulaeman
-----------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSap_GetXmlUploadPPNSubmit]    Script Date: 5/24/2017 5:01:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: GET XML DATA SUBMIT UploadPPNCredit EVIS vs SAP
-- Create date	: 2017-05-24
-- Description	: Preparing data for generate xml send to sap (UploadPPNCredit)
-- =============================================
CREATE PROCEDURE [dbo].[sp_CompEvisVsSap_GetXmlUploadPPNSubmit]
	-- Add the parameters for the stored procedure here
	@ids varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @tempTable TABLE
	(
		Idx int identity(1,1) NOT NULL
		,Id bigint
		,PostingDate date
		,AccountingDocNo nvarchar(max)
		,ItemNo nvarchar(max)
		,TglFaktur date
		,NamaVendor nvarchar(max)
		,ScanDate date
		,TaxInvoiceNumberEvis nvarchar(100)
		,TaxInvoiceNumberSap nvarchar(100)
		,DocumentHeaderText nvarchar(max)
		,Npwp nvarchar(100)
		,NpwpPenjual nvarchar(100)
		,AmountEvis decimal(18,2)
		,AmountSap decimal(18,2)
		,AmountDiff decimal(18,2)
		,StatusCompare nvarchar(max)
		,Notes nvarchar(max)
		,TotalItems int
		,UserNameCreator nvarchar(100)
		,Pembetulan int
		,MasaPajak int
		,TahunPajak int
		,ItemText nvarchar(max)
		,FiscalYearDebet int
		,GLAccount nvarchar(100)
		,StatusFaktur nvarchar(max)		
	)

	DECLARE @tempOriginalTable TABLE
	(
		 Id bigint
		,IdOriginal bigint
		,FpOriginal nvarchar(100) NULL
		,NpwpOriginal nvarchar(100) NULL
		,PembetulanOriginal int NULL
		,AccountingDocDebetOriginal nvarchar(max) NULL
		,FiscalYearDebetOriginal int NULL
		,GLAccountOriginal nvarchar(max) NULL
		,AmountPPNOriginal decimal(18, 2) NULL
	)

	DECLARE
		@currentIdx int
		,@maxIdx int
		,@currentTaxInvoiceNumberEvis nvarchar(100)
		,@currentPembetulan int
		,@currentId bigint

	INSERT INTO @tempTable(
			Id
			,PostingDate
			,AccountingDocNo
			,ItemNo
			,TglFaktur
			,NamaVendor
			,ScanDate
			,TaxInvoiceNumberEvis 
			,TaxInvoiceNumberSap
			,DocumentHeaderText
			,Npwp 
			,NpwpPenjual 
			,AmountEvis 
			,AmountSap 
			,AmountDiff 
			,StatusCompare 
			,Notes 
			,TotalItems 
			,UserNameCreator 
			,Pembetulan 
			,MasaPajak 
			,TahunPajak 
			,ItemText 
			,FiscalYearDebet 
			,GLAccount 
			,StatusFaktur)
	SELECT	 d.Id
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]			
			,d.StatusFaktur			
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT
	WHERE	d.Id IN (SELECT Data FROM dbo.Split(@ids))

	SET @maxIdx = (SELECT MAX(Idx) FROM @tempTable)

	SET @currentIdx = 1
	WHILE @currentIdx <= @maxIdx
	BEGIN
		
		SELECT	@currentTaxInvoiceNumberEvis = TaxInvoiceNumberEvis, 
				@currentId = Id, 
				@currentPembetulan = Pembetulan
		FROM	@tempTable
		WHERE	Idx = @currentIdx

		INSERT INTO @tempOriginalTable(Id, IdOriginal, FpOriginal, NpwpOriginal, PembetulanOriginal, AccountingDocDebetOriginal, FiscalYearDebetOriginal, GLAccountOriginal, AmountPPNOriginal)
		SELECT	TOP 1 @currentId, Id, TaxInvoiceNumberEVIS, NPWPPenjual, Pembetulan, AccountingDocNo, FiscalYearDebet, GLAccount, CASE WHEN LOWER(StatusCompare) = 'ok' THEN AmountEVIS ELSE AmountSAP END AmountPPN
		FROM	COMP_EVIS_SAP
		WHERE	IsDeleted = 0 AND SUBSTRING(TaxInvoiceNumberEVIS, 4, 19) = SUBSTRING(@currentTaxInvoiceNumberEvis, 4, 19) AND Id <> @currentId
		ORDER BY Pembetulan DESC

		SET @currentIdx = @currentIdx + 1
	END

	SELECT	t1.Id
			,t1.PostingDate
			,t1.AccountingDocNo
			,t1.ItemNo
			,t1.TglFaktur
			,t1.NamaVendor
			,t1.ScanDate
			,t1.TaxInvoiceNumberEvis 
			,t1.TaxInvoiceNumberSap
			,t1.DocumentHeaderText
			,t1.Npwp 
			,t1.NpwpPenjual 
			,t1.AmountEvis 
			,t1.AmountSap 
			,t1.AmountDiff 
			,t1.StatusCompare 
			,t1.Notes 
			,t1.TotalItems 
			,t1.UserNameCreator 
			,t1.Pembetulan 
			,t1.MasaPajak 
			,t1.TahunPajak 
			,t1.ItemText 
			,t1.FiscalYearDebet 
			,t1.GLAccount 
			,t1.StatusFaktur
			,t2.FpOriginal
			,t2.NpwpOriginal
			,t2.PembetulanOriginal
			,t2.AccountingDocDebetOriginal
			,t2.FiscalYearDebetOriginal
			,t2.GLAccountOriginal
			,t2.AmountPPNOriginal
	FROM	@tempTable t1 LEFT JOIN
			@tempOriginalTable t2 ON t1.Id = t2.IdOriginal

END

GO


/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSap_GetXmlUploadPPNSubmit_ByIdNo]    Script Date: 5/24/2017 5:01:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: GET XML DATA SUBMIT UploadPPNCredit EVIS vs SAP
-- Create date	: 2017-05-24
-- Description	: Preparing data for generate xml send to sap (UploadPPNCredit)
-- =============================================
CREATE PROCEDURE [dbo].[sp_CompEvisVsSap_GetXmlUploadPPNSubmit_ByIdNo]
	-- Add the parameters for the stored procedure here
	@idNo varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @tempTable TABLE
	(
		Idx int identity(1,1) NOT NULL
		,Id bigint
		,PostingDate date
		,AccountingDocNo nvarchar(max)
		,ItemNo nvarchar(max)
		,TglFaktur date
		,NamaVendor nvarchar(max)
		,ScanDate date
		,TaxInvoiceNumberEvis nvarchar(100)
		,TaxInvoiceNumberSap nvarchar(100)
		,DocumentHeaderText nvarchar(max)
		,Npwp nvarchar(100)
		,NpwpPenjual nvarchar(100)
		,AmountEvis decimal(18,2)
		,AmountSap decimal(18,2)
		,AmountDiff decimal(18,2)
		,StatusCompare nvarchar(max)
		,Notes nvarchar(max)
		,TotalItems int
		,UserNameCreator nvarchar(100)
		,Pembetulan int
		,MasaPajak int
		,TahunPajak int
		,ItemText nvarchar(max)
		,FiscalYearDebet int
		,GLAccount nvarchar(100)
		,StatusFaktur nvarchar(max)		
	)

	DECLARE @tempOriginalTable TABLE
	(
		 Id bigint
		,IdOriginal bigint
		,FpOriginal nvarchar(100) NULL
		,NpwpOriginal nvarchar(100) NULL
		,PembetulanOriginal int NULL
		,AccountingDocDebetOriginal nvarchar(max) NULL
		,FiscalYearDebetOriginal int NULL
		,GLAccountOriginal nvarchar(max) NULL
		,AmountPPNOriginal decimal(18, 2) NULL
	)

	DECLARE
		@currentIdx int
		,@maxIdx int
		,@currentTaxInvoiceNumberEvis nvarchar(100)
		,@currentPembetulan int
		,@currentId bigint

	INSERT INTO @tempTable(
			Id
			,PostingDate
			,AccountingDocNo
			,ItemNo
			,TglFaktur
			,NamaVendor
			,ScanDate
			,TaxInvoiceNumberEvis 
			,TaxInvoiceNumberSap
			,DocumentHeaderText
			,Npwp 
			,NpwpPenjual 
			,AmountEvis 
			,AmountSap 
			,AmountDiff 
			,StatusCompare 
			,Notes 
			,TotalItems 
			,UserNameCreator 
			,Pembetulan 
			,MasaPajak 
			,TahunPajak 
			,ItemText 
			,FiscalYearDebet 
			,GLAccount 
			,StatusFaktur)
	SELECT	 d.Id
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]			
			,d.StatusFaktur			
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT
	WHERE	d.IsDeleted = 0 AND d.IdNo = @idNo

	SET @maxIdx = (SELECT MAX(Idx) FROM @tempTable)

	SET @currentIdx = 1
	WHILE @currentIdx <= @maxIdx
	BEGIN
		
		SELECT	@currentTaxInvoiceNumberEvis = TaxInvoiceNumberEvis, 
				@currentId = Id, 
				@currentPembetulan = Pembetulan
		FROM	@tempTable
		WHERE	Idx = @currentIdx

		INSERT INTO @tempOriginalTable(Id, IdOriginal, FpOriginal, NpwpOriginal, PembetulanOriginal, AccountingDocDebetOriginal, FiscalYearDebetOriginal, GLAccountOriginal, AmountPPNOriginal)
		SELECT	TOP 1 @currentId, Id, TaxInvoiceNumberEVIS, NPWPPenjual, Pembetulan, AccountingDocNo, FiscalYearDebet, GLAccount, CASE WHEN LOWER(StatusCompare) = 'ok' THEN AmountEVIS ELSE AmountSAP END AmountPPN
		FROM	COMP_EVIS_SAP
		WHERE	IsDeleted = 0 AND SUBSTRING(TaxInvoiceNumberEVIS, 4, 19) = SUBSTRING(@currentTaxInvoiceNumberEvis, 4, 19) AND Id <> @currentId
		ORDER BY Pembetulan DESC

		SET @currentIdx = @currentIdx + 1
	END

	SELECT	t1.Id
			,t1.PostingDate
			,t1.AccountingDocNo
			,t1.ItemNo
			,t1.TglFaktur
			,t1.NamaVendor
			,t1.ScanDate
			,t1.TaxInvoiceNumberEvis 
			,t1.TaxInvoiceNumberSap
			,t1.DocumentHeaderText
			,t1.Npwp 
			,t1.NpwpPenjual 
			,t1.AmountEvis 
			,t1.AmountSap 
			,t1.AmountDiff 
			,t1.StatusCompare 
			,t1.Notes 
			,t1.TotalItems 
			,t1.UserNameCreator 
			,t1.Pembetulan 
			,t1.MasaPajak 
			,t1.TahunPajak 
			,t1.ItemText 
			,t1.FiscalYearDebet 
			,t1.GLAccount 
			,t1.StatusFaktur
			,t2.FpOriginal
			,t2.NpwpOriginal
			,t2.PembetulanOriginal
			,t2.AccountingDocDebetOriginal
			,t2.FiscalYearDebetOriginal
			,t2.GLAccountOriginal
			,t2.AmountPPNOriginal
	FROM	@tempTable t1 LEFT JOIN
			@tempOriginalTable t2 ON t1.Id = t2.IdOriginal

END

GO


/****** Object:  StoredProcedure [dbo].[sp_MTDownloadPPN_GetStatusReconcileByKey]    Script Date: 5/24/2017 5:01:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_MTDownloadPPN_GetStatusReconcileByKey]
	-- Add the parameters for the stored procedure here
	@AccountingDocNo nvarchar(255)
	,@LineItem nvarchar(255)
	,@PostingDate date
	,@ItemText nvarchar(255)
	,@StatusReconcile int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
	SET @StatusReconcile = ISNULL((SELECT StatusReconcile FROM SAP_MTDownloadPPN 
							WHERE IsDeleted = 0 
								AND AccountingDocNo = @AccountingDocNo 
								AND LineItem = @LineItem
								AND CAST(@PostingDate as date) = CAST(PostingDate AS date)
								AND ItemText = @ItemText), 0)

END
GO

-----------------------------------------
-- update	: 2017-06-13
-- author	: irman sulaeman
-- desc		: feedback change xml send to sap (ref : Error EVIS Simulasi 1.10 - 2017-06-12)
-----------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSap_Insert]    Script Date: 6/13/2017 3:04:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSap_Insert]
	-- Add the parameters for the stored procedure here
	@PostingDate date
           ,@AccountingDocNo nvarchar(255)
           ,@ItemNo nvarchar(255)
           ,@TglFaktur date
           ,@TaxInvoiceNumberEVIS nvarchar(100)
           ,@TaxInvoiceNumberSAP nvarchar(100)
           ,@DocumentHeaderText nvarchar(255)
           ,@NPWP nvarchar(100)
           ,@AmountEVIS decimal(18,2)
           ,@AmountSAP decimal(18,2)
           ,@AmountDiff decimal(18,2)
           ,@StatusCompare nvarchar(255)
           ,@Notes nvarchar(255)
           ,@CreatedBy nvarchar(100)
           ,@IdNo nvarchar(255)           
           ,@MasaPajak int
           ,@TahunPajak int
           ,@ItemText nvarchar(100)
           ,@FiscalYearDebet int
           ,@GLAccount nvarchar(100)
           ,@NPWPPenjual nvarchar(100)
           ,@StatusFaktur nvarchar(255)
		   ,@CompEvisSapId bigint out
AS
BEGIN

	DECLARE
		@pembetulanToInsert int
	
	SET @pembetulanToInsert = (SELECT TOP 1 Pembetulan 
		FROM COMP_EVIS_SAP 
		WHERE SUBSTRING(TaxInvoiceNumberEVIS, 4, 19) = SUBSTRING(@TaxInvoiceNumberEVIS, 4, 19) 
				AND IsDeleted = 0 ORDER BY Id DESC)

	IF @pembetulanToInsert IS NULL
	BEGIN
		SET @pembetulanToInsert = 0
	END
	ELSE
	BEGIN
		SET @pembetulanToInsert = @pembetulanToInsert + 1
	END
			
	INSERT INTO [dbo].[COMP_EVIS_SAP]
           ([PostingDate]
           ,[AccountingDocNo]
           ,[ItemNo]
           ,[TglFaktur]
           ,[TaxInvoiceNumberEVIS]
           ,[TaxInvoiceNumberSAP]
           ,[DocumentHeaderText]
           ,[NPWP]
           ,[AmountEVIS]
           ,[AmountSAP]
           ,[AmountDiff]
           ,[StatusCompare]
           ,[Notes]
           ,[CreatedBy]
           ,[IdNo]
           ,[Pembetulan]
           ,[MasaPajak]
           ,[TahunPajak]
           ,[ItemText]
           ,[FiscalYearDebet]
           ,[GLAccount]
           ,[NPWPPenjual]
           ,[StatusFaktur])
     VALUES
           (@PostingDate
           ,@AccountingDocNo
           ,@ItemNo
           ,@TglFaktur
           ,@TaxInvoiceNumberEVIS
           ,@TaxInvoiceNumberSAP
           ,@DocumentHeaderText
           ,@NPWP
           ,@AmountEVIS
           ,@AmountSAP
           ,@AmountDiff
           ,@StatusCompare
           ,@Notes
           ,@CreatedBy
           ,@IdNo
           ,@pembetulanToInsert
           ,@MasaPajak
           ,@TahunPajak
           ,CASE WHEN @TaxInvoiceNumberSAP IS NULL THEN @TaxInvoiceNumberEVIS + ' ' + FORMAT(GETDATE(), 'yyyyMMdd') ELSE @ItemText END
           ,@FiscalYearDebet
           ,@GLAccount
           ,@NPWPPenjual
           ,@StatusFaktur
		   ); SELECT @CompEvisSapId = @@IDENTITY

	DECLARE
		@modifiedDate datetime = GETDATE()

	-- UPDATE StatusReconcile on SAP_MTDownloadPPN Table to OnProgress (1)
	IF @TaxInvoiceNumberSAP IS NOT NULL AND @PostingDate IS NOT NULL
	BEGIN
		UPDATE SAP_MTDownloadPPN
		SET		StatusReconcile = 1 /*1 = OnProgress*/
				,Modified = @modifiedDate
				,ModifiedBy = @CreatedBy
		WHERE	IsDeleted = 0 AND CAST(PostingDate as date) = CAST(@postingDate as date) 
				AND AccountingDocNo = @AccountingDocNo AND LineItem = @itemNo AND ItemText = @itemText	
	END
	
	-- UPDATE StatusReconcile on FakturPajak Table to OnProgress (1)
	IF @TaxInvoiceNumberEVIS IS NOT NULL
	BEGIN
		UPDATE FakturPajak
		SET StatusReconcile = 1
			,Modified = @modifiedDate
			,ModifiedBy = @CreatedBy
		WHERE IsDeleted = 0 AND FormatedNoFaktur = @TaxInvoiceNumberEVIS
	END
END

GO


/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSap_GetXmlUploadPPNSubmit_ByIdNo]    Script Date: 6/13/2017 3:07:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author		: GET XML DATA SUBMIT UploadPPNCredit EVIS vs SAP
-- Create date	: 2017-05-24
-- Description	: Preparing data for generate xml send to sap (UploadPPNCredit)
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSap_GetXmlUploadPPNSubmit_ByIdNo]
	-- Add the parameters for the stored procedure here
	@idNo varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @tempTable TABLE
	(
		Idx int identity(1,1) NOT NULL
		,Id bigint
		,PostingDate date
		,AccountingDocNo nvarchar(max)
		,ItemNo nvarchar(max)
		,TglFaktur date
		,NamaVendor nvarchar(max)
		,ScanDate date
		,TaxInvoiceNumberEvis nvarchar(100)
		,TaxInvoiceNumberSap nvarchar(100)
		,DocumentHeaderText nvarchar(max)
		,Npwp nvarchar(100)
		,NpwpPenjual nvarchar(100)
		,AmountEvis decimal(18,2)
		,AmountSap decimal(18,2)
		,AmountDiff decimal(18,2)
		,StatusCompare nvarchar(max)
		,Notes nvarchar(max)
		,TotalItems int
		,UserNameCreator nvarchar(100)
		,Pembetulan int
		,MasaPajak int
		,TahunPajak int
		,ItemText nvarchar(max)
		,FiscalYearDebet int
		,GLAccount nvarchar(100)
		,StatusFaktur nvarchar(max)
	)

	DECLARE @tempOriginalTable TABLE
	(
		 Id bigint
		,IdOriginal bigint
		,FpOriginal nvarchar(100) NULL
		,NpwpOriginal nvarchar(100) NULL
		,PembetulanOriginal int NULL
		,AccountingDocDebetOriginal nvarchar(max) NULL
		,FiscalYearDebetOriginal int NULL
		,GLAccountOriginal nvarchar(max) NULL
		,AmountPPNOriginal decimal(18, 2) NULL
		,ItemTextOriginal nvarchar(max)
	)

	DECLARE
		@currentIdx int
		,@maxIdx int
		,@currentTaxInvoiceNumberEvis nvarchar(100)
		,@currentPembetulan int
		,@currentId bigint

	INSERT INTO @tempTable(
			Id
			,PostingDate
			,AccountingDocNo
			,ItemNo
			,TglFaktur
			,NamaVendor
			,ScanDate
			,TaxInvoiceNumberEvis 
			,TaxInvoiceNumberSap
			,DocumentHeaderText
			,Npwp 
			,NpwpPenjual 
			,AmountEvis 
			,AmountSap 
			,AmountDiff 
			,StatusCompare 
			,Notes 
			,TotalItems 
			,UserNameCreator 
			,Pembetulan 
			,MasaPajak 
			,TahunPajak 
			,ItemText 
			,FiscalYearDebet 
			,GLAccount 
			,StatusFaktur)
	SELECT	 d.Id
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]			
			,d.StatusFaktur			
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT 
	WHERE	d.IsDeleted = 0 AND d.IdNo = @idNo

	SET @maxIdx = (SELECT MAX(Idx) FROM @tempTable)

	SET @currentIdx = 1
	WHILE @currentIdx <= @maxIdx
	BEGIN
		
		SELECT	@currentTaxInvoiceNumberEvis = TaxInvoiceNumberEvis, 
				@currentId = Id, 
				@currentPembetulan = Pembetulan
		FROM	@tempTable
		WHERE	Idx = @currentIdx

		INSERT INTO @tempOriginalTable(Id, IdOriginal, FpOriginal, NpwpOriginal, PembetulanOriginal, AccountingDocDebetOriginal, FiscalYearDebetOriginal, GLAccountOriginal, AmountPPNOriginal, ItemTextOriginal)
		SELECT	TOP 1 @currentId, Id, TaxInvoiceNumberEVIS, CASE WHEN LOWER(StatusCompare) = 'ok' THEN NPWP ELSE NPWPPenjual END AS NpwpOriginal, Pembetulan, AccountingDocNo, FiscalYearDebet, GLAccount, CASE WHEN LOWER(StatusCompare) = 'ok' THEN AmountSAP ELSE AmountEVIS END AmountPPN, ItemText
		FROM	COMP_EVIS_SAP
		WHERE	IsDeleted = 0 AND SUBSTRING(TaxInvoiceNumberEVIS, 4, 19) = SUBSTRING(@currentTaxInvoiceNumberEvis, 4, 19) AND Id <> @currentId
		ORDER BY Pembetulan DESC

		SET @currentIdx = @currentIdx + 1
	END

	SELECT	t1.Id
			,t1.PostingDate
			,t1.AccountingDocNo
			,t1.ItemNo
			,t1.TglFaktur
			,t1.NamaVendor
			,t1.ScanDate
			,t1.TaxInvoiceNumberEvis 
			,t1.TaxInvoiceNumberSap
			,t1.DocumentHeaderText
			,t1.Npwp 
			,t1.NpwpPenjual 
			,t1.AmountEvis 
			,t1.AmountSap 
			,t1.AmountDiff 
			,t1.StatusCompare 
			,t1.Notes 
			,t1.TotalItems 
			,t1.UserNameCreator 
			,t1.Pembetulan 
			,t1.MasaPajak 
			,t1.TahunPajak 
			,t1.ItemText 
			,t1.FiscalYearDebet 
			,t1.GLAccount 
			,t1.StatusFaktur
			,t2.FpOriginal
			,t2.NpwpOriginal
			,t2.PembetulanOriginal
			,t2.AccountingDocDebetOriginal
			,t2.FiscalYearDebetOriginal
			,t2.GLAccountOriginal
			,t2.AmountPPNOriginal
			,t2.ItemTextOriginal
	FROM	@tempTable t1 LEFT JOIN
			@tempOriginalTable t2 ON t1.Id = t2.Id

END
GO

/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSap_GetXmlUploadPPNSubmit]    Script Date: 6/13/2017 3:07:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author		: GET XML DATA SUBMIT UploadPPNCredit EVIS vs SAP
-- Create date	: 2017-05-24
-- Description	: Preparing data for generate xml send to sap (UploadPPNCredit)
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSap_GetXmlUploadPPNSubmit]
	-- Add the parameters for the stored procedure here
	@ids varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @tempTable TABLE
	(
		Idx int identity(1,1) NOT NULL
		,Id bigint
		,PostingDate date
		,AccountingDocNo nvarchar(max)
		,ItemNo nvarchar(max)
		,TglFaktur date
		,NamaVendor nvarchar(max)
		,ScanDate date
		,TaxInvoiceNumberEvis nvarchar(100)
		,TaxInvoiceNumberSap nvarchar(100)
		,DocumentHeaderText nvarchar(max)
		,Npwp nvarchar(100)
		,NpwpPenjual nvarchar(100)
		,AmountEvis decimal(18,2)
		,AmountSap decimal(18,2)
		,AmountDiff decimal(18,2)
		,StatusCompare nvarchar(max)
		,Notes nvarchar(max)
		,TotalItems int
		,UserNameCreator nvarchar(100)
		,Pembetulan int
		,MasaPajak int
		,TahunPajak int
		,ItemText nvarchar(max)
		,FiscalYearDebet int
		,GLAccount nvarchar(100)
		,StatusFaktur nvarchar(max)	
	)

	DECLARE @tempOriginalTable TABLE
	(
		 Id bigint
		,IdOriginal bigint
		,FpOriginal nvarchar(100) NULL
		,NpwpOriginal nvarchar(100) NULL
		,PembetulanOriginal int NULL
		,AccountingDocDebetOriginal nvarchar(max) NULL
		,FiscalYearDebetOriginal int NULL
		,GLAccountOriginal nvarchar(max) NULL
		,AmountPPNOriginal decimal(18, 2) NULL		
		,ItemTextOriginal nvarchar(max)
	)

	DECLARE
		@currentIdx int
		,@maxIdx int
		,@currentTaxInvoiceNumberEvis nvarchar(100)
		,@currentPembetulan int
		,@currentId bigint

	INSERT INTO @tempTable(
			Id
			,PostingDate
			,AccountingDocNo
			,ItemNo
			,TglFaktur
			,NamaVendor
			,ScanDate
			,TaxInvoiceNumberEvis 
			,TaxInvoiceNumberSap
			,DocumentHeaderText
			,Npwp 
			,NpwpPenjual 
			,AmountEvis 
			,AmountSap 
			,AmountDiff 
			,StatusCompare 
			,Notes 
			,TotalItems 
			,UserNameCreator 
			,Pembetulan 
			,MasaPajak 
			,TahunPajak 
			,ItemText 
			,FiscalYearDebet 
			,GLAccount 
			,StatusFaktur)
	SELECT	 d.Id
			,d.PostingDate
			,d.AccountingDocNo
			,d.ItemNo
			,d.TglFaktur
			,fp.NamaPenjual AS NamaVendor
			,fp.Created AS ScanDate
			,d.TaxInvoiceNumberEvis
			,d.TaxInvoiceNumberSap
			,d.DocumentHeaderText
			,d.NPWP
			,d.NPWPPenjual
			,d.AmountEvis
			,d.AmountSap
			,d.AmountDiff
			,d.StatusCompare
			,d.Notes
			,COUNT(d.PostingDate) OVER() AS TotalItems
			,fp.CreatedBy AS UserNameCreator
			,d.[Pembetulan]
			,d.[MasaPajak]
			,d.[TahunPajak]
			,d.[ItemText]
			,d.[FiscalYearDebet]
			,d.[GLAccount]			
			,d.StatusFaktur
	FROM	[dbo].[COMP_EVIS_SAP] d LEFT JOIN
			[dbo].FakturPajak fp ON d.TaxInvoiceNumberEVIS COLLATE DATABASE_DEFAULT = fp.FormatedNoFaktur COLLATE DATABASE_DEFAULT
	WHERE	d.Id IN (SELECT Data FROM dbo.Split(@ids))

	SET @maxIdx = (SELECT MAX(Idx) FROM @tempTable)

	SET @currentIdx = 1
	WHILE @currentIdx <= @maxIdx
	BEGIN
		
		SELECT	@currentTaxInvoiceNumberEvis = TaxInvoiceNumberEvis, 
				@currentId = Id, 
				@currentPembetulan = Pembetulan
		FROM	@tempTable
		WHERE	Idx = @currentIdx

		INSERT INTO @tempOriginalTable(Id, IdOriginal, FpOriginal, NpwpOriginal, PembetulanOriginal, AccountingDocDebetOriginal, FiscalYearDebetOriginal, GLAccountOriginal, AmountPPNOriginal, ItemTextOriginal)
		SELECT	TOP 1 @currentId, Id, TaxInvoiceNumberEVIS, CASE WHEN LOWER(StatusCompare) = 'ok' THEN NPWP ELSE NPWPPenjual END AS NpwpOriginal, Pembetulan, AccountingDocNo, FiscalYearDebet, GLAccount, CASE WHEN LOWER(StatusCompare) = 'ok' THEN AmountSAP ELSE AmountEVIS END AmountPPN, ItemText
		FROM	COMP_EVIS_SAP
		WHERE	IsDeleted = 0 AND SUBSTRING(TaxInvoiceNumberEVIS, 4, 19) = SUBSTRING(@currentTaxInvoiceNumberEvis, 4, 19) AND Id <> @currentId
		ORDER BY Pembetulan DESC

		SET @currentIdx = @currentIdx + 1
	END

	SELECT	t1.Id
			,t1.PostingDate
			,t1.AccountingDocNo
			,t1.ItemNo
			,t1.TglFaktur
			,t1.NamaVendor
			,t1.ScanDate
			,t1.TaxInvoiceNumberEvis 
			,t1.TaxInvoiceNumberSap
			,t1.DocumentHeaderText
			,t1.Npwp 
			,t1.NpwpPenjual 
			,t1.AmountEvis 
			,t1.AmountSap 
			,t1.AmountDiff 
			,t1.StatusCompare 
			,t1.Notes 
			,t1.TotalItems 
			,t1.UserNameCreator 
			,t1.Pembetulan 
			,t1.MasaPajak 
			,t1.TahunPajak 
			,t1.ItemText 
			,t1.FiscalYearDebet 
			,t1.GLAccount 
			,t1.StatusFaktur
			,t2.FpOriginal
			,t2.NpwpOriginal
			,t2.PembetulanOriginal
			,t2.AccountingDocDebetOriginal
			,t2.FiscalYearDebetOriginal
			,t2.GLAccountOriginal
			,t2.AmountPPNOriginal
			,t2.ItemTextOriginal
	FROM	@tempTable t1 LEFT JOIN
			@tempOriginalTable t2 ON t1.Id = t2.Id

END
GO
/****** Object:  StoredProcedure [dbo].[sp_FakturPajak_GetStatusReconcileByTaxInvoiceNumber]    Script Date: 6/13/2017 3:07:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_FakturPajak_GetStatusReconcileByTaxInvoiceNumber]
	-- Add the parameters for the stored procedure here
	@TaxInvoiceNumber nvarchar(255)
	,@StatusReconcile int OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
	SET @StatusReconcile = ISNULL((SELECT TOP(1) StatusReconcile FROM FakturPajak WHERE IsDeleted = 0 AND FormatedNoFaktur = @TaxInvoiceNumber), 0)

END
GO
-- updated date : 2017-06-20
-- author		: irman sulaeman
-- desc			: fixing feedback tgl 17 juni 2017
/****** Object:  View [dbo].[View_FakturPajak]    Script Date: 6/20/2017 9:33:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-------
------- UPDATE 2017-06-20
ALTER VIEW [dbo].[View_FakturPajak]
AS
SELECT fp.[FakturPajakId]
      ,fp.[FCode]
      ,LTRIM(RTRIM(fp.[UrlScan])) AS UrlScan
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
      ,CASE WHEN fp.FPType = 3 AND [StatusFaktur] IS NULL THEN (CASE WHEN fp.FgPengganti = 1 THEN 'Faktur Pajak Normal-Pengganti' ELSE 'Faktur Pajak Normal' END) ELSE fp.[StatusFaktur] END AS [StatusFaktur]
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


/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerate]    Script Date: 6/21/2017 10:09:45 AM ******/
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
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerate]
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
	
	IF @masaPajak IS NOT NULL AND @tahunPajak IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
			SELECT	comp_result.*, CAST(0 AS int) AS Pembetulan, CAST(0 AS bigint) AS Id
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
									AND [StatusFaktur] <> 'Faktur Diganti'
									AND (StatusReconcile IS NULL OR StatusReconcile = 0 OR StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/
						) evissrc
						WHERE evissrc.vPartition = 1								
								AND (@scanDate IS NULL OR (@scanDate IS NOT NULL AND CAST(evissrc.Created as date) = CAST(@scanDate as date)))
								AND (@userName IS NULL OR (@userName IS NOT NULL AND LOWER(evissrc.CreatedBy) LIKE REPLACE(LOWER(@userName), '*', '%')))
								AND (@masaPajak IS NULL OR (@masaPajak IS NOT NULL AND evissrc.MasaPajak = @masaPajak))
								AND (@tahunPajak IS NULL OR (@tahunPajak IS NOT NULL AND evissrc.TahunPajak = @tahunPajak))
								AND (
									(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturStart as date))
									OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturEnd as date))
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) >= CAST(@tglFakturStart as date) 
										AND CAST(evissrc.TglFaktur as date) <= CAST(@tglFakturEnd as date)
										)
								)
								AND (
									(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL 
										AND evissrc.FormatedNoFaktur = @noFakturStart)
									OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL 
										AND evissrc.FormatedNoFaktur = @noFakturEnd)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL 
										AND (evissrc.FormatedNoFaktur BETWEEN @noFakturStart AND @noFakturEnd)
										)
								)
					) efis LEFT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR LTRIM(RTRIM(v1.ReverseDocNo)) = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 CAST(0 AS bigint) AS Id
					,1 AS vSequence
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
			SELECT	comp_result.*, CAST(0 AS int) AS Pembetulan, CAST(0 AS bigint) AS Id
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
									AND [StatusFaktur] <> 'Faktur Diganti'
									AND (StatusReconcile IS NULL OR StatusReconcile = 0 OR StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
						) evissrc
						WHERE evissrc.vPartition = 1								
					) efis RIGHT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR v1.ReverseDocNo = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
						AND (
								(@postingDateStart IS NULL AND @postingDateEnd IS NULL)
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateStart as date))
								OR (@postingDateStart IS NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateEnd as date))
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) >= CAST(@postingDateStart as date) 
									AND CAST(v1.PostingDate as date) <= CAST(@postingDateEnd as date)
									)
							)							
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
			FETCH NEXT @ItemPerPage ROWS ONLY
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					, CAST(0 AS bigint) AS Id
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END

END
GO
/****** Object:  StoredProcedure [dbo].[sp_CompEvisVsSapGetGenerateToDownload]    Script Date: 6/21/2017 10:10:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CompEvisVsSapGetGenerateToDownload]
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
	
	IF @masaPajak IS NOT NULL AND @tahunPajak IS NOT NULL
	BEGIN
		
		IF @postingDateStart IS NULL AND @postingDateEnd IS NULL
		BEGIN
			-- EVIS - SAP : LEFT JOIN
			SELECT	comp_result.*, CAST(0 AS int) AS Pembetulan, CAST(0 AS bigint) AS Id
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
									AND [StatusFaktur] <> 'Faktur Diganti'
									AND (StatusReconcile IS NULL OR StatusReconcile = 0 OR StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
						) evissrc
						WHERE evissrc.vPartition = 1								
								AND (@scanDate IS NULL OR (@scanDate IS NOT NULL AND CAST(evissrc.Created as date) = CAST(@scanDate as date)))
								AND (@userName IS NULL OR (@userName IS NOT NULL AND LOWER(evissrc.CreatedBy) LIKE REPLACE(LOWER(@userName), '*', '%')))
								AND (@masaPajak IS NULL OR (@masaPajak IS NOT NULL AND evissrc.MasaPajak = @masaPajak))
								AND (@tahunPajak IS NULL OR (@tahunPajak IS NOT NULL AND evissrc.TahunPajak = @tahunPajak))
								AND (
									(@tglFakturStart IS NULL AND @tglFakturEnd IS NULL)
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturStart as date))
									OR (@tglFakturStart IS NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) = CAST(@tglFakturEnd as date))
									OR (@tglFakturStart IS NOT NULL AND @tglFakturEnd IS NOT NULL 
										AND CAST(evissrc.TglFaktur as date) >= CAST(@tglFakturStart as date) 
										AND CAST(evissrc.TglFaktur as date) <= CAST(@tglFakturEnd as date)
										)
								)
								AND (
									(@noFakturStart IS NULL AND @noFakturEnd IS NULL)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NULL 
										AND evissrc.FormatedNoFaktur = @noFakturStart)
									OR (@noFakturStart IS NULL AND @noFakturEnd IS NOT NULL 
										AND evissrc.FormatedNoFaktur = @noFakturEnd)
									OR (@noFakturStart IS NOT NULL AND @noFakturEnd IS NOT NULL 
										AND (evissrc.FormatedNoFaktur BETWEEN @noFakturStart AND @noFakturEnd)
										)
								)
					) efis LEFT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR LTRIM(RTRIM(v1.ReverseDocNo)) = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					, CAST(0 AS bigint) AS Id
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
	ELSE
	BEGIN
		IF @postingDateStart IS NOT NULL OR @postingDateEnd IS NOT NULL
		BEGIN
			-- EVIS - SAP : RIGHT JOIN
			SELECT	comp_result.*, CAST(0 AS int) AS Pembetulan, CAST(0 AS bigint) AS Id
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
						,efis.NPWPPenjual
						,efis.AmountEvis AS AmountEvis
						,sap.AmountLocal AS AmountSap
						,ABS(ISNULL(efis.AmountEvis, 0) - ISNULL(sap.AmountLocal, 0)) AS AmountDiff
						,dbo.fnCompEvisVsSapGetStatusCompare(efis.FormatedNoFaktur, sap.TaxInvoiceNumber, efis.AmountEvis, sap.AmountLocal, @diffToleran, sap.ItemText) AS StatusCompare
						,'' AS Notes
						,COUNT(*) OVER() AS TotalItems
						,efis.CreatedBy AS UserNameCreator
						,sap.ItemText
						,efis.MasaPajak
						,efis.TahunPajak
						,CASE WHEN sap.FiscalYearDebet IS NULL THEN CAST(NULL AS int) ELSE CAST(sap.FiscalYearDebet AS int) END AS FiscalYearDebet
						,sap.GLAccount
						,dbo.fnCompEvisVsSapGetDocStatus(efis.FormatedNoFaktur, sap.TaxInvoiceNumber) AS DocumentStatus
						,efis.StatusFaktur AS StatusFaktur
				FROM	(
						SELECT	evissrc.*
						FROM	(
							SELECT	 fp.NoFakturPajak		
									,fp.FormatedNoFaktur
									,fp.NPWPPenjual
									,fp.JumlahPPN AS AmountEvis
									,fp.TglFaktur AS EvisTanggalFaktur			
									,fp.Created AS ScanDate
									,fp.NamaPenjual
									,fp.CreatedBy
									,fp.MasaPajak
									,fp.TahunPajak
									,fp.TglFaktur
									,fp.StatusFaktur
									,row_number() over(PARTITION BY fp.NoFakturPajak ORDER BY fp.FakturPajakId DESC) vPartition
									,fp.StatusReconcile
									,fp.Created
							FROM	dbo.FakturPajak fp
							WHERE	fp.IsDeleted = 0 
									AND [Status] = 2
									AND [StatusFaktur] <> 'Faktur Diganti'
									AND (StatusReconcile IS NULL OR StatusReconcile = 0 OR StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
						) evissrc
						WHERE evissrc.vPartition = 1								
					) efis RIGHT JOIN
					(
						SELECT	v1.PostingDate,
								v1.AccountingDocNo,
								v1.PostingKey,
								v1.ReverseDocNo,		
								v1.LineItem AS ItemNo,
								v1.ItemText,
								dbo.fnGetNoFakturFromItemText(v1.ItemText) AS TaxInvoiceNumber,									
								v1.HeaderText AS DocumentHeaderText,
								v1.AssignmentNo AS NPWP,
								v1.Id,
								CASE WHEN RTRIM(LTRIM(v1.AmountLocal)) = '' THEN NULL ELSE CAST(RTRIM(LTRIM(v1.AmountLocal)) AS decimal(18,0)) END AS AmountLocal,
								v1.StatusReconcile,
								v1.FiscalYear AS FiscalYearDebet,
								v1.GLAccount
						FROM	SAP_MTDownloadPPN v1
						WHERE	v1.IsDeleted = 0 AND v1.ItemText IN (
							SELECT	DISTINCT partdata.ItemText
									FROM	(
									SELECT	row_number() over(PARTITION BY SUBSTRING(dbo.fnGetNoFakturFromItemText(ItemText), 4, 19) ORDER BY Id DESC) vSequence,		
														ItemText
												FROM	SAP_MTDownloadPPN
												WHERE	IsDeleted = 0 AND (ItemText IS NOT NULL AND LTRIM(RTRIM(ItemText)) <> '' AND LEN(ItemText) = 28)
											) partdata
									WHERE partdata.vSequence = 1	
						)
						AND (v1.ReverseDocNo IS NULL OR v1.ReverseDocNo = '')
						AND (v1.ClearingDate IS NULL OR LTRIM(RTRIM(v1.ClearingDoc)) = '')
						AND (v1.StatusReconcile IS NULL OR v1.StatusReconcile = 0 OR v1.StatusReconcile = 3) /*NULL / 0 = No Yet Reconcile, 3 = Error Reconcile*/								
						AND (
								(@postingDateStart IS NULL AND @postingDateEnd IS NULL)
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateStart as date))
								OR (@postingDateStart IS NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) = CAST(@postingDateEnd as date))
								OR (@postingDateStart IS NOT NULL AND @postingDateEnd IS NOT NULL 
									AND CAST(v1.PostingDate as date) >= CAST(@postingDateStart as date) 
									AND CAST(v1.PostingDate as date) <= CAST(@postingDateEnd as date)
									)
							)							
					) sap ON efis.FormatedNoFaktur COLLATE DATABASE_DEFAULT = sap.TaxInvoiceNumber COLLATE DATABASE_DEFAULT
				) comp_result
			ORDER BY AccountingDocNo DESC
			OPTION (OPTIMIZE FOR UNKNOWN)
		END
		ELSE
		BEGIN
			-- ??
			SELECT	 1 AS vSequence
					, CAST(0 AS bigint) AS Id
					,CAST(NULL as date) AS PostingDate
					,'' AS AccountingDocNo
					,'' AS ItemNo
					,CAST(NULL as date) AS TglFaktur
					,'' AS NamaVendor
					,CAST(NULL as date) AS ScanDate
					,'' AS TaxInvoiceNumberEvis
					,'' AS TaxInvoiceNumberSap
					,'' AS DocumentHeaderText
					,'' AS NPWP
					,'' AS NPWPPenjual
					,CAST(NULL AS decimal) AS AmountEvis
					,CAST(NULL AS decimal) AS AmountSap
					,CAST(NULL AS decimal) AS AmountDiff
					,'' AS StatusCompare
					,'' AS Notes
					,CAST(0 AS int) AS TotalItems
					,'' AS UserNameCreator
					,'' AS ItemText
					,CAST(0 AS int) AS MasaPajak
					,CAST(0 AS int) AS TahunPajak
					,CAST(0 AS int) AS FiscalYearDebet
					,'' AS GLAccount
					,CAST(0 AS int) AS DocumentStatus
					,'' AS StatusFaktur
		END
	END
END
GO