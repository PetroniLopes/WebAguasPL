using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAguasPL.Data.Entities;

namespace WebAguasPL.Data
{
    public class DataContext : IdentityDbContext<User>
    {

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Contrato> Contratos { get; set; }

        public DbSet<Escalao> Escaloes { get; set; }

        public DbSet<Leitura> Leituras { get; set; }


        public DataContext(DbContextOptions<DataContext>options) : base(options)
        {

        }

    }
}
