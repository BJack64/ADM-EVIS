using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Web.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string UserAgent { get; set; }
        public bool? LogOutOtherDevice { get; set; }

        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <param name="_username">User name</param>
        /// <param name="_password">User password</param>
        /// <returns>True if user exist and password is correct</returns>
        public LoginResult IsValid(string _username, string _password, string _userAgent)
        {
            var getByuserName = Users.GetByUserName(_username);
            if (getByuserName == null) return null;

            //check if on locking by BadPasswordCount
            var configMaxBadPwdCount = GeneralConfigs.GetById((int) ApplicationEnums.GeneralConfig.MaxBadPasswordCount);

            if (configMaxBadPwdCount != null)
            {
                if (getByuserName.BadPasswordCount >= Convert.ToInt32(configMaxBadPwdCount.ConfigValue))
                {
                    return new LoginResult()
                    {
                        IsAuthenticated = LoginAuthentication.NotAuthenticated,
                        Message = string.Format("3 times wrong password, user {0} locked, Please Contact administrator", _username)
                    };
                }
            }
            else
            {
                return new LoginResult()
                {
                    IsAuthenticated = LoginAuthentication.NotAuthenticated,
                    Message = "Config Not Found for MaxBadPasswordCount"
                };
            }

            var userData = Users.GetLogin(_username, _password);
            if (userData == null)
            {
                getByuserName.BadPasswordCount = getByuserName.BadPasswordCount + 1;
                getByuserName.UpdateBadPasswordCount(getByuserName.BadPasswordCount);
                return new LoginResult()
                {
                    IsAuthenticated = LoginAuthentication.NotAuthenticated,
                    Message = "Wrong UserName or Password"
                };
            }

            var userRoleData = UserRoles.GetByUserId(userData.UserId);

            long _tick = DateTime.Now.Ticks;
            string token = SecurityManagement.GenerateToken(_username, _userAgent, _tick);

            var loginResult = new LoginResult()
            {
                IsAuthenticated = LoginAuthentication.Authenticated,
                UserName = userData.UserName,
                UserId = userData.UserId,
                RoleId = userRoleData.Count > 0 ? userRoleData.Select(d => d.RoleId).Distinct().ToList() : new List<int>(),
                RoleName = userData.UserRole,
                Token = token,
                IsOnResetPassword = userData.ResetPassword
            };
            return loginResult;
        }
    }

    public class LoginResult
    {
        public LoginResult()
        {
            IsAuthenticated = LoginAuthentication.NotAuthenticated;
        }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public List<int> RoleId { get; set; }
        public LoginAuthentication IsAuthenticated { get; set; }
        public string Token { get; set; }
        public bool IsOnResetPassword { get; set; }
        public string Message { get; set; }
    }
    
    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ReTypePassword { get; set; }
    }

    public enum LoginAuthentication
    {
        Authenticated = 1,
        NotAuthenticated = 2,
        LogOutOtherDevice = 3
    }

}