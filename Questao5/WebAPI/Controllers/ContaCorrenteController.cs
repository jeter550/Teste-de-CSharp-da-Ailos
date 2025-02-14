using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Events;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Questao5.WebAPI.Controllers
{
    [Route("api/contacorrente")]
    [ApiController]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContaCorrenteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria um movimento (Crédito ou Débito) em uma conta corrente.
        /// </summary>
        /// <param name="command">Comando contendo os detalhes do movimento a ser criado (ID da conta, valor, tipo de movimento, etc.).</param>
        /// <returns>Retorna o ID do movimento criado.</returns>
        /// <response code="200">Movimento criado com sucesso, retornando o ID do movimento.</response>
        /// <response code="400">Se ocorrer um erro ao criar o movimento, retorna uma mensagem de erro.</response>
        [HttpPost("movimento")]
        [SwaggerOperation(Summary = "Cria um movimento na conta corrente", Description = "Este método cria um movimento (crédito ou débito) em uma conta corrente com base no comando enviado.")]
        [SwaggerResponse(200, "Movimento criado com sucesso", typeof(object))]
        [SwaggerResponse(400, "Erro ao criar o movimento", typeof(object))]
        public async Task<IActionResult> CriarMovimento([FromBody] CriarMovimentoCommand command)
        {
            try
            {
                var movimentoId = await _mediator.Send(command);
                _mediator.Publish(new RequisicaoFinalizadaNotification(command.ChaveIdempotencia, movimentoId, movimentoId));
                return Ok(new { MovimentoId = movimentoId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Consulta o saldo de uma conta corrente.
        /// </summary>
        /// <param name="contaCorrenteId">ID da conta corrente para consulta.</param>
        /// <param name="chaveIdempotencia">Chave para garantir idempotência na requisição.</param>
        /// <returns>Retorna o saldo da conta corrente, incluindo detalhes adicionais como o nome da conta e a data/hora da resposta.</returns>
        /// <response code="200">Retorna o saldo da conta corrente.</response>
        /// <response code="400">Se ocorrer um erro na consulta do saldo, retorna uma mensagem de erro.</response>
        [HttpGet("saldo/{contaCorrenteId}")]
        [SwaggerOperation(Summary = "Consulta o saldo da conta corrente", Description = "Consulta o saldo atual de uma conta corrente com base no ID da conta e chave de idempotência.")]
        [SwaggerResponse(200, "Saldo da conta corrente consultado com sucesso", typeof(SaldoResponse))]
        [SwaggerResponse(400, "Erro ao consultar o saldo", typeof(object))]
        public async Task<IActionResult> ConsultarSaldo(
            [FromRoute] string contaCorrenteId,
            [FromHeader] string chaveIdempotencia)
        {
            try
            {
                var query = new ConsultarSaldoQuery { ContaCorrenteId = contaCorrenteId, IdRequisicao = chaveIdempotencia };
                var saldoResponse = await _mediator.Send(query);
                _mediator.Publish(new RequisicaoFinalizadaNotification(chaveIdempotencia, query, saldoResponse));
                return Ok(saldoResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

    }
}
