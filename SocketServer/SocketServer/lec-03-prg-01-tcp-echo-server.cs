using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    internal class echo_server : ServerBase
    {
        public override void Start()
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
            IPEndPoint? clientEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($" client connected by IP address {clientEndPoint?.Address} with Port number {clientEndPoint?.Port}");

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
            Console.WriteLine("> echo-server is de-activated");
        }
    }
}