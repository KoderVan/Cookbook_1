using Cookbook_1.Contracts;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;

namespace Cookbook_1.Services.Validators
{
    public class SignUpValidator : AbstractValidator<LogInUserDto>
    {
        private const int _loginMaxLength = 128;
        private const int _passwordMaxLength = 256;

        //в конструкторе описываем правила для проверки
        public SignUpValidator()
        {
            
            //выбираем свойство
            RuleFor(createDto => createDto.Login)
                .NotNull()
                .NotEmpty()
                .MaximumLength(_loginMaxLength);

            RuleFor(createDto => createDto.Password)
                .NotNull()
                .NotEmpty()
                .MaximumLength(_passwordMaxLength);
        }
    }
}
