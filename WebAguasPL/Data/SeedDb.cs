using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
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
            await _context.Database.MigrateAsync();


            if (!_context.Clientes.Any())
            {
                await _userHelper.CheckRoleAsync("admin");
                await _userHelper.CheckRoleAsync("client");
                await _userHelper.CheckRoleAsync("employee");




                await AddUser("admin", "testeadmin", "admin");
                await AddUser("Andre", "testeandre", "employee");



                await AddUser("Pedro","Alenquer", "client");
                await AddUser("Luis","Lisboa", "client");
                await AddUser("Brad","Porto", "client");
                await AddUser("Tom","USA", "client");
                await AddUser("Angelina","UK", "client");
                



            }

            if (!_context.Escaloes.Any())
            {
                AddEscalao(5, 0.3);
                AddEscalao(15, 0.8);
                AddEscalao(25, 1.2);
                AddEscalao(0, 1.6);
            }
            
            await _context.SaveChangesAsync();

        }

        private void AddEscalao(int limite, double valorUnitario)
        {
            _context.Escaloes.Add(new Escalao
            {
                Limite = limite,
                ValorUnitario = valorUnitario
            });
        }

        private async Task AddUser(string name, string morada, string roleName)
        {
            var email = (name + "@email.com");


            /// CRIAR USER
            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    NIF = _random.Next(100000000, 999999999).ToString(),
                    Adress = morada,
                    Postalcode = (_random.Next(9999) + "-" + _random.Next(999)).ToString(),
                    Email = email,
                    UserName = email,
                };

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException($"Could not create User - {name} in seeder");
                }

                //if (name == "admin")
                //{
                await _userHelper.AddUserToRoleAsync(user, roleName);
                //}

                var isInRole = await _userHelper.IsUserInRoleAsync(user, roleName);
                if (!isInRole)
                {
                    await _userHelper.AddUserToRoleAsync(user, roleName);
                }

            }

            if (name != "admin" && name != "employee")
            {

                ///CRIAR CLIENTE COM USER

                _context.Clientes.Add(new Cliente
                {
                    Name = name,
                    NIF = _random.Next(100000000, 999999999).ToString(),
                    Adress = morada,
                    Postalcode = (_random.Next(9999) + "-" + _random.Next(999)).ToString(),
                    Email = email,
                    User = user,
                    ImageUrl = $"~/images/clientes/noimage.png"
            });
            }

        }

        
    }
}
