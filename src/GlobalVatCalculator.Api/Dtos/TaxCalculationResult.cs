namespace GlobalVatCalculator.Api.Dtos
{
    public class TaxCalculationResult
    {
        public bool IsSuccessStatusCode { get; set; }
        public string Reason { get; set; }
        public PurchaseResponse PurchaseResponse { get; set; }
    }
}
