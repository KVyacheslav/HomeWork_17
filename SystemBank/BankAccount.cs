using System;
using System.ComponentModel;

namespace SystemBank
{
    /// <summary>
    /// Класс, реализующий логику расчётного счёта.
    /// </summary>
    public class BankAccount : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;   // Для обновления информации в UI

        private decimal _sum;       // Сумма на счету

        /// <summary>
        /// Номер счёта.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Капитализация?
        /// </summary>
        public bool Capitalization { get; set; }

        /// <summary>
        /// Сумма на счету.
        /// </summary>
        public decimal Sum
        {
            get => this._sum;
            set
            {
                this._sum = Math.Round(value, 2);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sum)));
            }
        }

        /// <summary>
        /// Дата открытия счёта.
        /// </summary>
        public DateTime DateOpen { get; set; }

        /// <summary>
        /// Количество раз увеличивалась сумма. Для увеличения дохода на n-ый процент.
        /// </summary>
        public int NumberTimesIncreased;

        /// <summary>
        /// Создаем рассчетный счёт.
        /// </summary>
        public BankAccount(DateTime open, decimal sum, bool capitalization)
        {
            this.Number = Guid.NewGuid().ToString();
            this.Sum = sum;
            this.Capitalization = capitalization;
            this.DateOpen = open;
            this.NumberTimesIncreased = 0;
        }

        /// <summary>
        /// Создаем рассчетный счёт.
        /// </summary>
        public BankAccount(string number, DateTime open, decimal sum, bool capitalization, int numberTimesIncreased)
        {
            this.Number = number;
            this.Sum = sum;
            this.Capitalization = capitalization;
            this.DateOpen = open;
            this.NumberTimesIncreased = numberTimesIncreased;
        }

        public override string ToString()
        {
            var arrData = this.Number.Split('-');
            var str = $"р/с ****-{arrData[arrData.Length - 1]}. Баланс: {_sum}";
            return str;
        }
    }

}
