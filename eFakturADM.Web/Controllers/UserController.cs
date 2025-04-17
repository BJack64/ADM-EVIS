using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Collections;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Models;
using System.Transactions;
using eFakturADM.Logic.Utilities;
using System.Text.RegularExpressions;

namespace eFakturADM.Web.Controllers
{
    public class UserController : BaseController
    {
        //
        // GET: /User/
        [HttpGet]
        public ActionResult Login()
        {
            var d = Rijndael.Decrypt(@"IxwqUWmF+VLT6o9JegpHzg==");
            return View();
        }

        [HttpPost]
        public JsonResult Login(LoginModel user)
        {
            var model = new LoginResult()
            {
                IsAuthenticated = LoginAuthentication.NotAuthenticated,
                Message = ""
            };
            if (ModelState.IsValid)
            {
                string _ip = Request.UserHostAddress;
                user.UserAgent = Request.UserAgent.ToString();
                LoginResult _loginResult = new LoginResult();
                _loginResult = user.IsValid(user.UserName, user.Password, user.UserAgent);

                if (_loginResult != null)
                {

                    if (_loginResult.IsAuthenticated == LoginAuthentication.NotAuthenticated)
                    {
                        model.Message = _loginResult.Message;
                        return Json(new { data = model }, JsonRequestBehavior.AllowGet);
                    }

                    model.IsOnResetPassword = _loginResult.IsOnResetPassword;

                    if (user.LogOutOtherDevice == true)
                    {
                        if (Session["AskLogoutOtherDevice"] == null)
                        {
                            model.IsAuthenticated = LoginAuthentication.NotAuthenticated;
                            model.IsOnResetPassword = false;
                            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
                        }
                        var datUpdateAuthUser = new UserAuthentication()
                        {
                            UserAuthenticationId = 1,
                            UserId = _loginResult.UserId
                        };

                        var updateResult = UserAuthentications.Save(datUpdateAuthUser);
                    }
                    
                    //check another login from other device
                    var userAuthenticationData = UserAuthentications.CheckLoginExists(_loginResult.UserId, _ip);
                    if (userAuthenticationData == null)
                    {
                        //Reset BadPasswordCount to zero
                        var upBadPwdCountSave = (new User()
                        {
                            UserName = _loginResult.UserName
                        }).UpdateBadPasswordCount(0);

                        double TokenDuration =
                            double.Parse(
                                GeneralConfigs.GetById((int) ApplicationEnums.GeneralConfig.TokenDuration).ConfigValue);
                        DateTime timeStart = DateTime.Now;
                        DateTime timeEnd = timeStart.AddHours(TokenDuration);
                        var datToSaveAuthUser = new UserAuthentication()
                        {
                            UserAuthenticationId = 0,
                            UserId = _loginResult.UserId,
                            IP = _ip,
                            UserAgent = user.UserAgent,
                            Token = _loginResult.Token,
                            TimeStart = timeStart,
                            TimeEnd = timeEnd,
                            Status = true
                        };

                        var saveAuthResult = UserAuthentications.Save(datToSaveAuthUser);

                        Session["Login"] = _loginResult;
                        Session["UserName"] = _loginResult.UserName.ToString();
                        Session["UserId"] = _loginResult.UserId.ToString();

                        model.IsAuthenticated = LoginAuthentication.Authenticated;

                        try
                        {
                            Session["SideBarSession"] =
                                GeneralConfigs.GetById((int) ApplicationEnums.GeneralConfig.DefaultAutoHideLeftMenu).ConfigValue;
                        }
                        catch (Exception exception)
                        {
                            string outLogKey2;
                            Logger.WriteLog(out outLogKey2, LogLevel.Error, "Error Get Config [DefaultAutoHideLeftMenu]" + exception.Message, MethodBase.GetCurrentMethod(), exception);
                            throw;
                        }

                        

                        return Json(new {data = model}, JsonRequestBehavior.AllowGet);

                    }
                    else // ask for logout another device
                    {
                        Session["AskLogoutOtherDevice"] = true;
                        model.IsAuthenticated = LoginAuthentication.LogOutOtherDevice;
                        model.IsOnResetPassword = false;
                        return Json(new { data = model }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    model.IsAuthenticated = LoginAuthentication.NotAuthenticated;
                    model.IsOnResetPassword = false;
                    model.Message = "Wrong Username or Password";
                    return Json(new { data = model }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                model.IsAuthenticated = LoginAuthentication.NotAuthenticated;
                model.IsOnResetPassword = false;
                model.Message = "Invalid Input, Please check again";
                return Json(new { data = model }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Logout()
        {
            Session["Login"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        public JsonResult ChangePasswordValidation(ChangePasswordModel data)
        {
            var dataLogin = (LoginResult)Session["Login"];
            var model = ChangePasswordValidationMethods(data, dataLogin);

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);

        }

        private RequestResultModel ChangePasswordValidationMethods(ChangePasswordModel data, LoginResult dataLogin)
        {
            var dbData = Users.GetById(Convert.ToInt32(HttpContext.Session["UserId"].ToString()));

            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };
            var messages = new List<string>();
            if (string.IsNullOrEmpty(data.CurrentPassword))
            {
                messages.Add("Current Password required.");
            }
            else
            {
                //check if current password correct                
                var encryptedPwd = Rijndael.Encrypt(data.CurrentPassword);
                if (dbData.Password != encryptedPwd)
                {
                    messages.Add("Current Password is not valid.");
                }
            }

            if (string.IsNullOrEmpty(data.NewPassword))
            {
                messages.Add("New Password required.");
            }
            else
            {
                //Regex regex = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[!#$%&'()*+,-.:;<=>?@[\\\]{}^_`|~]).{8,}$");
                Regex regex = new Regex(@"^[a-zA-Z0-9]{8,}$");
                Match match = regex.Match(data.NewPassword);

                if (!match.Success)
                {
                    messages.Add("New Password must contains 8 characters, aplhanumeric combination.");
                }
            }

            if (string.IsNullOrEmpty(data.ReTypePassword))
            {
                messages.Add("Re-Type Password required.");
            }
            else
            {
                if (!string.IsNullOrEmpty(data.NewPassword) && !string.IsNullOrEmpty(data.ReTypePassword))
                {
                    if (data.NewPassword != data.ReTypePassword)
                    {
                        messages.Add("Re-Type Password is not match.");
                    }
                    if (dbData.Password == Rijndael.Encrypt(data.NewPassword))
                    {
                        messages.Add("New Password cannot be same as Old Password.");
                    }
                }
            }

            if (messages.Count <= 0) return model;

            model.InfoType = RequestResultInfoType.Warning;
            var txtMessage = string.Join("<br />", messages);
            model.Message = txtMessage;

            return model;
        }

        public JsonResult ChangePasswordSave(ChangePasswordModel data)
        {
            var dataLogin = (LoginResult)Session["Login"];
            var userName = Session["UserName"].ToString();
            var validationResult = ChangePasswordValidationMethods(data, dataLogin);

            if (validationResult.InfoType == RequestResultInfoType.Warning)
            {
                return Json(new { Html = validationResult }, JsonRequestBehavior.AllowGet);
            }

            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            var dataToSave = new User()
            {
                UserId = Convert.ToInt32(Session["UserId"].ToString()),
                ModifiedBy = userName,
                Password = data.NewPassword
            };

            var userData = Users.GetById(dataToSave.UserId);

            try
            {
                using (var eScope = new TransactionScope())
                {
                    var result = dataToSave.ChangePassword();
                    if (userData.ResetPassword)
                    {
                        userData.ResetPassword = false;
                        userData.ModifiedBy = userName;
                        userData.UpdateResetPassword();   
                    }
                    eScope.Complete();
                    eScope.Dispose();
                }
            }
            catch (Exception exception)
            {
                model.Message = "Change Password failed. Please try again. <br />Error Info : " + exception.Message;
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }
    }
}
