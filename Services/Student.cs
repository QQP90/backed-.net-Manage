using Microsoft.Data.SqlClient;
using System.Collections;

namespace WebApplication1.Services
{
    public class Student
    {
        public int id { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public int[] asd = new int[10];
        string pw = "adasadadasdsadxcvcxv";

        public Student(int id, string name, int age)
        {
            this.id = id;
            this.name = name;
            this.age = age;
        }

        public string Eat()
        {
            Console.WriteLine(name + "吃吃吃");
            ArrayList arr = new ArrayList();
            arr.Add("asdasd");
            arr.Add(123);
            Console.WriteLine(arr[0]);

            List<int> list = new List<int>();
            list.Add(123);
            list.Add(456);
            list.Remove(123);

            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict.Add("asdasd", 123);
            foreach (KeyValuePair<string, int> item in dict)
            {
                Console.WriteLine(item.Key + ":" + item.Value);
            }
            return "Eating";
        }
        public string GetMessage()
        {
            return "Hello World!";
        }
        public string GetMessage(string name)
        {
            return $"Hello {name}!";
        }
        /*
         测试数据库连接池
         */
        public void TestConnection()
        {
            string connStr1 = "Data Source=localhost;Initial Catalog=luda;User ID=luda;Password=395353;Max Pool Size=5;TrustServerCertificate=True;";
            string connStr2 = "Data Source=localhost;Initial Catalog=luda; User ID=luda;Password=395353;Max Pool Size=5;TrustServerCertificate=True;";
            string connStr3 = "Data Source=localhost;Initial Catalog=luda;User ID=luda;Password=395353;Max Pool Size=5;TrustServerCertificate=True;";
            for (int i = 0; i < 5; i++)
            {
                SqlConnection conn2 = new SqlConnection(connStr2);
                conn2.Open();
                Console.WriteLine($"conn2第{i + 1}个连接已打开");
            }
            for (int i = 0; i < 5; i++)
            {

                SqlConnection conn1 = new SqlConnection(connStr1);
                conn1.Open();
                Console.WriteLine($"conn1第{i+1}个连接已打开");
                SqlConnection conn3 = new SqlConnection(connStr3);
                conn3.Open();
                Console.WriteLine($"conn3第{i + 1}个连接已打开");
            }

        }
    }
}
