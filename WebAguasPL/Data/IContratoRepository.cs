using System.Linq;
using System.Threading.Tasks;
using WebAguasPL.Data.Entities;
using WebAguasPL.Models;

namespace WebAguasPL.Data
{
    public interface IContratoRepository : IGenericRepository<Contrato>
    {
        Task<IQueryable<Contrato>> GetContractAsync(string cliente);

        Task AddContractAsync(NewContratoViewModel model);

        Task<Contrato> GetContractByIdAsync(int id);

        Task UpdateValidation(Contrato model, bool resultado);

        Task UpdateContractAsync(Contrato contrato);

        Task<bool> ContractExists(int id);

        Task DeleteContractAsync(Contrato contrato);


        //LEITURAS

        IQueryable GetContratosWithLeituras();

        Task<Contrato> GetContractWithLeiturasAsync(int id);

        Task<Leitura> GetLeituraAsync(int id);

        Task AddLeituraAsync(LeituraViewModel model);

        Task<int> UpdateLeituraAsync(Leitura leitura);

        Task<int> DeleteLeituraAsync(Leitura leitura);

        Task<Contrato> GetContratoByLeitura(Leitura leitura);

    }
}
