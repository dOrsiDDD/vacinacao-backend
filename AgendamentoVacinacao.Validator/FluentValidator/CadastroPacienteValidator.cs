using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Utilities.Messages;
using FluentValidation;

namespace AgendamentoVacinacao.Validator.FluentValidator
{
    public class CadastroPacienteValidator : AbstractValidator<CadastroPacienteModel>
    {
        public CadastroPacienteValidator()
        {
            RuleFor(paciente => paciente.nome)
                .NotNull()
                .WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Nome do paciente"))
                .NotEmpty()
                .WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Nome do paciente"));

            RuleFor(paciente => paciente.dataNascimento)
                .NotEmpty()
                .WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Data de nascimento"))
                .LessThan(DateOnly.FromDateTime(DateTime.Now))
                .WithMessage(string.Format(BusinessMessages.DataNascimentoFuturo))
                .GreaterThanOrEqualTo(new DateOnly(1900, 1, 1))
                .WithMessage(string.Format(BusinessMessages.DataNascimentoInvalida));
        }
    }
}
