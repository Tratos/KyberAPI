using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace KyberAPI
{
    public static class JSONServer
    {
        public static bool basicMode = false;
        public static readonly object _sync = new object();
        public static bool _exit;
        public static TcpListener lJSON = null;
        public static RichTextBox box = null;

        public static void Start()
        {
            SetExit(false);
            Logger.Log("Starting JSON Server...");

            new Thread(new ParameterizedThreadStart(tHTTPMain)).Start();
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(10);
            }
        }

        public static void Stop()
        {
            Logger.Log("JSON Server stopping...");
            if (lJSON != null) lJSON.Stop();
            SetExit(true);
            Logger.Log("Done.");
        }

        public static void tHTTPMain(object obj)
        {
            try
            {
                Logger.Log("[JSON] starting...");
                lJSON = new TcpListener(IPAddress.Parse("0.0.0.0"), 80);
                Logger.Log("[JSON] bound to 0.0.0.0:80");
                lJSON.Start();
                Logger.Log("[JSON] listening...");
                TcpClient client;
                while (!GetExit())
                {
                    client = lJSON.AcceptTcpClient();
                    NetworkStream ns = client.GetStream();
                    byte[] data = Helper.ReadContentTCP(ns);
                    if (!basicMode)
                        Logger.Log("[JSON] Recvdump:\n" + Encoding.ASCII.GetString(data));
                    try
                    {
                        ProcessJSON(Encoding.ASCII.GetString(data), ns);
                    }
                    catch { }
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("JSON", ex);
            }
        }

        public static void ProcessJSON(string data, Stream s)
        {
            string[] lines = data.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Logger.Log("[JSON] Request: " + lines[0]);
            string cmd = lines[0].Split(' ')[0];
            string url = lines[0].Split(' ')[1].Split(':')[0];
            if (cmd == "GET")
            {
                switch (url)
                {
                    case "/api/proxies":
                        string Replay = "";
                        Replay = Proxies.getProxis();
                        byte[] postBytes = Encoding.UTF8.GetBytes(Replay);
                        ReplyWithJSON(s, postBytes);
                        break;
                }

                if (url.StartsWith("/api/servers?limit=20&page="))
                {
                    string Replay = "";
                    Replay = Servers.getServers();
                    byte[] postBytes = Encoding.UTF8.GetBytes(Replay);
                    ReplyWithJSON(s, postBytes);
                }

                if (url.StartsWith("/static/images/flags/"))
                {
                    ReplyWithBinary(s, GetBinaryFile(url.Replace("/", "\\")));
                }

            }
            if (cmd == "POST" && !basicMode)
            {
                int pos = data.IndexOf("\r\n\r\n");
                if (pos != -1)
                    Logger.Log("[JSON] Content: \n" + data.Substring(pos + 4));
            }
        }

        public static void ReplyWithJSON(Stream s, byte[] c)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("HTTP/1.1 200 OK");
            sb.AppendLine("Date: " + DateTime.Now.ToUniversalTime().ToString("r"));
            sb.AppendLine("Server: Kyber Network");
            sb.AppendLine("Content-Type: application/json; charset=UTF-8");
            sb.AppendLine("Content-Encoding: UTF-8");
            sb.AppendLine("Content-Length: " + c.Length);
            sb.AppendLine("Keep-Alive: timeout=5, max=100");
            sb.AppendLine("Connection: Keep-Alive");
            sb.AppendLine();
            if (!basicMode)
            {
                Logger.Log("[JSON] Sending: \n" + sb.ToString());
            }
            byte[] buf = Encoding.ASCII.GetBytes(sb.ToString());
            s.Write(buf, 0, buf.Length);
            s.Write(c, 0, c.Length);
        }


        public static byte[] GetBinaryFile(string path)
        {
            if (File.Exists("html" + path))
                return File.ReadAllBytes("html" + path);
            Logger.Log("[JSON] Error file not found: " + path);
            return new byte[0];
        }

        public static void ReplyWithBinary(Stream s, byte[] b)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("HTTP/1.1 200 OK");
            sb.AppendLine("Date: " + DateTime.Now.ToUniversalTime().ToString("r"));
            sb.AppendLine("Server: Kyber Network");
            sb.AppendLine("Content-Type: application/octet-stream");
            sb.AppendLine("Content-Length: " + b.Length);
            sb.AppendLine("Keep-Alive: timeout=5, max=100");
            sb.AppendLine("Connection: close");
            sb.AppendLine();
            byte[] buf = Encoding.ASCII.GetBytes(sb.ToString());
            s.Write(buf, 0, buf.Length);
            s.Write(b, 0, b.Length);
        }

        public static void ReplyWithText(Stream s, string c)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("HTTP/1.1 200 OK");
            sb.AppendLine("Date: " + DateTime.Now.ToUniversalTime().ToString("r"));
            sb.AppendLine("Server: Kyber Network");
            sb.AppendLine("Content-Type: text/html; charset=UTF-8");
            sb.AppendLine("Content-Encoding: UTF-8");
            sb.AppendLine("Content-Length: " + c.Length);
            sb.AppendLine("Keep-Alive: timeout=5, max=100");
            sb.AppendLine("Connection: close");
            sb.AppendLine();
            sb.Append(c);
            byte[] buf = Encoding.ASCII.GetBytes(sb.ToString());
            s.Write(buf, 0, buf.Length);
        }

        public static void SetExit(bool state)
        {
            lock (_sync)
            {
                _exit = state;
            }
        }

        public static bool GetExit()
        {
            bool result;
            lock (_sync)
            {
                result = _exit;
            }
            return result;
        }
    }
}
