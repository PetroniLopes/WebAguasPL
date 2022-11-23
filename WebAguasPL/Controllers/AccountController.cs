using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAguasPL.Data;
using WebAguasPL.Data.Entities;
using WebAguasPL.Helpers;
using WebAguasPL.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace WebAguasPL.Controllers
{

    public class AccountController : Controller
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUserHelper _userHelper;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;

        private Random _random;

        public AccountController(IClienteRepository clienteRepository,
            IUserHelper userHelper,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IMailHelper mailHelper)
        {
            _clienteRepository = clienteRepository;
            _userHelper = userHelper;
            _roleManager = roleManager;
            _configuration = configuration;
            _mailHelper = mailHelper;
            _random = new Random();
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

            var user = _userHelper.GetUserByIdAsync(id);

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
                    NIF = user.NIF,
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
                if (user == null)
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
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "User could not be created");
                        return View(model);
                    }



                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    
                    string tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    Response response = _mailHelper.SendEmail(
                        model.UserName, 
                        "Email confirmation", 
                        
                        $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"please click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>"
                        );

                    if (response.IsSuccess)
                    {
                        ViewBag.Message = "The instructions to allow your access has been sent to email";
                        return View(model);
                        
                    }

                    ModelState.AddModelError(string.Empty, "User could not logIn");

                }
            }

            return View(model);
        }

        public IActionResult ResetPassword()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.email);

                if (user == null)
                {
                    this.ModelState.AddModelError(string.Empty, "User not found");
                    return View(model);
                }


                string newPassword = _random.Next(999999999).ToString();

                
                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                string tokenLink = Url.Action("ConfirmPassword", "Account", new
                {
                    userid = user.Id,
                    token = myToken,
                    password = newPassword
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendEmail(
                    model.email,
                    "Reset Password",

                    $"<h1>Reset password request</h1>" +
                    $"There was a request to change the password of this User.</br></br>" +
                    $"Your new password is: {newPassword}</br></br>" +
                    $"Click in this link to continue:<a href = \"{tokenLink}\"> Change Password</a>"
                    );

                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to allow your access has been sent to email";
                    return View();

                }

                ModelState.AddModelError(string.Empty, "Something went wrong, please contact us at appaguaslisboa@gmail.com");

            }
            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangeUserViewModel();

            if (user != null)
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


        //API
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LogInViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(10),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public async Task<IActionResult> ConfirmPassword(string userId, string token, string password)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(password))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ResetPasswordAsync(user, token, password);

            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }
    }
}
