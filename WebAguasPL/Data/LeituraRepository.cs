using WebAguasPL.Data.Entities;

namespace WebAguasPL.Data
{
    public class LeituraRepository : GenericRepository<Leitura>, ILeituraRepository
    {
        private readonly DataContext _context;

        public LeituraRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
