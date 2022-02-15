using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemBank.Clients
{
    /// <summary>
    /// Класс, описывающий логику клиента - юридического лица.
    /// </summary>
    public class LegalEntity : Client
    {
        /// <summary>
        /// Создаем клиента - юридическое лицо.
        /// </summary>
        /// <param name="fullName">Полное имя (ФИО).</param>
        /// <param name="clientType">Тип клиента.</param>
        /// <param name="bankAccount">Расчётный счёт.</param>
        /// <param name="isVip">Является ли клиент привилегированным?</param>
        public LegalEntity(string fullName, ClientTypes clientType, BankAccount bankAccount, bool isVip) : base(fullName, clientType, bankAccount, isVip)
        {
        }


        public LegalEntity(string fullName, ClientTypes clientType, ObservableCollection<BankAccount> bankAccounts, bool isVip)
            : base(fullName, clientType, bankAccounts, isVip)
        {
        }



        protected override void IncreaseAmountWithCapitalization(BankAccount bankAccount)
        {
            var percent = _isVip ? 0.025m : 0.02m;
            bankAccount.Sum += bankAccount.Sum * percent;
        }

        protected override void IncreaseAmountWithoutCapitalization(BankAccount bankAccount)
        {
            var percent = _isVip ? 0.25m : 0.2m;
            bankAccount.Sum += bankAccount.Sum * percent;
        }
    }
}
