using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using eFakturADM.Data;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using System.Reflection;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using WatcherLibrary.Objects;
using System.ServiceProcess;
using System.Diagnostics;

namespace WatcherLibrary
{
    public class Watcher
    {
        //private int _iterasi;

        public void StartReadFile(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return;
            var fileName = Path.GetFileName(fullPath);
            var sourceFile = new FileInfo(fullPath);
            //check is file success
            var dbCheck = LogProcessSaps.GetByFileName(fileName);
            if (dbCheck.Count > 0)
            {
                var chkStatus = dbCheck.FirstOrDefault(c => c.Status == (int)ApplicationEnums.SapStatusLog.Success);
                if (chkStatus != null)
                {
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Info, string.Format(@"{0} already process successfully at {1}", fullPath, chkStatus.Created.ToString("dd-MM-yyyy HH:mmss")), MethodBase.GetCurrentMethod());
                    return;
                }
            }
            try
            {

                var fileNamePart = fileName.Split('_');
                var backupFolder = string.Format(@"{0}\{1}", WatcherConfiguration.ResultFolder, DateTime.Now.ToString("MMMM yyyy"));
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                var backupFile = string.Format(@"{0}\{1}", backupFolder, fileName);
                Thread.Sleep(WatcherConfiguration.TimeSleep);

                //copy to backup
                try
                {
                    File.Copy(fullPath, backupFile, true);
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Info, "Success Backup File to " + backupFile, MethodBase.GetCurrentMethod());
                }
                catch (FileNotFoundException ex)
                {
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, string.Format("FileNotFoundException : Output file {0} not yet ready.", fullPath), MethodBase.GetCurrentMethod(), ex);
                    return;
                }
                catch (IOException ex)
                {
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, string.Format("IOException : Output file {0} not yet ready.", fullPath), MethodBase.GetCurrentMethod(), ex);
                    return;
                }
                catch (UnauthorizedAccessException ex)
                {
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, string.Format("UnauthorizedAccessException : Output file {0} not yet ready.", fullPath), MethodBase.GetCurrentMethod(), ex);
                    return;
                }
                catch (Exception ex)
                {
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, string.Format("UnknownException : Output file {0} not yet ready.", fullPath), MethodBase.GetCurrentMethod(), ex);
                    return;
                }

                Thread.Sleep(WatcherConfiguration.TimeSleep);
                int status = (int)ApplicationEnums.SapStatusLog.Error;
                string note = string.Empty;
                string idNo = string.Empty;
                var isProcesFile = false;

                if (fileNamePart[0] == Helper.DownloadPpnPrefixFile)
                {
                    ReadXMLDownloadPpn(sourceFile, out status, out note);
                    isProcesFile = true;
                }
                else if (fileNamePart[0] == Helper.OutUploadPpnCreditPrefixFile)
                {
                    ReadXmlOutUploadPpnCredit(sourceFile, out status, out note, out idNo);
                    isProcesFile = true;
                }
                if (!isProcesFile) return;
                //create log
                var logProcessData = new LogProcessSap()
                {
                    IdNo = idNo,
                    Status = status,
                    Note = note,
                    FileName = fileName,
                    FilePath = File.Exists(fullPath) ? fullPath : backupFile,
                    CreatedBy = "WatcherService",
                    XmlFileType = fileNamePart[0]
                };
                LogProcessSaps.Save(logProcessData);
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void ReadXMLDownloadPpn(FileInfo file, out int status, out string note)
        {
            status = (int)ApplicationEnums.SapStatusLog.Success;
            note = string.Empty;
            if (file.Length > 0)
            {
                try
                {
                    //Log.JustLog("Start Read XML... (ReadXMLDownloadPpn) with filename : " + file.Name, MethodBase.GetCurrentMethod());
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Info, "Start Read XML... (ReadXMLDownloadPpn) with filename : " + file.Name, MethodBase.GetCurrentMethod());
                    XmlDocument xml = new XmlDocument();
                    xml.Load(file.FullName);

                    //Set XML Namespace
                    NameTable nt = new NameTable();
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
                    nsmgr.AddNamespace("ns1", "http://admevis");

                    //Get Data 
                    //Get Data 
                    XmlNodeList itemNode = xml.SelectNodes("/ns1:MT_DownloadPPN/Output/Item", nsmgr);

                    ProcessMtDownloadPpn(itemNode);

                    //Log.JustLog("Finish Processing Read XML... (ReadXMLDownloadPpn) with filename : " + file.Name, MethodBase.GetCurrentMethod());
                    Logger.WriteLog(out logKey, LogLevel.Info, "Finish Processing Read XML... (ReadXMLDownloadPpn) with filename : " + file.Name, MethodBase.GetCurrentMethod());
                    file.Delete();
                    //Log.JustLog("Finish delete filename : " + file.Name, MethodBase.GetCurrentMethod());
                    Logger.WriteLog(out logKey, LogLevel.Info, "Finish delete filename : " + file.Name, MethodBase.GetCurrentMethod());
                    //_iterasi = 0;
                }
                catch (Exception exx)
                {
                    //Log.WriteLog("Error in processing Read XML step2 with filename : " + file.Name + " (ReadXMLDownloadPpn) with message " + exx.Message, MethodBase.GetCurrentMethod());
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, "Error in processing Read XML step2 with filename : " + file.Name + " (ReadXMLDownloadPpn) with message " + exx.Message, MethodBase.GetCurrentMethod(), exx);
                    status = (int)ApplicationEnums.SapStatusLog.Error;

                    if (exx.Message.ToLower().Contains("timeout"))
                    {
                        // Service will be restarted when error timeout
                        RestartWindowsService("XMLWatcherService", exx.Message);
                    }
                }
            }
        }

        private void ReadXmlOutUploadPpnCredit(FileInfo file, out int status, out string note, out string idNo)
        {
            status = (int)ApplicationEnums.SapStatusLog.Success;
            note = string.Empty;
            idNo = string.Empty;
            if (file.Length > 0)
            {
                try
                {
                    //Log.JustLog("Start Read XML... (ReadXMLDownloadPpn) with filename : " + file.Name, MethodBase.GetCurrentMethod());
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Info, "Start Read XML... (ReadXmlOutUploadPpnCredit) with filename : " + file.Name, MethodBase.GetCurrentMethod());
                    XmlDocument xml = new XmlDocument();
                    xml.Load(file.FullName);

                    //Set XML Namespace
                    NameTable nt = new NameTable();
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
                    nsmgr.AddNamespace("ns1", "http://admevis");

                    //Get Data 

                    var headerNode = xml.SelectSingleNode("/ns1:MT_UploadPPNCredit_Resp", nsmgr);

                    XmlNodeList itemNode = xml.SelectNodes("/ns1:MT_UploadPPNCredit_Resp/Output/Item", nsmgr);

                    ProcessOutUploadPpnCredit(headerNode, itemNode, out idNo, out status, out note);

                    //Log.JustLog("Finish Processing Read XML... (ReadXMLDownloadPpn) with filename : " + file.Name, MethodBase.GetCurrentMethod());
                    Logger.WriteLog(out logKey, LogLevel.Info, "Finish Processing Read XML... (ReadXmlOutUploadPpnCredit) with filename : " + file.Name, MethodBase.GetCurrentMethod());
                    file.Delete();
                    //Log.JustLog("Finish delete filename : " + file.Name, MethodBase.GetCurrentMethod());
                    Logger.WriteLog(out logKey, LogLevel.Info, "Finish delete filename : " + file.Name, MethodBase.GetCurrentMethod());
                    //_iterasi = 0;
                }
                catch (Exception exx)
                {
                    //Log.WriteLog("Error in processing Read XML step2 with filename : " + file.Name + " (ReadXMLDownloadPpn) with message " + exx.Message, MethodBase.GetCurrentMethod());
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, "Error in processing Read XML step2 with filename : " + file.Name + " (ReadXmlOutUploadPpnCredit) with message " + exx.Message, MethodBase.GetCurrentMethod(), exx);

                    if (exx.Message.ToLower().Contains("timeout"))
                    {
                        RestartWindowsService("XMLWatcherService", exx.Message);
                    }
                }
            }
        }

        private void ProcessMtDownloadPpn(XmlNodeList itemNode)
        {

            if (itemNode == null || itemNode.Count <= 0) return;

            var dsParamT = new dsParamTable();

            foreach (XmlNode item in itemNode)
            {
                var dRow = dsParamT.SAP_MTDownloadPPN_ParamTable.NewSAP_MTDownloadPPN_ParamTableRow();
                dRow.CompanyCode = item["CompanyCode"].InnerText.Trim();
                dRow.AccountingDocNo = item["AccountingDocNo"].InnerText.Trim();
                dRow.FiscalYear = item["FiscalYear"].InnerText.Trim();
                dRow.DocType = item["DocType"].InnerText.Trim();
                dRow.PostingDate = item["PostingDate"].InnerText.Trim();
                dRow.AmountLocal = item["AmountLocal"].InnerText.Trim();
                dRow.LineItem = item["LineItem"].InnerText.Trim();
                dRow.Reference = item["Reference"].InnerText.Trim();
                dRow.HeaderText = item["HeaderText"].InnerText.Trim();
                dRow.UserName = item["UserName"].InnerText.Trim();
                dRow.ItemText = item["ItemText"].InnerText.Trim();
                dRow.AssignmentNo = item["AssignmentNo"].InnerText.Trim();
                dRow.BusinessArea = item["BusinessArea"].InnerText.Trim();
                dRow.GLAccount = item["GLAccount"].InnerText.Trim();
                dRow.ReferenceLineItem = item["ReferenceLineItem"].InnerText.Trim();
                dRow.DocDate = item["DocDate"].InnerText.Trim();
                dRow.Currency = item["Currency"].InnerText.Trim();
                dRow.SalesTaxCode = item["SalesTaxCode"].InnerText.Trim();
                dRow.AmountDocCurrency = item["AmountDocCurrency"].InnerText.Trim();
                dRow.PostingKey = item["PostingKey"].InnerText.Trim();
                dRow.ClearingDoc = item["ClearingDoc"].InnerText.Trim();
                dRow.ClearingDate = item["ClearingDate"].InnerText.Trim();
                dRow.ReverseDocNo = item["ReverseDocNo"].InnerText.Trim();
                dRow.ReverseDocFiscalYear = item["ReverseDocFiscalYear"].InnerText.Trim();
                dRow.CreatedBy = "WatcherLibrary.Watcher";
                dRow.ModifiedBy = "WatcherLibrary.Watcher";
                dRow.ReferenceKey1 = item["ReferenceKey1"].InnerText.Trim();
                dRow.ReferenceKey2 = item["ReferenceKey2"].InnerText.Trim();
                dsParamT.SAP_MTDownloadPPN_ParamTable.AddSAP_MTDownloadPPN_ParamTableRow(dRow);
            }
            sqlsrv SQL = null;
            try
            {
                SQL = new sqlsrv();

                string spname;
                string tabletypename;

                spname = "sp_MTDownloadPPN_ProcessXML";
                tabletypename = "@paramTable";
                SQL.InsertTable_cmd(spname, tabletypename, dsParamT.SAP_MTDownloadPPN_ParamTable);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SQL.close_con();
            }

        }

        private void ProcessOutUploadPpnCredit(XmlNode node, XmlNodeList childNodes, out string idNo, out int status, out string notes)
        {
            idNo = node["IDNo"].InnerText.Trim();
            notes = string.Empty;
            //checkby idno
            var chkBy = LogSaps.GetByIdNo(idNo);
            if (chkBy == null)
            {
                string logKey;
                notes = "IDNo : " + idNo + " tidak terdaftar di system evis";
                status = (int)ApplicationEnums.SapStatusLog.ErrorProcessOutputXml;
                Logger.WriteLog(out logKey, LogLevel.Error, notes, MethodBase.GetCurrentMethod());
                //update database
                LogSaps.UpdateFromSap(idNo, status, notes, "", null, "WatcherServices");
                //digagalkan semua
                CompEvisSaps.UpdateStatusReconcileByIdNo(idNo, (int)ApplicationEnums.StatusReconcile.Error, "WatcherServices");
                return;
            }

            //get UploadPPNCredit by idNo
            var datsUploaded = CompEvisSaps.GetByIdNo(idNo);
            if (datsUploaded.Count <= 0)
            {
                string logKey;
                notes = "IDNo : " + idNo + " tidak ada data";
                status = (int)ApplicationEnums.SapStatusLog.ErrorProcessOutputXml;
                Logger.WriteLog(out logKey, LogLevel.Error, notes, MethodBase.GetCurrentMethod());
                //update database
                LogSaps.UpdateFromSap(idNo, status, notes, "", null, "WatcherServices");
                //digagalkan semua
                CompEvisSaps.UpdateStatusReconcileByIdNo(idNo, (int)ApplicationEnums.StatusReconcile.Error, "WatcherServices");
                return;
            }

            //next process
            var confirm = node["Confirm"].InnerText.Trim();
            var message = node["Message"].InnerText.Trim();
            var fiscalYear = node["FiscalYear"].InnerText.Trim();

            //success jika <Message> kosong dan <Confirm> tidak kosong
            if (!(string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(confirm)))
            {
                notes = "Error Process for ID No " + idNo + Environment.NewLine + "Details Info " +
                        Environment.NewLine + message;
                status = (int)ApplicationEnums.SapStatusLog.ErrorProcessOutputXml;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, notes, MethodBase.GetCurrentMethod());
                //update database
                LogSaps.UpdateFromSap(idNo, status, notes, "", null, "WatcherServices");
                //digagalkan semua
                CompEvisSaps.UpdateStatusReconcileByIdNo(idNo, (int)ApplicationEnums.StatusReconcile.Error, "WatcherServices");
                return;
            }
            if (!string.IsNullOrEmpty(fiscalYear))
            {
                int iTest;
                if (!int.TryParse(fiscalYear, out iTest))
                {
                    notes = "Value tidak valid <FiscalYear>";
                    status = (int)ApplicationEnums.SapStatusLog.ErrorProcessOutputXml;
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, notes, MethodBase.GetCurrentMethod());
                    //update database
                    LogSaps.UpdateFromSap(idNo, status, notes, "", null, "WatcherServices");
                    //digagalkan semua
                    CompEvisSaps.UpdateStatusReconcileByIdNo(idNo, (int)ApplicationEnums.StatusReconcile.Error, "WatcherServices");
                    return;
                }
            }

            var accDocNo = node["AccountingDocNo"].InnerText.Trim();
            var isSuccess = true;
            var listItems = new List<OutUploadPPNCreditItem>();
            var msgs = new List<string>();
            int iLineNumber = 1;
            foreach (XmlNode item in childNodes)
            {
                var sFp = item["FP"].InnerText;
                var sNpwp = item["NPWP"].InnerText;
                var sPembetulanKe = item["PembetulanKe"].InnerText;
                var sMasaPajakBulan = item["MasaPajakBulan"].InnerText;
                var sMasaPajakTahun = item["MasaPajakTahun"].InnerText;
                var sAccountingDocDebet = item["AccountingDocDebet"].InnerText;
                var sFiscalYearDebet = item["FiscalYearDebet"].InnerText;
                var sLineItem = item["LineItem"].InnerText;
                var sGLAccount = item["GLAccount"].InnerText;
                var sAmountPPN = item["AmountPPN"].InnerText;
                var sMessage = item["Message"].InnerText;
                listItems.Add(new OutUploadPPNCreditItem()
                {
                    FP = string.IsNullOrEmpty(sFp) ? string.Empty : sFp.Trim(),
                    NPWP = string.IsNullOrEmpty(sNpwp) ? string.Empty : sNpwp.Trim(),
                    PembetulanKe = string.IsNullOrEmpty(sPembetulanKe) ? string.Empty : sPembetulanKe.Trim(),
                    MasaPajakBulan = string.IsNullOrEmpty(sMasaPajakBulan) ? string.Empty : sMasaPajakBulan,
                    MasaPajakTahun = string.IsNullOrEmpty(sMasaPajakTahun) ? string.Empty : sMasaPajakTahun,
                    AccountingDocDebet = string.IsNullOrEmpty(sAccountingDocDebet) ? string.Empty : sAccountingDocDebet,
                    FiscalYearDebet = string.IsNullOrEmpty(sFiscalYearDebet) ? string.Empty : sFiscalYearDebet,
                    LineItem = string.IsNullOrEmpty(sLineItem) ? string.Empty : sLineItem,
                    GLAccount = string.IsNullOrEmpty(sGLAccount) ? string.Empty : sGLAccount,
                    AmountPPN = string.IsNullOrEmpty(sAmountPPN) ? string.Empty : sAmountPPN,
                    Message = string.IsNullOrEmpty(sMessage) ? string.Empty : sMessage
                });

                var msgToLog = new List<string>();

                if (!string.IsNullOrEmpty(sMessage))
                {
                    msgToLog.Add("<Message>" + sMessage + "</Message>");
                }

                //check if exists
                if (!string.IsNullOrEmpty(sFp))
                {
                    if (!string.IsNullOrEmpty(sAccountingDocDebet))
                    {
                        var datsCheck =
                            datsUploaded.FirstOrDefault(
                                c => c.ItemText == sFp && c.AccountingDocNo == sAccountingDocDebet);
                        if (datsCheck == null)
                        {
                            msgToLog.Add("FP : " + sFp + " & Accounting Doc No : " + sAccountingDocDebet +
                                         " tidak ditemukan di ID No : " + idNo);
                        }
                    }
                }
                else
                {
                    msgToLog.Add("FP Kosong");
                }
                
                if (msgToLog.Count > 0)
                {
                    isSuccess = false;
                    msgs.Add("[Item-" + iLineNumber + "]Error : " + string.Join(",", msgToLog));
                }
                iLineNumber++;
                
            }
            int? iFiscalYear = null;
            if (isSuccess)
            {
                //success jika <Message> kosong dan <Confirm> tidak kosong
                if (!(string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(confirm)))
                {
                    isSuccess = false;
                    notes = message;
                }
                if (!string.IsNullOrEmpty(fiscalYear))
                {
                    int iTest;
                    if (int.TryParse(fiscalYear, out iTest))
                    {
                        iFiscalYear = iTest;
                    }
                    else
                    {
                        isSuccess = false;
                        notes = "Value tidak valid <FiscalYear>";
                    }
                }

                //check jika ada data di database evis yang tidak ada di xml
                var qchek = (from db in datsUploaded
                    from xdats in
                        listItems.Where(
                            c =>
                                !string.IsNullOrEmpty(c.FP) &&
                                (c.FP == db.ItemText)
                                && c.AccountingDocDebet == db.AccountingDocNo).DefaultIfEmpty()
                    select new
                    {
                        FpXml = xdats == null ? string.Empty : xdats.FP,
                        FpDb = db == null ? string.Empty : db.ItemText,
                        AccountingDocDb = db == null ? string.Empty : db.AccountingDocNo
                    }).ToList().Where(c => string.IsNullOrEmpty(c.FpXml)).ToList();

                if (qchek.Count > 0)
                {
                    isSuccess = false;
                    notes = "FP : " + (string.Join(",", qchek.Select(c => c.FpDb + "[" + c.AccountingDocDb + "]"))) + " tidak ada dalam XML ID No " +
                            idNo;
                }
            }
            else
            {
                string logKey;
                var errorToLog = "Error Process for ID No " + idNo + Environment.NewLine + "Details Info " +
                                 Environment.NewLine + string.Join(Environment.NewLine, msgs.Select(d => "-" + d));
                Logger.WriteLog(out logKey, LogLevel.Error, errorToLog, MethodBase.GetCurrentMethod());
                notes = "[WatcherService]Error Log Key : " + logKey;
            }

            status = isSuccess ? (int)ApplicationEnums.SapStatusLog.SuccessProcessOutputXml : (int)ApplicationEnums.SapStatusLog.ErrorProcessOutputXml;

            //update database
            LogSaps.UpdateFromSap(idNo, status, notes, accDocNo, iFiscalYear, "WatcherServices");

            if (isSuccess)
            {
                //di-success-kan semua
                CompEvisSaps.UpdateStatusReconcileByIdNo(idNo, (int) ApplicationEnums.StatusReconcile.Success, "WatcherServices");
            }
            else
            {
                //digagalkan semua
                
                CompEvisSaps.UpdateStatusReconcileByIdNo(idNo, (int)ApplicationEnums.StatusReconcile.Error, "WatcherServices");
                
            }

        }

        private void RestartWindowsService(string serviceName, string message)
        {
            ServiceController serviceController = new ServiceController(serviceName);

            if ((serviceController.Status.Equals(ServiceControllerStatus.Running)) || (serviceController.Status.Equals(ServiceControllerStatus.StartPending)))
            {
                ThreadPool.QueueUserWorkItem(o => { throw new Exception(message); });
                EventLog.WriteEntry(serviceName, message, EventLogEntryType.Error);
                serviceController.Dispose();
            }
        }

    }
}
