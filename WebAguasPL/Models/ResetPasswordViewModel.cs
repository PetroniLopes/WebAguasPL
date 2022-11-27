using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebAguasPL.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name ="Email")]
        public string email { get; set; }
    }
}
