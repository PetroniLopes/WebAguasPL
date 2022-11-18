﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAguasPL.Data.Entities;
using WebAguasPL.Helpers;
using WebAguasPL.Models;

namespace WebAguasPL.Data
{
    public class ContratoRepository : GenericRepository<Contrato>, IContratoRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IClienteRepository _clientRepository;

        public ContratoRepository(DataContext context, IUserHelper userHelper, IClienteRepository clientRepository) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _clientRepository = clientRepository;
        }

        public async Task<IQueryable<Contrato>> GetContractAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if(user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, "employee"))
            {
                return _context.Contratos
                    .Include(c => c.Cliente)
                    .OrderByDescending(o => o.ContractDate);
            }

            var client = await _clientRepository.GetByUserAsync(user);

            return _context.Contratos
                    .Where(c => c.Cliente == client)
                    .OrderByDescending(o => o.ContractDate);
                    
        }

        public async Task UpdateValidation(Contrato model, bool resultado)
        {
            var contrato = await _context.Contratos.FindAsync(model.ID);
            if(contrato == null)
            {
                return;
            }

            contrato.IsApproved = resultado;

            _context.Contratos.Update(contrato);

            await _context.SaveChangesAsync();
        }

        public async Task<Contrato> GetContractByIdAsync(int id)
        {
            return await _context.Contratos.FirstOrDefaultAsync(e => e.ID == id);
        }

        public async Task AddContractAsync(NewContratoViewModel model)
        {
            

            var client = await _context.Clientes.FindAsync(model.ClienteId);
            if (client == null)
            {
                return;
            }

            var contract = new Contrato
            {
                Adress = model.Adress,
                Postalcode = model.Postalcode,
                ContractDate = DateTime.Now,
                IsApproved = false,
                Cliente = client
            };

            _context.Contratos.Add(contract);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ContractExists(int id)
        {
            if (await _context.Contratos.AnyAsync(e => e.ID == id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task UpdateContractAsync(Contrato contrato)
        {
            if(await ContractExists(contrato.ID))
            {
                _context.Contratos.Update(contrato);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteContractAsync(Contrato contrato)
        {
            
            if (await ContractExists(contrato.ID))
            {
                _context.Contratos.Remove(contrato);
                await _context.SaveChangesAsync();
            }
        }
    }
}
