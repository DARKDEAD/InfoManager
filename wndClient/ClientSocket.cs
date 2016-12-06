using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wndClient
{
    class ClientSocket
    {
        private const int port = 8888;
        private const string server = "127.0.0.1";
        public TcpClient client;
        public NetworkStream stream;
        public StringBuilder response;

        public void ConnectTCP(string Login)
        {

            //пытаемся установить соединение с удаленным сервером 
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(server, port);
                                
                StringBuilder response = new StringBuilder();
                NetworkStream stream = client.GetStream();

                // LoginForm f = new LoginForm();

                // преобразуем сообщение в массив байтов
                string message = String.Format("{0}: {1}", "Token", Login);
                byte[] data = Encoding.Unicode.GetBytes(message);

                //отправляем данные на сервер
                SendData(data);
                ReadData();

                //Console.WriteLine(response.ToString());

                // Закрываем потоки
                //CloseStream();
            }
            catch (SocketException e)
            {
                //return (e.Message);
            }
            catch (Exception e)
            {
                //return (e.Message);

            }
        }

        public bool SendData(byte[] data)
        {
            try
            {

                client.Client.Send(data, data.Length, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        //получаем данные с сервера
        public void ReadData()
        {           
            do
            {
                byte[] data = new byte[2560];
                int bytes = stream.Read(data, 0, data.Length);
                response.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable); // пока данные есть в потоке
        }

        // Закрываем потоки
        public void CloseStream()
        {
            try
            {
                stream.Close();
                client.Close();
            }
            catch (Exception)
            {

                //throw;
            }

            
        }
    }
}
