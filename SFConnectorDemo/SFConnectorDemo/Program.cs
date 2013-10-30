using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFRESTConnector;

namespace SFConnectorDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SFRESTConnection.Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
