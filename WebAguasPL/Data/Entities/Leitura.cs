using System;
using System.Collections.Generic;

namespace WebAguasPL.Data.Entities
{
    public class Leitura : IEntity
    {
        public int ID { get; set; }

        public DateTime DataLeitura { get; set; }

        public double Valor { get; set; }

        public bool Estado { get; set; }


        
    }
}
