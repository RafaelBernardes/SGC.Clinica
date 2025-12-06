using MediatR;
using SGC.Clinica.Api.Domain.Events;

namespace SGC.Clinica.Api.Application.EventHandlers
{
    public class SendWelcomeEmailHandler : INotificationHandler<PatientCreatedEvent>
    {
        private readonly ILogger<SendWelcomeEmailHandler> _logger;

        public SendWelcomeEmailHandler(ILogger<SendWelcomeEmailHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(PatientCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[EVENTO DISPARADO] 📧 Enviando e-mail de boas-vindas para o paciente: {notification.Patient.Name}");
            return Task.CompletedTask;
        }
    }
}