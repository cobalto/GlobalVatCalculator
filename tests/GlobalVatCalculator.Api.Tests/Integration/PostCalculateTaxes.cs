using GlobalVatCalculator.Api.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;

namespace GlobalVatCalculator.Api.Tests.Integration
{
    public class PostCalculateTaxes : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PostCalculateTaxes(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CalculateTaxesAsync_WithValidParams_ReturnsCalculatedTaxesWithSuccess()
        {
            // Arrange
            var requestUrl = $"/api/v1/purchase/taxes";

            var request = new PurchaseRequest()
            {
                NetAmount = 15.67m.ToString(),
                GrossAmount = 0.0m.ToString(),
                VatAmount = 0.0m.ToString(),
                VatRate = ((int)VatRate.TwentyPct).ToString(),
            };

            string jsonString = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(requestUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            var purchaseResult = JsonSerializer.Deserialize<PurchaseResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            purchaseResult.Should().NotBeNull();
            purchaseResult.NetAmount.Should().Be(15.67m);
            purchaseResult.GrossAmount.Should().Be(18.80m);
            purchaseResult.VatAmount.Should().Be(3.13m);
        }
    }
}
