using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Portal.Infrustructure;
using Portal.Infrustructure.Class;
using Portal.Infrustructure.Interface;
using Portal.Infrustructure.Service;
using Portal.Module.Authentication.Class;
using Portal.Module.Authentication.ViewModel;

namespace Portal.Module.Authentication.Controller
{
    [Area("Authentication")]
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ModuleConfiguration _moduleConfiguration;
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;

        private readonly IActionDescriptorCollectionProvider _provider;

        public HomeController(ICoreController coreController, IUserService userService, IActionDescriptorCollectionProvider provider, IPasswordHasher passwordHasher)
        {
            _moduleConfiguration = coreController.ModuleConfiguration;
            _userService = userService;
            _passwordHasher = passwordHasher;

            _provider = provider;

        }

        public IActionResult Index()
        {
            var targetPageId = _moduleConfiguration.GetSettingValue((int)Variable.ModuleSetting.PageId);
            if (_moduleConfiguration.HasDefinedPermission((int) Variable.ModulePermission.View))
            {

            }
            return View("/Areas/Authentication/Views/Home/Index.cshtml");
        }
        [HttpPost]
        public IActionResult Login(LoginDto loginData)
        {

            if (!ModelState.IsValid)
            {
                return View("/Areas/Authentication/Views/Home/Index.cshtml");
            }
            //Check the user name and password  
            //Here can be implemented checking logic from the database  
            ClaimsIdentity identity = null;
            bool isAuthenticated = false;
            var pass = _passwordHasher.Hash(loginData.Password);
            var user =  _userService.CheckUserNamePassword(loginData.Username,loginData.Password);

            if (user!=null)
            {

                //Create the identity for the user  
                identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Sid, user.UserId.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                isAuthenticated = true;
            }
            //"UnAuthenticated"

            if (!isAuthenticated)
            {
                ModelState.AddModelError("Password","نام کاربری یا رمز عبور اشتباه است");
                return View("/Areas/Authentication/Views/Home/Index.cshtml");
            }
            var principal = new ClaimsPrincipal(identity);

            var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            var targetPageId = _moduleConfiguration.GetSettingValue((int) Variable.ModuleSetting.PageId);


            var routes = _provider.ActionDescriptors.Items.Select(x => new {
                Action = x.RouteValues["Action"],
                Controller = x.RouteValues["Controller"],
                Name = x.AttributeRouteInfo?.Name,
                Template = x.AttributeRouteInfo?.Template
            }).ToList();



            
            //var res = RedirectToRoute("controller", "page?pageid="+targetPageId);
            return Redirect("~/page?pageid=8");
            //return "Authenticated";
            //return View();
        }
        
      }
}
