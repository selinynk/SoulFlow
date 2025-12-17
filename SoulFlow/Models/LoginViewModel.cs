using System.ComponentModel.DataAnnotations;

namespace SoulFlow.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunlu!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Şifre zorunlu!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
