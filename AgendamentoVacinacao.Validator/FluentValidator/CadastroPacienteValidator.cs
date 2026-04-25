using AgendamentoVacinacao.Entities.Entities;
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

            RuleFor(paciente => paciente.cpf)
                .NotEmpty()
                .WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "CPF"))
                .Length(11)
                .WithMessage(string.Format(BusinessMessages.CPFInvalido))
                .Matches("^[0-9]+$").WithMessage(string.Format(BusinessMessages.ApenasNumeros, "CPF"))
                .Must(ValidarCpfMatematicamente).WithMessage(string.Format(BusinessMessages.CPFInvalido));

            RuleFor(paciente => paciente.dataNascimento)
                .NotEmpty()
                .WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Data de nascimento"))
                .LessThan(DateOnly.FromDateTime(DateTime.Now))
                .WithMessage(string.Format(BusinessMessages.DataNascimentoFuturo))
                .GreaterThanOrEqualTo(new DateOnly(1900, 1, 1))
                .WithMessage(string.Format(BusinessMessages.DataNascimentoInvalida));
        }

        private bool ValidarCpfMatematicamente(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            if (cpf.Distinct().Count() == 1)
                return false;

            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            }

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            }

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}
