using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient
{
    internal class udp_echo_client_multithread : ClientBase
    {
        public override void Start()
        {
            try
            {
                Console.WriteLine("> echo-client is activated");

                const string HOST = "127.0.0.1";
                const ushort PORT = 65456;
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(HOST), PORT);

                using (Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    clientSocket.Bind(new IPEndPoint(IPAddress.Any, 0));

                    Thread clientThread = new Thread(() => RecvHandler(clientSocket));
                    clientThread.IsBackground = true;
                    clientThread.Start();

                    while (true)
                    {
                        Console.Write("> ");
                        string? sendMsg = Console.ReadLine();
                        if (!string.IsNullOrEmpty(sendMsg))
                        {
                            byte[] messageSent = Encoding.UTF8.GetBytes(sendMsg);
                            clientSocket.SendTo(messageSent, remoteEndPoint);
                        }
                        if (sendMsg == "quit") break;
                    }
                }
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

        private void RecvHandler(Socket clientSocket)
        {
            try
            {
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {
                    byte[] messageReceived = new byte[1024];
                    int byteRecv = clientSocket.ReceiveFrom(messageReceived, ref remoteEndPoint);
                    string recvData = Encoding.UTF8.GetString(messageReceived, 0, byteRecv);
                    Console.WriteLine($"> received: {recvData}");
                    if (recvData == "quit") break;
                }
            }
            catch (SocketException)
            {
                // Socket closed
            }
        }
    }
}