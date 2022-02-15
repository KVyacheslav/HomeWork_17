using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SystemBank;
using SystemBank.Clients;

namespace HomeWork_17
{
    /// <summary>
    /// Логика взаимодействия для WindowTransferToClient.xaml
    /// </summary>
    public partial class WindowTransferToClient : Window
    {
        private Client client;          // Текущий клиент
        private BankAccount bankAccount;    // Текущий р/с
        private Client toClient;        // Клиент, которому будут переводить
        private BankAccount toBankAccount;    // р/с на который будет осуществляться перевод
        private MainWindow window;      // Главное окно
        private ObservableCollection<Client> clients;   // Список клиентов
        private decimal amount;         // Сумма, которую будут переводить

        public WindowTransferToClient(MainWindow window, Client client)
        {
            this.client = client;
            this.window = window;
            this.clients = GetClients();

            InitializeComponent();
            
            this.cbCurrentBankAccounts.ItemsSource = this.client.BankAccounts;
            this.cbCurrentBankAccounts.SelectedIndex = 0;
            this.cbClients.ItemsSource = this.clients;
            this.cbClients.SelectedIndex = 0;
            this.cbClientBankAccounts.ItemsSource = this.toClient.BankAccounts;
            this.cbClientBankAccounts.SelectedIndex = 0;
        }

        /// <summary>
        /// Получить список клиентов
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<Client> GetClients()
        {
            ObservableCollection<Client> tmpClients = new ObservableCollection<Client>();

            foreach (var individual in Bank.Individuals.Clients)
            {
                if (individual == client)
                    continue;

                tmpClients.Add(individual);
            }

            foreach (var legalEntity in Bank.LegalEntities.Clients)
            {
                if (legalEntity == client)
                    continue;

                tmpClients.Add(legalEntity);
            }

            return tmpClients;
        }

        /// <summary>
        /// Выбор р/с клиента, на который будет осуществляться перевод
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbClientBankAccounts_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.toBankAccount = this.cbClientBankAccounts.SelectedItem as BankAccount;
            this.slSum.DataContext = this.bankAccount;
        }

        /// <summary>
        /// Выбор текущего р/с с которого будет перевод
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbCurrentBankAccounts_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.bankAccount = cbCurrentBankAccounts.SelectedItem as BankAccount;
        }

        /// <summary>
        /// Получить и отобразить сумму перевода.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slSum_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.amount = (decimal) Math.Round(this.slSum.Value, 2);
            this.tbSum.Text = this.amount.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Выполнить перевод
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Transfer(object sender, RoutedEventArgs e)
        {
            this.client.TransferTo(toClient, toBankAccount, bankAccount, amount);

            Bank.StartActionLogs($"Клиент {this.client.FullName} выполнил перевод перевод средств" +
                                       $" клиенту {this.toClient.FullName} на сумму {this.amount}");

            this.Close();
        }

        /// <summary>
        /// Выбор текущего клиента, кто будет переводить средства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbClients_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.toClient= cbClients.SelectedItem as Client;
            this.cbClientBankAccounts.ItemsSource = this.toClient?.BankAccounts;
            this.cbClientBankAccounts.SelectedIndex = 0;
        }

        private void WindowTransferToClient_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
