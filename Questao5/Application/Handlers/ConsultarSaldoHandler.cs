using MediatR;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Infrastructure.Repositories.Interfaces;

namespace Questao5.Application.Handlers
{
    public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoQuery, SaldoResponse>
    {
        private readonly ISqliteIdempotenciaRepository _sqliteIdempotenciaRepository;
        private readonly ISqliteContaCorrenteRepository _sqliteContaCorrenteRepository;
        public ConsultarSaldoHandler(
            ISqliteIdempotenciaRepository sqliteIdempotenciaRepository, 
            ISqliteContaCorrenteRepository sqliteContaCorrenteRepository)
        {
            _sqliteIdempotenciaRepository = sqliteIdempotenciaRepository;
            _sqliteContaCorrenteRepository = sqliteContaCorrenteRepository;
        }

        public async Task<SaldoResponse> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
        {
            var resultadoIdempotencia = await _sqliteIdempotenciaRepository.ConsultarAsync(request.IdRequisicao);
            if (!string.IsNullOrEmpty(resultadoIdempotencia))
                return Newtonsoft.Json.JsonConvert.DeserializeObject<SaldoResponse>(resultadoIdempotencia);

            var conta = await _sqliteContaCorrenteRepository.SelecionarContaAsync(request.ContaCorrenteId);

            if (conta == null)
                throw new Exception("INVALID_ACCOUNT");
            if (!conta.Ativo)
                throw new Exception("INACTIVE_ACCOUNT");

            var saldo = await _sqliteContaCorrenteRepository.ConsultarSaldoAsync(request.ContaCorrenteId);

            var response = new SaldoResponse
            {
                Numero = conta.Numero,
                Nome = conta.Nome,
                DataConsulta = DateTime.UtcNow,
                Saldo = saldo
            };

            return response;
        }
    }

}
