using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Services.Client;
using ADOProxy.Service1;

namespace ADOProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost:56023/TeremokService1.svc/"));
            var result= ctx.Execute<string>(new Uri(@"t_Teremok/$count", UriKind.Relative)).First();
        }

        private static void DeleteItem()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost:56023/TeremokService1.svc/"));
            t_Teremok terem = ctx.Execute<t_Teremok>(new Uri("t_Teremok(470)", UriKind.Relative)).First();
            terem.teremok_name = "Тестовый 22";
            ctx.DeleteObject(terem);
            ctx.SaveChanges();
        }

        private static void UpdateItem()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost:56023/TeremokService1.svc/"));
            t_Teremok terem = ctx.Execute<t_Teremok>(new Uri("t_Teremok(470)", UriKind.Relative)).First();
            terem.teremok_name = "Тестовый 22";
            ctx.UpdateObject(terem);
            ctx.SaveChanges();
        }

        private static void AddElement()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost:56023/TeremokService1.svc/"));
            Service1.t_Teremok terem = new Service1.t_Teremok();
            terem.teremok_1C = "testres2";
            terem.teremok_use_ARM = false;
            terem.teremok_name = "Тестовый 2";
            terem.teremok_city = "2";
            ctx.AddObject("t_Teremok", terem);
            ctx.SaveChanges();
        }

        private static void GetWforServ()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost:56023/TeremokService1.svc/"));
            var Teremok = ctx.Execute<Service1.t_Teremok>(new Uri("t_Teremok", UriKind.Relative));
            foreach (var ter in Teremok)
            {
                Console.WriteLine(ter.teremok_1C);
            }
            Console.ReadKey();
        }
    }
}
