using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAguasPL.Data;
using WebAguasPL.Data.Entities;
using WebAguasPL.Helpers;
using WebAguasPL.Models;

namespace WebAguasPL.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUserHelper _userHelper;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;

        public AccountController(IClienteRepository clienteRepository, IUserHelper userHelper, RoleManager<IdentityRole> roleManager)
        {
            _clienteRepository = clienteRepository;
            _userHelper = userHelper;
            _roleManager = roleManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _userHelper.GetAllUsers());

        }

        // GET: Users/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user =  _userHelper.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UsersViewModel
            {
                roles = _userHelper.GetComboRoles(),
                

            };


            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsersViewModel model)
        {
            //if (ModelState.IsValid)
            //{
                var user = await _userHelper.GetUserByIdAsync(model.Id);
                if (user == null)
                {
                    return View(model);
                }


                var roleName = await _userHelper.GetRoleNameById(model.RoleId);
                if (roleName == null)
                {
                    return View(model);
                }
                //var newRole = await _roleManager.GetRoleNameAsync(new IdentityRole{Id = model.RoleId });


                
                if (_userHelper.GetUSerRoles(user).Result.Any())
                {

                var roleApagar = _userHelper.GetUSerRoles(user).Result.First();
                    
                await _userHelper.RemoveUserToRoleAsync(user, roleApagar);

                }
                if (await _clienteRepository.GetByUserAsync(user) == null)
                {
                    Cliente cliente = new Cliente
                    {
                        Name = user.Name,
                        Email = user.Email,
                        NIF= user.NIF,
                        Adress = user.Adress,
                        Postalcode = user.Postalcode,
                        User = user                   
                    };

                    await _clienteRepository.CreateAsync(cliente);

                }
                await _userHelper.AddUserToRoleAsync(user, roleName);

                return RedirectToAction("Index");
            //}

            //return View(model);

    }


    public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LogInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LogInAsync(model);
                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //[Authorize(Roles = "admin")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                if(user == null)
                {
                    user = new User
                    {
                        Name = model.Name,
                        Email = model.UserName,
                        UserName = model.UserName,
                        NIF = model.NIF,
                        Adress = model.Adress,
                        Postalcode = model.Postalcode
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if(result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "User could not be created");
                        return View(model);
                    }

                    var loginViewModel = new LogInViewModel
                    {
                        Password = model.Password,
                        RememberMe = false,
                        UserName = model.UserName
                    };

                    var result2 = await _userHelper.LogInAsync(loginViewModel);
                    if (result2.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "User could not logIn");

                }
            }

            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangeUserViewModel();

            if(user!= null)
            {
                model.Name = user.Name;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    user.Name = model.Name;
                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User updated";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {

                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found");
                }
            }


            return View(model);
        }
    }
}
