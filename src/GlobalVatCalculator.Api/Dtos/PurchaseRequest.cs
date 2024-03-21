namespace GlobalVatCalculator.Api.Dtos
{
    public class PurchaseRequest
    {
        public string NetAmount { get; set; }
        public string GrossAmount { get; set; }
        public string VatAmount { get; set; }
        public string VatRate { get; set; }
    }
}
