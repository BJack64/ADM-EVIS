DELETE FROM LogRequestFakturPajak
DELETE FROM UserAuthentication
DELETE FROM ErrorLog
DELETE FROM LogProcessSap
DELETE FROM LogPrintFakturPajak
DELETE FROM LogSap
DELETE FROM FakturPajakDetail
DELETE FROM FakturPajakRetur
DELETE FROM COMP_EVIS_IWS
DELETE FROM COMP_EVIS_SAP
DELETE FROM LogRequestFakturPajak
DELETE FROM FakturPajak
DELETE FROM Vendor
DELETE FROM LogDownload
DELETE FROM OpenClosePeriod
DELETE FROM ReportSPMDetail
DELETE FROM ReportSPM
DBCC CHECKIDENT ('ReportSPMDetail', RESEED, 0);  
GO 
DBCC CHECKIDENT ('ReportSPM', RESEED, 0);  
GO 
DBCC CHECKIDENT ('UserAuthentication', RESEED, 0);  
GO 
DBCC CHECKIDENT ('ErrorLog', RESEED, 0);  
GO 
DBCC CHECKIDENT ('LogProcessSap', RESEED, 0);  
GO 
DBCC CHECKIDENT ('LogPrintFakturPajak', RESEED, 0);  
GO 
DBCC CHECKIDENT ('LogSap', RESEED, 0);  
GO 
DBCC CHECKIDENT ('FakturPajakDetail', RESEED, 0);  
GO 
DBCC CHECKIDENT ('FakturPajakRetur', RESEED, 0);  
GO 
DBCC CHECKIDENT ('COMP_EVIS_IWS', RESEED, 0);  
GO 
DBCC CHECKIDENT ('COMP_EVIS_SAP', RESEED, 0);  
GO 
DBCC CHECKIDENT ('LogRequestFakturPajak', RESEED, 0);  
GO
DBCC CHECKIDENT ('FakturPajak', RESEED, 0);  
GO 

DBCC CHECKIDENT ('LogRequestFakturPajak', RESEED, 0);  
GO

DBCC CHECKIDENT('Vendor', RESEED, 0)
GO


DBCC CHECKIDENT('LogDownload', RESEED, 0)
GO

DBCC CHECKIDENT('OpenClosePeriod', RESEED, 0)
GO