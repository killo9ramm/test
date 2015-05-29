using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SocketConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            bool Flag = true;
            while(Flag)
            {
                string s = Console.ReadLine();
                switch (s)
                {
                    case "1":
                        sendAdd();
                        break;
                    case "2":
                        sendPayment();
                        break;
                    case "3":
                        sendPrint();
                        break;
                    case "0":
                        Flag = false;
                        break;
                }
            }

            //try
            //{
            //    SendMessageFromSocket(11000);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
            //finally
            //{
            //    Console.ReadLine();
            //}
        }

        private static void sendPrint()
        {
            SendMessageFromSocket(27100,"1111");
        }

        private static void sendPayment()
        {
            SendMessageFromSocket(27100, GetMessage("_payment.txt"));
        }

        private static string GetMessage(string p)
        {
           return File.ReadAllText(p);
        }

        private static void sendAdd()
        {
            SendMessageFromSocket(27100, GetMessage("_add.txt"));
        }

        
        static void SendMessageFromSocket(int port,string message_)
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 27100);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Настройка разрешений
            //SocketPermission permisSocket = new SocketPermission(
            //    NetworkAccess.Connect, TransportType.Tcp, "localhost",
            //    SocketPermission.AllPorts);

            //permisSocket.PermitOnly();

            

            try
            {
                // Соединяем сокет с удаленной точкой
                sender.Connect(ipEndPoint);
                
                string message = message_;

                byte[] msg = UTF8Encoding.Default.GetBytes(message);
                    //Encoding.UTF8.GetBytes(message);

                // Отправляем данные через сокет
                int bytesSent = sender.Send(msg);

                // Получаем ответ от сервера
                //int bytesRec = sender.Receive(bytes);

               // Console.WriteLine("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));

                // Освобождаем сокет
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (sender.Connected)
                {
                    // Освобождаем сокет
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
            }
        }

    }
}
