using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System;
using TrilhaNetAzureDesafio.ViewModels;

namespace TrilhaNetAzureDesafio.Validation
{
    public class AddFuncionarioValidation : AbstractValidator<AddFuncionarioViewModel>
    {
        public AddFuncionarioValidation()
        {
            RuleFor(e => e.Nome).NotNull().NotEmpty().WithMessage("O campo Nome deve ser informado.");
            RuleFor(e => e.Nome).Length(5, 50).WithMessage("O campo Nome deve ter entre 5 e 50 caracteres.");

            RuleFor(e => e.Endereco).NotNull().NotEmpty().WithMessage("O campo Endereço deve ser informado.");
            RuleFor(e => e.Endereco).Length(5, 100).WithMessage("O campo Endereço deve ter entre 5 e 100 caracteres.");

            RuleFor(e => e.Ramal).NotNull().NotEmpty().WithMessage("O campo Ramal deve ser informado.");
            RuleFor(e => e.Ramal).Length(2, 30).WithMessage("O campo Ramal deve ter entre 2 e 30 caracteres.");

            RuleFor(e => e.Departamento).NotNull().NotEmpty().WithMessage("O campo Departamento deve ser informado.");
            RuleFor(e => e.Departamento).Length(5, 30).WithMessage("O campo Departamento deve ter entre 5 e 30 caracteres.");

            RuleFor(e => e.EmailProfissional).NotNull().NotEmpty().WithMessage("O campo Email Profissional deve ser informado.");
            RuleFor(e => e.EmailProfissional).MaximumLength(100).WithMessage("O campo Email Profissional deve ter no máximo 100 caracteres.");
            RuleFor(e => e.EmailProfissional).EmailAddress().WithMessage("Email Profissional informado inválido.");

            RuleFor(e => e.Salario).NotNull().NotEmpty().WithMessage("O campo Salário deve ser informado.");

            RuleFor(e => e.DataAdmissao).NotNull().NotEmpty().WithMessage("O campo Data admissão deve ser informado.");
        }
    }
}
