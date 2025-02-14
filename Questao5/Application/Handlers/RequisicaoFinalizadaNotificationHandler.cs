using MediatR;
using Questao5.Application.Events;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Repositories.Interfaces;

namespace Questao5.Application.Handlers.RequisicaoFinalizadaNotification
{
    public class RequisicaoFinalizadaNotificationHandler : INotificationHandler<Events.RequisicaoFinalizadaNotification>
    {
        private readonly ISqliteIdempotenciaRepository _sqliteIdempotenciaRepository;
        public RequisicaoFinalizadaNotificationHandler(ISqliteIdempotenciaRepository sqliteIdempotenciaRepository)
        {
            _sqliteIdempotenciaRepository = sqliteIdempotenciaRepository;
        }

        public async Task Handle(Events.RequisicaoFinalizadaNotification notification, CancellationToken cancellationToken)
        {
            await _sqliteIdempotenciaRepository.RegistrarAsync(
                chaveIdempotencia: notification.Idempotencia,
                requisicaoJson: notification.Requisicao,
                resultadoJson: notification.Resposta);
        }
    }
}
