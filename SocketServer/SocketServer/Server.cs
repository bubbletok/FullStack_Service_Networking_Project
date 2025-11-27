using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    class Server
    {
        static void Main(string[] args)
        {
            int num = 3;
            ServerBase server;
            switch (num)
            {
                case 3:
                    server = new echo_server();
                    break;
                case 4:
                    server = new echo_server_complete();
                    break;
                default:
                    server = new echo_server_complete();
                    break;
            }
            server.Start();
        }
    }

    public class ServerBase
    {
        public virtual void Start()
        {
             
        }
    }
}
