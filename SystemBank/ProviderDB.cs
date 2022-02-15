using System;
using System.Data.SqlClient;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SystemBank
{
    public static class ProviderDB
    {
        private static SqlConnection connection;
        private static SqlConnectionStringBuilder strCon;

        static ProviderDB()
        {
            var fileName = @"settings_connection.json";
            if (!File.Exists(fileName))
            {
                var js = new JObject();
                var sett = new JObject();
                js["settings_connection"] = sett;
                sett["data_source"] = @"localhost\SQLEXPRESS";
                sett["data_base"] = "SkillboxDB";
                File.WriteAllText("settings_connection.json", js.ToString());
            }

            var settings = JObject.Parse(File.ReadAllText(fileName))["settings_connection"];
            var dataSource = settings["data_source"].ToString();
            var dataBase = settings["data_base"].ToString();

            strCon = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                InitialCatalog = dataBase,
                IntegratedSecurity = true
            };

            connection = new SqlConnection(strCon.ConnectionString);


            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Настройки подключения хранятся в файле \"settings_connection.json\"");
            }
        }

        public static void ExecuteNonQuery(string sql, string info)
        {
            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var msg = $"Error in:\n{info}\n{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}\n{ex.Data}";
                MessageBox.Show(msg);
            }
            
        }

        public static SqlDataReader ExecuteQuery(string sql, string info)
        {
            try
            {
                SqlCommand command = new SqlCommand(sql, connection);
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                var msg = $"Error in:\n{info}\n{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}\n{ex.Data}";
                MessageBox.Show(msg);
                return null;
            }
        }

        public static void Close()
        {
            connection.Close();
        }



    }
}
