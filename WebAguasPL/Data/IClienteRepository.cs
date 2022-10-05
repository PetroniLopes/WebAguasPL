using System.Threading.Tasks;
using WebAguasPL.Data.Entities;

namespace WebAguasPL.Data
{
    public interface IClienteRepository : IGenericRepository<Cliente>
    {
        Task<Cliente> GetByUserAsync(User user);
    }
}
