using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ms2.Models;

namespace ms2.Components.Pages
{
    public partial class TransactionPage : ComponentBase
    {

       
    private AppData Data;
        private Transaction newTransaction = new Transaction();
        private List<string> defaultTags = new List<string>
    {
        "Bonus", "Salary", "Trading", "Gift", "Interest", "Rental Income", "Food", "Drinks", "Clothes", "Gadgets", "Miscellaneous", "Fuel", "Rent", "EMI", "Party"
    };

        private List<string> selectedTags = new List<string>();
        private bool isEditMode = false;
        private string searchDescription = "";
        private string filterType = "";
        private DateTime? startDate;
        private DateTime? endDate;
        private IEnumerable<Transaction> filteredTransactions;

        protected override void OnInitialized()
        {
            Data = UserService.LoadData();
            newTransaction.Date = DateTime.Now;
            filteredTransactions = Data.Transactions;
        }

        private void FilterTransactions()
        {
            filteredTransactions = Data.Transactions
                .Where(t =>
                    (string.IsNullOrWhiteSpace(searchDescription) ||
                     t.Description != null && t.Description.Contains(searchDescription, StringComparison.OrdinalIgnoreCase))
                )
                .Where(t => string.IsNullOrWhiteSpace(filterType) || t.Type == filterType)
                .Where(t => (!startDate.HasValue || t.Date.Date >= startDate.Value.Date) &&
                    (!endDate.HasValue || t.Date.Date <= endDate.Value.Date))
                .ToList();

            // If no transactions match, we can show a message or leave the list empty
            if (!filteredTransactions.Any())
            {
                filteredTransactions = new List<Transaction>();
            }
        }

        private void RefreshFilteredTransactions()
        {
            // Apply the same filtering logic used in the FilterTransactions method
            filteredTransactions = Data.Transactions
                .Where(t => string.IsNullOrWhiteSpace(searchDescription) || t.Description.Contains(searchDescription, StringComparison.OrdinalIgnoreCase))
                .Where(t => string.IsNullOrWhiteSpace(filterType) || t.Type == filterType)
                .Where(t =>
                    (!startDate.HasValue || t.Date.Date >= startDate.Value.Date) &&
                    (!endDate.HasValue || t.Date.Date <= endDate.Value.Date))
                .ToList();
        }

        private decimal GetMainBalance()
        {
            var totalCredit = Data.Transactions.Sum(t => t.Credit);
            var totalDebit = Data.Transactions.Sum(t => t.Debit);
            var totalDebtAddedToBalance = Data.Debts.Sum(d => d.Amount);
            var totalDebtCleared = Data.Debts.Sum(d => d.PaidAmount);

            // Main balance includes debt added but deducts cleared debt
            return totalCredit - totalDebit + totalDebtAddedToBalance - totalDebtCleared;
        }


        private void HandleTransactionSubmit()
        {
            if (newTransaction.Amount <= 0)
            {
                return; // Ignore invalid amounts
            }

            newTransaction.Tags = new List<string>(selectedTags);
            newTransaction.Date = DateTime.Now;

            if (newTransaction.Type == "Credit")
            {
                // Allocate credit to debts first
                var (remainingCredit, debtCleared) = AllocateCreditToDebts(newTransaction.Amount);

                newTransaction.Credit = remainingCredit;
                newTransaction.Description = newTransaction.Description ?? "Credit transaction processed.";
                if (debtCleared > 0)
                {
                    newTransaction.DebtClearedNotes = $"Debt of {debtCleared:C} cleared from credit.";
                }
            }
            else if (newTransaction.Type == "Debit")
            {
                if (!IsSufficientBalance(newTransaction.Amount))
                {
                    return; // Abort if insufficient balance
                }
                newTransaction.Debit = newTransaction.Amount;
                newTransaction.Credit = 0;
            }

            if (isEditMode)
            {
                // Edit existing transaction logic here
            }
            else
            {
                newTransaction.Id = Data.Transactions.Count > 0 ? Data.Transactions.Max(t => t.Id) + 1 : 1;
                Data.Transactions.Add(newTransaction);
            }

            UserService.SaveData(Data);
            UserService.UpdateMainBalance();  // Recalculate and update the main balance

            RefreshFilteredTransactions();

            // Reset for the next transaction
            newTransaction = new Transaction { Date = DateTime.Now };
            selectedTags.Clear();
            isEditMode = false;
        }

        private void AddTransactionToMainBalance(decimal netCredit)
        {
            // Add the remaining credit amount (net credit) to the main balance
            var balanceTransaction = new Transaction
            {
                Id = Data.Transactions.Count > 0 ? Data.Transactions.Max(t => t.Id) + 1 : 1,
                Type = "Credit",
                Credit = netCredit,
                Debit = 0,
                Tags = new List<string> { "Remaining Credit" },
                Date = DateTime.Now,
                Description = "Net credit after debt clearance."
            };
            Data.Transactions.Add(balanceTransaction);
        }

        private void OnTransactionTypeChange(ChangeEventArgs e)
        {
            newTransaction.Type = e.Value.ToString();
        }

        private void OnTagsChange(ChangeEventArgs e)
        {
            if (e.Value is IEnumerable<string> selected)
            {
                selectedTags = selected.ToList();
            }
            else if (e.Value is string singleValue)
            {
                selectedTags = singleValue.Split(',').Select(s => s.Trim()).ToList();
            }
            newTransaction.Tags = new List<string>(selectedTags);
        }

        private bool IsSufficientBalance(decimal outflow)
        {
            var currentBalance = Data.Transactions.Sum(t => t.Credit) - Data.Transactions.Sum(t => t.Debit);
            return outflow <= currentBalance;
        }

        private bool CanSubmitTransaction()
        {
            if (newTransaction.Amount <= 0)
            {
                return false;
            }

            if (newTransaction.Type == "Debit")
            {
                return IsSufficientBalance(newTransaction.Amount);
            }
            return true;
        }

        private void EditTransaction(Transaction transaction)
        {
            newTransaction = new Transaction
            {
                Id = transaction.Id,
                Type = transaction.Type,
                Amount = transaction.Type == "Credit" ? transaction.Credit : transaction.Debit,
                Tags = new List<string>(transaction.Tags ?? new List<string>()),
                Date = transaction.Date,
                Description = transaction.Description
            };
            selectedTags = new List<string>(transaction.Tags ?? new List<string>());
            isEditMode = true;
        }

        private void DeleteTransaction(int id)
        {
            var transaction = Data.Transactions.FirstOrDefault(t => t.Id == id);
            if (transaction != null)
            {
                Data.Transactions.Remove(transaction);
                UserService.UpdateMainBalance();  // Recalculate and update the main balance
                UserService.SaveData(Data);
            }
        }

        private (decimal remainingCredit, decimal debtCleared) AllocateCreditToDebts(decimal creditAmount)
        {
            decimal originalCredit = creditAmount;
            decimal debtCleared = 0;

            foreach (var debt in Data.Debts.Where(d => d.Amount > d.PaidAmount))
            {
                decimal unpaidDebtAmount = debt.Amount - debt.PaidAmount;

                if (creditAmount >= unpaidDebtAmount)
                {
                    debt.PaidAmount += unpaidDebtAmount;
                    creditAmount -= unpaidDebtAmount;
                    debtCleared += unpaidDebtAmount;
                }
                else
                {
                    debt.PaidAmount += creditAmount;
                    creditAmount = 0;
                    debtCleared += creditAmount;
                    break;
                }
            }

            return (creditAmount, debtCleared);
        }
    }

}

