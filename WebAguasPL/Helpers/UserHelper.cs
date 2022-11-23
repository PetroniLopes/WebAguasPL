using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebAguasPL.Data.Entities;
using WebAguasPL.Models;

namespace WebAguasPL.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserHelper(UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
            
            
        }

        public async Task RemoveUserToRoleAsync(User user, string roleName)
        {
            await _userManager.RemoveFromRoleAsync(user, roleName);
            


        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<IEnumerable<UsersViewModel>> GetAllUsers()
        {
            
            var users = _userManager.Users.ToList();
            var userRoles = new List<UsersViewModel>();
            

            foreach (User user in users)
            {

                var usersWithRoles = new UsersViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = await GetUSerRoles(user)
                };

                userRoles.Add(usersWithRoles);
                
            }
            

            return userRoles.OrderBy(p => p.Name);
        }



        

        public async Task<IEnumerable<string>> GetUSerRoles(User user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        

        public async Task<User> GetUserByIdAsync(string id)
        {

            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<string> GetRoleNameById(string id)
        {
            

            foreach (var role in _roleManager.Roles)
            {
                if (role.Id == id)
                {
                    return role.Name;
                }
            }

            return string.Empty;
        }
        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LogInAsync(LogInViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe, 
                false);
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public IEnumerable<SelectListItem> GetComboRoles()
        {
            //List<string> lista = new List<string>();

            //foreach(var role in _roleManager.Roles)
            //{

            //}
            try
            {
                var list = _roleManager.Roles
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }).OrderBy(l => l.Text).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "select role",
                    Value = "0"
                });

                return list;
            }
            catch
            {
                return null;
            }
        }

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }
    }
}
