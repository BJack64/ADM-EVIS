using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;

namespace eFakturADM.Web.Controllers
{
    public class UploadController : BaseController
    {
        public JsonResult Upload(AttachmentDocumentCategory category)
        {
            string msgerror = "";
            var files = Request.Files;
                var arrayObjects = new List<AttachmentViewModel>();
                var getsettings = ConfigCollection.GetFileShareSetting();
                if (files != null && files.Count > 0)
                {
                    for (var i = 0; i < files.Count; i++)
                    {
                        var file = files[i];

                        var doc = DoUpload(new HttpPostedFileBaseAdapter(file), category, getsettings, out msgerror);

                        arrayObjects.Add(doc);
                    }
                }
                return Json(new { Result = arrayObjects, MessageError = msgerror }, JsonRequestBehavior.AllowGet);
                
        }    

        public AttachmentViewModel DoUpload(IPostedFile file, AttachmentDocumentCategory category, SettingViewModel.FileshareSetting settings, out string msgerror)
        {
            msgerror = "";
            int appsettingmaxsize = settings.MaxUploadSize;
            string outlogkey;

            Logger.WriteLog(out outlogkey, LogLevel.Info, "Start Upload Document", MethodBase.GetCurrentMethod());

            int maxFileSize = appsettingmaxsize * 1024 * 1024;
            var doc = new AttachmentViewModel();
            if (file.ContentLength < maxFileSize)
            {
                var config = settings.FolderUploadTemp;

                var fileName = Path.GetFileName(file.FileName);

                var newFileName = category.ToString() + @"\" + Guid.NewGuid().ToString().Replace('-', '_') + Path.GetExtension(fileName);
                var url = Path.Combine(config, newFileName);
                var dir = Path.GetDirectoryName(url);
                var direxists = true;
                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                }
                catch (Exception e)
                {
                    Logger.WriteLog(out outlogkey, LogLevel.Error, "Error Created Directory. Message " + e.Message, MethodBase.GetCurrentMethod(), e);
                    direxists = false;
                }
                if (direxists)
                {
                    try
                    {
                        file.SaveAs(url);

                        doc = new AttachmentViewModel
                        {
                            FileName = fileName,
                            RelativeUrl = newFileName,
                            FileType = file.ContentType,
                            DownloadLink = Url.EncryptedAction("Temp", "Download", new { FilePath = newFileName, FileName = fileName })
                        };

                        Logger.WriteLog(out outlogkey, LogLevel.Info, "Upload Document Success. ", MethodBase.GetCurrentMethod());

                    }
                    catch (Exception exc)
                    {
                        Logger.WriteLog(out outlogkey, LogLevel.Error, "Error Upload Document. Message " + exc.Message, MethodBase.GetCurrentMethod(), exc);
                    }
                }
            }
            else
            {
                msgerror = string.Format("File size cannot exceed {0} MB", settings.MaxUploadSize);
                Logger.WriteLog(out outlogkey, LogLevel.Info, "Upload Document Failed. Message " + msgerror, MethodBase.GetCurrentMethod());
            }

            return doc;
        }
    }
}