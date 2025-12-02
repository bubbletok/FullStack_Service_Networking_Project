using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    internal class echo_server_socketserver : ServerBase
    {
        public override void Start()
        {
            const string HOST = "127.0.0.1";
            const ushort PORT = 65456;

            Console.WriteLine("> echo-server is activated");

            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(HOST);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint endPoint = new IPEndPoint(ipAddr, PORT);

                using (Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    listener.Bind(endPoint);
                    listener.Listen();

                    while (true)
                    {
                        Socket clientSocket = listener.Accept();
                        HandleClient(clientSocket);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Console.WriteLine("> echo-server is de-activated");
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            IPEndPoint? clientEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"> client connected by IP address {clientEndPoint?.Address} with Port number {clientEndPoint?.Port}");

            try
            {
                while (true)
                {
                    byte[] bytes = new byte[1024];
                    int numByte = clientSocket.Receive(bytes);
                    string? data = Encoding.UTF8.GetString(bytes, 0, numByte);
                    Console.WriteLine($"> echoed: {data}");
                    clientSocket.Send(Encoding.UTF8.GetBytes(data));
                    if (data == "quit") break;
                }

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
