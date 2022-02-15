using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemBank.Clients
{
    /// <summary>
    /// Класс, описывающий логику клиента - физического лица.
    /// </summary>
    public class Individual : Client
    {
        /// <summary>
        /// Создаем клиента - физическое лицо.
        /// </summary>
        /// <param name="fullName">Полное имя (ФИО).</param>
        /// <param name="clientType">Тип клиента.</param>
        /// <param name="bankAccount">Расчётный счёт.</param>
        /// <param name="isVip">Является ли клиент привилегированным?</param>
        public Individual(string fullName, ClientTypes clientType, BankAccount bankAccount, bool isVip) 
            : base(fullName, clientType, bankAccount, isVip)
        {
        }

        public Individual(string fullName, ClientTypes clientType, ObservableCollection<BankAccount> bankAccounts, bool isVip)
            : base(fullName, clientType, bankAccounts, isVip)
        {
        }



        protected override void IncreaseAmountWithCapitalization(BankAccount bankAccount)
        {
            var percent = _isVip ? 0.015m : 0.01m;
            bankAccount.Sum += bankAccount.Sum * percent;
        }

        protected override void IncreaseAmountWithoutCapitalization(BankAccount bankAccount)
        {
            var percent = _isVip ? 0.15m : 0.12m;
            bankAccount.Sum += bankAccount.Sum * percent;
        }
    }
}
