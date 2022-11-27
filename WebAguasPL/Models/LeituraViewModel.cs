using System;
using System.ComponentModel.DataAnnotations;

namespace WebAguasPL.Models
{
    public class LeituraViewModel
    {
        public int ContratoID { get; set; }

        public int LeituraID { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataLeitura { get; set; }

        [Required]
        public double Valor { get; set; }

        public bool Estado { get; set; }


    }
}
