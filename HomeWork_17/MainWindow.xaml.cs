using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SystemBank;
using SystemBank.Clients;

namespace HomeWork_17
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ClientTypes clientType;         // Используемый тип
        private Random rnd;



        public MainWindow()
        {
            this.rnd = new Random();

            InitializeComponent();

            this.tbDate.Text = $"Текущая дата: {Bank.Date:dd.MM.yyyy}";



            Task t1 = new Task(Bank.LoadClients);


            t1.Start();
            MessageBox.Show("Загружаются данные.", "Загрузка");
            Init();
        }

        public async void Init()
        {
            await Task.Run(() =>
            {
                while (!Bank.FinishedGenerate)
                {
                    Thread.Sleep(500);
                }


                MessageBox.Show("Данные загружены.", "Загрузка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information,
                    MessageBoxResult.OK,
                    MessageBoxOptions.ServiceNotification);
                Dispatcher.InvokeAsync(() =>
                {
                    cbClients.IsEnabled = true;
                    cbClients.SelectedIndex = 0;
                    lvClients.Items.Refresh();
                    lvClients.ItemsSource = Bank.Individuals.Clients;
                    lvClients.SelectedIndex = 0;
                    lvLogs.ItemsSource = Bank.Logs;
                    lvLogs.Items.Refresh();
                    lvLogs.SelectedIndex = 0;
                });
            });         // выполняется асинхронно
        }

        /// <summary>
        /// При изменении выбора списка клиентов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbClients_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = this.cbClients.SelectedIndex;
            // Присваиваем используемый тип.
            this.clientType = index == 0 
                ? ClientTypes.Individual 
                : ClientTypes.LegalEntity;

            lvClients.ItemsSource = index == 0
                ? (IEnumerable) Bank.Individuals.Clients
                :  Bank.LegalEntities.Clients;

            if (lvClients.Items.Count > 0)
                this.lvClients.ScrollIntoView(lvClients.Items[0]);

            this.tbName.Text = string.Empty;
            this.tbIsVip.Text = string.Empty;
            this.tbCountBankAcc.DataContext = lvClients.SelectedItem as Client;
            this.tbCountBankCredits.DataContext = lvClients.SelectedItem as Client;
            this.lvBankAccounts.Visibility = Visibility.Hidden;
            this.lvBankCredits.Visibility = Visibility.Hidden;
        }


        /// <summary>
        /// При выборе клиента из списка...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvClients_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Client client;

            if (this.clientType == ClientTypes.Individual) 
                client = lvClients.SelectedItem as Individual;
            else
                client = lvClients.SelectedItem as LegalEntity;

            if (client != null)
            {
                this.tbName.Text = client.FullName;
                this.tbIsVip.Text = client.IsVip ? "Да" : "Нет";
                this.tbCountBankAcc.DataContext = client;
                this.tbCountBankCredits.DataContext = client;
                this.lvBankAccounts.ItemsSource = client.BankAccounts;
                this.lvBankCredits.ItemsSource = client.BankCredits;
                this.lvBankAccounts.Visibility = Visibility.Visible;
                this.lvBankCredits.Visibility = Visibility.Visible;
                this.tbName.Visibility = Visibility.Visible;
                this.tbIsVip.Visibility = Visibility.Visible;
            }
            else
            {
                this.tbCountBankAcc.DataContext = null;
                this.tbCountBankCredits.DataContext = null;
            }
        }

        /// <summary>
        /// Следующий месяц.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextMonth(object sender, RoutedEventArgs e)
        {
            Bank.Date = Bank.Date.AddMonths(1);
            this.tbDate.Text = $"Текущая дата: {Bank.Date:dd.MM.yyyy}";
            Bank.Individuals.CheckBalanceClients(Bank.Date);
            Bank.LegalEntities.CheckBalanceClients(Bank.Date);
        }

        /// <summary>
        /// Вывести окно добавления клиента.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowWindowAddClient(object sender, RoutedEventArgs e)
        {
            new WindowAddClient(this).ShowDialog();
            lvLogs.ScrollIntoView(lvLogs.Items[lvLogs.Items.Count - 1]);
        }

        /// <summary>
        /// Удаляем клиента.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveClient(object sender, RoutedEventArgs e)
        {
            if (clientType == ClientTypes.Individual)
            {
                var client = lvClients.SelectedItem as Individual;
                Bank.Individuals.RemoveClient(client);

                Bank.StartActionLogs($"Клиент {client.FullName} уволился от {Bank.Date:dd.MM.yyyy}.");
            }
            else
            {
                var client = lvClients.SelectedItem as LegalEntity;
                Bank.LegalEntities.RemoveClient(client);

                Bank.StartActionLogs($"Клиент {client.FullName} уволился от {Bank.Date:dd.MM.yyyy}.");
            }

            lvLogs.ScrollIntoView(lvLogs.Items[lvLogs.Items.Count - 1]);
            tbName.Visibility = Visibility.Hidden;
            tbIsVip.Visibility = Visibility.Hidden;
            lvBankAccounts.Visibility = Visibility.Hidden;
            lvBankCredits.Visibility = Visibility.Hidden;

        }

        /// <summary>
        /// Добавить расчётный счёт.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBankAccount(object sender, RoutedEventArgs e)
        {
            Client client = lvClients.SelectedItem as Client;
            new WindowAddBankAccount(this, client).ShowDialog();
            lvLogs.ScrollIntoView(lvLogs.Items[lvLogs.Items.Count - 1]);
        }

        /// <summary>
        /// Удалить расчётный счёт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveBankAccount(object sender, RoutedEventArgs e)
        {
            BankAccount bankAccount = lvBankAccounts.SelectedItem as BankAccount;

            if (bankAccount == null)
                return;

            Client client = lvClients.SelectedItem as Client;
            client.RemoveBankAccount(bankAccount);
            lvLogs.ScrollIntoView(lvLogs.Items[lvLogs.Items.Count - 1]);
        }

        /// <summary>
        /// Показать окно редактирования клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowWindowEditClient(object sender, RoutedEventArgs e)
        {
            var client = lvClients.SelectedItem as Client;

            if (client == null)
                return;

            new WindowEditClient(this, client).ShowDialog();
        }

        /// <summary>
        /// Добавить расчётный счёт.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBankCredit(object sender, RoutedEventArgs e)
        {
            Client client = lvClients.SelectedItem as Client;

            if (client == null)
                return;

            var maxSum = client.BankAccounts.ToList().Max(ba => ba.Sum);

            if (maxSum <= 500)
            {
                MessageBox.Show("Невозможно взять кредит при максимальном\nбалансе 500 или менее.", "Ошибка");
                return;
            }


            new WindowAddBankCredit(this, client).ShowDialog();
            lvLogs.ScrollIntoView(lvLogs.Items[lvLogs.Items.Count - 1]);

        }

        /// <summary>
        /// При изменении размера логов, скролл показывает последний лог.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvLogs_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (lvLogs.Items.Count > 0)
                lvLogs.ScrollIntoView(lvLogs.Items[lvLogs.Items.Count - 1]);
        }

        /// <summary>
        /// Перевод между своими счетами.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransferToBankAccount(object sender, RoutedEventArgs e)
        {
            var client = lvClients.SelectedItem as Client;
            var bankAccount = lvBankAccounts.SelectedItem as BankAccount;

            if (bankAccount == null)
                return;

            if (lvBankAccounts.Items.Count <= 1)
                return;

            new WindowTransferToBankAccount(this, client, bankAccount).ShowDialog();

            lvLogs.ScrollIntoView(lvLogs.Items[lvLogs.Items.Count - 1]);
        }
        
        /// <summary>
        /// Пополнение расчётного счёта.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PutBankAccount(object sender, RoutedEventArgs e)
        {
            Client client = lvClients.SelectedItem as Client;
            BankAccount bankAccount = lvBankAccounts.SelectedItem as BankAccount;
            
            if (bankAccount == null)
                return;

            new WindowPutClient(this, client, bankAccount).ShowDialog();

            lvLogs.ScrollIntoView(lvLogs.Items[lvLogs.Items.Count - 1]);
        }

        /// <summary>
        /// Перевод другому клиенту на р/с
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransferToClientBankAccount(object sender, RoutedEventArgs e)
        {
            if (this.lvClients.Items.Count <= 1)
                return;

            var client = lvClients.SelectedItem as Client;
            
            new WindowTransferToClient(this, client).ShowDialog();

            lvLogs.ScrollIntoView(lvLogs.Items[lvLogs.Items.Count - 1]);
        }

        /// <summary>
        /// Закрыть окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Перемещение окна 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            lvClients.Items.Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ProviderDB.Close();
        }
    }
}
