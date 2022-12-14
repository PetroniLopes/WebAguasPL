using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAguasPL.Data.Entities
{
    public class Cliente : IEntity
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [MinLength(9, ErrorMessage = "O campo precisa de mais de 9 numeros")]
        [MaxLength(9, ErrorMessage = "O campo não pode ter mais de 9 numeros")]
        public string NIF { get; set; }

        [Required]
        public string Adress { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}(-\d{3})?$", ErrorMessage = "Invalid Postal Code!")]
        [Display(Name="Postal Code")]
        public string Postalcode { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [MinLength(9, ErrorMessage = "O campo precisa de mais de 9 numeros")]
        [MaxLength(9, ErrorMessage = "O campo não pode ter mais de 9 numeros")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string ImageUrl { get; set; }
        
        public User User { get; set; }


        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return null;
                }

                return $"https://https://localhost:44303{ImageUrl.Substring(1)}";
            }
        }
        

    }
}
