
using NSubstitute;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Repositories.Interfaces;
using System.Text.Json;
using Xunit;

namespace Questao5.Tests.Application
{
    public class ConsultarSaldoHandlerTests
    {
        private readonly ISqliteIdempotenciaRepository _idempotenciaRepository;
        private readonly ISqliteContaCorrenteRepository _contaCorrenteRepository;
        private readonly ConsultarSaldoHandler _handler;

        public ConsultarSaldoHandlerTests()
        {
            _idempotenciaRepository = Substitute.For<ISqliteIdempotenciaRepository>();
            _contaCorrenteRepository = Substitute.For<ISqliteContaCorrenteRepository>();
            _handler = new ConsultarSaldoHandler(_idempotenciaRepository, _contaCorrenteRepository);
        }

        [Fact]
        public async Task Handle_DeveRetornarSaldo_QuandoContaExisteEAtiva()
        {
            var query = new ConsultarSaldoQuery { ContaCorrenteId = "123", IdRequisicao = "req-1" };
            var conta = new ContaCorrente { Id = "123", Numero = 456, Nome = "Teste", Ativo = true };
            var saldo = 1000.50m;

            _idempotenciaRepository.ConsultarAsync(query.IdRequisicao).Returns(Task.FromResult<string>(null));
            _contaCorrenteRepository.SelecionarContaAsync(query.ContaCorrenteId).Returns(Task.FromResult(conta));
            _contaCorrenteRepository.ConsultarSaldoAsync(query.ContaCorrenteId).Returns(Task.FromResult(saldo));

            var response = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(response);
            Assert.Equal(conta.Numero, response.Numero);
            Assert.Equal(conta.Nome, response.Nome);
            Assert.Equal(saldo, response.Saldo);
        }

        [Fact]
        public async Task Handle_DeveRetornarErro_QuandoContaNaoExiste()
        {
            var query = new ConsultarSaldoQuery { ContaCorrenteId = "123", IdRequisicao = "req-2" };
            _idempotenciaRepository.ConsultarAsync(query.IdRequisicao).Returns(Task.FromResult<string>(null));
            _contaCorrenteRepository.SelecionarContaAsync(query.ContaCorrenteId).Returns(Task.FromResult<ContaCorrente>(null));

            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("INVALID_ACCOUNT", ex.Message);
        }

        [Fact]
        public async Task Handle_DeveRetornarErro_QuandoContaInativa()
        {
            var query = new ConsultarSaldoQuery { ContaCorrenteId = "123", IdRequisicao = "req-3" };
            var conta = new ContaCorrente { Id = "123", Numero = 456, Nome = "Teste", Ativo = false };

            _idempotenciaRepository.ConsultarAsync(query.IdRequisicao).Returns(Task.FromResult<string>(null));
            _contaCorrenteRepository.SelecionarContaAsync(query.ContaCorrenteId).Returns(Task.FromResult(conta));

            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("INACTIVE_ACCOUNT", ex.Message);
        }

        [Fact]
        public async Task Handle_DeveRetornarRespostaDoCache_QuandoIdempotenciaExiste()
        {
            var query = new ConsultarSaldoQuery { ContaCorrenteId = "123", IdRequisicao = "req-4" };
            var saldoResponse = new SaldoResponse { Numero = 456, Nome = "Teste", Saldo = 500.00m };
            var saldoResponseJson = JsonSerializer.Serialize(saldoResponse);

            _idempotenciaRepository.ConsultarAsync(query.IdRequisicao).Returns(Task.FromResult(saldoResponseJson));

            var response = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(response);
            Assert.Equal(saldoResponse.Numero, response.Numero);
            Assert.Equal(saldoResponse.Nome, response.Nome);
            Assert.Equal(saldoResponse.Saldo, response.Saldo);
        }
    }
}