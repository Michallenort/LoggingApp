using BackendApp.Entities;
using BackendApp.Models;
using FluentValidation;

namespace BackendApp.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator(AccountDbContext dbContext)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(25);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(25);
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .MinimumLength(8);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password);

        RuleFor(x => x.Email)
            .Custom((value, context) =>
            {
                var isEmailExists = dbContext.Users.Any(x => x.Email == value);
                if (isEmailExists)
                {
                    context.AddFailure("Email", "That email is already exists!");
                }
            });


    }
}