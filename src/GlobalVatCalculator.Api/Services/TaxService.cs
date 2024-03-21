using GlobalVatCalculator.Api.Dtos;
using GlobalVatCalculator.Api.Interfaces;
using GlobalVatCalculator.Api.Mappers;

namespace GlobalVatCalculator.Api.Services
{
    public class TaxService : ITaxService
    {
        public Task<TaxCalculationResult> CalculateTaxesAmountAsync(PurchaseRequest purchaseRequest)
        {
            try
            {
                var purchase = purchaseRequest.ToModel();

                _ = purchase.Validate();

                _ = purchase.CalculateTaxes();

                return Task.FromResult(new TaxCalculationResult()
                {
                    IsSuccessStatusCode = true,
                    PurchaseResponse = purchase.ToDto(),
                });

            }
            catch(Exception ex)
            {
                return Task.FromResult(
                    new TaxCalculationResult() 
                    { 
                        IsSuccessStatusCode = false,
                        Reason = ex.Message,
                    });
            }
        }
    }
}
