using FluentValidation;
using System.Globalization;
using System.Text.RegularExpressions;
using Test.Data.Models;

namespace Test.Data.Validators
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator() 
        {
            RuleFor(c => c.Id)
                .NotNull()
                .NotEmpty();

            RuleFor(c => c.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(c => c.DateOfBirth)
                .LessThan(DateTime.ParseExact("01.01.2006", "dd.MM.yyyy", CultureInfo.InvariantCulture));

            RuleFor(p => p.Phone)
                .NotEmpty()
                .NotNull().WithMessage("Phone Number is required.")
                .MinimumLength(10).WithMessage("PhoneNumber must not be less than 10 characters.")
                .MaximumLength(20).WithMessage("PhoneNumber must not exceed 50 characters.");

            RuleFor(c => c.Salary)
                .LessThan(9999999);
        }
    }
}
