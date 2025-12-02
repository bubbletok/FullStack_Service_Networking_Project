using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient
{
    internal class echo_client_multithread : ClientBase
    {
        public override void Start()
        {
            const string HOST = "127.0.0.1";
            const ushort PORT = 65456;

            Console.WriteLine("> echo-client is activated");

            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(HOST);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, PORT);

                using (Socket clientSocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    clientSocket.Connect(localEndPoint);

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
                            clientSocket.Send(messageSent);
                        }
                        if (sendMsg == "quit") break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"> connect() failed by exception: {e.Message}");
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
                while (true)
                {
                    byte[] messageReceived = new byte[1024];
                    int byteRecv = clientSocket.Receive(messageReceived);
                    string recvData = Encoding.UTF8.GetString(messageReceived, 0, byteRecv);
                    Console.WriteLine($"> received: {recvData}");
                    if (recvData == "quit") break;
                }
            }
            catch (SocketException)
            {
                // Socket closed. Nothing to print out
            }
        }
    }
}
