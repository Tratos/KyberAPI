using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace KyberAPI
{
    public static class Helper
    {
        public static byte[] ReadContentTCP(NetworkStream Stream)
        {
            MemoryStream res = new MemoryStream();
            byte[] buff = new byte[0x10000];
            Stream.ReadTimeout = 100;
            int bytesRead;
            try
            {
                while ((bytesRead = Stream.Read(buff, 0, 0x10000)) > 0)
                    res.Write(buff, 0, bytesRead);
            }
            catch { }
            Stream.Flush();
            return res.ToArray();
        }

        public static string GetLANIP()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        public static string GetWANIP()
        {
            WebRequest hwr = HttpWebRequest.Create(new Uri("http://checkip.dyndns.org"));
            WebResponse wr = hwr.GetResponse();
            Stream stream = wr.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            string htmlResult = streamReader.ReadToEnd();
            string[] htmlSplit = htmlResult.Split(new string[] { ":", "<" }, StringSplitOptions.RemoveEmptyEntries);
            string IP = htmlSplit[6].Trim();
            stream.Close();
            wr.Close();

            return IP;
        }

    }
}
