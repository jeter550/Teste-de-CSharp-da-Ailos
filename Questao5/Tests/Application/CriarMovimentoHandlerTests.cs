using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Infrastructure.Repositories.Interfaces;
using Xunit;

namespace Questao5.Tests.Application
{
    public class CriarMovimentoHandlerTests
    {
        private readonly ISqliteIdempotenciaRepository _idempotenciaRepository;
        private readonly ISqliteContaCorrenteRepository _contaCorrenteRepository;
        private readonly CriarMovimentoHandler _handler;

        public CriarMovimentoHandlerTests()
        {
            _idempotenciaRepository = Substitute.For<ISqliteIdempotenciaRepository>();
            _contaCorrenteRepository = Substitute.For<ISqliteContaCorrenteRepository>();

            _handler = new CriarMovimentoHandler(_idempotenciaRepository, _contaCorrenteRepository);
        }

        [Fact]
        public async Task Handle_DeveCriarMovimento_QuandoDadosValidos()
        {
            var command = new CriarMovimentoCommand { ChaveIdempotencia = "123", ContaCorrenteId = "abc", TipoMovimento = 'C', Valor = 500.00m };
            var conta = new ContaCorrente { Id = "abc", Ativo = true };

            _idempotenciaRepository.ConsultarAsync(command.ChaveIdempotencia).Returns(Task.FromResult<string>(null));
            _contaCorrenteRepository.SelecionarContaAsync(command.ContaCorrenteId).Returns(Task.FromResult(conta));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            await _contaCorrenteRepository.Received(1).MovimentarSaldoAsync(Arg.Any<Movimento>());
        }

        [Fact]
        public async Task Handle_DeveRetornarErro_QuandoContaNaoExiste()
        {
            var command = new CriarMovimentoCommand { ChaveIdempotencia = "123", ContaCorrenteId = "abc", TipoMovimento = 'C', Valor = 500.00m };

            _idempotenciaRepository.ConsultarAsync(command.ChaveIdempotencia).Returns(Task.FromResult<string>(null));
            _contaCorrenteRepository.SelecionarContaAsync(command.ContaCorrenteId).Returns(Task.FromResult<ContaCorrente>(null));

            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("INVALID_ACCOUNT", ex.Message);
        }

        [Fact]
        public async Task Handle_DeveRetornarErro_QuandoContaInativa()
        {
            var command = new CriarMovimentoCommand { ChaveIdempotencia = "123", ContaCorrenteId = "abc", TipoMovimento = 'C', Valor = 500.00m };
            var conta = new ContaCorrente { Id = "abc", Ativo = false };

            _idempotenciaRepository.ConsultarAsync(command.ChaveIdempotencia).Returns(Task.FromResult<string>(null));
            _contaCorrenteRepository.SelecionarContaAsync(command.ContaCorrenteId).Returns(Task.FromResult(conta));

            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("INACTIVE_ACCOUNT", ex.Message);
        }

        [Fact]
        public async Task Handle_DeveRetornarErro_QuandoValorInvalido()
        {
            var command = new CriarMovimentoCommand { ChaveIdempotencia = "123", ContaCorrenteId = "abc", TipoMovimento = 'C', Valor = -10.00m };
            var conta = new ContaCorrente { Id = "abc", Ativo = true };

            _idempotenciaRepository.ConsultarAsync(command.ChaveIdempotencia).Returns(Task.FromResult<string>(null));
            _contaCorrenteRepository.SelecionarContaAsync(command.ContaCorrenteId).Returns(Task.FromResult(conta));

            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("INVALID_VALUE", ex.Message);
        }

        [Fact]
        public async Task Handle_DeveRetornarErro_QuandoTipoMovimentoInvalido()
        {
            var command = new CriarMovimentoCommand { ChaveIdempotencia = "123", ContaCorrenteId = "abc", TipoMovimento = 'X', Valor = 100.00m };
            var conta = new ContaCorrente { Id = "abc", Ativo = true };

            _idempotenciaRepository.ConsultarAsync(command.ChaveIdempotencia).Returns(Task.FromResult<string>(null));
            _contaCorrenteRepository.SelecionarContaAsync(command.ContaCorrenteId).Returns(Task.FromResult(conta));

            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("INVALID_TYPE", ex.Message);
        }

        [Fact]
        public async Task Handle_DeveRetornarRespostaDoCache_QuandoIdempotenciaExiste()
        {
            var command = new CriarMovimentoCommand { ChaveIdempotencia = "123", ContaCorrenteId = "abc", TipoMovimento = 'C', Valor = 100.00m };
            var movimentoId = Guid.NewGuid().ToString();
            var movimentoIdJson = Newtonsoft.Json.JsonConvert.SerializeObject(movimentoId);

            _idempotenciaRepository.ConsultarAsync(command.ChaveIdempotencia).Returns(Task.FromResult(movimentoIdJson));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(movimentoId, result);
            await _contaCorrenteRepository.DidNotReceive().MovimentarSaldoAsync(Arg.Any<Movimento>());
        }
    }
}

