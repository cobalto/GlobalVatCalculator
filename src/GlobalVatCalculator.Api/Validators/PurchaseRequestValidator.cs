using FluentValidation;
using GlobalVatCalculator.Api.Dtos;

namespace GlobalVatCalculator.Api.Validators
{
    public class PurchaseRequestValidator : AbstractValidator<PurchaseRequest>
    {
        public PurchaseRequestValidator()
        {
            RuleFor(pr => pr.VatRate)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                    .WithMessage("VAT Rate cannot be null")
                 .Must(BeValidEnum)
                    .WithMessage("VAT Rate must be a valid value"); ;

            RuleFor(pr => pr.VatAmount)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                    .WithMessage("VAT Amount cannot be null")
                .Must(BeValidDecimal)
                    .WithMessage("VAT Amount must be a valid decimal");

            RuleFor(pr => pr.NetAmount)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                    .WithMessage("Net Amount cannot be null")
                .Must(BeValidDecimal)
                    .WithMessage("Net Amount must be a valid decimal");

            RuleFor(pr => pr.GrossAmount)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                    .WithMessage("Gross Amount cannot be null")
                .Must(BeValidDecimal)
                    .WithMessage("Gross Amount must be a valid decimal");

            RuleFor(x => x)
                .Must(HaveExactlyOneAmount)
                    .WithMessage("One of NetAmount, GrossAmount or VatAmount must have a value");
        }

        private bool BeValidEnum(string value)
        {
            var isValidType = Enum.TryParse(typeof(VatRate), value, out object parsedValue);

            if (parsedValue == null) return false;

            var isWithinRange = Enum.IsDefined(typeof(VatRate), parsedValue);
            var isValidValue = (VatRate)parsedValue != default;

            return isValidType && isWithinRange && isValidValue;
        }

        private bool BeValidDecimal(string value)
        {
            return decimal.TryParse(value, out _);
        }

        private bool HaveExactlyOneAmount(PurchaseRequest request)
        {
            _ = decimal.TryParse(request.NetAmount, out decimal netAmount);
            _ = decimal.TryParse(request.GrossAmount, out decimal grossAmount);
            _ = decimal.TryParse(request.VatAmount, out decimal vatAmount);

            int count = 0;

            if (netAmount != default) count++;
            if (grossAmount != default) count++;
            if (vatAmount != default) count++;

            return count == 1;
        }
    }
}
