using FluentValidation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.ID).GreaterThan(0).WithMessage("ID must be greater than 0");
        RuleFor(user => user.UserName).NotEmpty().Length(3, 50).WithMessage("UserName must be between 3 and 50 characters");
        RuleFor(user => user.UserAge).InclusiveBetween(0, 120).WithMessage("UserAge must be between 0 and 120");
    }
}