using Newbe.Mahua.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Newbe.Mahua.Plugins.yande.MahuaApis
{
    public class Helper
    {
        private static string file = "D:\\yande\\page.txt";
        public static string error_File = "D:\\yande\\error.txt";
        private static int _currentPage;
        public static int currentPage
        {
            get
            {
                if (_currentPage == 0)
                {
                    if (!File.Exists(file))
                    {
                        currentPage = 1;
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
                            currentPage = 1;
                        }

                    }
                }
                return _currentPage;
            }
            set
            {
                _currentPage = value;
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

        private static int _currentIndex;
        public static int currentIndex
        {
            get
            {
                if (_currentIndex == 0)
                {
                    if (!File.Exists(file))
                    {
                        currentIndex = 0;
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
                            currentIndex = 0;
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
                        currentDateTime = GetGMT0Time(DateTime.Now);
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
                            currentDateTime = GetGMT0Time(DateTime.Now);
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

        private static int score = 40; //搜索最低评分
        private static int limit = 64; //每页最多图片
        private static string order = "score"; //排序规则

        private static string url
        {
            get
            {
                return string.Format("https://yande.re/post.json?page={0}&tags=date:{1}+score:>={2}+order:{3}+limit:{4}+-rating:explicit",
                    currentPage.ToString(),
                    currentDateTime.ToString("yyyy-MM-dd"),
                    score.ToString(),
                    order,
                    limit.ToString()
                    );
            }
        }

        private static DateTime GetGMT0Time(DateTime dateTime)
        {
            DateTime dateTime_ = dateTime.AddHours(-8);
            return dateTime_;
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

        private static bool ImageSize(JToken size)
        {
            bool flag = (int)size / (1024f * 1024f) >= 4;
            return flag;
        }

        private static void GET()
        {
            bool flag_InitIndex = imageUrl.Count == 0 && currentIndex != 0;
            loading = true;

            try
            {
                bool flag = imageUrl.Count == 0 && currentIndex != 0;
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

                if (string.IsNullOrEmpty(result) || result == "[]")
                {
                    //进行前一天
                    currentDateTime = currentDateTime.AddDays(-1);
                    currentPage = 1;
                    GET();
                }
                else
                {
                    JArray jd = JArray.Parse(result);

                    lock (imageUrl)
                    {
                        imageUrl.Clear();
                        for (int i = 0; i < jd.Count; i++)
                        {
                            //bool flag = ImageSize(jd[i]["jpeg_file_size"]);
                            //if (!flag)
                            //{
                            //    imageUrl.Add(jd[i]["jpeg_url"].ToString());
                            //}
                            //else
                            //{
                            bool flag1 = ImageSize(jd[i]["sample_file_size"]);
                            if (!flag1)
                            {
                                imageUrl.Add(jd[i]["sample_url"].ToString());
                            }
                            //}
                        }
                        if (!flag_InitIndex)
                        {
                            currentIndex = 0;
                        }
                    }
                    System.GC.Collect();
                    loading = false;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                WriteError(ex.ToString());
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
                    try
                    {
                        GET();
                    }
                    catch (Exception ex)
                    {
                        WriteError(ex.ToString());
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
                    WriteError(ex.ToString());
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
                    WriteError(ex.ToString());
                }
            })).Start();
        }

        public static void WriteError(string error)
        {
            StreamWriter sw = File.AppendText(error_File);
            sw.WriteLine("error");
            sw.Flush();
            sw.Close();
        }

    }
}
