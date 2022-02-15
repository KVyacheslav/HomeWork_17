using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using SystemBank;
using SystemBank.Clients;

namespace HomeWork_17
{
    /// <summary>
    /// Логика взаимодействия для WindowPutClient.xaml
    /// </summary>
    public partial class WindowPutClient : Window
    {
        private MainWindow window;          // Главное окно
        private Client client;              // Текущий клиент
        private BankAccount ba;    // Текущий р/с
        private decimal amount;             // Сумма пополнения р/с

        public WindowPutClient(MainWindow window, Client client, BankAccount bankAccount)
        {
            InitializeComponent();

            this.window = window;
            this.client = client;
            this.ba = bankAccount;
            this.amount = (decimal) Math.Round(slAmount.Value, 2);
            this.tbAmount.Text = Convert.ToString(amount, CultureInfo.InvariantCulture);
            this.tbBankAccount.Text = bankAccount.ToString();

        }

        /// <summary>
        /// Добавить в баланс р/с сумму.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPutBalance_OnClick(object sender, RoutedEventArgs e)
        {
            this.ba.Sum += amount;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            var sql = $@"update table bankaccounts set balance = {ba.Sum} where number = '{ba.Number}'";
            ProviderDB.ExecuteNonQuery(sql, "line 43");

            Bank.StartActionLogs($"Клиент {client.FullName} пополнил баланс р/с на сумму {amount}.");

            this.Close();
        }

        /// <summary>
        /// Задает значение и отображает на форме сумму пополнения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlAmount_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.amount = (decimal)Math.Round(slAmount.Value, 2);
            this.tbAmount.Text = Convert.ToString(amount, CultureInfo.InvariantCulture);
        }

        private void WindowPutClient_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
