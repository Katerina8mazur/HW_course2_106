using HttpServer_1.Attributes;
using HttpServer_1.Models;
using HttpServer_1.ORM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
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

        [HttpGET(@"^\d+$")]
        public MethodResponse<Account?> GetAccountById(int id)
            => new MethodResponse<Account?>(accountDAO.Get(id));

        [HttpGET("^$")]
        [OnlyForAuthorized]
        public MethodResponse<List<Account>>  GetAccounts()
            => new MethodResponse<List<Account>>(accountDAO.GetAll());

        [HttpPOST("^save$")]
        public MethodResponse SaveAccount(string login, string password)
        {
            accountDAO.Insert(login, password);
            return new MethodResponse();
        }

        //Get /accounts/ - список аккаунтов в формате json
        //Get /accounts/{id} - информация об одном аккаунте
        //Post /accounts/ - добавлять инф на сервер - принимает параметры через body

        //надо брать все данные из бд

        [HttpPOST("^$")]
        public MethodResponse<bool> Login(string login, string password)
        {
            var accountId = accountDAO.Check(login, password);

            Cookie? cookie = null;
            if (accountId >= 0)
            {
                var session = SessionManager.CreateSession(accountId, login);
                cookie = new Cookie("SessionId", session.Id.ToString());
            }    

            return new MethodResponse<bool>(accountId >= 0, cookie);
        }

        [HttpGET("^info$")]
        [OnlyForAuthorized(needAccountId: true)]
        public MethodResponse<Account> GetAccountInfo(string _, int id)
            => GetAccountById(id);
    }
}
