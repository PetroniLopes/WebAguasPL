using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAguasPL.Data.Entities;
using WebAguasPL.Helpers;

namespace WebAguasPL.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await AddUser("admin", "");

            
            if (!_context.Clientes.Any())
            {
                await AddUser("Pedro","Alenquer");
                await AddUser("Luis","Lisboa");
                await AddUser("Brad","Porto");
                await AddUser("Tom","USA");
                await AddUser("Angelina","UK");

                await _context.SaveChangesAsync();

            }

        }

        private async Task AddUser(string name, string morada)
        {
            var email = (name + "@email.com");


            /// CRIAR USER
            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    Email = email,
                    UserName = email,
                };

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException($"Could not create User - {name} in seeder");
                }
            }

            if (name != "admin")
            {

                ///CRIAR CLIENTE COM USER

                _context.Clientes.Add(new Cliente
                {
                    Name = name,
                    NIF = _random.Next(100000000, 999999999).ToString(),
                    Adress = morada,
                    Postalcode = (_random.Next(9999) + "-" + _random.Next(999)).ToString(),
                    Email = email,
                    User = user
                });
            }

        }

        
    }
}
