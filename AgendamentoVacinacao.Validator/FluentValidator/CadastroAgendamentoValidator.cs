using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Utilities.Messages;
using FluentValidation;

namespace AgendamentoVacinacao.Validator.FluentValidator
{
    public class CadastroAgendamentoValidator : AbstractValidator<CadastroAgendamentoModel>
    {
        public CadastroAgendamentoValidator()
        {
            RuleFor(agendamento => agendamento.idPaciente)
                .NotEmpty()
                .WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "ID do paciente"))
                .GreaterThan(0)
                .WithMessage(agendamento => string.Format(BusinessMessages.IdInvalido, agendamento.idPaciente));

            RuleFor(agendamento => agendamento.dataAgendamento)
                .NotEmpty()
                .WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Data do agendamento"))
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
                .WithMessage(string.Format(BusinessMessages.DataPassada));

            RuleFor(agendamento => agendamento.horaAgendamento)
                .NotEmpty()
                .WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Hora do agendamento"))
                .Custom((hora, context) =>
                {
                    if (hora.Minute != 0)
                    {
                        context.AddFailure(string.Format(BusinessMessages.HorarioInvalido, hora));
                    }
                });
        }
    }
}
