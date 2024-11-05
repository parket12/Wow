using System.ComponentModel.DataAnnotations;

namespace Yakshin.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Passwords { get; set; }

    }
}
