using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    internal class echo_server
    {
        public void Start()
        {
            try
            {
                const string HOST = "127.0.0.1";
                const ushort PORT = 65456;
                IPHostEntry ipHost = Dns.GetHostEntry(HOST);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint endPoint = new IPEndPoint(ipAddr, PORT);

                Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(endPoint);
                listener.Listen();

                Socket clientSocket = listener.Accept();
                Console.WriteLine($" client connected by IP address {0} with Port number {1}");

                while (true)
                {
                    byte[] bytes = new byte[1024];
                    int numByte = clientSocket.Receive(bytes);
                    string? data = Encoding.ASCII.GetString(bytes, 0, numByte); ;
                    Console.WriteLine($"> echoed: {data}");

                    byte[] message = Encoding.ASCII.GetBytes(data);
                    clientSocket.Send(message);

                    if (data == "quit") break;
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
                Console.WriteLine("> echo-server is de-activated");
            }
        }
    }
}