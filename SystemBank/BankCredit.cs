using System;
using System.ComponentModel;

namespace SystemBank
{
    /// <summary>
    /// Класс, реализующий логику кредита.
    /// </summary>
    public class BankCredit : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;   // Для обновления информации в UI

        private decimal _credit;            // Сколько нужно всего выплатить с процентами
        private decimal _paidOut;           // Сколько уже выплачено

        /// <summary>
        /// Номер счета.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Дата открытия.
        /// </summary>
        public DateTime DateOpen { get; set; }

        /// <summary>
        /// Срок.
        /// </summary>
        public int CountYears { get; set; }

        /// <summary>
        /// Долг.
        /// </summary>
        public decimal Credit
        {
            get => this._credit;
            set
            {
                this._credit = Math.Round(value, 2);
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Credit)));
            }
        }

        /// <summary>
        /// Выплачено.
        /// </summary>
        public decimal PaidOut
        {
            get => this._paidOut;
            set
            {
                this._paidOut = Math.Round(value, 2);
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PaidOut)));
            }
        }

        /// <summary>
        /// Ежемесячная плата.
        /// </summary>
        public decimal MonthlyPayment { get; set; }

        /// <summary>
        /// Расчётный счёт, с которого будет сниматься сумма за кредит.
        /// </summary>
        public BankAccount BankAccount { get; set; }

        /// <summary>
        /// Выдаем кредит.
        /// </summary>
        /// <param name="credit">Сумма долга.</param>
        /// <param name="dateOpen">Дата открытия кредита.</param>
        /// <param name="countYears">Срок кредита.</param>
        /// <param name="bankAccount">Расчётный счёт, для снятия денег за кредит.</param>
        /// <param name="sum">Сумма пополнения расчётного счета.</param>
        public BankCredit(decimal credit, DateTime dateOpen, int countYears, 
            BankAccount bankAccount, decimal sum)
        {
            this.Number = Guid.NewGuid().ToString();
            this.DateOpen = dateOpen;
            this.CountYears = countYears;
            this._credit = Math.Round(credit, 2);
            this._paidOut = 0;
            this.MonthlyPayment = credit / (countYears * 12);
            this.BankAccount = bankAccount;
            bankAccount.Sum += sum;
        }

        /// <summary>
        /// Выдаем кредит.
        /// </summary>
        /// <param name="credit">Сумма долга.</param>
        /// <param name="dateOpen">Дата открытия кредита.</param>
        /// <param name="countYears">Срок кредита.</param>
        /// <param name="bankAccount">Расчётный счёт, для снятия денег за кредит.</param>
        public BankCredit(string number, decimal credit, DateTime dateOpen, int countYears, decimal paidOut,
            BankAccount bankAccount)
        {
            this.Number = number;
            this.DateOpen = dateOpen;
            this.CountYears = countYears;
            this._credit = Math.Round(credit, 2);
            this._paidOut = paidOut;
            this.MonthlyPayment = credit / (countYears * 12);
            this.BankAccount = bankAccount;
        }
    }
}
