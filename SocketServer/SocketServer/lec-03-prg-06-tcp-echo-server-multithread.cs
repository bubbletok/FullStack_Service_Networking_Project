using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    internal class echo_server_multithread : ServerBase
    {
        private Socket? listener;
        private volatile bool isRunning = true; // volatile for synchronizing in multi-thread
        private int clientThreadNum = 0;
        private readonly object lockObj = new object(); // locking variable for race-condition

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

                Thread serverThread = new Thread(MainLoop);
                serverThread.Name = $"Thread-{serverThread.ManagedThreadId}";
                serverThread.IsBackground = true;
                serverThread.Start();
                Console.WriteLine($"> server loop running in thread (main thread): {serverThread.Name}");

                while (true)
                {
                    Console.Write("> ");
                    string? msg = Console.ReadLine();
                    if (msg == "quit")
                    {
                        lock (lockObj)
                        {
                            if (clientThreadNum == 0)
                            {
                                Console.WriteLine("> stop procedure started");
                                isRunning = false;
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"> active threads are remained : {clientThreadNum} threads");
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
                listener?.Close();
            }
        }

        private void MainLoop()
        {
            while (isRunning)
            {
                try
                {
                    Socket clientSocket = listener!.Accept();
                    lock (lockObj)
                    {
                        clientThreadNum++;
                    }
                    Thread clientThread = new Thread(() => HandleClient(clientSocket));
                    clientThread.Name = $"Thread-{clientThread.ManagedThreadId}";
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

                while (true)
                {
                    byte[] bytes = new byte[1024];
                    int numByte = clientSocket.Receive(bytes);
                    string? data = Encoding.UTF8.GetString(bytes, 0, numByte);
                    Thread curThread = Thread.CurrentThread;
                    Console.WriteLine($"> echoed: {data} by {curThread.Name}");
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
            finally
            {
                lock (lockObj)
                {
                    clientThreadNum--;
                }
            }
        }
    }
}
