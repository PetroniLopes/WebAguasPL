using WebAguasPL.Data.Entities;
using WebAguasPL.Models;

namespace WebAguasPL.Helpers
{
    public interface IConverterHelper
    {
        Cliente ToCliente(ClienteViewModel model, string path);

        ClienteViewModel ToClienteViewModel(Cliente cliente);
    }
}
