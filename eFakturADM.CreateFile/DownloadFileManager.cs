using System;
using System.IO;
using System.Net;
using System.Reflection;
using eFakturADM.Logic.Core;
using eFakturADM.Shared.Utility;

namespace eFakturADM.FileManager
{
    public class DownloadFileManager
    {
        public static FileInfo GetFileInfo(string type, string fileFolder, string fileName)
        {

            try
            {
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var strRootPath = "";
                    var path = "";
                    if (type == EnumHelper.GetDescription(ApplicationEnums.DownloadFolderType.Export))
                    {
                        strRootPath = "Export";
                        path = string.Format(@"{0}\{1}\{2}\{3}", FileManagerConfiguration.RepoRootPath, strRootPath, fileFolder, fileName);
                    }
                    else if (type == EnumHelper.GetDescription(ApplicationEnums.DownloadFolderType.Upload))
                    {
                        strRootPath = "Upload";
                        path = string.Format(@"{0}\{1}\{2}\{3}", FileManagerConfiguration.RepoRootPath, strRootPath, fileFolder, fileName);
                    }
                    else if (type == EnumHelper.GetDescription(ApplicationEnums.DownloadFolderType.Sp2))
                    {
                        strRootPath = "SP2";
                        path = string.Format(@"{0}\{1}\{2}", FileManagerConfiguration.RepoRootPath, strRootPath, fileName);
                    }

                    var newFile = new FileInfo(path);
                    return newFile;
                }
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return null;

        }
    }
}
