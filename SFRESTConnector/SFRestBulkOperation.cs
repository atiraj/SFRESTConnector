using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Configuration;

namespace SFNetRestLib
{
    public static class SFRestBulkOperation
    {

        /// <summary>
        /// Creates a job in Salesforce
        /// </summary>
        /// <param name="sfSessionId"></param>
        /// <param name="sfOperation"></param>
        /// <param name="sfObjectName"></param>
        /// <param name="jobId"></param>
        /// <remarks></remarks>
        private static string schemaDir
        {
            get;
            set;
        }

        public static void CreateJob(string sfSessionId, string sfOperation, string sfObjectName, ref string jobId)
        {
                string str = string.Empty;
                string schemaFile = string.Empty;
                string reqURL = "";
                byte[] bytes = null;
                XmlDocument reqDoc = null;
                XmlDocument responseXmlDocument = new XmlDocument();
                Stream responseStream = null;
                try
                {
                    if(schemaDir==string.Empty || schemaDir==null)
                    {
                        schemaDir = ConfigurationManager.AppSettings.Get("schemaDir");
                        if (schemaDir == string.Empty || schemaDir == null)
                        {
                            throw new Exception("Please provide valid value for schemaDir in app settings.");
                        }
                        schemaFile=AppDomain.CurrentDomain.BaseDirectory +schemaDir + "\\" + sfObjectName + "_Job.xml";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                try
                {
                    //str = "" + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + "<jobInfo xmlns=\"http://www.force.com/2009/06/asyncapi/dataload\">" + "    <operation></operation>" + "    <object></object>" + "    <contentType>CSV</contentType>" + "</jobInfo>";
                    //Eg for XML content type
                    //"    <contentType>XML</contentType>" &
                    reqURL = "https://na15-api.salesforce.com/services/async/28.0/job";
                    reqDoc = new XmlDocument();
                    //reqDoc.LoadXml(str);
                    // added XML modifications
                    reqDoc.Load(schemaFile);
                    reqDoc.GetElementsByTagName("operation")[0].InnerText = sfOperation;
                    reqDoc.GetElementsByTagName("object")[0].InnerText = sfObjectName;
                    bytes = System.Text.Encoding.ASCII.GetBytes(reqDoc.InnerXml);
                    //bytes = System.Text.Encoding.UTF8.GetBytes(reqDoc.InnerXml)

                    using (responseStream = Post(bytes, reqURL, sfSessionId, "POST", "text/csv; charset=UTF-8"))
                    {
                        responseXmlDocument.Load(responseStream);
                        //Get jobId
                        jobId = ((((responseXmlDocument) != null)) ? responseXmlDocument.GetElementsByTagName("id").Item(0).InnerText : "");
                    }

                }

                catch (Exception ex)
                {
                    throw ex;
                }
        }

        /// <summary>
        /// Adds the Batch to SalesForce
        /// </summary>
        /// <param name="sfSessionId"></param>
        /// <param name="fileBytes"></param>
        /// <param name="sfJobId"></param>
        /// <param name="sfBatchId"></param>
        /// <param name="sfResultId"></param>
        /// <remarks></remarks>

        public static void AddBatch(string sfSessionId, byte[] fileBytes, string sfJobId, string sfBatchId = null, string sfResultId = null)
        {
            string requestURI = ("https://login.salesforce.com/services/async/23.0/job/" + (sfJobId + "/batch"));
            Post(fileBytes, requestURI, sfSessionId, "POST", "text/csv; charset=UTF-8");

        }

        /// <summary>
        /// Function to POST the HTTP rest request
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="reqURL"></param>
        /// <param name="sfSessionId"></param>
        /// <param name="method"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Stream Post(byte[] bytes, string reqURL, string sfSessionId, string method, string contentType)
        {

            //Create the request object
            WebRequest requestHttp = WebRequest.Create(reqURL);
            //Assign the type of request POST,GET..
            requestHttp.Method = method;
            //Assign Content Type
            requestHttp.ContentType = contentType;
            //"text/csv; charset=UTF-8" or "application/xml; charset=UTF-8"
            //Assign the session id to the header
            requestHttp.Headers.Add(("X-SFDC-Session: " + sfSessionId));

            //Assign byte length
            if ((bytes != null))
            {
                requestHttp.ContentLength = bytes.Length;

                System.IO.Stream strmHttpContent = requestHttp.GetRequestStream();
                strmHttpContent.Write(bytes, 0, bytes.Length);
                strmHttpContent.Close();

            }

            //Get the response object

            String responseFromServer;
            try
            {
                //Call the service and get the response
                HttpWebResponse response = (HttpWebResponse)requestHttp.GetResponse();

                if (HttpStatusCode.OK == response.StatusCode)
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    responseFromServer = reader.ReadToEnd();
                    response.Close();
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    responseFromServer = string.Format("Error code: {0}  ", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    {
                        responseFromServer += new StreamReader(data).ReadToEnd();
                        //   return responseFromServer;
                        Console.WriteLine("reponse from server" + responseFromServer);
                    }
                }
            }
            //Return response Stream
            WebResponse responseHttpRequest = requestHttp.GetResponse();
            return responseHttpRequest.GetResponseStream();
        }
    }
}
