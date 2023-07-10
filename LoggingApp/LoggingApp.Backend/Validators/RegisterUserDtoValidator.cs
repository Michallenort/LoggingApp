using LoggingApp.Shared.Models;
using FluentValidation;
using LoggingApp.Backend.Entities;

namespace LoggingApp.Backend.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator(AccountDbContext dbContext)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name should not be empty.")
            .MinimumLength(2)
            .WithMessage("First name should have more than 1 char.")
            .MaximumLength(25)
            .WithMessage("First name should have less than 25 chars.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name should not be empty.")
            .MinimumLength(2)
            .WithMessage("Last name should have more than 1 char.")
            .MaximumLength(25)
            .WithMessage("Last name should have less than 25 chars.");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email should not be empty.")
            .EmailAddress()
            .WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .MinimumLength(8)
            .WithMessage("Password should have more than 8 chars.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords are different.");

        RuleFor(x => x.Email)
            .Custom((value, context) =>
            {
                var isEmailExists = dbContext.Users.Any(x => x.Email == value);
                if (isEmailExists)
                {
                    context.AddFailure("Email", "That email is already exists!");
                }
            });

        RuleFor(x => x.DateOfBirth)
            .Custom((value, context) =>
            {
                if (value >= DateTime.Today)
                {
                    context.AddFailure("DateOfBirth", "Date of birth should have place before today!");
                }
            });


    }
}