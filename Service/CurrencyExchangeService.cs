using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ms2.Service
{
  public class CurrencyExchangeService
{
    public async Task<decimal> ConvertAsync(string fromCurrency, string toCurrency, decimal amount)
    {
        // Simulate conversion rate for USD to NPR
        if (fromCurrency == "USD" && toCurrency == "NPR")
        {
            return await Task.FromResult(amount * 120); // Assume 1 USD = 120 NPR
        }
        else if (fromCurrency == "NPR" && toCurrency == "USD")
        {
            return await Task.FromResult(amount / 120); // Reverse conversion
        }
        return amount; // Fallback to the same amount if currencies match
    }
}


}
