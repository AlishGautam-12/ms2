using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ms2.Models;

namespace ms2.Components.Pages
{
    public partial class Debts : ComponentBase
    {
        private AppData Data;
        private Debt newDebt = new Debt();
        private bool HasUnpaidDebt => Data.Debts.Any(d => d.Amount > d.PaidAmount);

        protected override void OnInitialized()
        {
            Data = UserService.LoadData();
        }

        private void HandleDebtSubmit()
        {
            if (HasUnpaidDebt)
            {
                JSRuntime.InvokeVoidAsync("alert", "Please pay off existing debts before adding new ones.");
                return;
            }

            newDebt.Id = Data.Debts.Count + 1;
            newDebt.Date = DateTime.Now;
            Data.Debts.Add(newDebt);
            UserService.SaveData(Data);

            newDebt = new Debt();
        }

        private void DeleteDebt(int debtId)
        {
            var debtToRemove = Data.Debts.FirstOrDefault(d => d.Id == debtId);
            if (debtToRemove != null)
            {
                Data.Debts.Remove(debtToRemove);
                UserService.SaveData(Data);
            }
        }
    }
}
