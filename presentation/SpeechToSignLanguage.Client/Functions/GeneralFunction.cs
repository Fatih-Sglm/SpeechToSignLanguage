using System;
using System.IO;
using System.Net.NetworkInformation;

namespace SpeechToSignLanguage.Client.Functions
{
    public class GeneralFunction
    {
        public bool CheckForInternetConnection()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void CreateDirectory(string uName)
        {
            if (!Directory.Exists("c:\\record\\" + uName))
                Directory.CreateDirectory("c:\\record\\" + uName);
        }
    }
}
