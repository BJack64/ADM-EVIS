using System;
using System.IO;
using System.Security.Principal;
using System.ComponentModel;
using System.Reflection;
using eFakturADM.Shared.Utility;

namespace WatcherOutboxLibrary
{
    public class Watcher
    {
        private int iterasi;
        public Watcher()
        {


        }

        public void onRunning()
        {


            #region Impersonate
            IntPtr token;

            if (!NativeMethods.LogonUser(
               WatcherOutboxConfiguration.UserNameWatcherOutboxService,
                //ConfigurationManager.AppSettings["DOMAIN"],
                WatcherOutboxConfiguration.ServerAddessWatcherOutboxService,
                WatcherOutboxConfiguration.PasswordWatcherOutboxService,
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
                        int maxSize = WatcherOutboxConfiguration.MaxCopyWatcherOutboxService; 
                        string destinationFolder = WatcherOutboxConfiguration.DestinationFolderWatcherOutboxService;
                        string sourceFolder = WatcherOutboxConfiguration.DataFolderWatcherOutboxService;
                        DirectoryInfo dir = new DirectoryInfo(sourceFolder);
                        FileInfo[] files = dir.GetFiles(); //get file
                        string filename = "";
                        foreach (FileInfo file in files)
                        {
                            count++;
                            if (file.Length > 0 && count < maxSize)
                            {
                                filename = file.Name;
                                File.Copy(file.FullName, destinationFolder + "//" + file.Name, true);
                                //Log.JustLog("Finish copy file with filename :" + file.Name + " to destination folder :" + destinationFolder, MethodBase.GetCurrentMethod());
                                string logKey;
                                Logger.WriteLog(out logKey, LogLevel.Info, "Finish copy file with filename :" + file.Name + " to destination folder :" + destinationFolder, MethodBase.GetCurrentMethod());
                                File.Delete(file.FullName);
                                //Log.JustLog("Finish delete file with filename :" + file.Name, MethodBase.GetCurrentMethod());
                                Logger.WriteLog(out logKey, LogLevel.Info, "Finish delete file with filename :" + file.Name, MethodBase.GetCurrentMethod());
                                iterasi = 0;
                            }
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
