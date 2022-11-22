using WebAguasPL.Data.Entities;
using WebAguasPL.Models;

namespace WebAguasPL.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Cliente ToCliente(ClienteViewModel model, string path)
        {
            return new Cliente
            {
                ID = model.ID,
                Name = model.Name,
                NIF = model.NIF,
                Adress = model.Adress,
                Postalcode = model.Postalcode,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                ImageUrl = path,
                User = model.User
            };
        }

        public ClienteViewModel ToClienteViewModel(Cliente cliente)
        {
            return new ClienteViewModel
            {
                ID = cliente.ID,
                Name = cliente.Name,
                NIF = cliente.NIF,
                Adress = cliente.Adress,
                Postalcode = cliente.Postalcode,
                Email = cliente.Email,
                PhoneNumber = cliente.PhoneNumber,
                ImageUrl = cliente.ImageUrl,
                User = cliente.User
            };
        }
    }
}
