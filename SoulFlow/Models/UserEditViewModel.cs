using System.ComponentModel.DataAnnotations;

namespace SoulFlow.Models
{
    public class UserEditViewModel
    {


        [Display(Name = "Hakkımda")]
        public string? Bio { get; set; }

        [Display(Name = "İlgi Alanları")]
        public string? Interests { get; set; }

        [Display(Name = "Profil Resmi")]
        public IFormFile? ProfilePicture { get; set; }

        public string? ExistingImage { get; set; }
    }
}