using eFakturADM.Logic.Collections;
using eFakturADM.Shared;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Controllers;
using eFakturADM.Web.Helpers;
using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Web.Mvc;

namespace eFakturADM.Web.Controllers
{
    public class DownloadController : BaseController
    {
        public void Document(long id, AttachmentDocumentCategory category)
        {
            string outlogkey;
            try
            {
                var settings = ConfigCollection.GetFileShareSetting();
                var uploadFolder = settings.FolderUpload;

                var relativeUrl = "";
                if (category == AttachmentDocumentCategory.FakturPajakTerlapor)
                {
                    var getData = FakturPajakTerlaporCollections.GetById(id);
                    relativeUrl = getData.AttachmentPath;
                }
                
                var url = Path.Combine(uploadFolder, relativeUrl);
                

                Response.Clear();
                Response.AddHeader("Access-Control-Allow-Origin", "*");
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("content-disposition", "attachment;filename=" + Path.GetFileName(url));
                Response.WriteFile(url);
                Response.Flush();
                Response.End();
            }
            catch (Exception exc)
            {

                Logger.WriteLog(out outlogkey, LogLevel.Error, "Download Document Failed. Message " + exc.Message, MethodBase.GetCurrentMethod(), exc);
                throw;
            }

        }

        [EncryptedParameter]
        public void Temp(string FilePath, string FileName)
        {
            string outlogkey;

            try
            {
                var settings = ConfigCollection.GetFileShareSetting();
                var uploadFolder = settings.FolderUploadTemp;

                var url = Path.Combine(uploadFolder, FilePath);
                Response.Clear();
                Response.AddHeader("Access-Control-Allow-Origin", "*");
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("content-disposition", "attachment;filename=" + Path.GetFileName(FileName));
                Response.WriteFile(url);
                Response.Flush();
                Response.End();
            }
            catch (Exception exc)
            {
                Logger.WriteLog(out outlogkey, LogLevel.Error, "Download Temp Failed. Message " + exc.Message, MethodBase.GetCurrentMethod(), exc);
                throw;
            }
        }


        public ActionResult ViewFile(long id, AttachmentDocumentCategory category)
        {
            string outlogkey;
            try
            {
                if (id != 0)
                {

                    var settings = ConfigCollection.GetFileShareSetting();
                    var uploadFolder = settings.FolderUpload;

                    var relativeUrl = "";
                    var fileType = "";
                    if (category == AttachmentDocumentCategory.FakturPajakTerlapor)
                    {
                        var getData = FakturPajakTerlaporCollections.GetById(id);
                        relativeUrl = getData.AttachmentPath;
                        fileType = getData.FileType;

                    }
                    var url = Path.Combine(uploadFolder, relativeUrl);

                    /*
                     application/pdf
                     image/jpeg
                     */
                    byte[] FileBytes = System.IO.File.ReadAllBytes(url);
                    return File(FileBytes, fileType);

                }
                else
                {
                    return new HttpNotFoundResult();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(out outlogkey, LogLevel.Error, "View file Failed. Message " + ex.Message, MethodBase.GetCurrentMethod(), ex);
                return new HttpNotFoundResult("Error " + outlogkey);

            }
        }

        [EncryptedParameter]
        public ActionResult ViewFileTemp(string FilePath, string FileName)
        {
            string outlogkey;
            try
            {
                var settings = ConfigCollection.GetFileShareSetting();
                var uploadFolder = settings.FolderUploadTemp;

                var url = Path.Combine(uploadFolder, FilePath);

                FileInfo fi = new FileInfo(url);
                byte[] FileBytes = System.IO.File.ReadAllBytes(url);
                return File(FileBytes, fi.Extension);

            }
            catch (Exception ex)
            {
                Logger.WriteLog(out outlogkey, LogLevel.Error, "View file Failed. Message " + ex.Message, MethodBase.GetCurrentMethod(), ex);
                return new HttpNotFoundResult("Error " + outlogkey);

            }
        }

    }
}