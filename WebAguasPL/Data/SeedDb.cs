using System;
using System.Linq;
using System.Threading.Tasks;
using WebAguasPL.Data.Entities;

namespace WebAguasPL.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private Random _random;

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!_context.Clientes.Any())
            {
                AddCliente("Pedro","Alenquer");
                AddCliente("Luis","Lisboa");
                AddCliente("Brad","Porto");
                AddCliente("Tom","USA");
                AddCliente("Angelina","UK");

                await _context.SaveChangesAsync();

            }

        }

        private void AddCliente(string name, string morada)
        {
            _context.Clientes.Add(new Cliente
            {
                Name = name,
                NIF = _random.Next(100000000, 999999999).ToString(),
                Adress = morada,
                Postalcode = (_random.Next(9999) + "-" + _random.Next(999)).ToString(),
                Email = (_random.Next(1000000) + "@email.com")
            });
        }
    }
}
