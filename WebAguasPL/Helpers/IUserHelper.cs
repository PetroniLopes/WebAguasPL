using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WebAguasPL.Data.Entities;
using WebAguasPL.Models;

namespace WebAguasPL.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LogInAsync(LogInViewModel model);

        Task LogOutAsync();
    }
}
