using System.ComponentModel.DataAnnotations;

namespace Portal.Module.Authentication.ViewModel
{
    public class LoginDto
    {
        [Required(ErrorMessage = "نام کاربری اجباری است")]
        public string Username { get; set; }
        [Required(ErrorMessage = "رمز عبور اجباری است")]
        public string Password { get; set; }
    }
}
