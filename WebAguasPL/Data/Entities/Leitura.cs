using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace WebAguasPL.Data.Entities
{
    public class Leitura : IEntity
    {
        public int ID { get; set; }

        [Required]
        [Display(Name ="Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataLeitura { get; set; }

        [Required]
        [Display(Name = "Reading")]
        public double Valor { get; set; }

        public bool Estado { get; set; }

        [Display(Name = "Cost")]
        public double ValorConsumo
        {
            get
            {
                if (this.Valor == 0)
                {
                    return 0;
                }

                var TabelaDeEscaloes = new List<Escalao>();
                TabelaDeEscaloes.Add(new Escalao { Limite = 0, ValorUnitario = 1.6 });
                TabelaDeEscaloes.Add(new Escalao { Limite = 5, ValorUnitario = 0.3 });
                TabelaDeEscaloes.Add(new Escalao { Limite = 15, ValorUnitario = 0.8 });
                TabelaDeEscaloes.Add(new Escalao { Limite = 25, ValorUnitario = 1.3 });

                double total = 0;
                double consumo = this.Valor;
                double valorteto = 0;
                double limiteAtratar = 0;

                foreach (var escalao in TabelaDeEscaloes)
                {

                    if (escalao.Limite == 0)
                    {
                        valorteto = escalao.ValorUnitario;
                    }
                    else
                    {

                        if (consumo - escalao.Limite >= 0)
                        {

                            total += escalao.ValorUnitario * (escalao.Limite - limiteAtratar);
                            limiteAtratar = escalao.Limite;
                        }
                        else
                        {
                            total += escalao.ValorUnitario * (escalao.Limite - consumo);
                            return total;
                        }
                        if (consumo - escalao.Limite == 0)
                        {

                            return total;
                        }

                    }


                }


                total += (consumo - limiteAtratar) * valorteto;


                return total;
            }
        }

    }
}
