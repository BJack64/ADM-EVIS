using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using eFakturADM.DJPLib.Objects;
using eFakturADM.Logic.Utilities;
using eFakturADM.Shared.Utility;

namespace eFakturADM.Shared.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            var isStop = false;
            while (!isStop)
            {
                Console.WriteLine("ShareLib Test Console Application");
                Console.WriteLine("---------------------------------");
                Console.WriteLine("1. Test Proxy DJP Request");
                Console.WriteLine("2. Test Encrypt");
                Console.WriteLine("---------------------------------");
                Console.Write("Input number or (X) to close : ");
                var inputMenu = Console.ReadLine();

                switch (inputMenu)
                {
                    case "1":
                        TestDjpService();
                        break;
                    case "2":
                        TestEncrypt();
                        break;
                    case "x":
                    case "X":
                        isStop = true;
                        break;
                }

                Console.WriteLine("---------------------------------");
            }

        }

        static void TestDjpService()
        {
            Console.Write("Scan Url : ");
            var scanUrl = Console.ReadLine();
            if (string.IsNullOrEmpty(scanUrl))
            {
                Console.WriteLine("Url Kosong");
                return;
            }
            var isUseProxy = Convert.ToBoolean(ConfigurationManager.AppSettings["IsUseProxy"]);
            var request = (HttpWebRequest)WebRequest.Create(scanUrl);
            if (isUseProxy)
            {
                var myproxy = new WebProxy(ConfigurationManager.AppSettings["ProxyServer"], Convert.ToInt32(ConfigurationManager.AppSettings["ProxyPort"]));
                myproxy.BypassProxyOnLocal = false;
                request.Proxy = myproxy;    
            }
            
            request.Method = "GET";

            using (var response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        Console.WriteLine("Null Response Stream");
                    }
                    else
                    {
                        var doc = new XmlDocument();
                        doc.Load(responseStream);
                        var strXml = doc.InnerXml;
                        Console.WriteLine(strXml);

                        responseStream.Close();
                        responseStream.Dispose();

                    }

                }

                response.Close();
            }
        }

        static void TestEncrypt()
        {
            var instr = "";
            Console.Write("String PWD : ");
            instr = Console.ReadLine();
            Console.WriteLine("Encrypted : " + Rijndael.Encrypt(instr));
        }

    }
}
