using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAguasPL.Data.Entities
{
    public class Contrato : IEntity
    {
        [Key]
        public int ID { get; set; }


        [Required]
        public string Adress { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}(-\d{3})?$", ErrorMessage = "Invalid Postal Code!")]
        [Display(Name = "Postal Code")]
        public string Postalcode { get; set; }

        [Required]
        [Display (Name = "Contract Date")]
        [DisplayFormat (DataFormatString ="{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime ContractDate { get; set; }

        [Display(Name = "Approved")]
        public bool IsApproved { get; set; }

        public Cliente Cliente { get; set; }

        public ICollection<Leitura> Leituras { get; set; }
    }
}
