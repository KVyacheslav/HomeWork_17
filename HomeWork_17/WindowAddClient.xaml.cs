using System;
using System.Windows;
using System.Windows.Input;
using SystemBank;
using SystemBank.Clients;

namespace HomeWork_17
{
    /// <summary>
    /// Логика взаимодействия для WindowAddClient.xaml
    /// </summary>
    public partial class WindowAddClient : Window
    {
        private MainWindow window;          // Главное окно
        private decimal balance;            // Баланс р/с

        public WindowAddClient(MainWindow window)
        {
            InitializeComponent();

            this.window = window;
        }

        /// <summary>
        /// Задает баланс при изменении положения ползунка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slBalance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.balance = (decimal)Math.Round(slBalance.Value, 2);
            this.tbBalance.Text = this.balance.ToString();
        }

        /// <summary>
        /// Добавляем клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddClient(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbName.Text))
            {
                MessageBox.Show("Поле ФИО не может быть пустым!", "Ошибка");
                return;
            }

            var name = this.tbName.Text;
            var type = this.cbTypes.SelectedItem.ToString().Equals("Физическое лицо")
                ? ClientTypes.Individual : ClientTypes.LegalEntity;
            var isVip = (bool)this.chbIsVip.IsChecked;
            var capitalization = (bool)this.chbCapitalization.IsChecked;
            var date = Bank.Date;
            BankAccount bankAccount = new BankAccount(date, this.balance, capitalization);

            var id = Bank.Individuals.Clients.Count + Bank.LegalEntities.Clients.Count + 1;

            if (type == ClientTypes.Individual)
            {
                var client = new Individual(name, type, bankAccount, isVip);
                Bank.Individuals.AddClient(client);

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                var sql = $@"insert into clients(id, fullName, typeId, privileged) values({id}, {name}, 1, {(client.IsVip ? 1 : 0)})";
                ProviderDB.ExecuteNonQuery(sql, "line 64");

                var typeString = "физическое лицо";
                var msg = $"Клиент {client.FullName} зарегистрировался как {typeString} от {Bank.Date:dd.MM.yyyy}.";
                Bank.StartActionLogs(msg);
            }
            else
            {
                var client = new LegalEntity(name, type, bankAccount, isVip);
                Bank.LegalEntities.AddClient(client);

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                var sql = $@"insert into clients(id, fullName, typeId, privileged) values({id}, {name}, 2, {(client.IsVip ? 1 : 0)})";
                ProviderDB.ExecuteNonQuery(sql, "line 76");

                var typeString = "юридическое лицо";
                var msg = $"Клиент {client.FullName} зарегистрировался как {typeString} от {Bank.Date:dd.MM.yyyy}.";
                Bank.StartActionLogs(msg);
            }


            this.Close();
        }

        private void WindowAddClient_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
