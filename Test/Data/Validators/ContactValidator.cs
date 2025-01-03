using FluentValidation;
using System.Globalization;
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

            RuleFor(c => c.Phone)
                .MaximumLength(15);

            RuleFor(c => c.Salary)
                .LessThan(9999999);
        }
    }
}
