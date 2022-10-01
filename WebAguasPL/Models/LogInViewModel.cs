using System.ComponentModel.DataAnnotations;

namespace WebAguasPL.Models
{
    public class LogInViewModel
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        
    }
}
