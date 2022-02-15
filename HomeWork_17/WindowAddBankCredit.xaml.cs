using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SystemBank;
using SystemBank.Clients;

namespace HomeWork_17
{
    /// <summary>
    /// Логика взаимодействия для WindowAddBankCredit.xaml
    /// </summary>
    public partial class WindowAddBankCredit : Window
    {
        private decimal sum;            // Сумма пополнения р/с при взятии кредита
        private Client client;          // Текущий клиент
        private MainWindow window;      // Главное окно
        private int countYears;         // Срок кредита
        private decimal percent;        // Процент кредита

        public WindowAddBankCredit(MainWindow window, Client client)
        {
            InitializeComponent();
            cbPercent.IsEnabled = false;

            this.window = window;
            this.client = client;
            if (client.IsVip)
            {
                cbPercent.IsEnabled = true;
                cbPercent.SelectedIndex = 1;
            }

            cbBankAccounts.ItemsSource = client.BankAccounts;
            cbBankAccounts.SelectedIndex = 0;
            slBalance.DataContext = client.BankAccounts[cbBankAccounts.SelectedIndex];

            sum = (decimal)Math.Round(slBalance.Value, 2);
            tbBalance.Text = sum.ToString();

        }

        /// <summary>
        /// Добавить кредит клиенту.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBankCredit(object sender, RoutedEventArgs e)
        {
            var credit = sum + sum * percent;
            var ba = cbBankAccounts.SelectedItem as BankAccount;
            var bc = new BankCredit(credit, Bank.Date, countYears, ba, sum);
            try
            {
                client.AddBankCredit(bc, sum);

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                var sql = $@"insert into bankcredits(number, dateOpen, creditTerm, sumCredi, clientId, numberBankAccount, paidOut) values('{bc.Number}', '{Bank.Date:dd.MM.yyyy}', {bc.CountYears * 12}, {bc.Credit}, {client.Id}, '{ba.Number}', {bc.PaidOut})";
                ProviderDB.ExecuteNonQuery(sql, "line 58");

                Bank.StartActionLogs(
                    $"Клиент {client.FullName} взял кредит №{bc.Number} на " +
                    $"сумму {sum} от {bc.DateOpen:dd.MM.yyyy}."
                );
            }

            catch (CountCreditsOutOfRangeException ex)
            {
                MessageBox.Show("Выпало обработанное исключение!");
                MessageBox.Show(ex.Message);
            }

            this.Close();
        }

        /// <summary>
        /// Присваивание максимального значения, равной сумме выбранного р/с
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbBankAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            slBalance.DataContext = client.BankAccounts[cbBankAccounts.SelectedIndex];
        }

        /// <summary>
        /// Отображение значения суммы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slBalance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.sum = (decimal)Math.Round(slBalance.Value, 2);
            this.tbBalance.Text = sum.ToString();
        }

        /// <summary>
        /// Задает значение переменной "Срок кредита"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbTimeCredit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(cbTimeCredit.SelectedIndex)
            {
                case 0:
                    countYears = 1;
                    break;
                case 1:
                    countYears = 3;
                    break;
                default:
                    countYears = 5;
                    break;
            }
        }

        /// <summary>
        /// Задает значение процента кредита
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPercent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            percent = cbPercent.SelectedIndex == 0 ? 0.3m : 0.2m;
        }

        private void WindowAddBankCredit_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
