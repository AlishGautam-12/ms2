using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ms2.Service
{
    public class CurrencyExchangeService
    {
        private readonly Dictionary<(string, string), decimal> exchangeRates = new()
    {
        { ("USD", "NPR"), 132.50m }, // Example rate: 1 USD = 132.50 NPR
        { ("NPR", "USD"), 0.0075m }  // Example rate: 1 NPR = 0.0075 USD
    };

        public Task<string> ConvertAsync(string from, string to, decimal amount)
        {
            if (from == to)
                return Task.FromResult($"No conversion needed. Amount: {amount}");

            if (exchangeRates.TryGetValue((from, to), out var rate))
            {
                var converted = amount * rate;
                return Task.FromResult($"{converted:0.00} {to}");
            }

            return Task.FromResult("Conversion rate not available.");
        }
    }
}
