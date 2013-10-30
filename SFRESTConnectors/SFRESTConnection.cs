using System;
using System.Configuration;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json.Linq;

namespace SFRESTConnector
{
    public class RESTAppSettingsNotFoundException : Exception
    {
        Exception ex ;

        public RESTAppSettingsNotFoundException()
        {
            string msg = "Please check you appSetting. Following keys need to be there:sfdcUserName, sfdcPassword, sfdcToken ";
            msg += " , sfdcCKey and sfdcCSecret";
            ex = new Exception(msg);
        }
    }

    public static class SFRESTConnection
    {

        public static bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }

        private static bool isConnected
        {
            get
            {
                return isConnected;
            }
            set
            {
                isConnected = value;
            }
        }

        private static string oauthToken
        {
            get
            {
                return oauthToken;
            }
            set
            {
                oauthToken = value;
            }
        }

        private static string serviceURL
        {
            get
            {
                return serviceURL;
            }
            set
            {
                serviceURL = value;
            }
        }

        public static string OauthToken
        {
            get
            {
                return oauthToken;
            }
        }

        public static string ServiceURL
        {
            get
            {
                return serviceURL;
            }
        }
    
        
        public static void Connect()
        {
            HttpClient authClient = new HttpClient();
            HttpContent content = null;
            string responseString = string.Empty;
            string serviceUrl = string.Empty;
            string oauthToken = string.Empty;
            string userName = string.Empty;
            string password = string.Empty;
            string token = string.Empty;
            string sfdcConsumerKey = string.Empty;
            string sfdcConsumerSecret = string.Empty;
            HttpResponseMessage message=null;
            try
            {

                if (CheckConfigurationSetting() != false)
                {
                    userName = ConfigurationManager.AppSettings.Get("sfdcUserName");
                    password = ConfigurationManager.AppSettings.Get("sfdcPassword");
                    token = ConfigurationManager.AppSettings.Get("sfdcToken");
                    sfdcConsumerKey = ConfigurationManager.AppSettings.Get("sfdcCKey");
                    sfdcConsumerSecret = ConfigurationManager.AppSettings.Get("sfdcCSecret");

                    if (OauthToken != null && OauthToken != string.Empty)
                    {
                        MakeConnection();
                    }
                }
                else
                {
                    throw new RESTAppSettingsNotFoundException();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
         }

        private async static void MakeConnection()
        {
            HttpClient authClient = new HttpClient();
            string responseString = string.Empty;
            string userName;
            string password;
            string token;
            string sfdcConsumerKey;
            string sfdcConsumerSecret;
            string sfdcURI;
            HttpResponseMessage message;
            //set OAuth key and secret variables 
            message = null;
            userName = ConfigurationManager.AppSettings.Get("sfdcUserName");
            password = ConfigurationManager.AppSettings.Get("sfdcPassword");
            token = ConfigurationManager.AppSettings.Get("sfdcToken");
            sfdcConsumerKey = ConfigurationManager.AppSettings.Get("sfdcCKey");
            sfdcConsumerSecret = ConfigurationManager.AppSettings.Get("sfdcCSecret");
            sfdcURI = ConfigurationManager.AppSettings.Get("sfdcURI");
            //set to Force.com user account that has API access enabled 
            password = password + token;
            HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            { 
               {"grant_type","password"}, 
               {"client_id",sfdcConsumerKey}, 
               {"client_secret",sfdcConsumerSecret},
               {"username",userName}, 
               {"password",password} 
            });

            try
            {
                message = authClient.PostAsync(sfdcURI, content).Result;
                responseString = await message.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(responseString);
                oauthToken = (string)obj["access_token"];
                serviceURL = (string)obj["instance_url"];
                isConnected = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
            }
        }

        private static bool CheckConfigurationSetting()
        {
            bool isValid = false;
            string userName = string.Empty;
            string password = string.Empty;
            string token = string.Empty;
            string sfdcConsumerKey = string.Empty;
            string sfdcConsumerSecret = string.Empty;
            try
            {
                userName = ConfigurationManager.AppSettings.Get("sfdcUserName");
                password = ConfigurationManager.AppSettings.Get("sfdcPassword");
                token = ConfigurationManager.AppSettings.Get("sfdcToken");
                sfdcConsumerKey = ConfigurationManager.AppSettings.Get("sfdcCKey");
                sfdcConsumerSecret = ConfigurationManager.AppSettings.Get("sfdcCSecret");
                if ((userName != string.Empty && userName != null) && (password != string.Empty && password != null)
                    && (token != string.Empty && token != null) && (sfdcConsumerKey != string.Empty && sfdcConsumerKey != null)
                    && (sfdcConsumerSecret != string.Empty && sfdcConsumerSecret != null))
                {
                    isValid = true;
                }
            }
            catch(Exception ex)
            {
                isValid = false;
                ex = new RESTAppSettingsNotFoundException();
                throw ex;
            }
            return isValid;
        }
    }
}
