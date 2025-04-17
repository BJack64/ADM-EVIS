using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eFakturADM.Logic.Collections;
using eFakturADM.Web.Models;
using eFakturADM.Logic.Objects;
using System.Transactions;
using System.IO;

namespace eFakturADM.Web.Controllers
{
    public class ErrorsController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger("eFakturADM Logger");
        //
        // GET: /Error/
        [CompressFilter]
        public ActionResult General(Exception exception)
        {
            if (exception.Message.Equals("Object reference not set to an instance of an object.")
                && HttpContext.Session["GGI"] == null
                && HttpContext.Session["EmployeeName"] == null
                )
            {
                String message = "UserHostName: (" + HttpContext.Request.UserHostName + ")" + Environment.NewLine;
                message += "UserHostAddress: (" + HttpContext.Request.UserHostAddress + ")" + Environment.NewLine;
                message += "URL: (" + HttpContext.Request.Url + ")" + Environment.NewLine;
                message += "Session Expired";
                logger.Error(message);
                return View(@"_SessionExpired");
            }
            else 
            {
                String message = "USER: (" + HttpContext.Session["GGI"] + ")" + HttpContext.Session["EmployeeName"] + Environment.NewLine;
                message += "URL: (" + HttpContext.Request.Url + ")" + Environment.NewLine;
                message += exception.Message + Environment.NewLine;
                message += exception.StackTrace + Environment.NewLine;
                logger.Error(message);
                HttpContext.Session["ExceptionObj"] = exception;
                return View(@"_GeneralException500", exception);
            }
        }

        public ActionResult ErrorMessage()
        {
            Exception exception = null;
            if (HttpContext.Session["ExceptionObj"] != null)
            {
                exception = (Exception)HttpContext.Session["ExceptionObj"];
                return View(@"_GeneralException500", exception);
            }
            else
                return View(@"_SessionExpired");
        }

        public ActionResult Http401()
        {
            return View("AccessDenied");
        }

        public ActionResult Http404()
        {
            return View("_PageNotFound404");
        }

        public ActionResult Http403()
        {
            return View("_Forbidden403");
        }

        public ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        [AuthActivity("View Error Logs")]
        public ActionResult ErrorLog()
        {
            return View("ErrorLog");
        }

        public ActionResult DownloadFile(string file)
        {
            string filePath = "";
            byte[] fileBytes = null;
            try
            {
                log4net.Appender.RollingFileAppender appender = (log4net.Appender.RollingFileAppender)log4net.LogManager.GetCurrentLoggers()[0].Logger.Repository.GetAppenders()[0];
                String FolderPath = System.IO.Path.GetDirectoryName(appender.File);
                filePath = FolderPath + "\\" + file;
                fileBytes = System.IO.File.ReadAllBytes(filePath);
            }
            catch (Exception)
            {

            }
            if (fileBytes == null)
            {
                using (System.IO.FileStream fs = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                {
                    int numBytesToRead = Convert.ToInt32(fs.Length);
                    fileBytes = new byte[(numBytesToRead)];
                    fs.Read(fileBytes, 0, numBytesToRead);
                }
            }
            string contentType = "application/log";
            return File(fileBytes, contentType, file);
        }
    }
}
