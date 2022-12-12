using HttpServer_1.Attributes;
using HttpServer_1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer_1.ORM
{
    internal class MyORM
    {
        public MyORM(string connectionString, string tableName)
        {
            this.connectionString = connectionString;
            this.tableName = tableName;
        }

        private string connectionString;
        private string tableName;

        public List<T> Select<T>()
        {
            List<T> result = new List<T>();


            //Подключение к БД

            //  @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";

            string sqlExpression = $"SELECT * FROM [dbo].[{tableName}]";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {

                    while (reader.Read()) // построчно считываем данные
                    {
                        var item = Activator.CreateInstance(typeof(T), GetValues(reader));
                        
                        result.Add((T)item);
                    }
                }

                reader.Close();
            }

            return result;
        }

        public T? Select<T>(int id)
        {
            T? result = default;

            string sqlExpression = $"SELECT * FROM [dbo].[{tableName}] WHERE id = {id}";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {

                    while (reader.Read()) // построчно считываем данные
                    {
                        var item = Activator.CreateInstance(typeof(T), GetValues(reader));

                        result = (T)item;
                    }
                }

                reader.Close();
            }

            return result;
        }

        public void Insert<T>(T item)
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute(typeof(DBField)) != null);

            var columns = properties
                .Select(p => ((DBField)p.GetCustomAttribute(typeof(DBField))).ColumnName);

            var values = properties
                .Select(p => $"'{p.GetValue(item)}'");

            // ["login", "password", "name","age"]
            // "login,password,name,age"

            // ["katya777", 12345678, "Katya", 19]
            // ["'katya777'", "'12345678'", "'Katya'", "'19'"]
            // "'katya777','12345678','Katya','19'"

            string sqlExpression = $"INSERT INTO [dbo].[{tableName}] ({string.Join(',', columns)}) VALUES ({string.Join(',', values)})";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
        }

        public void Update<T>(int id, T item)
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute(typeof(DBField)) != null);

            var columns = properties
                .Select(p => ((DBField)p.GetCustomAttribute(typeof(DBField))).ColumnName);

            var values = properties
                .Select(p => $"'{p.GetValue(item)}'");

            var pairs = columns
                .Zip(values)
                .Select(p => $"{p.First} = {p.Second}");

            // ["login", "password", "name","age"]
            // ["'katya777'", "'12345678'", "'Katya'", "'19'"]

            // [("login", "'katya777'"), ("password", "'12345678'"), ("name", "'Katya'"), ("age","'19'")]
            // ["login = 'katya777'", "password = '12345678'", ...]


            string sqlExpression = $"UPDATE [dbo].[{tableName}] SET {string.Join(',', pairs)} WHERE id = {id}";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            string sqlExpression = $"DELETE FROM [dbo].[{tableName}] WHERE id = {id}";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
        }


        private object[] GetValues(SqlDataReader reader)
        {
            var result = new object[reader.FieldCount];
            for(int i = 0; i < reader.FieldCount; i++)
                result[i] = reader.GetValue(i);
            return result;  
        }
    }
}
