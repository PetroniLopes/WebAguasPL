using System;

namespace WebAguasPL.Models
{
    public class LeituraViewModel
    {
        public int ContratoID { get; set; }

        public int LeituraID { get; set; }

        public DateTime DataLeitura { get; set; }

        public double Valor { get; set; }

        public bool Estado { get; set; }


    }
}
