using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KyberAPI
{
    public static class Servers
    {
        public static string getServers()
        {
            string ip = Helper.GetLANIP();
            var jsonDatas = "{\"page\":1,\"pageCount\":1,\"serverCount\":1,\"servers\":[{\"id\":\"00000000000000000000000000000001\",\"name\":\"Test\",\"description\":\"TestServer\",\"map\":\"Levels/Naboo_01/Naboo_01\",\"mode\":\"Mode1\",\"mods\":[],\"users\":1,\"host\":\"KyberTestServer\",\"maxPlayers\":40,\"autoBalanceTeams\":true,\"startedAt\":1673706556136,\"startedAtPretty\":\"1m ago\",\"requiresPassword\":false,\"region\":\"BETA\",\"proxy\":{ \"ip\":\"" + ip + "\",\"name\":\"Kyber HomeServer\",\"flag\":\"http://" + ip + "/static/images/flags/unknow.svg\"}}]}";
            return jsonDatas;
        }
    }
}
