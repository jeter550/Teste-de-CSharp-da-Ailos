using MediatR;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Queries.Requests
{
    public class ConsultarSaldoQuery : IRequest<SaldoResponse>
    {
        public string IdRequisicao { get; set; }
        public string ContaCorrenteId { get; set; }
    }
}
