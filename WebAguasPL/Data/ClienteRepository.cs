using Microsoft.EntityFrameworkCore;
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

            
    }
}
