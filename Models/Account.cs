 using HttpServer_1.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1.Models
{
    internal class Account
    {
        public int Id { get; set; }

        [DBField("login")]
        public string Login { get; set; }

        [DBField("password")]
        public string Password { get; set; }


        public Account(int id, string login, string password)
        {
            Id = id;
            Login = login;
            Password = password;
        }

        public Account()
        {
        }
    }
}
