using GlobalVatCalculator.Api.Models;
using System.Diagnostics.CodeAnalysis;

namespace GlobalVatCalculator.Api.Tests.Unit.Models
{
    public class PurchaseTests
    {
        public readonly Fixture _fixture;

        public PurchaseTests()
        {
            _fixture = new Fixture();
        }

        [ExcludeFromCodeCoverage]
        public class TheoryParam()
        {
            public decimal NetAmount { get; set; }
            public decimal GrossAmount { get; set; }
            public decimal VatAmount { get; set; }
            public decimal VatRate { get; set; }
            public decimal NetExpected { get; set; }
            public decimal GrossExpected { get; set; }
            public decimal VatExpected { get; set; }
        }

        public static IEnumerable<object[]> GetInvalidRequest()
        {
            return new List<object[]>
            {
                new object[]
                {
                    new TheoryParam
                    {
                        NetAmount = 150.00m,
                        GrossAmount = 0m,
                        VatAmount = 0m,
                        VatRate = 0.10m,
                        NetExpected = 150.00m,
                        GrossExpected = 165.00m,
                        VatExpected = 15.0m,
                    },
                },
                new object[]
                {
                    new TheoryParam
                    {
                        NetAmount = 0m,
                        GrossAmount = 123.45m,
                        VatAmount = 0m,
                        VatRate = 0.20m,
                        NetExpected = 102.88m,
                        GrossExpected = 123.45m,
                        VatExpected = 20.57m,
                    },
                },
                new object[]
                {
                    new TheoryParam
                    {
                        NetAmount = 0m,
                        GrossAmount = 0m,
                        VatAmount = 25.60m,
                        VatRate = 0.13m,
                        NetExpected = 196.92m,
                        GrossExpected = 222.52m,
                        VatExpected = 25.60m,
                    },
                },
            };
        }

        [Fact]
        public void Create_WithNegativeValues_ThrowException()
        {
            // Arrange
            var net = -10.0m;
            var gross = 0m;
            var vat = 0m;
            var rate = 0.20m;

            // Act
            Action act = () => { _ = Purchase.Create(net, gross, vat, rate); };

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Amounts cannot be negative.");
        }

        [Fact]
        public void Create_WithValidValues_CreateWithSuccess()
        {
            // Arrange
            var net = 10.0m;
            var gross = 0m;
            var vat = 0m;
            var rate = 0.20m;

            // Act
            var resultPurchaseCreation = Purchase.Create(net, gross, vat, rate);

            // Assert
            resultPurchaseCreation.NetAmount.Should().Be(net);
            resultPurchaseCreation.GrossAmount.Should().Be(gross);
            resultPurchaseCreation.VatAmount.Should().Be(vat);
            resultPurchaseCreation.VatRate.Should().Be(rate);
        }

        [Fact]
        public void Validate_WithMoreThanOneAmountSet_ThrowException()
        {
            // Arrange
            var net = 10.0m;
            var gross = 10.0m;
            var vat = 10.0m;
            var rate = 0.20m;

            var purchase = Purchase.Create(net, gross, vat, rate);

            // Act
            Action act = () => { purchase.Validate(); };

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Purchase model is not in a valid state");
        }

        [Fact]
        public void Validate_WithValidValues_ReturnsTrue()
        {
            // Arrange
            var net = 10.0m;
            var gross = 0.0m;
            var vat = 0.0m;
            var rate = 0.20m;

            var purchase = Purchase.Create(net, gross, vat, rate);

            // Act
            var validateResult = purchase.Validate();

            // Assert
            validateResult.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInvalidRequest))]
        public void CalculateTaxes_WithListOfValues_ReturnsExpectedResults(TheoryParam theoryParam)
        {
            // Assert
            var purchase = Purchase.Create(
                theoryParam.NetAmount,
                theoryParam.GrossAmount,
                theoryParam.VatAmount,
                theoryParam.VatRate);

            // Act
            _ = purchase.CalculateTaxes();

            // Assert
            purchase.NetAmount.Should().Be(theoryParam.NetExpected);
            purchase.GrossAmount.Should().Be(theoryParam.GrossExpected);
            purchase.VatAmount.Should().Be(theoryParam.VatExpected);
        }
    }
}
