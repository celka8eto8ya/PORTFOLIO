using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;


namespace KAM___Kursovaya___IVsem
{
    public class Account : Vessel
    {
        // тип счёта
        TypeOfAccount typeOfAcc = new TypeOfAccount();

        // валюта счета
        CurrencyOfAccount curr;

        // дата последнего изменения счета
        private DateTime dateOfRefresh;

        // примечание к счёту
        private string notation;



        // создание даты обновления если она "<=" даты и времени на ПК
        public DateTime DateOfRefresh
        {
            get
            {
                return dateOfRefresh;
            }
            set
            {
                if (value <= DateTime.Now)
                {
                    dateOfRefresh = value;
                }
                else
                {
                    MessageBox.Show("Счет не может быть обновлен в будущем!", "Ошибка!");
                }
            }
        }

        // для записи примечания длиной не больше 50 символов
        public string Notation
        {
            get
            {
                return notation;
            }
            set
            {
                if (value.Length <= 50)
                {
                    notation = value;
                }
                else
                {
                    MessageBox.Show("Длина \"примечание\" превышает 50 символов", "Ошибка!");
                }
            }
        }

        // для записи примечания длиной не больше 50 символов
        public string Currency
        {
            get
            {
                return curr.Name;
            }
            set
            {
                if (value.Length <= 5 && value.Length > 0)
                {
                    curr.Name = value;
                }
                else
                {
                    MessageBox.Show("\"Валюта\" указана некорректно!", "Ошибка!");
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="typeOfAcc1"></param>
        /// <param name="curr1"></param>
        /// <param name="notation1"></param>
        // депозит
        public Account(string name1, TypeOfAccount typeOfAcc1, CurrencyOfAccount curr1, string notation1)
            : base(name1)
        {
            typeOfAcc = typeOfAcc1;// new TypeOfAccount("депозит",12,true,30);
            curr = curr1;
            DateOfRefresh = DateTime.Now;
            Notation = notation1;
        }

        // нулевой
        public Account()
            : base()
        {

        }

        // текущий
        public Account(string name1, string typeOfAccount, string notation1)
            : base(name1)
        {
            typeOfAcc.Name = typeOfAccount;// new TypeOfAccount("депозит",12,true,30);
            DateOfRefresh = DateTime.Now;
            Notation = notation1;
        }

        // валютный
        public Account(string name1, string typeOfAccount, CurrencyOfAccount curr1, string notation1)
            : base(name1)
        {
            typeOfAcc.Name = typeOfAccount;// new TypeOfAccount("депозит",12,true,30);
            curr = curr1;
            DateOfRefresh = DateTime.Now;
            Notation = notation1;
        }



        // Form1 - ("Добавить сумму / Изъять сумму ...")
        // изменить сумму счёта
        //
        public static void AddGetMoney(string name1, string sum1, string operation, int storId)
        {
            try
            {
                if (name1.Length > 0 && sum1.Length > 0)
                {
                    double sum2 = double.Parse(sum1);
                    MySqlConnection conn = new MySqlConnection(Bank.AccessInDB); // создается объект подключения (типо поток файловый)
                    conn.Open(); // открываем поток

                    string query0 = $"select Name from AccountsOfStorage where ( IdOfStorage= '{storId}' and Name='{name1}');";
                    MySqlCommand com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                    string name = Convert.ToString(com0.ExecuteScalar());

                    if (name.Length > 0)
                    {

                        query0 = $"select Sum from AccountsOfStorage where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name1}');";
                        com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                        double sum0 = Convert.ToDouble(com0.ExecuteScalar());

                        query0 = $"select Name from TypeOfAccount where ( IdOfAccount=(select Id from AccountsOfStorage where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name1}')));";
                        com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                        string typeOfAcc = Convert.ToString(com0.ExecuteScalar());

                        query0 = $"select Feedback from TypeOfAccount where ( IdOfAccount=(select Id from AccountsOfStorage where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name1}')));";
                        com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                        string Feedback = Convert.ToString(com0.ExecuteScalar());

                        if ((operation == "-" && sum0 >= sum2 && Feedback != "Безотзывный") || (operation == "+"))
                        {
                            string s = sum1.ToString();
                            s = s.Replace(",", ".");
                            string query1 = $"UPDATE AccountsOfStorage SET Sum=Sum{operation}{s} where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name1}');";

                            MySqlCommand com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                            com1.ExecuteScalar();
                            conn.Close(); // закрываем поток 

                            if (operation == "-")
                            {
                                MessageBox.Show($"Сумма ({sum2}) успешно изъята со счёта \"{name1}\"!", "Успех!");
                                UpdateDateRefresh(name1);
                            }
                            else if (operation == "+")
                            {
                                MessageBox.Show($"Сумма ({sum2}) успешно добавлена на счёт \"{name1}\"!", "Успех!");
                                UpdateDateRefresh(name1);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Сумма ({sum2}) превышает баланс счёта \"{name1}\" или счёт является безотзывным депозитом!", "Ошибка!");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Счёт ({name1}) не существует!", "Ошибка!");
                    }
                }
                else
                {
                    MessageBox.Show("Не указано \"Name1\" и/или \"Sum1\" !", "Ошибка!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        //
        // Выводит информацию о счетах
        //
        public static void Info(DataGridView dgv1)
        {
            try
            {
                dgv1.Rows.Clear();
                MySqlConnection conn = new MySqlConnection(Bank.AccessInDB); // создается объект подключения (типо поток файловый)
                conn.Open(); // открываем поток

                string query1 =
                "SELECT " +

                "AccountsOfStorage.Id," +
                "AccountsOfStorage.Name," +
                "AccountsOfStorage.Sum," +
                "AccountsOfStorage.Currency," +
                "AccountsOfStorage.DateOfCreate," +
                "AccountsOfStorage.DateOfRefresh," +
                "AccountsOfStorage.Notation," +

                "TypeOfAccount.Name," +
                "TypeOfAccount.Percent," +
                "TypeOfAccount.Feedback," +
                "TypeOfAccount.Duration, " +
                "TypeOfAccount.DaysLeft " +

                "FROM AccountsOfStorage " +
                "JOIN TypeOfAccount " +
                $"ON AccountsOfStorage.Id = TypeOfAccount.IdOfAccount and AccountsOfStorage.IdOfStorage='{Bank.IdOfCurrentStorage}';";

                MySqlCommand com = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                MySqlDataReader r1 = com.ExecuteReader(); // хранит все данные запроса (поток чтения)

                List<string[]> data = new List<string[]>();

                while (r1.Read())
                {
                    data.Add(new string[12]);

                    data[data.Count - 1][0] = r1[0].ToString();
                    data[data.Count - 1][1] = r1[1].ToString();
                    data[data.Count - 1][2] = r1[2].ToString();
                    data[data.Count - 1][3] = r1[3].ToString();
                    data[data.Count - 1][4] = r1[4].ToString();
                    data[data.Count - 1][5] = r1[5].ToString();
                    data[data.Count - 1][6] = r1[6].ToString();
                    data[data.Count - 1][7] = r1[7].ToString();
                    data[data.Count - 1][8] = r1[8].ToString();
                    data[data.Count - 1][9] = r1[9].ToString();
                    data[data.Count - 1][10] = r1[10].ToString();
                    data[data.Count - 1][11] = r1[11].ToString();

                }

                r1.Close();

                foreach (string[] s in data)
                {
                    dgv1.Rows.Add(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception!");
            }
        }



        // Form2 - btn ("Создать счёт")
        // внесение данных о счёте в БД
        //
        public void AddAccountInDB(string StringOfAccess)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(StringOfAccess); // создается объект подключения (типо поток файловый)
                conn.Open(); // открываем поток
                string query1 = "";

                if (typeOfAcc.Name == "Депозит")
                {

                    if (Name.Length > 0 && Currency.Length > 0 && typeOfAcc.Percent.ToString().Length > 0 && typeOfAcc.FeedBack.Length > 0 && typeOfAcc.Duration.ToString().Length > 0)
                    {
                        query1 = $"INSERT AccountsOfStorage (IdOfStorage,Name, Sum, Currency, DateOfCreate,DateOfRefresh,Notation)" +
                        $"VALUES('{Bank.IdOfCurrentStorage}','{Name}','{Sum}','{Currency}'," +
                        $"'{DateCreate.ToString($"yyyy-MM-dd HH:mm:ss")}','{DateOfRefresh.ToString("yyyy-MM-dd HH:mm:ss")}', '{Notation}' );";
                        MySqlCommand com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                        com1.ExecuteScalar();

                        query1 = $"SELECT Id from AccountsOfStorage where (Name='{Name}' and IdOfStorage='{Bank.IdOfCurrentStorage}') ";
                        com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                        int IdOfAccount = Convert.ToInt32(com1.ExecuteScalar());

                        query1 = $"INSERT TypeOfAccount (IdOfAccount,Name, Percent, Feedback, Duration,DaysLeft)" +
                        $"VALUES('{IdOfAccount}','{typeOfAcc.Name}','{typeOfAcc.Percent}','{typeOfAcc.FeedBack}','{typeOfAcc.Duration}','{typeOfAcc.DaysLeft}');";
                        com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                        com1.ExecuteScalar();

                        conn.Close(); // закрываем поток 

                        MessageBox.Show($"Счёт \"{Name}\" создано успешно!");
                    }
                    else
                    {
                        MessageBox.Show("Не все данные был указаны!", "Ошибка!");
                    }

                }
                else if (typeOfAcc.Name == "Текущий(только в BYN)")
                {
                    if (Name.Length > 0)
                    {
                        query1 = $"INSERT AccountsOfStorage (IdOfStorage,Name, Sum, Currency, DateOfCreate,DateOfRefresh,Notation)" +
                        $"VALUES('{Bank.IdOfCurrentStorage}','{Name}','{Sum}','BYN'," +
                        $"'{DateCreate.ToString("yyyy-MM-dd  HH:mm:ss")}','{DateOfRefresh.ToString("yyyy-MM-dd HH:mm:ss")}', '{Notation}' );";
                        MySqlCommand com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                        com1.ExecuteScalar();

                        query1 = $"SELECT Id from AccountsOfStorage where (Name='{Name}' and IdOfStorage='{Bank.IdOfCurrentStorage}') ";
                        com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                        int IdOfAccount = Convert.ToInt32(com1.ExecuteScalar());

                        query1 = $"INSERT TypeOfAccount (IdOfAccount,Name, Percent, Feedback, Duration,DaysLeft)" +
                        $"VALUES('{IdOfAccount}','{typeOfAcc.Name}','0','-','0','0');";
                        com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                        com1.ExecuteScalar();

                        conn.Close(); // закрываем поток 

                        MessageBox.Show($"Счёт \"{this.Name}\" создано успешно!");
                    }
                    else
                    {
                        MessageBox.Show("Не указано имя счёта!", "Ошибка!");
                    }

                }
                else if (typeOfAcc.Name == "Валютный")
                {
                    if (Name.Length > 0 && Currency.Length > 0)
                    {
                        query1 = $"INSERT AccountsOfStorage (IdOfStorage,Name, Sum, Currency, DateOfCreate,DateOfRefresh,Notation)" +
                        $"VALUES( '{Bank.IdOfCurrentStorage}','{Name}','{Sum}','{Currency}'," +
                        $"'{DateCreate.ToString("yyyy-MM-dd HH:mm:ss")}','{DateOfRefresh.ToString("yyyy-MM-dd HH:mm:ss")}', '{Notation}' );";
                        MySqlCommand com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                        com1.ExecuteScalar();

                        query1 = $"SELECT Id from AccountsOfStorage where (Name='{Name}' and IdOfStorage='{Bank.IdOfCurrentStorage}') ";
                        com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                        int IdOfAccount = Convert.ToInt32(com1.ExecuteScalar());

                        query1 = $"INSERT TypeOfAccount (IdOfAccount,Name, Percent, Feedback, Duration,DaysLeft)" +
                        $"VALUES('{IdOfAccount}','{typeOfAcc.Name}','0','-','0','0');";
                        com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                        com1.ExecuteScalar();

                        conn.Close(); // закрываем поток 

                        MessageBox.Show($"Счёт \"{this.Name}\" создано успешно!");
                    }
                    else
                    {
                        MessageBox.Show("Не указано имя счёта!", "Ошибка!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception!");

            }

        }



        // Form2 - "Создать счёт"
        // проверка на уникальность имени аккаунта
        //
        public bool CheckOnExclusiveAccountName(int storeId)
        {
            try
            {
                if (Name != null)
                {
                    MySqlConnection conn = new MySqlConnection(Bank.AccessInDB); // создается объект подключения (типо поток файловый)
                    conn.Open(); // открываем поток

                    string query1 = $"Select Name FROM AccountsOfStorage where (Name='{Name}' and IdOfStorage='{ storeId}');";


                    MySqlCommand com = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                    MySqlDataReader r1 = com.ExecuteReader(); // хранит все данные запроса (поток чтения)
                    DataTable table1 = new DataTable();
                    table1.Load(r1);       // Загружаем таблицу из базы данных

                    r1.Close();
                    conn.Close(); // закрываем поток 

                    if (table1.Rows.Count == 0)
                    {

                        return true;

                    }
                    else
                    {
                        //MessageBox.Show($"Счёт \"{Name}\" уже сужетствует!", "Ошика!");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show($"Имя аккаунта не указано!", "Ошибка!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}!", "Ошибка!");
                return false;
            }
        }




        // Form2 - btn ("Создать счёт")
        // удалени данных о счёте из БД
        //
        public static void DeleteAccountInDB(string name, int sotorId)
        {
            try
            {
                if (name.Length > 0)
                {
                    MySqlConnection conn = new MySqlConnection(Bank.AccessInDB); // создается объект подключения (типо поток файловый)
                    conn.Open(); // открываем поток

                    string query1 = $"DELETE FROM TypeOfAccount where ( IdOfAccount= (select Id from AccountsOfStorage where (Name='{name}' and IdOfStorage='{Bank.IdOfCurrentStorage}')));" +
                             $"DELETE FROM AccountsOfStorage    where ( IdOfStorage= '{sotorId}' and Name='{name}');";

                    MySqlCommand com = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                    com.ExecuteScalar();
                    conn.Close(); // закрываем поток 

                    MessageBox.Show($"Счёт {name} успешно удалён!", "Успех!");
                }
                else
                {
                    MessageBox.Show("Не указано имя для удаления!", "Ошибка!");
                }
            }
            catch
            {

            }
        }



        // Form1 - btn ("Конвертировать валюту счёта")
        // внесение данных о счёте в БД
        //
        public static void ChangeCurrency(string name, string nameOfCurr)
        {
            try
            {
                if (name.Length > 0 && nameOfCurr.Length > 0)
                {
                    MySqlConnection conn = new MySqlConnection(Bank.AccessInDB); // создается объект подключения (типо поток файловый)
                    conn.Open(); // открываем поток

                    string query0 = $"select Name from AccountsOfStorage where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name}');";
                    MySqlCommand com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                    string name0 = Convert.ToString(com0.ExecuteScalar());

                    if (name0.Length > 0)
                    {
                        query0 = $"select Currency from AccountsOfStorage where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name}');";
                        com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                        string curr1 = com0.ExecuteScalar().ToString();

                        query0 = $"select Sum from AccountsOfStorage where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name}');";
                        com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                        double sum1 = Convert.ToDouble(com0.ExecuteScalar());

                        query0 = $"select Name from TypeOfAccount where ( IdOfAccount= (select Id from AccountsOfStorage where (Name='{name}' and IdOfStorage='{Bank.IdOfCurrentStorage}')));";
                        com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                        string typeName = com0.ExecuteScalar().ToString();

                        if (typeName == "Валютный")
                        {
                            CurrencyOfAccount.ConvertOfCurrencies(curr1, nameOfCurr, ref sum1);
                            string s = sum1.ToString();
                            s = s.Replace(",", ".");

                            string query1 = $"UPDATE AccountsOfStorage SET Sum='{s}' ,Currency='{nameOfCurr}' where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name}');";
                            MySqlCommand com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
                            com1.ExecuteScalar();

                            MessageBox.Show("Конвертирование завершилось успешно!", "Успех!");
                            // изменение даты обновления счёта
                            UpdateDateRefresh(name);
                        }
                        else
                        {
                            MessageBox.Show("Конвертирование возможно только для счетов с типом \"Валютный\" !", "Ошибка!");
                        }
                        conn.Close(); // закрываем поток 
                    }
                    else
                    {
                        MessageBox.Show($"Счёт ({name}) не существует!", "Ошибка!");
                    }
                }
                else
                {
                    MessageBox.Show("Не указано имя \"Name1\" и/или валюта \"Currency1\" !", "Ошибка!");
                }
            }
            catch
            {

            }
        }



        // Form1 - btn ("Переместить сумму1 со счёта1 на счёт2")
        // 
        //
        public static void ReplaceMoney(string name1, string name2, string sum1)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(Bank.AccessInDB); // создается объект подключения (типо поток файловый)
                conn.Open(); // открываем поток

                string query0 = $"select Name from AccountsOfStorage where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name1}');";
                MySqlCommand com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                string name11 = Convert.ToString(com0.ExecuteScalar());

                query0 = $"select Name from AccountsOfStorage where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name2}');";
                com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                string name22 = Convert.ToString(com0.ExecuteScalar());

                query0 = $"select Currency from AccountsOfStorage where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name1}');";
                com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                string curr11 = Convert.ToString(com0.ExecuteScalar());

                query0 = $"select Currency from AccountsOfStorage where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name2}');";
                com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                string curr22 = Convert.ToString(com0.ExecuteScalar());

                query0 = $"select Sum from AccountsOfStorage where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name1}');";
                com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                double sum00 = Convert.ToDouble(com0.ExecuteScalar());

                if (name11.Length > 0 && name22.Length > 0 && double.Parse(sum1) <= sum00)
                {
                    double sum11 = double.Parse(sum1);

                    // изъятие суммы со счёта
                    AddGetMoney(name1, sum11.ToString(), "-", Bank.IdOfCurrentStorage);

                    CurrencyOfAccount.ConvertOfCurrencies(curr11, curr22, ref sum11);

                    // добавление суммы на счёт
                    AddGetMoney(name2, sum11.ToString(), "+", Bank.IdOfCurrentStorage);
                    conn.Close(); // закрываем поток 
                }
                else
                {
                    MessageBox.Show($"Счёт ({name1}) или ({name2}) не существует или указана сумма превышающая баланс счёта ({name1}) !", "Ошибка!");
                }
            }
            catch
            {

            }
        }



        // Form1 - timer3
        // проверяет не закончился ли срок депозита
        public static void CheckDeposit()
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(Bank.AccessInDB); // создается объект подключения (типо поток файловый)
                conn.Open(); // открываем поток

                string query0 = $"select MAX(Id) from AccountsOfStorage ;";
                MySqlCommand com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                int MaxId = Convert.ToInt32(com0.ExecuteScalar());

                query0 = $"select MIN(Id) from AccountsOfStorage ;";
                com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                int MinId = Convert.ToInt32(com0.ExecuteScalar());

                int DaysLeft = -1;
                double sum = -1;
                double percent = -1;
                double EndSum = -1;
                string DepName = "-1";
                string CurrAcc = "-1";
                int IdStor = -1;

                for (int i = MinId; i <= MaxId; i++)
                {
                    query0 = $"select DaysLeft from  TypeOfAccount where ( IdOfAccount= '{i}');";
                    com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                    DaysLeft = Convert.ToInt32(com0.ExecuteScalar());

                    query0 = $"select Name from  TypeOfAccount where ( IdOfAccount= '{i}');";
                    com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                    string TypeOfAcc = Convert.ToString(com0.ExecuteScalar());

                    if (DaysLeft == 0 && TypeOfAcc == "Депозит")
                    {
                        query0 = $"select Sum from AccountsOfStorage where ( Id='{i}');";
                        com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                        sum = Convert.ToDouble(com0.ExecuteScalar());

                        query0 = $"select Percent from  TypeOfAccount where ( IdOfAccount= '{i}');";
                        com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                        percent = Convert.ToInt32(com0.ExecuteScalar());

                        // вычисление суммы после завершения депозита
                        EndSum = sum + (sum * percent / 100);

                        query0 = $"select Name from AccountsOfStorage where ( Id='{i}');";
                        com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                        DepName = Convert.ToString(com0.ExecuteScalar());

                        query0 = $"select Currency from  AccountsOfStorage where ( Id= '{i}');";
                        com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                        CurrAcc = Convert.ToString(com0.ExecuteScalar());

                        query0 = $"select IdOfStorage from AccountsOfStorage where ( Id='{i}');";
                        com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                        IdStor = Convert.ToInt32(com0.ExecuteScalar());

                        Account acc1 = new Account(DepName + i + "Money", "Валютный", new CurrencyOfAccount(CurrAcc), "");

                        if (acc1.CheckOnExclusiveAccountName(IdStor))
                        {
                            acc1.AddAccountInDB(Bank.AccessInDB);
                        }

                        // добавление суммы на счёт
                        AddGetMoney(DepName + i + "Money", EndSum.ToString(), "+", IdStor);

                        // удаление строки о счёте и типе счёта из соответсвующих таблиц БД
                        DeleteAccountInDB(DepName, IdStor);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Счёт ({ex}) ");
            }
        }



        // Form1 - timer3
        // производит подсчёт количества дней до завершения депозита
        public static void CalcDaysLeftDeposit()
        {
            MySqlConnection conn = new MySqlConnection(Bank.AccessInDB); // создается объект подключения (типо поток файловый)
            conn.Open(); // открываем поток

            string query0 = $"select MAX(Id) from AccountsOfStorage ;";
            MySqlCommand com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
            int MaxId = Convert.ToInt32(com0.ExecuteScalar());

            query0 = $"select MIN(Id) from AccountsOfStorage ;";
            com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
            int MinId = Convert.ToInt32(com0.ExecuteScalar());

            for (int i = MinId; i <= MaxId; i++)
            {
                query0 = $"select DaysLeft from  TypeOfAccount where ( IdOfAccount= '{i}');";
                com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                int DaysLeft = Convert.ToInt32(com0.ExecuteScalar());

                query0 = $"select Duration from  TypeOfAccount where ( IdOfAccount= '{i}');";
                com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                int Duration = Convert.ToInt32(com0.ExecuteScalar());

                query0 = $"select DateOfCreate from  AccountsOfStorage where ( Id= '{i}');";
                com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                DateTime DateCreate = Convert.ToDateTime(com0.ExecuteScalar());

                query0 = $"select Name from  TypeOfAccount where ( IdOfAccount= '{i}');";
                com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                string TypeOfAcc = Convert.ToString(com0.ExecuteScalar());

                TimeSpan date0 = DateTime.Now - DateCreate;
                int AllSecond = date0.Seconds + (date0.Minutes * 60) + (date0.Hours * 3600) + (date0.Days * 24 * 3600);

                if (DaysLeft >= 0 && TypeOfAcc == "Депозит")
                {
                    int a = Duration - AllSecond;

                    if (a < 0)
                    {
                        a = 0;
                    }

                    query0 = $"UPDATE TypeOfAccount SET DaysLeft='{a}' where ( IdOfAccount= '{i}');";
                    com0 = new MySqlCommand(query0, conn); // создаем объект, который выполняет наш запрос
                    com0.ExecuteScalar();
                }
            }
        }



        // 
        // вычисляет дату изменения счёта
        //
        public static void UpdateDateRefresh(string name)
        {
            MySqlConnection conn = new MySqlConnection(Bank.AccessInDB); // создается объект подключения (типо поток файловый)
            conn.Open(); // открываем поток

            string query1 = $"UPDATE AccountsOfStorage SET DateOfRefresh='{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}'  where ( IdOfStorage= '{Bank.IdOfCurrentStorage}' and Name='{name}');";
            MySqlCommand com1 = new MySqlCommand(query1, conn); // создаем объект, который выполняет наш запрос
            com1.ExecuteScalar();
        }



    }
}
