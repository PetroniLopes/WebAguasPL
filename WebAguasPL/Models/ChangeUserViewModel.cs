using System.ComponentModel.DataAnnotations;

namespace WebAguasPL.Models
{
    public class ChangeUserViewModel
    {
        [Required]
        public string Name { get; set; }

    }
}
