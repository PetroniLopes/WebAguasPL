using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAguasPL.Data;
using WebAguasPL.Data.Entities;
using WebAguasPL.Helpers;
using WebAguasPL.Models;

namespace WebAguasPL.Controllers
{
    public class ContratosController : Controller
    {
        
        private readonly IContratoRepository _contrato;
        private readonly IClienteRepository _cliente;
        private readonly IUserHelper _userHelper;

        public ContratosController(IContratoRepository contrato, IClienteRepository cliente, IUserHelper userHelper)
        {
           
            _contrato = contrato;
            _cliente = cliente;
            _userHelper = userHelper;
        }

        // GET: Contratos
        public async Task<IActionResult> Index()
        {
            var model = await _contrato.GetContractAsync(this.User.Identity.Name);
            return View(model);
        }


        public async Task<IActionResult> SelectCLiente()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (await _userHelper.IsUserInRoleAsync(user, "client"))
            {
                var cliente = _cliente.GetByUserAsync(user);

                //var model = new NewContratoViewModel
                //{
                //    ClienteId = cliente.Id
                //};

                return RedirectToAction(nameof(Create), cliente.Id);

            }
            return View(_cliente.GetAll().OrderBy(c => c.Name));
        }

        public async Task<IActionResult> ValidateContract(int id)
        {
            Contrato contract = await _contrato.GetByIdAsync(id);
            if (contract != null)
            {
                await _contrato.UpdateValidation(contract, true);
            }



            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CancelContract(int id)
        {
            Contrato contract = await _contrato.GetByIdAsync(id);
            if (contract != null)
            {
                await _contrato.UpdateValidation(contract, false);
            }



            return RedirectToAction("Index");
        }


        // GET: Contratos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contrato = await _contrato.GetContractByIdAsync(id.Value);

            if (contrato == null)
            {
                return NotFound();
            }

            return View(contrato);
        }

        // GET: Contratos/Create
        public IActionResult Create(int id)
        {

            var model = new NewContratoViewModel
            {
                ClienteId = id
            };

            return View(model);
        }

        public IActionResult CreateRequest(string id)
        {
            var user = _userHelper.GetUserByIdAsync(id);

            var Getuser = _userHelper.GetUserByEmailAsync(user.Result.Email);

            return View();
        }

        // POST: Contratos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewContratoViewModel model)
        {
            if (ModelState.IsValid)
            {

                var cliente = await _cliente.GetByIdAsync(model.ClienteId);

                if (cliente == null)
                {
                    var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                    if (await _userHelper.IsUserInRoleAsync(user, "client"))
                    {
                        cliente =  await _cliente.GetByUserAsync(user);
                        model.ClienteId = cliente.ID;
                    }
                }


                await _contrato.AddContractAsync(model);
                    

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        ////GET: Contratos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contrato = await _contrato.GetContractByIdAsync(id.Value);
            if (contrato == null)
            {
                return NotFound();
            }



            return View(contrato);
        }

        ////POST: Contratos/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contrato contrato)
        {
            if (id != contrato.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    await _contrato.UpdateContractAsync(contrato);
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _contrato.ContractExists(contrato.ID))
                    {
                        throw;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(contrato);
        }

        //GET: Contratos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contrato = await _contrato.GetContractByIdAsync(id.Value);
            
            if (contrato == null)
            {
                return NotFound();
            }

            return View(contrato);
        }

        // POST: Contratos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contrato = await _contrato.GetContractByIdAsync(id);
            if (contrato != null)
            {
                await _contrato.DeleteAsync(contrato);
            }
            
            return RedirectToAction(nameof(Index));
        }

    }
}
