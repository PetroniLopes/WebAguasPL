using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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




        ///////////////LEITURAS
        ///////////////////////////////////////////////////

        public async Task<Contrato> GetContractWithLeiturasAsync(int id)
        {
            return await _context.Contratos
                    .Include(c => c.Leituras)
                    .Where(c => c.ID == id)
                    .FirstOrDefaultAsync();
        }

        public IQueryable GetContratosWithLeituras()
        {
            return _context.Contratos
                    .Include(c => c.Leituras)
                    .OrderBy(c => c.ContractDate);
        }

        public async Task AddLeituraAsync(LeituraViewModel model)
        {
            var contrato = await this.GetContractWithLeiturasAsync(model.ContratoID);
            if(contrato == null)
            {
                return;
            }

            contrato.Leituras.Add(new Leitura
            {
                DataLeitura = model.DataLeitura,
                Valor = ValorConsumo(model.Valor),
                Estado = model.Estado
            });

            _context.Contratos.Update(contrato);
            await _context.SaveChangesAsync();
        }


        public async Task<int> DeleteLeituraAsync(Leitura leitura)
        {
            var contrato = await _context.Contratos
                            .Where(c => c.Leituras.Any(cl => cl.ID == leitura.ID))
                            .FirstOrDefaultAsync();

            if (contrato == null)
            {
                return 0;
            }

            _context.Leituras.Remove(leitura);
            await _context.SaveChangesAsync();

            return contrato.ID;
        }

        public async Task<int> UpdateLeituraAsync(Leitura leitura)
        {
            var contrato = await _context.Contratos
                            .Where(c => c.Leituras.Any(cl => cl.ID == leitura.ID))
                            .FirstOrDefaultAsync();

            if (contrato == null)
            {
                return 0;
            }

            _context.Leituras.Update(leitura);
            await _context.SaveChangesAsync();

            return contrato.ID;
        }

        public async Task<Leitura> GetLeituraAsync(int id)
        {
            return await _context.Leituras.FindAsync(id);
        }

        public async Task<Contrato> GetContratoByLeitura(Leitura leitura)
        {
            return await _context.Contratos
                    .Where(c => c.Leituras.Any(l => l.ID == leitura.ID))
                    .FirstOrDefaultAsync();
        }




        public double ValorConsumo(double valor)
        {

            var TabelaDeEscaloes = new List<Escalao>();
            TabelaDeEscaloes.Add(new Escalao { Limite = 0, ValorUnitario = 1.6 });
            TabelaDeEscaloes.Add(new Escalao { Limite = 5, ValorUnitario = 0.3 });
            TabelaDeEscaloes.Add(new Escalao { Limite = 15, ValorUnitario = 0.8 });
            TabelaDeEscaloes.Add(new Escalao { Limite = 25, ValorUnitario = 1.3 });

            double total = 0;
            double consumo = valor;
            double valorteto = 0;
            double limiteAtratar = 0;

            foreach (var escalao in TabelaDeEscaloes)
            {

                if (escalao.Limite == 0)
                {
                    valorteto = escalao.ValorUnitario;
                }
                else
                {

                    if (consumo - escalao.Limite >= 0)
                    {

                        total += escalao.ValorUnitario * (escalao.Limite - limiteAtratar);
                        limiteAtratar = escalao.Limite;
                    }
                    else
                    {
                        total += escalao.ValorUnitario * (escalao.Limite - consumo);
                        return total;
                    }
                    if (consumo - escalao.Limite == 0)
                    {

                        return total;
                    }

                }


            }


            total += (consumo - limiteAtratar) * valorteto;


            return total;
        }
    }
}
