namespace GlobalVatCalculator.Api.Models
{
    public class Purchase
    {
        private decimal _netAmount;
        private decimal _grossAmount;
        private decimal _vatAmount;
        private decimal _vatRate;

        public decimal NetAmount => Math.Round(_netAmount, 2, MidpointRounding.ToEven);
        public decimal GrossAmount => Math.Round(_grossAmount, 2, MidpointRounding.ToEven);
        public decimal VatAmount => Math.Round(_vatAmount, 2, MidpointRounding.ToEven);
        public decimal VatRate => Math.Round(_vatRate, 2, MidpointRounding.ToEven);

        private Purchase(decimal netAmount, decimal grossAmount, decimal vatAmount, decimal vatRate)
        {
            _netAmount = netAmount;
            _grossAmount = grossAmount;
            _vatAmount = vatAmount;
            _vatRate = vatRate;
        }

        public static Purchase Create(decimal netAmount, decimal grossAmount, decimal vatAmount, decimal vatRate)
        {
            if (netAmount < 0 || grossAmount < 0 || vatAmount < 0)
            {
                throw new ArgumentException("Amounts cannot be negative.");
            }

            return new Purchase(netAmount, grossAmount, vatAmount, vatRate);
        }

        public bool Validate()
        {
            if (!IsValid())
            {
                throw new ArgumentException("Purchase model is not in a valid state");
            }

            return true;
        }

        public bool CalculateTaxes()
        {
            if (IsValid())
            {
                bool netAmountSet = NetAmount != default;
                bool grossAmountSet = GrossAmount != default;
                bool vatAmountSet = VatAmount != default;

                if (netAmountSet)
                {
                    _grossAmount = NetAmount * (1 + VatRate);
                    _vatAmount = GrossAmount - NetAmount;
                }

                if (grossAmountSet)
                {
                    _netAmount = GrossAmount / (1 + VatRate);
                    _vatAmount = GrossAmount - NetAmount;
                }

                if (vatAmountSet)
                {
                    _netAmount = VatAmount / VatRate;
                    _grossAmount = NetAmount + VatAmount;
                }
            }

            return true;
        }

        private bool IsValid()
        {
            bool netAmountSet = NetAmount != default;
            bool grossAmountSet = GrossAmount != default;
            bool vatAmountSet = VatAmount != default;

            if (VatRate == default)
            {
                return false;
            }

            if (!netAmountSet && !grossAmountSet && !vatAmountSet)
            {
                return false;
            }

            int countSet = (netAmountSet ? 1 : 0) + (grossAmountSet ? 1 : 0) + (vatAmountSet ? 1 : 0);

            return countSet == 1;
        }
    }
}
