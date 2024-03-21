using GlobalVatCalculator.Api.Dtos;
using GlobalVatCalculator.Api.Mappers;
using GlobalVatCalculator.Api.Models;

namespace GlobalVatCalculator.Api.Tests.Unit.Mappers
{
    public class PurchaseMapperTests
    {
        public readonly Fixture _fixture;

        public PurchaseMapperTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void ToDto_Null_ReturnsNull()
        {
            // Arrange
            var originalPurchase = default(Purchase);

            // Act
            var resultPurchaseResponse = originalPurchase.ToDto();

            // Assert
            resultPurchaseResponse.Should().BeNull();
        }

        [Fact]
        public void ToDto_WithValidParameters_ReturnsValidResponse()
        {
            // Arrange
            var originalPurchase = _fixture.Create<Purchase>();

            // Act
            var resultPurchaseResponse = originalPurchase.ToDto();

            // Assert
            resultPurchaseResponse.Should().NotBeNull();
            resultPurchaseResponse.NetAmount.Should().Be(originalPurchase.NetAmount);
            resultPurchaseResponse.GrossAmount.Should().Be(originalPurchase.GrossAmount);
            resultPurchaseResponse.VatAmount.Should().Be(originalPurchase.VatAmount);
        }

        [Fact]
        public void ToModel_Null_ReturnsNull()
        {
            // Arrange
            var originalPurchaseRequest = default(PurchaseRequest);

            // Act
            var resultPurchase = originalPurchaseRequest.ToModel();

            // Assert
            resultPurchase.Should().BeNull();
        }

        [Fact]
        public void ToModel_WithValidParameters_ReturnsValidResponse()
        {
            // Arrange
            var originalPurchaseRequest = _fixture.Build<PurchaseRequest>()
                    .With(p => p.NetAmount, _fixture.Create<decimal>().ToString())
                    .With(p => p.GrossAmount, _fixture.Create<decimal>().ToString())
                    .With(p => p.VatAmount, _fixture.Create<decimal>().ToString())
                    .With(p => p.VatRate, ((int)_fixture.Create<VatRate>()).ToString())
                    .Create();

            // Act
            var resultPurchase = originalPurchaseRequest.ToModel();

            // Assert
            resultPurchase.Should().NotBeNull();
            resultPurchase.NetAmount.Should().Be(Convert.ToDecimal(originalPurchaseRequest.NetAmount));
            resultPurchase.GrossAmount.Should().Be(Convert.ToDecimal(originalPurchaseRequest.GrossAmount));
            resultPurchase.VatAmount.Should().Be(Convert.ToDecimal(originalPurchaseRequest.VatAmount));
            resultPurchase.VatRate.Should().Be(Convert.ToDecimal(originalPurchaseRequest.VatRate));
        }
    }
}
