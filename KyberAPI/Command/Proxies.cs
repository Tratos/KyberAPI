using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KyberAPI
{
    public static class Proxies
    {
        public class JSProxis
        {
            public string ip { get; set; }
            public string name { get; set; }
            public string flag { get; set; }
        }

        public static string getProxis()
        {
            string jsonData = "";
            JSProxis empObj = new JSProxis();
            empObj.ip = Helper.GetLANIP();
            empObj.name = "Kyber HomeServer";
            empObj.flag = "http://" + Helper.GetLANIP() + "/static/images/flags/unknow.svg";

            jsonData = JsonConvert.SerializeObject(empObj);
            return jsonData;
        }

    }
}
