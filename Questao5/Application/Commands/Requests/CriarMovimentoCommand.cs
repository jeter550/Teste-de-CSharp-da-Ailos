using MediatR;

namespace Questao5.Application.Commands.Requests
{
    public class CriarMovimentoCommand : IRequest<string>
    {
        public string ChaveIdempotencia { get; set; }
        public string ContaCorrenteId { get; set; }
        public char TipoMovimento { get; set; }
        public decimal Valor { get; set; }
    }

}
