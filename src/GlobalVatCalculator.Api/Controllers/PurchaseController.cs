using FluentValidation.Results;
using GlobalVatCalculator.Api.Dtos;
using GlobalVatCalculator.Api.Interfaces;
using GlobalVatCalculator.Api.Validators;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;

namespace GlobalVatCalculator.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class PurchaseController : ControllerBase
    {
        private readonly ILogger<PurchaseController> _logger;
        private readonly ITaxService _taxService;

        public PurchaseController(ILogger<PurchaseController> logger, ITaxService taxService)
        {
            _logger = logger;
            _taxService = taxService;
        }

        [HttpPost, Route("taxes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(PurchaseResponse), 200)]
        public async Task<ActionResult<PurchaseResponse>> PostAsync(PurchaseRequest purchaseRequest)
        {
            var validator = new PurchaseRequestValidator();
            ValidationResult validationResult = validator.Validate(purchaseRequest);

            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join("; ", validationResult.Errors.Select(error => error.ErrorMessage));

                var problemDetails = LogAndReturnProblemDetails(
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.BadRequest.ToString(),
                    $"One or more validation errors occurred: {errorMessage}");

                return BadRequest(problemDetails);
            }

            try
            {
                var taxCalculationResult = await _taxService.CalculateTaxesAmountAsync(purchaseRequest);

                if (taxCalculationResult.IsSuccessStatusCode)
                {
                    return Ok(taxCalculationResult.PurchaseResponse);
                }
                else
                {
                    var problemDetails = LogAndReturnProblemDetails(
                        HttpStatusCode.InternalServerError,
                        HttpStatusCode.InternalServerError.ToString(),
                        $"An error occurred: {taxCalculationResult.Reason}");

                    return StatusCode((int)HttpStatusCode.InternalServerError, problemDetails);
                }
            }
            catch (Exception ex)
            {
                var problemDetails = LogAndReturnProblemDetails(
                    HttpStatusCode.InternalServerError,
                    HttpStatusCode.InternalServerError.ToString(),
                    $"An error occurred: {ex.Message}");

                return StatusCode((int)HttpStatusCode.InternalServerError, problemDetails);
            }
        }

        private ProblemDetails LogAndReturnProblemDetails(HttpStatusCode status, string title, string details)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)status,
                Title = title,
                Detail = details,
                Instance = HttpContext.Request.Path
            };

            _logger.LogError("An error code {Status} occurred: {Detail}", problemDetails.Status, problemDetails.Detail);

            return problemDetails;
        }
    }
}
