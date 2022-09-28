using Microsoft.EntityFrameworkCore;
using WebAguasPL.Data.Entities;

namespace WebAguasPL.Data
{
    public class DataContext:DbContext
    {

        public DbSet<Cliente> Clientes { get; set; }

        public DataContext(DbContextOptions<DataContext>options) : base(options)
        {

        }
    }
}
