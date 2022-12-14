using HttpServer_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1.ORM
{
    internal class AccountDAO
    {
        private MyORM orm;
        public AccountDAO(string connectionString)
        {
            orm = new MyORM(connectionString, "Accounts");
        }

        public List<Account> GetAll()
            => orm.Select<Account>();

        public Account? Get(int id)
            => orm.Select<Account>(id);

        public void Insert(string login, string password)
            => orm.Insert(new Account() { Login = login, Password = password });

        public void Delete(int id)
            => orm.Delete(id);

        public void Update(int id, string login, string password)
            => orm.Update(id, new Account() { Login = login, Password = password });
        

        public void ChangeLogin(int id, string login)
        {
            var account = orm.Select<Account>(id);
            account.Login = login;
            orm.Update(id, account);
        }

        public void ChangePassword(int id, string password)
        {
            var account = orm.Select<Account>(id);
            account.Password = password;
            orm.Update(id, account);
        }
    }
}
