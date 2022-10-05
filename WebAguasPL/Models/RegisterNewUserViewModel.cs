using System.ComponentModel.DataAnnotations;

namespace WebAguasPL.Models
{
    public class RegisterNewUserViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [Required]
        [MinLength(9, ErrorMessage = "O campo precisa de mais de 9 numeros")]
        [MaxLength(9, ErrorMessage = "O campo não pode ter mais de 9 numeros")]
        public string NIF { get; set; }

        [Required]
        public string Adress { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}(-\d{3})?$", ErrorMessage = "Invalid Postal Code!")]
        [Display(Name = "Postal Code")]
        public string Postalcode { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }

    }
}
