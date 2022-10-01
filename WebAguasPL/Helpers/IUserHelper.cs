using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WebAguasPL.Data.Entities;

namespace WebAguasPL.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);
    }
}
