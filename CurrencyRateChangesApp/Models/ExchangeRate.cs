using System;

namespace CurrencyRateChangesApp.Models
{
    public class ExchangeRate
    {
        public string Date { get; set; }
        public string Currency { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public string Unit { get; set; }
    }
}
