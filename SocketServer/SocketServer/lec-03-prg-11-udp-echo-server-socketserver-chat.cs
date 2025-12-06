using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    internal class udp_echo_server_socketserver_chat : ServerBase
    {
        private static List<EndPoint> group_queue = new List<EndPoint>();

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
                        byte[] bytes = new byte[1024];
                        EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        int numByte = serverSocket.ReceiveFrom(bytes, ref clientEndPoint);
                        string? recvCmd = Encoding.UTF8.GetString(bytes, 0, numByte).Trim();

                        // client registration and deregistration
                        if (recvCmd.StartsWith("#") || recvCmd == "quit")
                        {
                            if (recvCmd == "#REG")
                            {
                                Console.WriteLine($"> client registered {clientEndPoint}");
                                group_queue.Add(clientEndPoint);
                            }
                            else if (recvCmd == "#DEREG" || recvCmd == "quit")
                            {
                                if (group_queue.Contains(clientEndPoint))
                                {
                                    Console.WriteLine($"> client de-registered {clientEndPoint}");
                                    group_queue.Remove(clientEndPoint);
                                }
                            }
                        }
                        else
                        {
                            if (group_queue.Count == 0)
                            {
                                Console.WriteLine("> no clients to echo");
                            }
                            else if (!group_queue.Contains(clientEndPoint))
                            {
                                Console.WriteLine("> ignores a message from un-registered client");
                            }
                            else
                            {
                                // Forward a client message to whole clients (currently a broadcast)
                                Console.WriteLine($"> received ( {recvCmd} ) and echoed to {group_queue.Count} clients");
                                byte[] sendData = Encoding.UTF8.GetBytes(recvCmd);
                                foreach (EndPoint clientConn in group_queue)
                                {
                                    serverSocket.SendTo(sendData, clientConn);
                                }
                            }
                        }
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
