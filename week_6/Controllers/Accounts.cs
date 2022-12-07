using HttpServer_1.Attributes;
using HttpServer_1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1.Controllers
{
    [HttpController("accounts")]
    internal class Accounts
    {
        //GetAccounts, GetAccountById и SaveAccount

        [HttpGET(@"\d+")]
        public Account GetAccountById(int id)
        {
            var account = new Account();

            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            string sqlExpression = $"SELECT * FROM [dbo].[Accounts] WHERE id = {id}";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {

                    while (reader.Read()) // построчно считываем данные
                    {
                        account = new Account { Id = reader.GetInt32(0), Login = reader.GetString(1), Password = reader.GetString(2) };
                    }
                }

                reader.Close();
            }

            return account;

        }

        [HttpGET("")]
        public List<Account> GetAccounts()
        {
            List<Account> accounts = new List<Account>();


            //Подключение к БД

            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            string sqlExpression = "SELECT * FROM [dbo].[Accounts]";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {

                    while (reader.Read()) // построчно считываем данные
                    {
                        accounts.Add(new Account { Id = reader.GetInt32(0), Login = reader.GetString(1), Password = reader.GetString(2) });
                    }
                }

                reader.Close();
            }

            return accounts;
        }

        [HttpPOST("")]
        public void SaveAccount(string login, string password)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            string sqlExpression = $"INSERT INTO [dbo].[Accounts] (login, password) VALUES ('{login}', '{password}')";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
        }

        //Get /accounts/ - список аккаунтов в формате json
        //Get /accounts/{id} - информация об одном аккаунте
        //Post /accounts/ - добавлять инф на сервер - принимает параметры через body

        //надо брать все данные из бд
    }
}
