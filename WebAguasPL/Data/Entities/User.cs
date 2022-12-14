using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAguasPL.Data.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }


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

    }
}
