using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Infrastructure.Repositories.Interfaces;

namespace Questao5.Application.Handlers
{
    public class CriarMovimentoHandler : IRequestHandler<CriarMovimentoCommand, string>
    {
        private readonly ISqliteIdempotenciaRepository _sqliteIdempotenciaRepository;
        private readonly ISqliteContaCorrenteRepository _sqliteContaCorrenteRepository;
        public CriarMovimentoHandler(
            ISqliteIdempotenciaRepository sqliteIdempotenciaRepository, 
            ISqliteContaCorrenteRepository sqliteContaCorrenteRepository)
        {
            _sqliteIdempotenciaRepository = sqliteIdempotenciaRepository;
            _sqliteContaCorrenteRepository = sqliteContaCorrenteRepository;
        }

        public async Task<string> Handle(CriarMovimentoCommand request, CancellationToken cancellationToken)
        {

            var resultadoIdempotencia = await _sqliteIdempotenciaRepository.ConsultarAsync(request.ChaveIdempotencia);
            if (!string.IsNullOrEmpty(resultadoIdempotencia))
                return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resultadoIdempotencia);

            var conta = await _sqliteContaCorrenteRepository.SelecionarContaAsync(request.ContaCorrenteId);

            if (conta == null)
                throw new Exception("INVALID_ACCOUNT");
            if (!conta.Ativo)
                throw new Exception("INACTIVE_ACCOUNT");
            if (request.Valor <= 0)
                throw new Exception("INVALID_VALUE");
            if (request.TipoMovimento != 'C' && request.TipoMovimento != 'D')
                throw new Exception("INVALID_TYPE");

            var movimento = new Movimento()
            {
                Id = Guid.NewGuid().ToString(),
                DataMovimento = DateTime.UtcNow,
                TipoMovimento = (TipoMovimento)request.TipoMovimento,
                Valor = request.Valor
            };

            await _sqliteContaCorrenteRepository.MovimentarSaldoAsync(movimento);

            return movimento.Id;
        }
    }
}
