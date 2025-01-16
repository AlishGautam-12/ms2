using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ms2.Models;
using Microsoft.JSInterop;

namespace ms2.Components.Pages
{
    public partial class Dashboard : ComponentBase
    {


        private AppData Data;
        private bool showHighestDebits = true;
        private bool showLowestDebits = false;
        private bool showHighestCredits = true;
        private bool showLowestCredits = false;
        private bool showHighestDebtsSection = true;
        private bool showLowestDebtsSection = false;
        private decimal mainBalance;
        private string selectedCurrency;
        private string mainBalanceDisplay;

        protected override void OnInitialized()
        {
            mainBalance = UserService.GetMainBalance();
            Data = UserService.LoadData();
            selectedCurrency = "USD"; // Set default currency
            UpdateMainBalanceDisplay();  // Initialize the display
        }


        private async Task UpdateMainBalance(ChangeEventArgs e)
        {
            selectedCurrency = e.Value?.ToString() ?? "USD";
            await UpdateMainBalanceDisplay();
        }

        private async Task UpdateMainBalanceDisplay()
        {
            if (selectedCurrency == "USD")
            {
                mainBalanceDisplay = mainBalance.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }
            else if (selectedCurrency == "NPR")
            {
                // Convert USD to NPR
                var convertedAmount = await ExchangeService.ConvertAsync("USD", "NPR", mainBalance);
                mainBalanceDisplay = $"{convertedAmount} NPR";
            }
        }

        // Methods to toggle the display of the Highest/Lowest Debits
        private void ShowHighestDebits() { showHighestDebits = true; showLowestDebits = false; }
        private void ShowLowestDebits() { showHighestDebits = false; showLowestDebits = true; }

        // Methods to toggle the display of the Highest/Lowest Credits
        private void ShowHighestCredits() { showHighestCredits = true; showLowestCredits = false; }
        private void ShowLowestCredits() { showHighestCredits = false; showLowestCredits = true; }

        // Methods to toggle the display of the Highest/Lowest Debts Section
        private void ShowHighestDebtsSection() { showHighestDebtsSection = true; showLowestDebtsSection = false; }
        private void ShowLowestDebtsSection() { showHighestDebtsSection = false; showLowestDebtsSection = true; }
    }
}



