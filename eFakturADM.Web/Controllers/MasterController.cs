using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using eFakturADM.FileManager;
using eFakturADM.Logic.Core;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Collections;
using eFakturADM.Web.Models;

namespace eFakturADM.Web.Controllers
{
    public class MasterController : BaseController
    {
        #region Propoerties
        #region Vendor
        public class VendorInfo
        {
            public int VendorId { get; set; }
            
            public string JenisNPWP { get; set; }
            public string NPWP { get; set; }
            public string Nama { get; set; }
            public string Alamat { get; set; }
            public string PkpDicabut { get; set; }
            public string TglPkpDicabut { get; set; }
            public string KeteranganTambahan { get; set; }
            public string FormatedNpwp { get; set; }
        }
        public class VendorInfoUpload : VendorInfo
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
        }
        #endregion
        #endregion
   

        #region Vendor
        [AuthActivity("47")]
        public ActionResult Vendor()
        {
            return View("Vendor");
        }

        public JsonResult ListVendor()
        {
            List<Vendor> Vendor = Vendors.Get();

            return Json(new
            {
                aaData = Vendor
            },
            JsonRequestBehavior.AllowGet);
        }

        [AuthActivity("48")]
        public JsonResult GetAddVendorDialog()
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"AddVendor", null),

            }, JsonRequestBehavior.AllowGet);
        }

        [AuthActivity("50")]
        public JsonResult GetEditVendorDialog(int VendorId)
        {
            Vendor vendor = Vendors.GetById(VendorId);
            var model = new VendorInfo()
            {
                VendorId = vendor.VendorId,
                JenisNPWP = vendor.JenisNPWP,
                NPWP = vendor.NPWP,
                FormatedNpwp = vendor.FormatedNpwp,
                Nama = vendor.Nama,
                Alamat = vendor.Alamat,
                PkpDicabut = vendor.PkpDicabut ? EnumHelper.GetDescription(ApplicationEnums.VendorPkpDicabut.Ya) : EnumHelper.GetDescription(ApplicationEnums.VendorPkpDicabut.Tidak),
                TglPkpDicabut = vendor.TglPkpDicabutString,
                KeteranganTambahan = vendor.KeteranganTambahan
            };

            return Json(new
            {
                Html = this.RenderPartialView(@"EditVendor", model),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BrowseVendorDialog(string AccessFrom)
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"BrowseVendor", AccessFrom),

            }, JsonRequestBehavior.AllowGet);
        }

        [AuthActivity("51")]
        public JsonResult RemoveVendor(int VendorId)
        {
            RequestResultModel _model = new RequestResultModel();

            using (TransactionScope scope = new TransactionScope())
            {
                if (Vendors.Delete(VendorId, HttpContext.Session["UserName"].ToString()))
                {
                    _model.Message = "Vendor has been deleted.";
                    _model.InfoType = RequestResultInfoType.Success;
                }
                scope.Complete();
                scope.Dispose();
            }

            return Json(new { Html = _model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidationVendor(VendorInfo Info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            var validationMessage = "";

            if (Info.NPWP == null || Info.NPWP.Trim().Length == 0)
            {
                validationMessage += "NPWP harus diisi." + "<br/>";
            }
            else
            {
                int limitNpwp = 16;
                if(EnumHelper.GetDescription(ApplicationEnums.VendorJenisNPWP.BadanUsahaCabang) == Info.JenisNPWP)
                {
                    limitNpwp = 22;
                }
                //check panjang karakter dan harus angka
                Regex regex = new Regex(@"^[0-9]{"+ limitNpwp +"}$");
                var originalNpwp = Info.NPWP;
                var match = regex.Match(originalNpwp);

                if (!match.Success)
                {
                    validationMessage += "NPWP harus "+ limitNpwp + " karakter dan berupa angka" + "<br />";
                }
                else
                {
                    if (Info.VendorId <= 0)
                    {
                        var chkIsExists = Vendors.GetByFormatedNpwp(Info.NPWP);
                        if (chkIsExists != null)
                        {
                            validationMessage += "NPWP Vendor sudah ada" + "<br/>";
                        }
                    }

                }

            }
            if (Info.Nama == null || Info.Nama.Trim().Length == 0)
            {
                validationMessage += "Nama harus diisi." + "<br/>";
            }

            if (string.IsNullOrEmpty(Info.PkpDicabut))
            {
                validationMessage += "PKP Dicabut harus diisi." + "<br/>";
            }
            else
            {
                if (EnumHelper.GetDescription(ApplicationEnums.VendorPkpDicabut.Tidak) == Info.PkpDicabut)
                {
                    if (!string.IsNullOrEmpty(Info.TglPkpDicabut))
                    {
                        validationMessage += "Tanggal Dicabut tidak boleh diisi." + "<br/>";
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Info.TglPkpDicabut))
                    {
                        validationMessage += "Tanggal Dicabut harus diisi." + "<br/>";
                    }
                }
            }

            if (Info.VendorId > 0)
            {
                Vendor vendor = Vendors.GetById(Info.VendorId);

                if (vendor.IsDeleted == true)
                {
                    validationMessage += String.Format("Data sudah dihapus oleh '{0}'.", vendor.ModifiedBy);

                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = validationMessage;

                    return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                }
            }

            if (string.IsNullOrEmpty(validationMessage)) return Json(new { Html = model }, JsonRequestBehavior.AllowGet);

            model.InfoType = RequestResultInfoType.Warning;
            model.Message = validationMessage;

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveVendor(VendorInfo Info)
        {
            RequestResultModel _model = new RequestResultModel();
            var usernamelogin = Session["UserName"] as string;
            using (TransactionScope scope = new TransactionScope())
            {
                Vendor vendor = new Vendor();
                vendor.VendorId = Info.VendorId;
                vendor.JenisNPWP = Info.JenisNPWP;
                vendor.NPWP = Info.NPWP;
                vendor.FormatedNpwp = Info.NPWP;
                vendor.Nama = Info.Nama;
                vendor.Alamat = Info.Alamat;
                vendor.CreatedBy = usernamelogin;
                vendor.ModifiedBy = usernamelogin;
                vendor.PkpDicabut = Info.PkpDicabut != EnumHelper.GetDescription(ApplicationEnums.VendorPkpDicabut.Tidak);
                vendor.TglPkpDicabut = null;
                if (!string.IsNullOrEmpty(Info.TglPkpDicabut))
                {
                    vendor.TglPkpDicabut = Convert.ToDateTime(Info.TglPkpDicabut);
                }
                vendor.KeteranganTambahan = Info.KeteranganTambahan;

                Vendors.Save(vendor);

                scope.Complete();
                scope.Dispose();

                if (Info.VendorId > 0)
                {
                    _model.Message = String.Format("Vendor '{0}' has been updated.", vendor.Nama.ToString());
                    _model.InfoType = RequestResultInfoType.Success;
                }
                else
                {
                    _model.Message = String.Format("Vendor '{0}' has been created.", vendor.Nama.ToString());
                    _model.InfoType = RequestResultInfoType.Success;
                }
            }
            return Json(new { Html = _model }, JsonRequestBehavior.AllowGet);
        }

        [AuthActivity("49")]
        public JsonResult UploadVendor(string accessForm)
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"UploadVendor", accessForm)
            }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /FileUpload/        
        [HttpPost]
        public JsonResult ProcessUploadVendor(string sEcho)
        {
            HttpPostedFileBase file = Request.Files["file-vendor-upload"];
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Upload Vendor Success"
            };

            var userNameLogin = Session["UserName"] as string;
            var uploadResult = UploadFileManager.UploadVendor(file, userNameLogin);

            if (uploadResult.InfoType == CommonOutputType.ErrorWithFileDownload)
            {
                model.InfoType = RequestResultInfoType.ErrorWithFileDownload;
                model.Message = "<a href='" +
                                        Url.Action("DownloadFile", "ExportDownload",
                                            new
                                            {
                                                type = EnumHelper.GetDescription(ApplicationEnums.DownloadFolderType.Upload),
                                                fileFolder = EnumHelper.GetDescription(ApplicationEnums.DownloadFileType.Vendor),
                                                fileName = uploadResult.FilePath
                                            }) +
                                        "' target='_blank'>Error Validasi</a>";
            }
            else if (uploadResult.InfoType == CommonOutputType.Error)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = uploadResult.MessageInfo;
            }

            //return Json(new
            //{
            //    Html = model
            //}, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                Html = model
            }, "text/html");
        }
        
        #endregion

        #region -------------- Browse User -----------

        public JsonResult BrowseUserDialog(string AccessFrom)
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"BrowseUser", AccessFrom),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListUser()
        {
            var userDats = Users.Get();
            return Json(new
            {
                aaData = userDats
            },
            JsonRequestBehavior.AllowGet);
        }

        public JsonResult FecthListUser(string query)
        {
            var userDats = Users.Get();
            if (!string.IsNullOrEmpty(query) && userDats.Count > 0)
            {
                userDats = userDats.Where(c => c.UserName.ToLower().Contains(query)).ToList();
            }

            return Json(userDats, JsonRequestBehavior.AllowGet);

        }

        #endregion

        public JsonResult GetOpenTahunPajakByMasaPajak(string masaPajak)
        {
            if (string.IsNullOrEmpty(masaPajak))
            {
                return Json(new
                {
                    aaData = new List<TahunPajak>()
                },
            JsonRequestBehavior.AllowGet);
            }
            var dbData = TahunPajaks.GetOpenRegularByMasaPajak(Convert.ToInt32(masaPajak));
            return Json(new
            {
                aaData = dbData
            },
            JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCloseTahunPajakByMasaPajak(string masaPajak)
        {
            if (string.IsNullOrEmpty(masaPajak))
            {
                return Json(new
                {
                    aaData = new List<TahunPajak>()
                },
            JsonRequestBehavior.AllowGet);
            }
            var dbData = TahunPajaks.GetCloseRegularByMasaPajak(Convert.ToInt32(masaPajak));
            return Json(new
            {
                aaData = dbData
            },
            JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTahunPajakByMasaPajak(string masaPajak)
        {
            if (string.IsNullOrEmpty(masaPajak))
            {
                return Json(new
                {
                    aaData = new List<TahunPajak>()
                },
            JsonRequestBehavior.AllowGet);
            }
            var dbData = TahunPajaks.GetByMasaPajak(Convert.ToInt32(masaPajak));
            return Json(new
            {
                aaData = dbData
            },
            JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJenisDokumenByJenisTransaksi(string jenisTransaksi)
        {
            if (string.IsNullOrEmpty(jenisTransaksi))
            {
                return Json(new
                {
                    aaData = new List<FPJenisDokumen>()
                },
            JsonRequestBehavior.AllowGet);
            }
            var dbData =
                FPJenisDokumens.GetByFCodeAndJnsTransaksi(
                    EnumHelper.GetDescription(ApplicationEnums.FCodeFpKhusus.Dm), jenisTransaksi);
            return Json(new
            {
                aaData = dbData
            },
            JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKdJenisTransaksiByJenisTransaksi(string jenisTransaksi)
        {
            if (string.IsNullOrEmpty(jenisTransaksi))
            {
                return Json(new
                {
                    aaData = new List<FPKdJenisTransaksi>()
                },
            JsonRequestBehavior.AllowGet);
            }
            var dbData =
                FPKdJenisTransaksis.GetByFCodeAndJnsTransaksi(
                    EnumHelper.GetDescription(ApplicationEnums.FCodeFpKhusus.Dm), jenisTransaksi);
            return Json(new
            {
                aaData = dbData
            },
            JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJenisTransaksi()
        {
            var dbData =
                FPJenisTransaksis.GetByFCode(EnumHelper.GetDescription(ApplicationEnums.FCodeFpKhusus.Dm));
            return Json(new
            {
                aaData = dbData
            },
            JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPembetulanSpmList(string masaPajak, string tahunPajak)
        {
            var datList = new List<SelectListItem>();
            if (string.IsNullOrEmpty(masaPajak))
            {
                return Json(new
                {
                    aaData = datList
                },
            JsonRequestBehavior.AllowGet);
            }

            int iMasaPajak;
            if (!int.TryParse(masaPajak, out iMasaPajak))
            {
                return Json(new
                {
                    aaData = datList
                },
            JsonRequestBehavior.AllowGet);
            }

            int iTahunPajak;
            if (!int.TryParse(tahunPajak, out iTahunPajak))
            {
                return Json(new
                {
                    aaData = datList
                },
            JsonRequestBehavior.AllowGet);
            }

            var spmData = ReportSuratPemberitahuanMasas.GetByMasaPajak(iMasaPajak, iTahunPajak);
            
            datList.AddRange(spmData.Select(item => new SelectListItem()
            {
                Value = item.Versi.ToString(),
                Text = item.Versi.ToString()
            }));
            
            return Json(new
            {
                aaData = datList
            },
            JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMasaPajak(bool isOpen)
        {
            var monthData = isOpen ? MasaPajaks.GetOpenRegular() : MasaPajaks.GetCloseRegular();
            var monthList = new List<SelectListItem>();
            var dtNow = DateTime.Now;
            monthList.AddRange(monthData.Select(item => new SelectListItem()
            {
                Value = item.MonthNumber.ToString(),
                Text = item.MonthName,
                Selected = item.MonthNumber == dtNow.Month
            }));
            return Json(new
            {
                aaData = monthList
            },
            JsonRequestBehavior.AllowGet);
        }

    }
}

