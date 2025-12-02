using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketClient
{
    internal class echo_client_complete : ClientBase
    {
        public override void Start()
        {
            try
            {
                Console.WriteLine("> echo-client is activated");

                const string HOST = "127.0.0.1";
                const ushort PORT = 65456;
                IPHostEntry ipHost = Dns.GetHostEntry(HOST);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, PORT);

                Socket clientSocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(localEndPoint);

                while (true)
                {
                    Console.Write("> ");
                    string? sendMsg = Console.ReadLine();
                    if (!string.IsNullOrEmpty(sendMsg))
                    {
                        byte[] messageSent = Encoding.UTF8.GetBytes(sendMsg);
                        clientSocket.Send(messageSent);
                    }

                    byte[] messageReceived = new byte[1024];
                    int byteRecv = clientSocket.Receive(messageReceived);
                    Console.WriteLine($"> received: {Encoding.UTF8.GetString(messageReceived, 0, byteRecv)}");

                    if (sendMsg == "quit") break;
                }

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Console.WriteLine("> echo-client is de-activated");
            }
        }
    }
}
