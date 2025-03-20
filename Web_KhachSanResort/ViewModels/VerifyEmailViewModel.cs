using System.ComponentModel.DataAnnotations;

namespace Web_KhachSanResort.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress]
        public string Email { get; set; } 
    }
}
