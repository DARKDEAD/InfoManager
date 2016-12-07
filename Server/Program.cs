using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Lidgren.Network;
using SamplesCommon;
using System.Threading;

namespace Server
{
    static class Program
    {

        private static frmServer s_form;
        private static NetServer s_server;
        private static NetPeerSettingsWindow s_settingsWindow;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            s_form = new frmServer();

            // set up network
            NetPeerConfiguration config = new NetPeerConfiguration("Login");
            config.MaximumConnections = 100;

            int port;
            Int32.TryParse(s_form.txtPort.Text, out port);
            config.Port = port;

            s_server = new NetServer(config);

            Application.Idle += new EventHandler(Application_Idle);
            Application.Run(s_form);

        }
        // called by the UI
        public static void StartServer()
        {
            s_server.Start();
        }
        private static void Output(string text)
        {
            NativeMethods.AppendText(s_form.richTextBox1, text);
        }

        // отключаем сервер
        public static void Shutdown()
        {
            s_server.Shutdown("Соединение закрыто со сторны сервера.");
        }
        private static void Application_Idle(object sender, EventArgs e)
        {
            while (NativeMethods.AppStillIdle)
            {
                NetIncomingMessage im;
                while ((im = s_server.ReadMessage()) != null)
                {
                    // handle incoming message
                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string text = im.ReadString();
                            Output(text);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            string reason = im.ReadString();
                            Output(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            UpdateConnectionsList();
                            break;
                        case NetIncomingMessageType.Data:
                            // входящее сообщение от клиентов
                            string chat = im.ReadString();

                            Output("Broadcasting '" + chat + "'");

                            // broadcast this to all connections, except sender
                            List<NetConnection> all = s_server.Connections; // get copy
                            all.Remove(im.SenderConnection);

                            if (all.Count > 0)
                            {
                                NetOutgoingMessage om = s_server.CreateMessage();
                                om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
                                s_server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            break;
                        default:
                            Output("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                            break;
                    }
                }
                Thread.Sleep(1);
            }
        }
        private static void UpdateConnectionsList()
        {
            s_form.lstClients.Items.Clear();

            foreach (NetConnection conn in s_server.Connections)
            {
                string str = NetUtility.ToHexString(conn.RemoteUniqueIdentifier) + " from " + conn.RemoteEndPoint.ToString() + " [" + conn.Status + "]";
                s_form.lstClients.Items.Add(str);
            }
        }

    }
}
