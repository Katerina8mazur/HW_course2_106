using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1
{
    internal struct Session
    {
        //{Id, AccountId, Login, CreateDateTime}
        public Guid Id;
        public int AccountId;
        public string Login;
        public DateTime CreateDateTime;


        public Session(Guid id, int accountId, string login, DateTime createDateTime)
        {
            Id = id;
            AccountId = accountId;
            Login = login;
            CreateDateTime = createDateTime;
        }
    }
}
