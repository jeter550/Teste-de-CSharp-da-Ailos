using MediatR;

namespace Questao5.Application.Commands.Requests
{
    public class CriarContaCorrenteCommand : IRequest<bool>
    {
        public int Numero { get; set; }
        public string Nome { get; set; }
    }
}
