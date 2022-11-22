using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using WebAguasPL.Data.Entities;

namespace WebAguasPL.Models
{
    public class ClienteViewModel : Cliente
    {
        [Display(Name="Fotografia")]
        public IFormFile ImageFile { get; set; }
    }
}
