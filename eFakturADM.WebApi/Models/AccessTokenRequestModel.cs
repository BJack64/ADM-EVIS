using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;
using System;

namespace eFakturADM.WebApi.Models
{
    public class AccessTokenRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <param name="_username">User name</param>
        /// <param name="_password">User password</param>
        /// <returns>True if user exist and password is correct</returns>
        public AccessTokenResponseModel IsValid(string _username, string _password, string _userAgent, string _ip)
        {
            var getByuserName = Users.GetByUserName(_username);
            if (getByuserName == null) return null;

            //check if on locking by BadPasswordCount
            var configMaxBadPwdCount = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MaxBadPasswordCount);

            if (configMaxBadPwdCount != null)
            {
                if (getByuserName.BadPasswordCount >= Convert.ToInt32(configMaxBadPwdCount.ConfigValue))
                {
                    return new AccessTokenResponseModel()
                    {
                        Status = false,
                        Message = string.Format("3 times wrong password, user {0} locked, Please Contact administrator", _username),
                        Token = string.Empty
                    };
                }
            }
            else
            {
                return new AccessTokenResponseModel()
                {
                    Status = false,
                    Message = "Config Not Found for MaxBadPasswordCount",
                    Token = string.Empty
                };
            }

            var userData = Users.GetLogin(_username, _password);
            if (userData == null)
            {
                getByuserName.BadPasswordCount = getByuserName.BadPasswordCount + 1;
                getByuserName.UpdateBadPasswordCount(getByuserName.BadPasswordCount);
                return new AccessTokenResponseModel()
                {
                    Status = false,
                    Message = "Wrong UserName or Password",
                    Token = string.Empty
                };
            }

            var userRoleData = UserRoles.GetByUserId(userData.UserId);

            long _tick = DateTime.Now.Ticks;
            string token = SecurityManagement.GenerateToken(_username, _userAgent, _tick);

            var loginResult = new AccessTokenResponseModel()
            {
                Status = true,
                Token = token,
                Message = string.Empty,
            };

            if (!string.IsNullOrEmpty(token))
            {
                //Reset BadPasswordCount to zero
                var upBadPwdCountSave = (new User()
                {
                    UserName = _username
                }).UpdateBadPasswordCount(0);

                //Truncate the token first
                UserAuthentications.Delete(userData.UserId, _userAgent);
                
                double TokenDuration =
                    double.Parse(
                        GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.TokenDuration).ConfigValue);
                DateTime timeStart = DateTime.Now;
                DateTime timeEnd = timeStart.AddHours(TokenDuration);
                loginResult.ExpiredAt = timeEnd;
                var datToSaveAuthUser = new UserAuthentication()
                {
                    UserAuthenticationId = 0,
                    UserId = userData.UserId,
                    IP = _ip,
                    UserAgent = _userAgent,
                    Token = token,
                    TimeStart = timeStart,
                    TimeEnd = timeEnd,
                    Status = true
                };

                var saveAuthResult = UserAuthentications.Save(datToSaveAuthUser);
            }
            

            return loginResult;
        }
    }
}