using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    internal class udp_echo_server_socketserver : ServerBase
    {
        public override void Start()
        {
            const string HOST = "127.0.0.1";
            const ushort PORT = 65456;

            Console.WriteLine("> echo-server is activated");

            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(HOST), PORT);

                using (Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    serverSocket.Bind(localEndPoint);

                    while (true)
                    {
                        // [=start=]
                        byte[] bytes = new byte[1024];
                        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        int numByte = serverSocket.ReceiveFrom(bytes, ref remoteEndPoint);
                        string? data = Encoding.UTF8.GetString(bytes, 0, numByte).Trim();
                        Console.WriteLine($"> echoed: {data}");
                        serverSocket.SendTo(Encoding.UTF8.GetBytes(data), remoteEndPoint);
                        // [==end==]
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
    }
}
