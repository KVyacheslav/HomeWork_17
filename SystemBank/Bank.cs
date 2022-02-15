using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SystemBank.Clients;

namespace SystemBank
{
    public static class Bank
    {
        private static Random rnd;
        private static event Action<Log> ActionLogs;


        public static ObservableCollection<Log> Logs { get; set; }
        public static BankClients<Individual> Individuals { get; set; } // Список клиентов - физических лиц.
        public static BankClients<LegalEntity> LegalEntities { get; set; } // Список клиентов - юридических лиц.
        public static DateTime Date { get; set; } // Текущая дата
        public static bool FinishedGenerate { get; set; } // Закончина ли генерация клиентов


        static Bank()
        {
            Date = DateTime.Now;
            Individuals = new BankClients<Individual>();
            LegalEntities = new BankClients<LegalEntity>();
            Logs = new ObservableCollection<Log>();
            rnd = new Random();
            ActionLogs += log => Logs.Add(log);
        }


        public static void LoadClients()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            for (int i = 1; i <= 100; i++)
            {
                var sql = $@"select * from bankaccounts where clientId = {i}";

                ObservableCollection<BankAccount> ba = new ObservableCollection<BankAccount>();

                var r = ProviderDB.ExecuteQuery(sql, "line 40");

                while (r.Read())
                {
                    var number = r.GetString(0);
                    var dateOpen = r.GetDateTime(1);
                    var balance = r.GetDecimal(2);
                    var cap = r.GetBoolean(3);
                    var numInc = r.GetInt32(5);
                    ba.Add(new BankAccount(number, dateOpen, balance, cap, numInc));
                }
                r.Close();

                sql = $@"select * from clients where id = {i}";
                var reader = ProviderDB.ExecuteQuery(sql, "line 57");

                Client client = new Individual(string.Empty, ClientTypes.Individual, new ObservableCollection<BankAccount>(), false);

                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var name = reader.GetString(1);
                    var type = reader.GetInt32(2) == 1
                        ? ClientTypes.Individual
                        : ClientTypes.LegalEntity;
                    var privileged = reader.GetBoolean(3);

                    if (type == ClientTypes.Individual)
                    {
                        client = new Individual(name, type, ba, privileged);
                        Individuals.AddClient(client as Individual);
                    }
                    else
                    {
                        client = new LegalEntity(name, type, ba, privileged);
                        LegalEntities.AddClient(client as LegalEntity);
                    }

                }
                
                reader.Close();
                sql = $@"select * from bankcredits where clientId = {i}";
                r = ProviderDB.ExecuteQuery(sql, "line 85");
                while (r.Read())
                {
                    var number = r.GetString(0);
                    var dateOpen = r.GetDateTime(1);
                    var creditTerm = r.GetInt32(2) / 12;
                    var sumCredit = r.GetDecimal(3);
                    var numberBankAccount = r.GetString(5);
                    var paidOut = r.GetDecimal(6);
                    BankAccount bankAccount;

                    foreach (var acc in ba)
                    {
                        if (acc.Number == numberBankAccount)
                        {
                            bankAccount = acc;
                            client?.LoadBankCredit(new BankCredit(number, sumCredit, dateOpen, creditTerm, paidOut, bankAccount));
                            break;
                        }
                    }
                }

                r.Close();
            }


            FinishedGenerate = true;
        }

        #region Generate

        /// <summary>
        /// Генерация клиентов.
        /// </summary>
        public static void GenerateClients()
        {
            for (int i = 1; i <= 10_000; i++)
            {
                var name = GetRandomFullName();
                var typeInt = rnd.Next(1, 3);
                var type = typeInt == 1
                    ? ClientTypes.Individual
                    : ClientTypes.LegalEntity;
                var isVip = rnd.Next(5) == 0;
                var bankAccount = GetGenerateBankAccount();

                Client client;



                if (type == ClientTypes.Individual)
                {
                    client = new Individual(name, type, bankAccount, isVip);
                    Individuals.AddClient(client as Individual);
                }
                else
                {
                    client = new LegalEntity(name, type, bankAccount, isVip);
                    LegalEntities.AddClient(client as LegalEntity);
                }

                client.Id = i;

                var sql = $@"insert into clients (fullName, typeId, privileged) values('{name}', {typeInt}, {(isVip ? 1 : 0)})";
                ProviderDB.ExecuteNonQuery(sql, "77 line");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                sql = $@"insert into bankaccounts (number, dateOpen, balance, capitalization, numberTimesIncreased, clientId) values('{bankAccount.Number}', '{Date:dd.MM.yyyy}', {bankAccount.Sum}, {(bankAccount.Capitalization ? 1 : 0)}, 0, {client.Id})";
                ProviderDB.ExecuteNonQuery(sql, "79 line");

                var typeString = client.ClientType == ClientTypes.Individual ? "физическое лицо" : "юридическое лицо";
                var msg = $"Клиент {client.FullName} зарегистрировался как {typeString} от {Date:dd.MM.yyyy}.";
                

                ActionLogs?.Invoke(new Log(msg));

                if (rnd.Next(5) == 0)
                {
                    var ba = GetGenerateBankAccount();
                    client.AddBankAccount(ba);
                    sql = $@"insert into bankaccounts (number, dateOpen, balance, capitalization, numberTimesIncreased, clientId) values('{ba.Number}', '{Date:dd.MM.yyyy}', {ba.Sum}, {(ba.Capitalization ? 1 : 0)}, 0, {client.Id})";
                    ProviderDB.ExecuteNonQuery(sql, "92 line");
                }

                if (rnd.Next(3) != 2)
                    client.AddBankCredit(GetGenerateBankCredit(client, out decimal sum), sum);

            }

            FinishedGenerate = true;
        }

        /// <summary>
        /// Сгенерировать клиенту кредит.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="sum">Сумма взятая в кредит.</param>
        /// <returns></returns>
        private static BankCredit GetGenerateBankCredit(Client client, out decimal sum)
        {
            var ba = client.BankAccounts[0];
            sum = rnd.Next(500, (int)ba.Sum);
            var credit = sum + sum * (decimal)(client.IsVip ? 0.2 : 0.3);
            var bk = new BankCredit(credit, Date, 3, ba, sum);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            var sql = $@"insert into bankcredits(number, dateOpen, creditTerm, sumCredit, clientId, numberBankAccount, paidOut) values('{bk.Number}', '{Date:dd.MM.yyyy}', 36, {bk.Credit}, {client.Id}, '{client.BankAccounts[0].Number}', {bk.PaidOut})";
            ProviderDB.ExecuteNonQuery(sql, "117 line");

            sql = $@"update bankaccounts set balance = {ba.Sum} where number = '{ba.Number}'";
            ProviderDB.ExecuteNonQuery(sql, "120 line");

            return bk;
        }

        /// <summary>
        /// Сгенерировать расчётные счета.
        /// </summary>
        /// <returns></returns>
        private static BankAccount GetGenerateBankAccount()
        {
            return new BankAccount(
                DateTime.Now,
                rnd.Next(1000, 10001),
                rnd.Next(2) == 0);
        }

        /// <summary>
        /// Сгенерировать полное имя.
        /// </summary>
        /// <returns>Полное имя.</returns>
        private static string GetRandomFullName()
        {
            var data = @"Бичурин Алексей Платонович
Царёва Ева Якововна
Бочарова Оксана Виталиевна
Грефа Софья Филипповна
Гусева Роза Мефодиевна
Дёмшина Арина Елизаровна
Архипов Артем Ираклиевич
Цветкова Людмила Павеловна
Ямзин Леонид Филимонович
Плюхина Нина Емельяновна
Григорьева Инна Василиевна
Анисимова Полина Борисовна
Лешев Виктор Богданович
Бессуднов Станислав Евстафиевич
Топоров Анатолий Самсонович
Васильева Владлена Серафимовна
Бикулов Аскольд Капитонович
Головкина Алина Федоровна
Насонова Лада Мироновна
Островерха Ульяна Станиславовна
Шамякин Терентий Тихонович
Кидирбаева Валентина Анатолиевна
Булыгина Диана Никитевна
Беломестнов Фока Никанорович
Гайдученко Тимофей Зиновиевич
Казьмин Агафон Семенович
Соломахина Юлия Михеевна
Хорошилова Ярослава Романовна
Волынкина Валерия Леонидовна
Садовничий Алиса Петровна
Чичерин Кондратий Титович
Рыкова Зинаида Олеговна
Крутой Наталия Брониславовна
Есипов Герман Касьянович
Лачков Аркадий Назарович
Яикбаева Инга Фомевна
Семенов Иосиф Кондратиевич
Курушин Прокл Валериевич
Денисова Анна Кузьмевна
Рыжанов Богдан Моисеевич
Канадина Светлана Данииловна
Никаева Изольда Юлиевна
Кочинян Никон Феликсович
Бурмакина Элеонора Георгиевна
Висенина Ульяна Владиленовна
Валиев Вениамин Яковович
Ярилов Зиновий Епифанович
Гибазов Эдуард Сергеевич
Клокова Антонина Серафимовна
Волобуева Раиса Семеновна
Бабышев Гавриил Феликсович
Задков Филипп Миронович
Варфоломеева Варвара Феликсовна
Селиванов Герман Карлович
Томсин Аскольд Эрнестович
Енотова Евгения Юлиевна
Мандрыкин Владислав Богданович
Голубцов Аскольд Давидович
Рыжов Прокл Всеволодович
Кораблин Иннокентий Наумович
Черенчикова Светлана Несторовна
Арсеньева Римма Виталиевна
Громыко Лука Елизарович
Архаткин Леонид Евграфович
Дубинина Арина Леонидовна
Дуркина Надежда Фомевна
Шкиряк Аким Ипполитович
Солдатов Петр Вячеславович
Иванников Ефрем Григориевич
Липова Пелагея Казимировна
Янкин Модест Ираклиевич
Машлыкин Станислав Евгениевич
Погребной Прохор Сигизмундович
Кетов Лавр Иосифович
Степихова Мирослава Казимировна
Кучава Всеволод Касьянович
Кустов Вадим Назарович
Борзилов Макар Миронович
Блатова Светлана Олеговна
Лапотников Семён Мартьянович
Аронова Клара Никитевна
Кудяшова Розалия Никитевна
Киприянов Антип Вячеславович
Ягунова Дарья Геннадиевна
Ручкина Варвара Юлиевна
Малинина Ярослава Ростиславовна
Завражный Кондратий Эмилевич
Крымов Андрон Матвеевич
Голубов Тимур Андриянович
Клоков Нестор Кондратиевич
Гоминова Роза Евгениевна
Петухов Ефрем Савелиевич
Вьялицына Виктория Несторовна
Игнатенко Эвелина Иосифовна
Фернандес Аким Савелиевич
Блатова Эвелина Якововна
Любимцев Ярослав Мирославович
Уголева ﻿Агата Петровна
Саянов Виталий Адрианович
Якунова Зоя Леонидовна
Дорохова ﻿Агата Германовна
Журавлёв Евгений Игоревич
Цветков Игнатий Наумович
Дагина Эвелина Мироновна
Гика Алла Яновна
Дубровский Роман Александрович
Касатый Агафья Иларионовна
Березовский Артём Игнатиевич
Чекмарёв Никита Куприянович
Смотров Георгий Демьянович
Кошелева Элеонора Антониновна
Калашников Борислав Кондратиевич
Травкина Ангелина Леонидовна
Кочубей Роза Александровна
Шурдукова Антонина Родионовна
Голованова Полина Всеволодовна
Карчагина Каролина Святославовна
Золотухин Михей Гордеевич
Прокашева Анисья Павеловна
Кулешов Роман Георгиевич
Воронцов Яков Моисеевич
Рунов Марк Ульянович
Солодский Елизар Адамович
Васенин Фока Ерофеевич
Кидина Роза Данииловна
Кологреев Валерий Андреевич
Козариса Василиса Тимуровна
Тупицын Вацлав Святославович
Жариков Петр Модестович
Якубович Платон Иосифович
Нуряев Владилен Миронович
Бебчука Виктория Тимофеевна
Шамякин Гавриил Мартьянович
Елешев Аким Ираклиевич
Лагошина Каролина Яновна
Кантонистов Николай Куприянович
Мусин Михей Анатолиевич
Ямковой Анфиса Данииловна
Ажищенкова Инга Тимуровна
Окрокверцхов Иннокентий Яковович
Зууфина Ника Виталиевна
Янин Алексей Кондратович
Мацовкин Филипп Эдуардович
Камбарова Лада Марковна
Тимофеева Софья Мефодиевна
Зёмина Кристина Андрияновна
Сагунова Яна Яновна
Распутина Мария Геннадиевна
Стегнова Рада Трофимовна
Фанина Жанна Родионовна
Мосякова Инга Иосифовна
Шамякин Артемий Маркович
Драгомирова ﻿Агата Ефимовна
Ельченко Валерий Пахомович
Добролюбов Порфирий Севастьянович
Кругликова Елена Ростиславовна
Бабышев Осип Богданович
Дудника Ангелина Евгениевна
Бондарчука Агния Трофимовна
Кобелева Таисия Данииловна
Сапалёва Всеслава Игнатиевна
Дуболазов Всеволод Титович
Яшвили Агап Евграфович
Коллерова Анисья Василиевна
Палюлин Юрий Сигизмундович
Цыгвинцев Дмитрий Филимонович
Большаков Трофим Демьянович
Яндарбиева Софья Алексеевна
Валуев Лаврентий Адамович
Колотушкина Наталья Вячеславовна
Бруевича Жанна Казимировна
Масмеха Кира Несторовна
Меншикова Кира Василиевна
Шаршин Мстислав Сократович
Малафеев Харитон Кириллович
Завражина Виктория Брониславовна
Заболотный Самуил Семенович
Яскунов Фадей Макарович
Зимин Виссарион Григориевич
Крестьянинов Евгений Давидович
Земляков Клавдий Ростиславович
Соловьёв Андриян Прохорович
Якурин Илья Гаврилевич
Мещерякова Алина Кузьмевна
Катаева Бронислава Ираклиевна
Грязнов ﻿Август Матвеевич
Никулина Доминика Карповна
Марьин Геннадий Капитонович
Зюлёва Инга Игоревна
Лобана Ярослава Несторовна
Счастливцева Василиса Всеволодовна
Набоко Егор Капитонович
Аничков Всеслав Капитонович
Костюк Венедикт Святославович
Синдеева Ираида Яновна
Москвитина Ефросинья Феликсовна
Арданкина Оксана Мироновна
Казанцева Анастасия Игоревна
Якутин Виталий Брониславович
Безрукова Альбина Владленовна"
                .Split('\n');
            return data[rnd.Next(data.Length)];
        }

        #endregion


        /// <summary>
        /// Добавление лога в список.
        /// </summary>
        /// <param name="text">Текст лога.</param>
        public static void StartActionLogs(string text)
        {
            Log log = new Log(text);
            ActionLogs?.Invoke(log);
        }
    }
}
