using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronStrifeMasterServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new MasterServer();
            server.Start();
            while (server.isRunning)
            {

            }

        }
    }
}
