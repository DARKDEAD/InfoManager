using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using Lidgren.Network;

using SamplesCommon;


namespace Client
{
    static class Program
    {
        private static NetClient s_client;
        private static NetPeer s_peer;
        private static frmLogin s_form;
        private static NetPeerSettingsWindow s_settingsWindow;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            s_form = new frmLogin();

            NetPeerConfiguration config = new NetPeerConfiguration("Login");
            config.AutoFlushSendQueue = false;
            s_client = new NetClient(config);

            s_client.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));

            TryConnect(); //попытка соединения с сервером

            Application.Run(s_form);

            s_client.Shutdown("Bye");

        }
        private static void Output(string text)
        {
            NativeMethods.AppendText(s_form.richTextBox1, text);
        }

        public static void TryConnect()
        {

            // s_form.DisableInput();
            int port;
            Int32.TryParse(s_form.textBox3.Text, out port);
            Program.Connect(s_form.textBox4.Text, port);            

        }

        public static void CheckLogin(string Login_Pass)
        {
            Program.Send(Login_Pass);
            //s_client.Shutdown("Bay");
        }

        public static void Connect(string host, int port)
        {
            s_client.Start();
            NetOutgoingMessage hail = s_client.CreateMessage("This is the hail message");
            s_client.Connect(host, port, hail);            
        }
        // called by the UI
        public static void Send(string text)
        {
            NetOutgoingMessage om = s_client.CreateMessage(text);

            s_client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);

            Output("Sending '" + text + "'");
            s_client.FlushSendQueue();
        }

        public static void GotMessage(object peer)
        {

            NetIncomingMessage im;
            while ((im = s_client.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        s_form.toolStripStatusLabel1.Text = text;
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                            s_form.EnableInput();
                        else
                            s_form.DisableInput();

                        if (status == NetConnectionStatus.Disconnected)
                            s_form.toolStripStatusLabel1.Text = "Соединение с сервером...";

                        string reason = im.ReadString();
                        Output(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        string chat = im.ReadString();
                        Output(chat);
                        break;
                    default:
                        Output("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
            }

        }

    }
}
