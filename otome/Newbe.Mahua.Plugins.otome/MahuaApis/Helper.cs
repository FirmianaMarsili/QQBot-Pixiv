using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Newbe.Mahua.Plugins.otome.MahuaApis
{
    public class Helper
    {
        private static string exceptionSender = "";
        private static int _currentPage;
        private static int currentPage
        {
            get
            {
                if (_currentPage == 0)
                {
                    string file = "D:\\otome\\page.txt";
                    if (!File.Exists(file))
                    {
                        _currentPage = 1;
                    }
                    else
                    {
                        string str = File.ReadAllText(file);
                        JObject jd = JObject.Parse(str);
                        if (jd.Property("page") != null)
                        {
                            _currentPage = (int)jd["page"];
                        }
                        else
                        {
                            _currentPage = 1;
                        }

                    }
                }
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                string file = "D:\\otome\\page.txt";
                string tmp_File = Path.GetDirectoryName(file);
                if (!Directory.Exists(tmp_File))
                {
                    Directory.CreateDirectory(tmp_File);
                }
                JObject jd = new JObject();
                if (File.Exists(file))
                {
                    string str = File.ReadAllText(file);

                    if (str != null)
                    {
                        jd = JObject.Parse(str);
                    }
                }
                jd["page"] = value;

                FileInfo myFile = new FileInfo(file);
                StreamWriter sw = myFile.CreateText();
                sw.Write(jd.ToString());
                sw.Close();
            }
        }

        public static DateTime dateTime = DateTime.Now;
        public static double timeCD = 5;

        private static int _currentIndex = -1;
        private static int currentIndex
        {
            get
            {
                if (_currentIndex == -1)
                {
                    string file = "D:\\otome\\page.txt";
                    if (!File.Exists(file))
                    {
                        _currentIndex = 0;
                    }
                    else
                    {
                        string str = File.ReadAllText(file);
                        JObject jd = JObject.Parse(str);
                        if (jd.Property("index") != null)
                        {
                            _currentIndex = (int)jd["index"];
                        }
                        else
                        {
                            _currentIndex = 0;
                        }

                    }
                }
                return _currentIndex;
            }
            set
            {
                _currentIndex = value;
                string file = "D:\\otome\\page.txt";
                string tmp_File = Path.GetDirectoryName(file);
                if (!Directory.Exists(tmp_File))
                {
                    Directory.CreateDirectory(tmp_File);
                }
                JObject jd = new JObject();
                if (File.Exists(file))
                {
                    string str = File.ReadAllText(file);

                    if (str != null)
                    {
                        jd = JObject.Parse(str);
                    }
                }
                jd["index"] = value;

                FileInfo myFile = new FileInfo(file);
                StreamWriter sw = myFile.CreateText();
                sw.Write(jd.ToString());
                sw.Close();
            }
        }

        private static string url
        {
            get
            {
                return string.Format("http://otome.me/post/index/ninki/mouth/p/{0}.html", currentPage.ToString());
            }
        }

        private static List<string> imageUrl = new List<string>();
        private static bool isLast
        {
            get
            {
                lock (imageUrl)
                {
                    if (imageUrl.Count == 0)
                    {
                        return true;                    
                    }
                    else
                    {
                        bool flag = currentIndex >= imageUrl.Count;
                        if (flag)
                        {
                            currentPage += 1;
                            return true;
                        }
                        return false;
                    }                    
                }
            }
        }


        private static bool loading = false;


        private static void GET()
        {
            bool flag = imageUrl.Count == 0 && currentIndex != 0;
            loading = true;
            string result = string.Empty;
            //IWebProxy proxy = GetProxy();
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.AllowWriteStreamBuffering = false;
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = false;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
            request.KeepAlive = true;
            request.Method = "GET";
            request.Headers[HttpRequestHeader.Cookie] = "PHPSESSID=k61pcs2o2lkn4n9gfkjqlmm2i4; theme=black";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.ContentEncoding.ToLower().Contains("gzip"))//解压
                {
                    using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))//解压
                {
                    using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    using (Stream stream = response.GetResponseStream())//原始
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            request.Abort();

            string start = "data-original=\"";
            string end = "\"";
            Regex rg = new Regex(string.Format("(?<=({0}))[.\\s\\S]*?(?=({1}))", start, end), RegexOptions.Multiline | RegexOptions.Singleline);
            MatchCollection str = rg.Matches(result);
            imageUrl.Clear();
            lock (imageUrl)
            {
                foreach (Match item in str)
                {
                    imageUrl.Add(item.ToString());
                }
                //currentPage += 1;
                if (!flag)
                {
                    currentIndex = 0;
                }                
            }
            System.GC.Collect();
            loading = false;
        }

        public static string GetPath( IMahuaApi mahuaApi)
        {
            if (loading)
            {
                return null;
            }
            if (isLast)
            {
                new Thread(new ThreadStart(delegate
                {
                    try
                    {
                        GET();
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.ToString());
                        mahuaApi.SendPrivateMessage(exceptionSender).Text(ex.ToString()).Done();
                        //throw;
                    }
                })).Start();
                return null;
            }
            else
            {
                string str = null;
                try
                {                   
                    lock (imageUrl)
                    {
                        str = imageUrl[currentIndex];
                        currentIndex += 1;
                    }                 
                }
                catch (Exception ex)
                {
                    mahuaApi.SendPrivateMessage(exceptionSender).Text(ex.ToString()).Done();
                    //throw;
                }              
                return str;


            }
        }

        public static void Init()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    GET();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    //throw;
                }
            })).Start();
        }
    }
}
