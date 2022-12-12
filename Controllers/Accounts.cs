using HttpServer_1.Attributes;
using HttpServer_1.Models;
using HttpServer_1.ORM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        private static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";
        private static AccountDAO accountDAO = new AccountDAO(connectionString);

        [HttpGET(@"\d+")]
        public Account? GetAccountById(int id)
            => accountDAO.Get(id);

        [HttpGET("")]
        public List<Account> GetAccounts()
            => accountDAO.GetAll();

        [HttpPOST("save")]
        public void SaveAccount(string login, string password)
            => accountDAO.Insert(login, password);

        //Get /accounts/ - список аккаунтов в формате json
        //Get /accounts/{id} - информация об одном аккаунте
        //Post /accounts/ - добавлять инф на сервер - принимает параметры через body

        //надо брать все данные из бд

        [HttpPOST("")]
        public bool Login(string login, string password)
        {
            return accountDAO.Check(login, password);
        }
    }
}
