using eFakturADM.FileManager;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;
using eFakturADM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace eFakturADM.Web.Controllers
{
    public class LaporFpController : BaseController
    {
        //
        // GET: /LaporFp/
        //#if TOTAL_DEV

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetListFakturPajakTerlaporDataTable(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength,
            string MasaPajak,  string TahunPajak
            )
        {
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<FakturPajakTerlapor>()
                },
            JsonRequestBehavior.AllowGet);
            }

            var filter = new Logic.Utilities.Filter
            {
                CurrentPage = (iDisplayStart / iDisplayLength) + 1,
                ItemsPerPage = iDisplayLength,
                SortOrderAsc = Request["sSortDir_0"] == "desc",
                SortColumn = Convert.ToInt32(Request["iSortCol_0"]),
                Search = HttpUtility.UrlDecode(Request["sSearch"]),
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 0: filter.SortColumnName = "Nama"; break;
                case 1: filter.SortColumnName = "TanggalLapor"; break;
                case 2: filter.SortColumnName = "MasaPajak"; break;
                case 3: filter.SortColumnName = "TahunPajak"; break;
                case 4: filter.SortColumnName = "AttachmentPath"; break;
                case 5: filter.SortColumnName = "Created"; break;
                case 6: filter.SortColumnName = "CreatedBy"; break;
            }


            int totalItems;


            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Error, "Start Get FP Terlapor", MethodBase.GetCurrentMethod());

            List<FakturPajakTerlapor> listFakturPajak = FakturPajakTerlaporCollections.GetList(filter, out totalItems, MasaPajak, TahunPajak);

            Logger.WriteLog(out logKey, LogLevel.Error, "End Get FP  Terlapor : " + totalItems + " records", MethodBase.GetCurrentMethod());

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listFakturPajak
            },
            JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetLaporFakturPajakDialog()
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"LaporFakturPajakDialog", null),

            }, JsonRequestBehavior.AllowGet);
        }




        public JsonResult ValidationLaporFp(FakturPajakTerlapor Info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            var validationMessage = "";

          
            if (Info.Nama == null || Info.Nama.Trim().Length == 0)
            {
                validationMessage += "Nama harus diisi." + "<br/>";
            }

            if (string.IsNullOrEmpty(Info.TanggalLapor.ToString()))
            {
                validationMessage += "Tanggal Lapor harus diisi." + "<br/>";
            }

            if (string.IsNullOrEmpty(Info.MasaPajak.ToString()))
            {
                validationMessage += "Masa Pajak harus diisi." + "<br/>";
            }

            if (string.IsNullOrEmpty(Info.TahunPajak.ToString()))
            {
                validationMessage += "Tahun Pajak harus diisi." + "<br/>";
            }

            if (string.IsNullOrEmpty(validationMessage)) return Json(new { Html = model }, JsonRequestBehavior.AllowGet);

            model.InfoType = RequestResultInfoType.Warning;
            model.Message = validationMessage;

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Submit(FakturPajakTerlapor data) {
            RequestResultModel _model = new RequestResultModel();
            var usernamelogin = Session["UserName"] as string;
            var msgError = "";
            var outlogKey = "";
            bool saveData = FakturPajakTerlaporCollections.Submit(data, usernamelogin, out msgError, out outlogKey);

            if (saveData)
            {
                _model.Message = String.Format("Faktur Pajak Terlapor '{0}' has been submit.", data.Nama.ToString());
                _model.InfoType = RequestResultInfoType.Success;
            }
            else {
                _model.Message = String.Format("Faktur Pajak Terlapor '{0}' has failed.\n{1}", data.Nama.ToString(),msgError);
                _model.InfoType = RequestResultInfoType.ErrorOrDanger;
            }

            return Json(new { Html = _model }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProcessUpload(string sEcho)
        {
            HttpPostedFileBase file = Request.Files["file-faktur-upload"];
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Upload File Success"
            };

            var userNameLogin = Session["UserName"] as string;
            var uploadResult = UploadFileManager.UploadTempFakturPajakTerlapor(file, userNameLogin);
            if (uploadResult.InfoType == CommonOutputType.Success)
            {
                model.InfoType = RequestResultInfoType.Success;
                model.Message = uploadResult.FilePath;
            }
            else if (uploadResult.InfoType == CommonOutputType.ErrorWithFileDownload)
            {
                model.InfoType = RequestResultInfoType.ErrorWithFileDownload;
                model.Message = uploadResult.MessageInfo;
            }
            else if (uploadResult.InfoType == CommonOutputType.Error)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = uploadResult.MessageInfo;
            }
            
            return Json(new
            {
                Html = model
            }, "text/html");
        }



        [HttpPost]
        public JsonResult ProcessEditUpload(string sEcho,string ID)
        {
            HttpPostedFileBase file = Request.Files["file-faktur-editupload"];
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Upload File Success"
            };
            var msgError = "";
            var outlogKey = "";

            var getData = FakturPajakTerlaporCollections.GetById(Convert.ToInt32(ID));

            var userNameLogin = Session["UserName"] as string;
            var uploadResult = UploadFileManager.UploadFakturPajakTerlapor(file, userNameLogin);
            if (uploadResult.InfoType == CommonOutputType.Success)
            {

                bool saveData = FakturPajakTerlaporCollections.UpdateFile(Convert.ToInt32(ID),  uploadResult.FilePath, userNameLogin, out msgError, out outlogKey);

                if (saveData)
                {
                    model.InfoType = RequestResultInfoType.Success;
                    model.Message = uploadResult.FilePath;
                    
                    //delete file sebelumnya
                    var delete = UploadFileManager.DeleteFileFakturPajakTerlapor(getData.AttachmentPath);
                }
                else
                {
                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = msgError;
                }

            }
            else if (uploadResult.InfoType == CommonOutputType.ErrorWithFileDownload)
            {
                model.InfoType = RequestResultInfoType.ErrorWithFileDownload;
                model.Message = uploadResult.MessageInfo;
            }
            else if (uploadResult.InfoType == CommonOutputType.Error)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = uploadResult.MessageInfo;
            }

            return Json(new
            {
                Html = model
            }, "text/html");
        }


        public JsonResult GetTotalPajakTerlapor(string masaPajak, string tahunPajak)
        {
            if (string.IsNullOrEmpty(masaPajak) || string.IsNullOrEmpty(tahunPajak))
            {
                return Json(new
                {
                    result = new {TotalRow = 0, TotalPPN = 0 }
                },
            JsonRequestBehavior.AllowGet);
            };

            
            decimal totalPPN = FakturPajakTerlaporCollections.GetTotalPPN(
                Convert.ToInt32(masaPajak),
                Convert.ToInt32(tahunPajak));

            int totalRow = FakturPajakTerlaporCollections.GetTotalRecord(
                Convert.ToInt32(masaPajak), 
                Convert.ToInt32(tahunPajak));


            int totalPembetulan = FakturPajakTerlaporCollections.GetPembetulan(
                Convert.ToInt32(masaPajak),
                Convert.ToInt32(tahunPajak));

            var monthData = MasaPajaks.GetAll().Where(a=> a.MonthNumber == Convert.ToInt32(masaPajak));

            string NamaPelaporan = string.Concat(monthData.FirstOrDefault().MonthName, " ", tahunPajak, " Pembetulan ", totalPembetulan.ToString());

            return Json(new
            {
                result = new { TotalRow = totalRow.ToString("N0"), TotalPPN = totalPPN.ToString("N0"), NamaPelaporan = NamaPelaporan }
            },
            JsonRequestBehavior.AllowGet);
        }


    }
}
