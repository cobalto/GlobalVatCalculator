using GlobalVatCalculator.Api.Dtos;

namespace GlobalVatCalculator.Api.Interfaces
{
    public interface ITaxService
    {
        Task<TaxCalculationResult> CalculateTaxesAmountAsync(PurchaseRequest purchaseRequest);
    }
}
