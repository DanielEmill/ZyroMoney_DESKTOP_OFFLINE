namespace ZyroMoney_DESKTOP.Models
{
    public class CreditCardSummary
    {
        public int CreditCardId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = "DOP";
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal AvailableCredit => CreditLimit - CurrentBalance;
        public int ClosingDay { get; set; }
        public int DueDay { get; set; }
        public string FormattedLimit => $"{CurrencyCode} {CreditLimit:N2}";
        public string FormattedBalance => $"{CurrencyCode} {CurrentBalance:N2}";
        public string FormattedAvailable => $"{CurrencyCode} {AvailableCredit:N2}";
        public string CycleInfo => $"Corte: Día {ClosingDay} • Pago: Día {DueDay}";
    }
}