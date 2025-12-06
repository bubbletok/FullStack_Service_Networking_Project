using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient
{
    class Client
    {
        static void Main(string[] args)
        {
            int num = 9;
            ClientBase client;
            switch(num)
            {
                case 3:
                    client = new echo_client();
                    break;
                case 4:
                    client = new echo_client_complete();
                    break;
                case 7:
                    client = new echo_client_multithread();
                    break;
                case 9:
                    client = new udp_echo_client_multithread();
                    break;
                default:
                    client = new echo_client_complete();
                    break;
            }
            client.Start();
        }
    }

    public class ClientBase()
    {
        public virtual void Start()
        {

        }
    }

}
