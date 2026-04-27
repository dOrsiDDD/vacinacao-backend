using AgendamentoVacinacao.Entities.Enum;
using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Utilities.Messages;
using FluentValidation;

namespace AgendamentoVacinacao.Validator.FluentValidator
{
    public class CadastroUsuarioValidator : AbstractValidator<CadastroUsuarioModel>
    {
        public CadastroUsuarioValidator()
        {
            RuleFor(t => t.nome)
                .NotNull().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Nome"))
                .NotEmpty().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Nome"));

            RuleFor(t => t.email).EmailAddress().WithMessage(string.Format(BusinessMessages.CampoInvalido, "Email"))
                .NotNull().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Email"))
                .NotEmpty().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Email"));

            RuleFor(t => t.perfil)
                .Must(perfil => Enum.IsDefined(typeof(PerfilEnum), perfil)).WithMessage(string.Format(BusinessMessages.CampoInvalido, "Perfil"))
                .NotNull().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Perfil"));
        }
    }
}
