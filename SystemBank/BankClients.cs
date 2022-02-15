using System;
using System.Collections.ObjectModel;
using SystemBank.Clients;

namespace SystemBank
{
    /// <summary>
    /// Класс, описывающий логику банка.
    /// </summary>
    public class BankClients<T> 
        where T : Client
    {
        /// <summary>
        /// Список клиентов.
        /// </summary>
        public ObservableCollection<T> Clients { get; set; }




        /// <summary>
        /// Создаём банк.
        /// </summary>
        public BankClients()
        {
            this.Clients = new ObservableCollection<T>();
        }



        /// <summary>
        /// Добавляем клиента.
        /// </summary>
        /// <param name="client">Клиент</param>
        public void AddClient(T client)
        {
            this.Clients.Add(client);
        }

        /// <summary>
        /// Удаляем клиента.
        /// </summary>
        /// <param name="client">Клиент.</param>
        public void RemoveClient(T client)
        {
            this.Clients.Remove(client);
        }

        /// <summary>
        /// Проверка баланса у клиентов.
        /// </summary>
        /// <param name="currentDate">Текущая дата.</param>
        public void CheckBalanceClients(DateTime currentDate)
        {
            foreach (var client in Clients)
            {
                client.CheckBankAccountsAndCredits(currentDate);
            }
        }

    }
}
