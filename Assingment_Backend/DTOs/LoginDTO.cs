using System.ComponentModel.DataAnnotations;

namespace Assignment_Backend.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Không được bỏ trống tên tài khoản")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Không được bỏ trống mật khẩu")]
        public string Password { get; set; }
    }
}
