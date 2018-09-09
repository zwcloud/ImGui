using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace ImGui
{
    public class EchoLogger : ILogger
    {
        private static TcpClient client;
        private static NetworkStream stream;

        public static void Show()
        {
            client = new TcpClient("127.0.0.1", 13000);
            stream = client.GetStream();
        }

        public static void Hide()
        {
            //dummy
        }

        public static void Close()
        {
            SendMessage("q\n");
            stream.Close();
            client.Close();
        }

        public void Clear()
        {
            //dummy
            SendMessage("cls\n");
        }

        private static void SendMessage(string message)
        {
            if (!stream.CanWrite)
            {
                return;
            }
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message+'\n');
            stream.Write(data, 0, data.Length);
        }

        public void Msg(string format, params object[] args)
        {
            SendMessage(string.Format(format, args));
        }

        public void Warning(string format, params object[] args)
        {
            SendMessage(string.Format(format, args));
        }

        public void Error(string format, params object[] args)
        {
            SendMessage(string.Format(format, args));
        }
    }
}