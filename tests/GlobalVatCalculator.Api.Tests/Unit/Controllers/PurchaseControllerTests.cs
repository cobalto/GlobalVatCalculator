using GlobalVatCalculator.Api.Controllers;
using GlobalVatCalculator.Api.Dtos;
using GlobalVatCalculator.Api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace GlobalVatCalculator.Api.Tests.Unit.Controllers
{
    public class PurchaseControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<PurchaseController>> _loggerMock;
        private readonly Mock<ITaxService> _taxServiceMock;
        private readonly PurchaseController _purchaseController;

        public PurchaseControllerTests()
        {
            _fixture = new Fixture();
            _loggerMock = new Mock<ILogger<PurchaseController>>();
            _taxServiceMock = new Mock<ITaxService>();

            var request = new Mock<HttpRequest>();
            request.SetupGet(r => r.Path).Returns("/api/v1/purchase/taxes");

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Request).Returns(request.Object);

            var controllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            _purchaseController = new PurchaseController(_loggerMock.Object, _taxServiceMock.Object)
            {
                ControllerContext = controllerContext
            };
        }

        [Fact]
        public async void PostAsync_ValidRequest_ReturnsPurchaseResponseWithSuccess()
        {
            // Arrange
            var purchaseRequest = new PurchaseRequest()
            {
                VatRate = VatRate.TenPct.ToString(),
                NetAmount = 10.0m.ToString(),
                GrossAmount = default(decimal).ToString(),
                VatAmount = default(decimal).ToString(),
            };

            var taxResult = _fixture.Build<TaxCalculationResult>()
                    .With(t => t.IsSuccessStatusCode, true)
                    .Without(t => t.Reason)
                    .Create();

            _taxServiceMock.Setup(s => s.CalculateTaxesAmountAsync(It.IsAny<PurchaseRequest>()))
                .ReturnsAsync(taxResult);

            // Act
            var result = await _purchaseController.PostAsync(purchaseRequest);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okObjectResult = result.Result as OkObjectResult;

            okObjectResult.Value.Should().NotBeNull().And.BeOfType<PurchaseResponse>();
            var purchaseResponseResult = okObjectResult.Value as PurchaseResponse;

            purchaseResponseResult.Should().NotBeNull();

            purchaseResponseResult.NetAmount.Should().Be(taxResult.PurchaseResponse.NetAmount);
            purchaseResponseResult.GrossAmount.Should().Be(taxResult.PurchaseResponse.GrossAmount);
            purchaseResponseResult.VatAmount.Should().Be(taxResult.PurchaseResponse.VatAmount);

            _taxServiceMock.Verify(s => s.CalculateTaxesAmountAsync(It.IsAny<PurchaseRequest>()), Times.Once);
        }

        [Fact]
        public async void PostAsync_VatRateValidationError_ReturnsBadRequestProblemDetails()
        {
            // Arrange
            var purchaseRequest = new PurchaseRequest()
            {
                VatRate = VatRate.NotSet.ToString(),
                NetAmount = 10.0m.ToString(),
                GrossAmount = default(decimal).ToString(),
                VatAmount = default(decimal).ToString(),
            };

            var taxResult = _fixture.Create<TaxCalculationResult>();

            _taxServiceMock.Setup(s => s.CalculateTaxesAmountAsync(It.IsAny<PurchaseRequest>()))
                .ReturnsAsync(taxResult);

            // Act
            var result = await _purchaseController.PostAsync(purchaseRequest);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();

            var badRequestObjectResult = result.Result as BadRequestObjectResult;

            badRequestObjectResult.Value.Should().NotBeNull().And.BeOfType<ProblemDetails>();
            var problemDetailsResult = badRequestObjectResult.Value as ProblemDetails;

            problemDetailsResult.Should().NotBeNull();

            problemDetailsResult.Status.Should().Be((int)HttpStatusCode.BadRequest);
            problemDetailsResult.Detail.Should().Contain("One or more validation errors occurred");

            _taxServiceMock.Verify(s => s.CalculateTaxesAmountAsync(It.IsAny<PurchaseRequest>()), Times.Never);
        }

        [Fact]
        public async void PostAsync_InternalServerError_ReturnsInternalServerErrorProblemDetails()
        {
            // Arrange
            var purchaseRequest = new PurchaseRequest()
            {
                VatRate = VatRate.TenPct.ToString(),
                NetAmount = 10.0m.ToString(),
                GrossAmount = default(decimal).ToString(),
                VatAmount = default(decimal).ToString(),
            };

            var serviceException = _fixture.Create<ArgumentException>();

            _taxServiceMock.Setup(s => s.CalculateTaxesAmountAsync(It.IsAny<PurchaseRequest>()))
                .ThrowsAsync(serviceException);

            // Act
            var result = await _purchaseController.PostAsync(purchaseRequest);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();

            var objectResult = result.Result as ObjectResult;

            objectResult.Value.Should().NotBeNull().And.BeOfType<ProblemDetails>();
            var problemDetailsResult = objectResult.Value as ProblemDetails;

            problemDetailsResult.Should().NotBeNull();

            problemDetailsResult.Status.Should().Be((int)HttpStatusCode.InternalServerError);
            problemDetailsResult.Detail.Should().Contain(serviceException.Message);

            _taxServiceMock.Verify(s => s.CalculateTaxesAmountAsync(It.IsAny<PurchaseRequest>()), Times.Once);
        }
    }
}