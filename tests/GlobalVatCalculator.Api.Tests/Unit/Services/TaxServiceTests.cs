using GlobalVatCalculator.Api.Dtos;
using GlobalVatCalculator.Api.Services;

namespace GlobalVatCalculator.Api.Tests.Unit.Services
{
    public class TaxServiceTests
    {
        private readonly Fixture _fixture;
        private readonly TaxService _taxService;

        public TaxServiceTests()
        {
            _fixture = new Fixture();
            _taxService = new TaxService();
        }

        [Fact]
        public async Task CalculateTaxesAmountAsync_WithValidationError_ReturnsTaxCalculationResultWithError()
        {
            // Arrange
            var purchaseRequest = _fixture.Build<PurchaseRequest>()
                .With(p => p.NetAmount, "100")
                .With(p => p.GrossAmount, "100")
                .With(p => p.VatAmount, "100")
                .With(p => p.VatRate, ((int)VatRate.ThirteenPct).ToString())
                .Create();

            // Act
            var result = await _taxService.CalculateTaxesAmountAsync(purchaseRequest);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccessStatusCode.Should().BeFalse();
            result.Reason.Should().Be("Purchase model is not in a valid state");
            result.PurchaseResponse.Should().BeNull();
        }

        [Fact]
        public async Task CalculateTaxesAmountAsync_WithValidRequest_ReturnsTaxCalculationResultWithSuccess()
        {
            // Arrange
            var purchaseRequest = _fixture.Build<PurchaseRequest>()
                .With(p => p.NetAmount, 100.0m.ToString())
                .With(p => p.GrossAmount, 0.0m.ToString())
                .With(p => p.VatAmount, 0.0m.ToString())
                .With(p => p.VatRate, ((int)VatRate.ThirteenPct).ToString())
                .Create();

            // Act
            var result = await _taxService.CalculateTaxesAmountAsync(purchaseRequest);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccessStatusCode.Should().BeTrue();
            result.Reason.Should().BeNull();
            result.PurchaseResponse.Should().NotBeNull();
        }
    }
}
