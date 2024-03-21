namespace GlobalVatCalculator.Api.Dtos
{
    public class PurchaseResponse
    {
        public decimal NetAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal VatAmount { get; set; }
    }
}
