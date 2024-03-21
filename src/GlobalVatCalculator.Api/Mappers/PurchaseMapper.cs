using GlobalVatCalculator.Api.Dtos;
using GlobalVatCalculator.Api.Models;

namespace GlobalVatCalculator.Api.Mappers
{
    public static class PurchaseMapper
    {
        private static readonly Dictionary<VatRate, decimal> vatRate = new Dictionary<VatRate, decimal>()
        {
            { VatRate.NotSet, default },
            { VatRate.TenPct, 0.10m },
            { VatRate.ThirteenPct, 0.13m },
            { VatRate.TwentyPct, 0.20m },
        };

        public static PurchaseResponse ToDto(
            this Purchase purchase)
        {
            if (purchase is null)
            {
                return null;
            }

            return new PurchaseResponse()
            {
                GrossAmount = purchase.GrossAmount,
                NetAmount = purchase.NetAmount,
                VatAmount = purchase.VatAmount,
            };
        }

        public static Purchase ToModel(
            this PurchaseRequest purchaseRequest)
        {
            if (purchaseRequest is null)
            {
                return null;
            }

            return Purchase.Create(
                Convert.ToDecimal(purchaseRequest.NetAmount),
                Convert.ToDecimal(purchaseRequest.GrossAmount),
                Convert.ToDecimal(purchaseRequest.VatAmount),
                vatRate[(VatRate)Convert.ToInt32(purchaseRequest.VatRate)]);
        }
    }
}
