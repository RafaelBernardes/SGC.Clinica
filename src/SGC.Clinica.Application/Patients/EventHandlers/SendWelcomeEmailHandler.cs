using MediatR;
using SGC.Clinica.Domain.Events.Patients;

namespace SGC.Clinica.Application.Patients.EventHandlers
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
            _logger.LogInformation(
                "[EVENTO DISPARADO] Enviando e-mail de boas-vindas para o paciente: {PatientName} ({Email})",
                notification.PatientName,
                notification.Email);

            return Task.CompletedTask;
        }
    }
}
