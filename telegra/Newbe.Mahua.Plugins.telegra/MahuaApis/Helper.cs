using Newbe.Mahua.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Newbe.Mahua.Plugins.telegra.MahuaApis
{
    public class Helper
    {
        private static string file = "D:\\telegra\\page.txt";

        private static int tempIndex = 0;
        private static int _currentIndex;
        public static int currentIndex
        {
            get
            {
                if (_currentIndex == 0)
                {
                    if (!File.Exists(file))
                    {
                        currentIndex = 3;
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
                            currentIndex = 3;
                        }

                    }
                }
                return _currentIndex;
            }
            set
            {
                _currentIndex = value;
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
        private static DateTime? _currentDateTime;
        public static DateTime currentDateTime
        {
            get
            {
                if (_currentDateTime == null)
                {
                    if (!File.Exists(file))
                    {
                        currentDateTime = new DateTime(2018, 08, 29);
                    }
                    else
                    {
                        string str = File.ReadAllText(file);
                        JObject jd = JObject.Parse(str);
                        if (jd.Property("DateTime") != null)
                        {
                            _currentDateTime = (DateTime)jd["DateTime"];
                        }
                        else
                        {
                            currentDateTime = new DateTime(2018, 08, 29);
                        }

                    }
                }
                return (DateTime)_currentDateTime;
            }
            set
            {
                _currentDateTime = value;
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
                jd["DateTime"] = value;

                FileInfo myFile = new FileInfo(file);
                StreamWriter sw = myFile.CreateText();
                sw.Write(jd.ToString());
                sw.Close();
            }
        }

        public static DateTime dateTimeCD = DateTime.Now;
        public static double timeCD = 8;

        private static string url
        {
            get
            {
                return string.Format("https://telegra.ph/涩图time-No{0}-{1}",
                    currentIndex.ToString(),
                    currentDateTime.ToString("MM-dd")
                    );
            }
        }


        private static bool loading = false;

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
                        bool flag = tempIndex >= imageUrl.Count;
                        if (flag)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
        }

        private static bool ImageSize(JToken size)
        {
            bool flag = (int)size / (1024f * 1024f) >= 4;
            return flag;
        }

        private static void GET()
        {

            try
            {
                loading = true;
                string result = string.Empty;
                //IWebProxy proxy = GetProxy();
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 10000;
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
                request.ServicePoint.Expect100Continue = false;
                request.ServicePoint.UseNagleAlgorithm = false;
                request.AllowWriteStreamBuffering = false;
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate, br");
                request.ContentType = "application/json; charset=utf-8";
                request.AllowAutoRedirect = false;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.80 Safari/537.36";
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

                if (!string.IsNullOrEmpty(result))
                {
                    lock (imageUrl)
                    {
                        
                        imageUrl.Clear();
                        tempIndex = 0;

                        string start = "<img src=\"";
                        string end = "\">";
                        Regex rg = new Regex(string.Format("(?<=({0}))[.\\s\\S]*?(?=({1}))", start, end), RegexOptions.Multiline | RegexOptions.Singleline);
                        MatchCollection str = rg.Matches(result);
                        string path = "D:\\telegra\\" + currentIndex.ToString();
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        foreach (Match item in str)
                        {
                            string file = path + "\\" + item.ToString().Replace("/file/",null);

                            if (!File.Exists(file))
                            {
                                WebClient wc = new WebClient();
                                byte[] b = wc.DownloadData("https://telegra.ph" + item.ToString());
                                using (MemoryStream ms = new MemoryStream(b))
                                {
                                    Image image = Image.FromStream(ms);
                                    image.Save(file);                                    
                                }
                            }
                            imageUrl.Add(file);                            
                           
                        }
                        currentIndex++;
                    }
                }
                loading = false;
            }
            catch (Exception ex)
            {
                currentDateTime = currentDateTime.AddDays(1);
                GET();
                //throw;
            }
        }

        public static string GetPath()
        {
            if (loading)
            {
                return null;
            }
            if (isLast)
            {
                new Thread(new ThreadStart(delegate
                {
                    GET();
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
                        str = imageUrl[tempIndex];
                        tempIndex += 1;
                    }
                }
                catch (Exception ex)
                {
                }
                return str;


            }
        }

        public static void Init()
        {
            new Thread(new ThreadStart(delegate
            {
                GET();
            })).Start();
        }


    }
}
