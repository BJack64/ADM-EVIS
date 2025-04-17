using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using System.Transactions;
using eFakturADM.FileManager;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Collections;
using eFakturADM.Web.Models;
using System.Text.RegularExpressions;
using System.Web;
using eFakturADM.Logic.Core;
using System.IO;

namespace eFakturADM.Web.Controllers
{
    public class SettingController : BaseController
    {
        #region Propoerties
        #region Role Management
        public class RoleManagementInfo
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public ActivityInfo[] Activity { get; set; }
            public ActivityDeleteInfo[] Delete { get; set; }
        }

        public class ActivityInfo
        {
            public int ActivityId { get; set; }
            public string ActivityName { get; set; }
        }

        public class ActivityDeleteInfo
        {
            public int ActivityId { get; set; }
        }


        #endregion

        #region User Management
        public class UserManagementInfo
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string RetypePassword { get; set; }
            public string Email { get; set; }
            public string UserInitial { get; set; }
            public UserRoleInfo[] Role { get; set; }
            public UserRoleDeleteInfo[] DeleteRole { get; set; }
        }

        public class UserRoleInfo
        {
            public int UserRoleId { get; set; }
            public int RoleId { get; set; }
        }

        public class UserRoleDeleteInfo
        {
            public int RoleId { get; set; }
        }
        #endregion

        #region File Upload
        public class FileUploadModel
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public bool UploadStatus { get; set; }
            public string Messages { get; set; }
        }

        public class SaveFileResult
        {
            public bool isSave { get; set; }
            public string UploadFileId { get; set; }
            public string Message { get; set; }

        }
        #endregion
        #endregion

        #region Role Management
        [AuthActivity("38")]
        public ActionResult RoleManagement()
        {
            return View("RoleManagement");
        }

        public JsonResult ListRoleManagement()
        {
            List<Role> roleManagement = Roles.Get();

            return Json(new
            {
                aaData = roleManagement
            },
            JsonRequestBehavior.AllowGet);
        }


        [AuthActivity("39")]
        public JsonResult GetAddRoleManagementDialog()
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"AddRoleManagement", null),

            }, JsonRequestBehavior.AllowGet);
        }

        [AuthActivity("40")]
        public JsonResult GetEditRoleManagementDialog(int RoleId)
        {
            Role role = Roles.GetById(RoleId);

            return Json(new
            {
                Html = this.RenderPartialView(@"EditRoleManagement", role),

            }, JsonRequestBehavior.AllowGet);
        }


        [AuthActivity("41")]
        public JsonResult RemoveRoleManagement(int RoleId)
        {
            RequestResultModel _model = new RequestResultModel();

            using (TransactionScope scope = new TransactionScope())
            {
                Role role = Roles.GetById(RoleId);

                List<RoleActivity> roleActivity = RoleActivities.GetByRoleId(RoleId);

                if (roleActivity.Count > 0)
                {
                    roleActivity[0].ModifiedBy = "System";
                    roleActivity[0].DeleteByRoleId();
                }

                role.ModifiedBy = "System";
                if (role.Delete())
                {
                    _model.Message = "Role has been deleted.";
                    _model.InfoType = RequestResultInfoType.Success;
                }
                scope.Complete();
                scope.Dispose();
            }

            return Json(new { Html = _model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidationRoleManagement(RoleManagementInfo Info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            var validationMessage = "";

            if (Info.RoleName == null || Info.RoleName.Trim().Length == 0)
            {
                validationMessage += "Role Name is empty. Please, enter role name." + "<br/>";
            }
            else
            {
                List<Role> roleExists = Roles.GetByName(Info.RoleName).ToList();
                bool valid = roleExists.Where(x => x.RoleId != Info.RoleId).Count() > 0 ? false : true;
                if (!valid)
                {
                    validationMessage += String.Format("'{0}' already exists. Please, change role name and try again.", roleExists.FirstOrDefault().Name.ToString());
                }
            }
            if (Info.RoleId > 0)
            {
                Role role = Roles.GetById(Info.RoleId);

                if (role.IsDeleted == true)
                {
                    validationMessage += String.Format("Data has been deleted by '{0}'.", role.ModifiedBy);

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

        public JsonResult SaveRoleManagement(RoleManagementInfo Info)
        {
            RequestResultModel _model = new RequestResultModel();

            using (TransactionScope scope = new TransactionScope())
            {
                Role role = new Role();
                role.RoleId = Info.RoleId;
                role.Name = Info.RoleName;
                role.CreatedBy = "System";
                role.ModifiedBy = "System";

                role.Save();

                //save RoleActivity
                SaveRoleActivity(Info, role.RoleId);

                _model.Message = String.Format("Role '{0}' has been created.", role.Name.ToString());
                _model.InfoType = RequestResultInfoType.Success;

                scope.Complete();
                scope.Dispose();
            }
            return Json(new { Html = _model }, JsonRequestBehavior.AllowGet);
        }

        protected void SaveRoleActivity(RoleManagementInfo Info, int _roleID)
        {
            if (Info.Delete != null)
            {
                List<RoleActivity> listActivity = RoleActivities.GetByRoleId(Info.RoleId);
                for (int i = 0; i < listActivity.Count(); i++)
                {
                    for (int j = 0; j < Info.Delete.Count(); j++)
                    {
                        if (listActivity[i].ActivityId == Info.Delete[j].ActivityId)
                        {
                            RemoveRoleActivity(listActivity[i].RoleActivityId);
                        }
                    }
                }
            }

            if (Info.Activity != null)
            {
                foreach (ActivityInfo _activity in Info.Activity)
                {
                    RoleActivity roleActivity = RoleActivities.GetByActivityId_RoleId(_activity.ActivityId, _roleID);

                    roleActivity.RoleId = _roleID;
                    roleActivity.ActivityId = _activity.ActivityId;
                    roleActivity.CreatedBy = "System";
                    roleActivity.ModifiedBy = "System";
                    roleActivity.Save();

                }
            }
        }

        private void RemoveRoleActivity(int RoleActivityID)
        {
            RoleActivity roleActivity = RoleActivities.GetById(RoleActivityID);

            if (roleActivity != null)
            {
                roleActivity.ModifiedBy = "System";
                roleActivity.Delete();
            }
        }
        #endregion

        #region UserManagement

        [AuthActivity("42")]
        public ActionResult UserManagement()
        {
            return View("UserManagement");
        }

        public JsonResult ListUserManagement()
        {
            List<User> userManagement = Users.Get();

            return Json(new
            {
                aaData = userManagement
            },
            JsonRequestBehavior.AllowGet);
        }


        [AuthActivity("43")]
        public JsonResult GetAddUserManagementDialog()
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"AddUserManagement", null),

            }, JsonRequestBehavior.AllowGet);
        }


        [AuthActivity("44")]
        public JsonResult GetEditUserManagementDialog(int UserId)
        {
            User _user = Users.GetById(UserId);
            return Json(new
            {
                Html = this.RenderPartialView(@"EditUserManagement", _user),

            }, JsonRequestBehavior.AllowGet);
        }


        [AuthActivity("45")]
        public JsonResult RemoveUserManagement(int UserId)
        {
            RequestResultModel _model = new RequestResultModel();

            using (TransactionScope scope = new TransactionScope())
            {
                User user = Users.GetById(UserId);

                List<UserRole> userRole = UserRoles.GetByUserId(UserId);
                if (userRole.Count > 0)
                {
                    userRole[0].ModifiedBy = HttpContext.Session["UserName"].ToString();
                    userRole[0].DeleteByUserId();
                }

                user.ModifiedBy = HttpContext.Session["UserName"].ToString();
                if (user.Delete())
                {
                    _model.Message = "User has been deleted.";
                    _model.InfoType = RequestResultInfoType.Success;
                }
                scope.Complete();
                scope.Dispose();
            }

            return Json(new { Html = _model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidationUserManagement(UserManagementInfo Info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            var validationMessage = "";

            if (Info.UserId > 0)
            {
                User user = Users.GetById(Info.UserId);

                if (user.IsDeleted == true)
                {
                    validationMessage += String.Format("Data has been deleted by '{0}'.", user.ModifiedBy);

                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = validationMessage;

                    return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (Info.UserName == null || Info.UserName.Trim().Length == 0)
                {
                    validationMessage += "UserName is required, " + "<br/>";
                }
                else
                {
                    var userExists = Users.GetByUserName(Info.UserName);
                    if (userExists != null)
                    {
                        validationMessage += String.Format("User '{0}' already exists. Please change UserName and try again, ", Info.UserName);
                    }
                }

                if (string.IsNullOrEmpty(Info.UserInitial))
                {
                    validationMessage += "Initial is required, " + "<br/>";
                }

                if (string.IsNullOrEmpty(Info.Password))
                {
                    validationMessage += "Password required,  " + "<br/>";
                }
                else
                {
                    //Regex regex = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[!#$%&'()*+,-.:;<=>?@[\\\]{}^_`|~]).{8,}$");
                    Regex regex = new Regex(@"^[a-zA-Z0-9]{8,}$");
                    Match match = regex.Match(Info.Password);

                    if (!match.Success)
                    {
                        validationMessage += "Password must contains 8 characters, aplhanumeric combination.";
                    }
                }

                if (string.IsNullOrEmpty(Info.RetypePassword))
                {
                    validationMessage += "Re-Type Password required.";
                }
                else
                {
                    if (!string.IsNullOrEmpty(Info.Password) && !string.IsNullOrEmpty(Info.RetypePassword))
                    {
                        if (Info.Password != Info.RetypePassword)
                        {
                            validationMessage += "Re-Type Password is not match.";
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(validationMessage)) return Json(new { Html = model }, JsonRequestBehavior.AllowGet);

            model.InfoType = RequestResultInfoType.Warning;
            model.Message = validationMessage;

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveUserManagement(UserManagementInfo Info)
        {
            var _model = new RequestResultModel();

            using (TransactionScope scope = new TransactionScope())
            {
                User user = new User();
                user.UserId = Info.UserId;
                user.UserName = Info.UserName;
                user.Password = Info.Password;
                user.Email = Info.Email;
                user.UserInitial = Info.UserInitial;
                user.CreatedBy = HttpContext.Session["UserName"].ToString();
                user.ModifiedBy = HttpContext.Session["UserName"].ToString();

                user.Save();

                //save UserRole
                SaveUserRole(Info, user.UserId);
                if (Info.UserId == -1)
                {
                    _model.Message = String.Format("User '{0}' has been inserted.", Info.UserName.ToString());
                }
                else
                {
                    _model.Message = String.Format("User '{0}' has been updated.", Info.UserName.ToString());
                }

                _model.InfoType = RequestResultInfoType.Success;

                scope.Complete();
                scope.Dispose();
            }
            return Json(new { Html = _model }, JsonRequestBehavior.AllowGet);
        }

        protected void SaveUserRole(UserManagementInfo Info, int _userId)
        {
            List<UserRole> listUserRole = UserRoles.GetByUserId(Info.UserId);
            if (Info.DeleteRole != null)
            {
                for (int i = 0; i < listUserRole.Count(); i++)
                {
                    for (int j = 0; j < Info.DeleteRole.Count(); j++)
                    {
                        if (listUserRole[i].RoleId == Info.DeleteRole[j].RoleId)
                        {
                            RemoveUserRole(listUserRole[i].UserRoleId);
                        }
                    }
                }
            }

            if (Info.Role != null)
            {
                foreach (UserRoleInfo _role in Info.Role)
                {
                    bool exists = listUserRole.Where(x => x.RoleId == _role.RoleId).Any();
                    if (!exists)
                    {
                        UserRole userRole = new UserRole();
                        userRole.UserId = _userId;
                        userRole.RoleId = _role.RoleId;
                        userRole.CreatedBy = HttpContext.Session["UserName"].ToString();
                        userRole.ModifiedBy = HttpContext.Session["UserName"].ToString();
                        userRole.Save();
                    }
                }
            }
        }

        private void RemoveUserRole(int UserRoleId)
        {
            UserRole userRole = UserRoles.GetById(UserRoleId);

            if (userRole != null)
            {
                userRole.ModifiedBy = HttpContext.Session["UserName"].ToString();
                userRole.Delete();
            }
        }


        [AuthActivity("46")]
        public JsonResult ResetPassword(int userId)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Reset Password successfully."
            };

            var getByUserId = Users.GetById(userId);
            if (getByUserId == null)
            {
                model.InfoType = RequestResultInfoType.Warning;
                model.Message = "Data not found";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            var datLogin = (LoginResult)Session["Login"];

            //if (datLogin.UserName.ToLower() != "admin")
            //{
            //    model.InfoType = RequestResultInfoType.Warning;
            //    model.Message = "You don't have access to Reset Password, Please contact administrator";
            //    return Json(new { Html = model }, JsonRequestBehavior.AllowGet);    
            //}
            using (var scope = new TransactionScope())
            {
                var randomPasswordGenerated = GenerateRandomPassword();
                getByUserId.Password = randomPasswordGenerated;
                getByUserId.ModifiedBy = datLogin.UserName;
                var updatePwdResult = getByUserId.ChangePassword();

                getByUserId.ResetPassword = true;
                getByUserId.UpdateResetPassword();

                getByUserId.UpdateBadPasswordCount(0);

                scope.Complete();
                scope.Dispose();

                //model.Message = model.Message + "<br />Generated Password is <b>" + randomPasswordGenerated + "</b>";

                if (updatePwdResult)
                {
                    var userDetail = Users.GetById(userId);
                    //Send Email here for Reset Email Notification
                    var em = new EmailModel();
                    var emails = new List<string> { userDetail.Email };
                    em.EmailIds = emails;
                    em.EmailSubject = "Reset Password Notification";
                    em.NotificationType = MailNotificationType.ResetPassword;

                    var emb = new EmailBodyModel
                    {
                        To = userDetail.UserName,
                        From = "eFaktur Application Administrator",
                        GeneratedPassword = randomPasswordGenerated
                    };
                    bool isErrorSendEmail;
                    var mh = new MailHelper();
                    var mailSendResult = mh.SendMail(out isErrorSendEmail, em, emb);

                    model.Message = (string.IsNullOrEmpty(model.Message)
                        ? mailSendResult
                        : model.Message + "<br />Send Mail Notification Result : <br />" + mailSendResult);
                }

            }
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        private string GenerateRandomPassword()
        {
            var generator = new Random();
            var r = generator.Next(0, 1000000).ToString("D6");
            return r;
        }

        #endregion

        #region File Upload
        [HttpPost]
        public JsonResult UploadFileSp2()
        {
            var fileUploadModel = new FileUploadModel();

            HttpPostedFileBase file = Request.Files["FileToUpload"];

            if (file != null && file.ContentLength > 0)
            {
                var ext = Path.GetExtension(file.FileName);
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                try
                {
                    var imp = new ImpersonationHelper();
                    imp.Impersonate(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);

                    var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);

                    using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                    {
                        var rootDestFolder = string.Format(@"{0}\{1}", FileManagerConfiguration.RepoRootPath,
                               @"\SP2");

                        if (!Directory.Exists(rootDestFolder))
                        {
                            Directory.CreateDirectory(rootDestFolder);
                        }

                        var destFileName = string.Format("{0}_{1}{2}", fileName, DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                            ext);

                        var path = string.Format(@"{0}\{1}", rootDestFolder, destFileName);
                        file.SaveAs(path);

                        fileUploadModel.FileName = destFileName;
                        fileUploadModel.FilePath = path;

                        fileUploadModel.Messages = "File has been uploaded";
                        fileUploadModel.UploadStatus = true;
                        
                    }
                }
                catch (Exception ex)
                {
                    fileUploadModel.Messages = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                fileUploadModel.Messages = "You have not specified a file.";
            }

            var _fileUploadModel = new FileUploadModel
            {
                FilePath = !string.IsNullOrEmpty(fileUploadModel.FilePath) ? fileUploadModel.FilePath : "",
                Messages = fileUploadModel.Messages,
                UploadStatus = fileUploadModel.UploadStatus
            };
            return Json(_fileUploadModel, "text/html");
        }
        #endregion

        #region File Download
        public FileResult Download(string filePath)
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
                    string filename = Path.GetFileName(filePath);
                    string extension = Path.GetExtension(filename).ToLower();

                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                    string contentType = extension;
                    return File(fileBytes, contentType, Path.GetFileName(filePath));
                }
                
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }
            
            //return null;

        }
        #endregion

        public JsonResult SetSideBarSession(string expandCollapse)
        {
            if (expandCollapse == "expand")
            {
                Session["SideBarSession"] = "collapse";
            }
            else if (expandCollapse == "collapse")
            {
                Session["SideBarSession"] = "expand";
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

    }
}

