using System;
using System.Reflection;
using System.IO;
using System.Security.Principal;
using System.ComponentModel;
using eFakturADM.Shared.Utility;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;

namespace WatcherLibraryMoveFile
{
    public class WatcherMove
    {
        private int iterasi;
        public void onRunning()
        {


            #region Impersonate
            IntPtr token;

            if (!NativeMethods.LogonUser(
                WatcherMoveConfiguration.UserName,
                WatcherMoveConfiguration.ServerAddress,
                WatcherMoveConfiguration.Password,
                NativeMethods.LogonType.NewCredentials,
                NativeMethods.LogonProvider.Default,
                out token))
            {
                throw new Win32Exception();
            }

            try
            {
                IntPtr tokenDuplicate;

                if (!NativeMethods.DuplicateToken(
                    token,
                    NativeMethods.SecurityImpersonationLevel.Impersonation,
                    out tokenDuplicate))
                {
                    throw new Win32Exception();
                }

                try
                {
                    using (WindowsImpersonationContext impersonationContext =
                        new WindowsIdentity(tokenDuplicate).Impersonate())
                    {
                        #region copyFile
                        int count = 0;
                        int maxSize = WatcherMoveConfiguration.MaxCopy; ;
                        string destinationFolder = WatcherMoveConfiguration.DataFolder2;
                        string sourceFolder = WatcherMoveConfiguration.DataFolder;
                        DirectoryInfo dir = new DirectoryInfo(sourceFolder);
                        FileInfo[] files = dir.GetFiles(); //get file
                        string filename = "";
                        foreach (FileInfo file in files)
                        {
                            filename = file.Name;
                            //Update Log SAP
                            var getDataLog = LogSaps.GetByFileName(filename);
                            if (getDataLog == null)
                            {
                                continue;//tidak terdaftar secara data log sap di database
                            }
                            if (file.Length > 0 && count < maxSize)
                            {
                                bool isSuccessTransfer = false;
                                var destFile = string.Format(@"{0}\{1}", destinationFolder, filename);
                                var msgTransferFile = "";
                                try
                                {
                                    File.Copy(file.FullName, destFile, true);
                                    isSuccessTransfer = true;
                                }
                                catch (Exception exceptionCopy)
                                {
                                    string logKey;
                                    Logger.WriteLog(out logKey, LogLevel.Error, exceptionCopy.Message, MethodBase.GetCurrentMethod(), exceptionCopy);
                                    msgTransferFile = "Failed Transfer " + filename + " to " + destinationFolder;
                                }
                                //backup to BackupOutbox
                                var backupOutBoxFolder = string.Format(@"{0}",
                                    WatcherMoveConfiguration.BackupFolderWatcherInboxService);

                                if (!Directory.Exists(backupOutBoxFolder))
                                {
                                    Directory.CreateDirectory(backupOutBoxFolder);
                                }
                                var backupFile = string.Format(@"{0}\{1}", backupOutBoxFolder, filename);
                                File.Copy(file.FullName, backupFile, true);

                                //delete
                                File.Delete(file.FullName);

                                //Update LogSap
                                getDataLog.LocalExecution = DateTime.Now;
                                getDataLog.Status = isSuccessTransfer
                                    ? (int)ApplicationEnums.SapStatusLog.Success
                                    : (int)ApplicationEnums.SapStatusLog.Error;
                                getDataLog.ModifiedBy = "WatcherOutboxService";
                                getDataLog.TransferDate = isSuccessTransfer ? DateTime.Now : (DateTime?)null;
                                getDataLog.Note = msgTransferFile;
                                getDataLog.SapPath = isSuccessTransfer ? destFile : null;
                                getDataLog.LocalPath = backupFile;

                                LogSaps.Save(getDataLog);
                            }

                            System.Threading.Thread.Sleep(WatcherMoveConfiguration.TimeSleep);

                        }
                        #endregion
                        impersonationContext.Undo();
                    }
                }
                catch (Exception e)
                {
                    //  ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + er.Message + "');", true);
                    ++iterasi;
                    if (iterasi <= 3)
                    {
                        string logKey;
                        //Log.WriteLog("Error impersonate with iterasi " + iterasi + " and message :" + e.Message, MethodBase.GetCurrentMethod());
                        Logger.WriteLog(out logKey, LogLevel.Error, "Error impersonate with iterasi " + iterasi + " and message :" + e.Message, MethodBase.GetCurrentMethod(), e);
                    }
                    else
                    {
                        throw e;
                    }

                }
                finally
                {
                    if (tokenDuplicate != IntPtr.Zero)
                    {
                        if (!NativeMethods.CloseHandle(tokenDuplicate))
                        {

                            // Uncomment if you need to know this case.
                            throw new Win32Exception();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + er.Message + "');", true);
                ++iterasi;
                if (iterasi <= 3)
                {
                    //Log.WriteLog("Error impersonate with iterasi " + iterasi + " and message :" + ex.Message, MethodBase.GetCurrentMethod());
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, "Error impersonate with iterasi " + iterasi + " and message :" + ex.Message, MethodBase.GetCurrentMethod(), ex);
                }
                else
                {
                    throw ex;
                }

            }
            finally
            {
                if (token != IntPtr.Zero)
                {
                    if (!NativeMethods.CloseHandle(token))
                    {
                        // Uncomment if you need to know this case.
                        throw new Win32Exception();
                    }
                }
            }
            #endregion
        }
    }
}
