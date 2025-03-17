using System.ComponentModel.DataAnnotations;

namespace Assignment_Backend.DTOs
{
    public class RegisterModel
    {

        [RegularExpression(@"^[a-zA-Z0-9_]{6,}", ErrorMessage = "Tài khoản ít nhất 6 kí tự, không có kí tự đặc biệt.")]
        [Required(ErrorMessage = "Tên tài khoản không được bỏ trống")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Tên không được bỏ trống")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{9,}$", ErrorMessage = "Mật khẩu tối thiểu 9 ký từ, gồm chữ hoa chữ thường và ký tự đặc biệt")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không trùng khớp")]
       [Required]
       [DataType(DataType.Password)]
       public string ConfirmPassword { get; set; }

       public string PhoneNumber { get; set; }

    }
}
