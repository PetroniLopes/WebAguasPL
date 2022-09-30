using WebAguasPL.Data.Entities;

namespace WebAguasPL.Data
{
    public class ClienteRepository : GenericRepository<Cliente>, IClienteRepository
    {
        public ClienteRepository(DataContext context) : base(context)
        {
        }
    }
}
