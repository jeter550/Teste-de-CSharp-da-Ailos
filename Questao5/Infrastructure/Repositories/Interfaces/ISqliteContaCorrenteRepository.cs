using Questao5.Domain.Entities;
using System.Data;

namespace Questao5.Infrastructure.Repositories.Interfaces
{
    public interface ISqliteContaCorrenteRepository
    {
        Task<ContaCorrente> SelecionarContaAsync(string contaCorrenteId);
        Task<decimal> ConsultarSaldoAsync(string contaCorrenteId);
        Task MovimentarSaldoAsync(Movimento movimento);

    }
}
