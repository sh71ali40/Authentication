using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Portal.Infrustructure;
using Portal.Infrustructure.Class;
using Portal.Infrustructure.Interface;
using Portal.Infrustructure.Service;
using Portal.Infrustructure.ViewModel;
using Portal.Module.Authentication.Class;
using Portal.Module.Authentication.ViewModel;

namespace Portal.Module.Authentication.Controller.api
{
    [Area("Authentication")]
    public class AuthController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IUserService _userService;
        private readonly ModuleConfiguration _moduleConfiguration;
        private readonly IPasswordHasher _passwordHasher;

        public AuthController(IUserService userService,ICoreController coreController, IPasswordHasher passwordHasher)
        {
            _userService = userService;
            _moduleConfiguration = coreController.ModuleConfiguration;
            _passwordHasher = passwordHasher;
        }
        [HttpPost]
        public virtual async Task<ApiOutput> Login([FromBody] MobileLoginDto loginDto)
        {
            try
            {
                var confirmCode = (new Random()).Next(10000, 99999);
                var user = await _userService.GetUserByMobile(loginDto.Mobile);
                var rroleId = _moduleConfiguration.GetSettingValue((int) Variable.ModuleSetting.RoleId);

                if (user == null)
                {
                    var roleId = _moduleConfiguration.GetSettingValue((int)Variable.ModuleSetting.RoleId);
                    var userObj = new UserDto()
                    {
                        UserPass =_passwordHasher.Hash(loginDto.Mobile),
                        FirstName = "",
                        LastName = "",
                        RoleId = int.Parse(roleId) , 
                        UserName = loginDto.Mobile, 
                        Mobile = loginDto.Mobile,
                        JoinIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        LastIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        MobileConfirmed = false,
                        JoinDate = DateTime.Now,
                        LastModificationDateTime = DateTime.Now,
                        };
                    user = await _userService.AddNewUser(userObj);
                }

                user.SentCode = confirmCode.ToString();
                user.SentCodeExpirationDateTime = DateTime.Now.AddMinutes(5);
                var isUpdated = await _userService.UpdateUser(user);

                return new ApiOutput {StatusCode = Infrustructure.Class.StatusCode.Succeed , Message = "کد باموفقیت برای شما ارسال شد",Data = null};
            }
            catch (Exception e)
            {
                return new ApiOutput {StatusCode = Infrustructure.Class.StatusCode.Failed, Message = "مشکلی در ثبت اتفاق افتاد",Data = null};
            }

            
            
        }

        [HttpPost]
        public virtual async Task<ApiOutput> ConfirmCode([FromBody] MobileConfirmDto confirmCodeData)
        {
            try
            {
                var user = await _userService.GetUserByMobile(confirmCodeData.Mobile);
                if (user.SentCode == confirmCodeData.ConfirmCode && user.SentCodeExpirationDateTime >= DateTime.Now)
                {

                    var packedJwt = _userService.GenerateJsonWebToken(user.UserId);
                    return new ApiOutput { StatusCode = Infrustructure.Class.StatusCode.Succeed, Data = new {Token = packedJwt }, Message = "ورود باموفقیت انجام شد" };
                }
                if (user.SentCodeExpirationDateTime < DateTime.Now)
                {
                    return new ApiOutput { StatusCode = Infrustructure.Class.StatusCode.Failed, Data = null, Message = "کد وارد شده منقضی شده است، لطفا مجددا امتحان کنید." };
                }

                return new ApiOutput { StatusCode = Infrustructure.Class.StatusCode.Failed, Data = null, Message = "کد وارد شده اشتباه است" };

            }
            catch (Exception e)
            {
                return new ApiOutput { StatusCode = Infrustructure.Class.StatusCode.Failed, Message = "مشکلی در ثبت اتفاق افتاد", Data = null };
            }
        }
    }
}
