using System.ComponentModel.DataAnnotations;

namespace WebAguasPL.Data.Entities
{
    public class Cliente
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }

        [MinLength(9, ErrorMessage = "O campo precisa de mais de 9 numeros")]
        [MaxLength(9, ErrorMessage = "O campo não pode ter mais de 9 numeros")]
        public string NIF { get; set; }

        public string Adress { get; set; }

        [RegularExpression(@"^\d{4}(-\d{3})?$", ErrorMessage = "Invalid Postal Code!")]
        [Display(Name="Postal Code")]
        public string Postalcode { get; set; }

        public string Email { get; set; }

        [MinLength(9, ErrorMessage = "O campo precisa de mais de 9 numeros")]
        [MaxLength(9, ErrorMessage = "O campo não pode ter mais de 9 numeros")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        


    }
}
