using System;
using System.Net;
using System.Net.Sockets;

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
