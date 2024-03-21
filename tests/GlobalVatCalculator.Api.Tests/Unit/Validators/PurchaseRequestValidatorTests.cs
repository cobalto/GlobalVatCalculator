using GlobalVatCalculator.Api.Dtos;
using GlobalVatCalculator.Api.Validators;

namespace GlobalVatCalculator.Api.Tests.Unit.Validators
{
    public class PurchaseRequestValidatorTests
    {
        private readonly PurchaseRequestValidator validator;
        private readonly Fixture fixture;

        public PurchaseRequestValidatorTests()
        {
            validator = new PurchaseRequestValidator();
            fixture = new Fixture();
        }

        [Fact]
        public void Validate_NewObject_ReturnsError()
        {
            // Arrange
            var request = new PurchaseRequest();

            // Act
            var results = validator.Validate(request);

            // Assert
            results.Errors.Should().HaveCount(5);
        }

        [Fact]
        public void Validate_AmountNotInDecimal_ReturnsError()
        {
            // Arrange
            var request = new PurchaseRequest()
            {
                NetAmount = "Net",
                GrossAmount = "0",
                VatAmount = "0",
                VatRate = "2",
            };

            // Act
            var results = validator.Validate(request);

            // Assert
            results.Errors.Should().HaveCount(2);
            results.Errors[0].ErrorMessage.Should().Be("Net Amount must be a valid decimal");
        }

        [Fact]
        public void Validate_MoreThanOneAmountSet_ReturnsError()
        {
            // Arrange
            var request = new PurchaseRequest()
            {
                NetAmount = "100",
                GrossAmount = "100",
                VatAmount = "0",
                VatRate = "2",
            };

            // Act
            var results = validator.Validate(request);

            // Assert
            results.Errors.Should().HaveCount(1);
            results.Errors[0].ErrorMessage.Should().Be("One of NetAmount, GrossAmount or VatAmount must have a value");
        }

        [Fact]
        public void Validate_InvalidVatRate_ReturnsError()
        {
            // Arrange
            var request = new PurchaseRequest()
            {
                NetAmount = "100",
                GrossAmount = "0",
                VatAmount = "0",
                VatRate = "0",
            };

            // Act
            var results = validator.Validate(request);

            // Assert
            results.Errors.Should().HaveCount(1);
            results.Errors[0].ErrorMessage.Should().Be("VAT Rate must be a valid value");
        }

        [Fact]
        public void Validate_EmptyVatRate_ReturnsError()
        {
            // Arrange
            var request = new PurchaseRequest()
            {
                NetAmount = "100",
                GrossAmount = "0",
                VatAmount = "0",
                VatRate = string.Empty,
            };

            // Act
            var results = validator.Validate(request);

            // Assert
            results.Errors.Should().HaveCount(1);
            results.Errors[0].ErrorMessage.Should().Be("VAT Rate must be a valid value");
        }

        [Fact]
        public void Validate_MissingVatRate_ReturnsError()
        {
            // Arrange
            var request = new PurchaseRequest()
            {
                NetAmount = "100",
                GrossAmount = "0",
                VatAmount = "0",
            };

            // Act
            var results = validator.Validate(request);

            // Assert
            results.Errors.Should().HaveCount(1);
            results.Errors[0].ErrorMessage.Should().Be("VAT Rate cannot be null");
        }

        [Fact]
        public void Validate_ValidRequest_ReturnsZeroErrors()
        {
            // Arrange
            var request = new PurchaseRequest()
            {
                NetAmount = "100",
                GrossAmount = "0",
                VatAmount = "0",
                VatRate = "2",
            };

            // Act
            var results = validator.Validate(request);

            // Assert
            results.Errors.Should().HaveCount(0);
        }
    }
}
