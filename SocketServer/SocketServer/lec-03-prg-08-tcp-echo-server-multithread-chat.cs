using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    internal class echo_server_multithread_chat : ServerBase
    {
        private Socket? listener;
        private volatile bool isRunning = true;

        //DB to register all client's socket information
        private static List<Socket> group_queue = new List<Socket>();
        private static readonly object lockObj = new object();

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

                listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(endPoint);
                listener.Listen();

                // Start a thread server
                Thread serverThread = new Thread(ServerLoop);
                serverThread.IsBackground = true;
                serverThread.Start();
                Console.WriteLine($"> server loop running in thread (main thread): {serverThread.Name}");

                // Server termination when all clients are disconnected
                int baseThreadCount = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
                while (true)
                {
                    Console.Write("> ");
                    string? msg = Console.ReadLine();
                    if (msg == "quit")
                    {
                        int currentThreadCount = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
                        if (currentThreadCount <= baseThreadCount + 2)
                        {
                            Console.WriteLine("> stop procedure started");
                            isRunning = false;
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"> active threads are remained : {currentThreadCount - baseThreadCount} threads");
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
                listener?.Close();
            }
        }

        private void ServerLoop()
        {
            while (isRunning)
            {
                try
                {
                    Socket clientSocket = listener!.Accept();
                    Thread clientThread = new Thread(() => HandleClient(clientSocket));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                catch (SocketException)
                {
                    break;
                }
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            try
            {
                IPEndPoint? clientEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
                Console.WriteLine($"> client connected by IP address {clientEndPoint?.Address} with Port number {clientEndPoint?.Port}");

                // Add a new client connection to client DB
                lock (lockObj)
                {
                    group_queue.Add(clientSocket);
                }

                while (true)
                {
                    byte[] bytes = new byte[1024];
                    int numByte = clientSocket.Receive(bytes);
                    string? data = Encoding.UTF8.GetString(bytes, 0, numByte);

                    if (data == "quit")
                    {
                        // Deregister a disconnected client from client DB
                        lock (lockObj)
                        {
                            group_queue.Remove(clientSocket);
                        }
                        break;
                    }
                    else
                    {
                        // Send a client message to all clients
                        lock (lockObj)
                        {
                            Console.WriteLine($"> received ( {data} ) and echoed to {group_queue.Count} clients");
                            foreach (Socket conn in group_queue)
                            {
                                conn.Send(Encoding.UTF8.GetBytes(data));
                            }
                        }
                    }
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
