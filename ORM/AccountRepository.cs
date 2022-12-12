using HttpServer_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1.ORM
{
    internal class AccountRepository
    {
        private MyORM orm;
        private List<Account> accounts;



        public AccountRepository(string connectionString)
        {
            orm = new MyORM(connectionString, "Accounts");
            accounts = orm.Select<Account>();
        }

        public List<Account> GetAll()
        {
            var copy = new Account[accounts.Count];
            accounts.CopyTo(copy);
            return copy.ToList();
        }

        public Account? Get(int id)
            => accounts.FirstOrDefault(a => a.Id == id);

        public void Insert(string login, string password)
        {
            var account = new Account() { Login = login, Password = password };
            orm.Insert(account);
            accounts.Add(account);
        }

        public void Delete(int id)
        {
            orm.Delete(id);
            accounts.Remove(Get(id));
        }

        public void Update(int id, string login, string password)
        {
            var account = new Account() { Login = login, Password = password };
            orm.Update(id, account);
            accounts[accounts.IndexOf(Get(id))] = account;
        }

        public void ChangeLogin(int id, string login)
        {
            var account = orm.Select<Account>(id);
            account.Login = login;
            orm.Update(id, account);
            accounts[accounts.IndexOf(Get(id))] = account;
        }

        public void ChangePassword(int id, string password)
        {
            var account = orm.Select<Account>(id);
            account.Password = password;
            orm.Update(id, account);
            accounts[accounts.IndexOf(Get(id))] = account;
        }
    }
}
