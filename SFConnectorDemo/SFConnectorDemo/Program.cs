using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFNetRestLib;

namespace SFConnectorDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string jobId=string.Empty;
            string sfBatchId = string.Empty;
            string sfResultId = string.Empty;
            try
            {
                SFRESTConnection.Connect();
                SFRestBulkOperation.CreateJob(SFRESTConnection.OauthToken, "query", "Contact", ref jobId);
                SFRestBulkOperation.AddQueryBatch(SFRESTConnection.OauthToken, jobId, "Contact", sfBatchId, sfResultId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
