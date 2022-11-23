using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAguasPL.Data.Entities;

namespace WebAguasPL.Data
{
    public class ClienteRepository : GenericRepository<Cliente>, IClienteRepository
    {
        private readonly DataContext _context;

        public ClienteRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cliente> GetByUserAsync(User user)
        {
            return await _context.Set<Cliente>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.User == user);
        }

        public async Task<bool> ClientHasContract(Cliente cliente)
        {
            
            if(await _context.Contratos.Where(c => c.Cliente == cliente).FirstOrDefaultAsync() == null)
            {
                return false;
            }
            return true;
        }
            
    }
}
