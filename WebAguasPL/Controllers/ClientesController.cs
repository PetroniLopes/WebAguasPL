using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAguasPL.Data;
using WebAguasPL.Data.Entities;
using WebAguasPL.Helpers;
using WebAguasPL.Models;

namespace WebAguasPL.Controllers
{
    
    public class ClientesController : Controller
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        public ClientesController(IClienteRepository clienteRepository, IUserHelper userHelper, IConverterHelper converterHelper)
        {
            _clienteRepository = clienteRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }

        // GET: Clientes
        public IActionResult Index()
        {
            return View(_clienteRepository.GetAll().OrderBy(p=> p.Name));
            
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _clienteRepository.GetByIdAsync(id.Value);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                /// CRIAR USER
                model.User = await _userHelper.GetUserByEmailAsync(model.Email);

                if (model.User == null)
                {
                    model.User = new User
                    {
                        Name = model.Name,
                        Email = model.Email,
                        UserName = model.Email,
                        NIF = model.NIF,
                        Adress = model.Adress,
                        Postalcode = model.Postalcode
                    };

                    var result = await _userHelper.AddUserAsync(model.User, "123456");

                    if (result != IdentityResult.Success)
                    {
                        throw new InvalidOperationException($"Could not create User - {model.Name} in seeder");
                    }
                }


                var path = string.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";


                    path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\clientes",
                            file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/clientes/{file}";
                }
                else
                {
                    path = $"~/images/clientes/noimage.png";
                }

                var cliente = _converterHelper.ToCliente(model, path);

                await _clienteRepository.CreateAsync(cliente);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _clienteRepository.GetByIdAsync(id.Value);
            if (cliente == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToClienteViewModel(cliente);


            return View(model);
        }

        

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel model)
        {
            if (id != model.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var clienteAntigo = await _clienteRepository.GetByIdAsync(id);
                    var user = await _userHelper.GetUserByEmailAsync(clienteAntigo.Email);

                    
                    user.UserName = model.Email;
                    user.Email = model.Email;
                    user.NIF = model.NIF;
                    user.Adress = model.Adress;
                    user.Postalcode = model.Postalcode;


                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        var file = $"{guid}.jpg";

                        path = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot\\images\\clientes",
                                file);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }

                        path = $"~/images/clientes/{file}";
                    }

                    var cliente = _converterHelper.ToCliente(model, path);

                    await _userHelper.UpdateUserAsync(user);

                    await _clienteRepository.UpdateAsync(cliente);
                                        
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _clienteRepository.ExistAsync(model.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _clienteRepository.GetByIdAsync(id.Value);
                
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if(!await _clienteRepository.ClientHasContract(cliente))
            {

                await _clienteRepository.DeleteAsync(cliente);

                return RedirectToAction(nameof(Index));
            }

            this.ModelState.AddModelError(string.Empty, "O cliente não pode ser apagado porque já tem contratos associados!");
            return View(cliente);
        }

        public async Task<IActionResult> ShowProfile()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            var cliente = await _clienteRepository.GetByUserAsync(user);



            return View(cliente);
        }
    }
}
