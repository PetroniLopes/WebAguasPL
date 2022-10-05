using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
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

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);
        
        Task CheckRoleAsync(string roleName);
        
        Task AddUserToRoleAsync(User user, string roleName);
        
        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<IEnumerable<UsersViewModel>> GetAllUsers();

        Task<User> GetUserByIdAsync(string id);

        IEnumerable<SelectListItem> GetComboRoles();

        Task<string> GetRoleNameById(string id);

        Task RemoveUserToRoleAsync(User user, string roleName);

        Task<IEnumerable<string>> GetUSerRoles(User user);
    }

}
