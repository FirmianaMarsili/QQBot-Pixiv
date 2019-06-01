using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Plugins.Parrot.MahuaEvents;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Newbe.Mahua.Plugins.Parrot.MahuaApis
{
    public class Login
    {
        private CancellationTokenSource cts;
        public static Login instance = null;
        public string refresh_token = null;
        public pixivAPI pixivAPI;
        public pixivUser pixivUser;
        public Login()
        {
            LoginAsync();
        }

        private async Task listview_load(int per_page, string mode, int page = 1, List<pixivIllust> illustbeforeList = null)
        {

            if (Login.instance == null)
            {
                return;
            }
            CancellationTokenSource CancelTokenSource = new CancellationTokenSource();
            JObject jd = null;
            try
            {
                jd = await Login.instance.pixivAPI.rankingAsync("all", mode, page, per_page,
                    null, CancelTokenSource);
            }
            catch (Exception ex)
            {
                throw;
            }
            if (illustbeforeList == null)
            {
                illustbeforeList = new List<pixivIllust>();
            }
            foreach (JObject response in jd.Value<JArray>("response"))//actually will be only one
            {
                foreach (JObject works in response.Value<JArray>("works"))
                {//TODO: try to put it in a task
                    JObject work = works.Value<JObject>("work");

                    if (work["id"].Type == JTokenType.Null) continue;
                    pixivIllust illust_before = new pixivIllust();
                    illust_before.illustID = (string)work["id"];
                    illust_before.titleName = (string)work["title"];
                    illust_before.authorID = (string)work["user"]["id"];
                    illust_before.authorName = (string)work["user"]["name"];
                    illust_before.FavNum = (int)works["rank"];

                    if (work["stats"].Type != JTokenType.Null && work["stats"].HasValues)
                        illust_before.Scores = (int)work["stats"]["score"];

                    illust_before.ageLimit = false;
                    if (!work["age_limit"].ToString().Equals("all-age")) illust_before.ageLimit = true;

                    //TODO:set Type

                    illust_before.MediumURL = new List<string>();
                    illust_before.MediumURL.Add((string)work["image_urls"]["px_480mw"]);
                    illust_before.isSetComplete = true;

                    illustbeforeList.Add(illust_before);
                }
            }
            if (page < (int)jd["pagination"]["pages"])
            {
                //if (Profile._mahuaApi != null)
                //{
                //    Profile._mahuaApi.SendPrivateMessage(Profile.ExceptionSender).Text(mode + "     " + page).Done();
                //}                
                await listview_load(50, mode, page + 1, illustbeforeList);
            }
            else
            {
                List<pixivIllust> illust_ = new List<pixivIllust>();
                foreach (var item in illustbeforeList)
                {
                    CancelTokenSource = new CancellationTokenSource();
                    var task = Login.instance.pixivAPI.illust_workAsync(item.illustID, CancelTokenSource);
                    JObject returns = null;
                    try
                    {
                        returns = await task;//run first item's detail                
                    }
                    catch (Exception ex)
                    {
                        returns = null;
                    }
                    if (returns != null)
                    {
                        pixivIllust tmpillust = fromJsonSetIllust_detail(returns);
                        illust_.Add(tmpillust);
                    }
                }
                illustbeforeList = illust_;
                foreach (var items in illustbeforeList)
                {
                    if (Profile.limitCount == 0 || Profile.path[mode].Count <= Profile.limitCount && !Profile.black.Contains((int)items.Type))
                    {
                        CancelTokenSource = new CancellationTokenSource();
                        List<string> url = null;
                        if (Profile.DownloadOriginalURL)
                        {
                            url = items.OriginalURL;
                        }
                        else
                        {
                            url = items.MediumURL;
                        }
                        foreach (var item in url)
                        {
                            var task_imagedownload = Login.instance.pixivAPI.DownloadFileAsync(string.Format("{0}{1}", Profile.ImagePath, mode), item, null, CancelTokenSource);
                            string imagepath = null;
                            try
                            {
                                imagepath = await task_imagedownload;
                            }
                            catch (Exception)
                            {
                                imagepath = null;
                                //throw;
                            }

                            if (imagepath != null)
                            {                                
                                try
                                {
                                    if (Profile.imageLength != 0)
                                    {
                                        System.Drawing.Image img = System.Drawing.Image.FromFile(imagepath);
                                        System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                                        img.Dispose();
                                        var memory = Zip(bmp, ImageFormat.Jpeg, Profile.imageLength);
                                        bmp.Dispose();
                                        if (File.Exists(imagepath))
                                        {
                                            File.Delete(imagepath);
                                        }
                                        MemoryStream m = new MemoryStream();
                                        FileStream fs = new FileStream(imagepath, FileMode.OpenOrCreate);
                                        BinaryWriter w = new BinaryWriter(fs);
                                        w.Write(memory.ToArray());
                                        fs.Close();
                                        memory.Close();
                                    }                                                                     
                                    Profile.path[mode].Add(imagepath);
                                    Profile.path[mode] = Shuffle(Profile.path[mode]);                             
                                }
                                catch (Exception ex)
                                {
                                    //Console.WriteLine(ex.ToString());
                                    //throw;
                                }

                            }
                        }
                    }

                }
            }


        }

        private List<string> Shuffle(List<string> array)
        {
            Random random = new Random();
            for (int i = 0; i < array.Count; i++)
            {
                int index = random.Next(0, array.Count - i);
                string temp = array[index];
                array[index] = array[array.Count - i - 1];
                array[array.Count - i - 1] = temp;
            }
            return array;
        }
        private pixivIllust fromJsonSetIllust_detail(JObject json_illust)
        {
            if (json_illust == null) return null;
            pixivIllust illust_before = null;
            foreach (JObject response_illust in json_illust.Value<JArray>("response"))//though now it will be only one response
            {
                illust_before = new pixivIllust();
                illust_before.illustID = (string)response_illust["id"];
                illust_before.titleName = (string)response_illust["title"];
                illust_before.authorID = (string)response_illust["user"]["id"];
                illust_before.authorName = (string)response_illust["user"]["name"];
                illust_before.authorIconURL = (string)response_illust["user"]["profile_image_urls"]["px_50x50"];
                illust_before.created_time = (string)response_illust["created_time"];

                illust_before.FavNum = (int)response_illust["stats"]["favorited_count"]["public"] + (int)response_illust["stats"]["favorited_count"]["private"];
                illust_before.favouriteID = (string)response_illust["favorite_id"];

                illust_before.ageLimit = false;
                if (!response_illust["age_limit"].ToString().Equals("all-age")) illust_before.ageLimit = true;

                illust_before.authorIsFollowing = false;
                if (response_illust["user"]["is_following"] != null)
                    if ((bool)response_illust["user"]["is_following"])
                        illust_before.authorIsFollowing = true;

                illust_before.OriginalURL = new List<string>();//start to get original and medium pic URL
                illust_before.MediumURL = new List<string>();
                if (!response_illust["metadata"].HasValues)//illust
                {
                    illust_before.Type = pixivIllust.illustType.illustration;
                    illust_before.MediumURL.Add((string)response_illust["image_urls"]["px_480mw"]);
                    illust_before.OriginalURL.Add(response_illust["image_urls"]["large"].ToString());
                }
                else //优先遍历metadata中的原图
                {
                    if (!(bool)response_illust["is_manga"])//ugoira
                    {
                        illust_before.Type = pixivIllust.illustType.ugoira;
                        illust_before.MediumURL.Add((string)response_illust["image_urls"]["px_480mw"]);
                        illust_before.OriginalURL.Add(response_illust["image_urls"]["large"].ToString());
                        illust_before.ugoiraZipURL = (response_illust["metadata"]["zip_urls"]["ugoira600x600"].ToString());
                    }
                    else// manga
                    {
                        illust_before.Type = pixivIllust.illustType.manga;
                        foreach (JObject image in response_illust["metadata"]["pages"].Value<JArray>())
                        {
                            illust_before.MediumURL.Add((string)image["image_urls"]["px_480mw"]);
                            illust_before.OriginalURL.Add(image["image_urls"]["large"].ToString());
                        }
                    }
                }
            }
            return illust_before;
        }

        /// <summary>
        /// 压缩图片至n Kb以下
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="format">图片格式</param>
        /// <param name="targetLen">压缩后大小</param>
        /// <param name="srcLen">原始大小</param>
        /// <returns>压缩后的图片内存流</returns>
        public MemoryStream Zip(Image img, ImageFormat format, long targetLen, long srcLen = 0)
        {
            //设置允许大小偏差幅度 默认10kb
            const long nearlyLen = 10240;

            //返回内存流  如果参数中原图大小没有传递 则使用内存流读取
            var ms = new MemoryStream();
            if (0 == srcLen)
            {
                img.Save(ms, format);
                srcLen = ms.Length;
            }

            //单位 由Kb转为byte 若目标大小高于原图大小，则满足条件退出
            targetLen *= 1024;
            if (targetLen >= srcLen)
            {
                ms.SetLength(0);
                ms.Position = 0;
                img.Save(ms, format);
                return ms;
            }

            //获取目标大小最低值
            var exitLen = targetLen - nearlyLen;

            //初始化质量压缩参数 图像 内存流等
            var quality = (long)Math.Floor(100.00 * targetLen / srcLen);
            var parms = new EncoderParameters(1);

            //获取编码器信息
            ImageCodecInfo formatInfo = null;
            var encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo icf in encoders)
            {
                if (icf.FormatID == format.Guid)
                {
                    formatInfo = icf;
                    break;
                }
            }

            //使用二分法进行查找 最接近的质量参数
            long startQuality = quality;
            long endQuality = 100;
            quality = (startQuality + endQuality) / 2;

            while (true)
            {
                //设置质量
                parms.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                //清空内存流 然后保存图片
                ms.SetLength(0);
                ms.Position = 0;
                img.Save(ms, formatInfo, parms);

                //若压缩后大小低于目标大小，则满足条件退出
                if (ms.Length >= exitLen && ms.Length <= targetLen)
                {
                    break;
                }
                else if (startQuality >= endQuality) //区间相等无需再次计算
                {
                    break;
                }
                else if (ms.Length < exitLen) //压缩过小,起始质量右移
                {
                    startQuality = quality;
                }
                else //压缩过大 终止质量左移
                {
                    endQuality = quality;
                }

                //重新设置质量参数 如果计算出来的质量没有发生变化，则终止查找。这样是为了避免重复计算情况{start:16,end:18} 和 {start:16,endQuality:17}
                var newQuality = (startQuality + endQuality) / 2;
                if (newQuality == quality)
                {
                    break;
                }
                quality = newQuality;

                //Console.WriteLine("start:{0} end:{1} current:{2}", startQuality, endQuality, quality);
            }
            return ms;
        }

        public async void LoginAsync()
        {
            cts = new CancellationTokenSource();
            OAuth oAuth = new OAuth();
            bool result = false;
            try
            {
                result = (!string.IsNullOrWhiteSpace(this.refresh_token)) ? await oAuth.authAsync(this.refresh_token, cts) : await oAuth.authAsync(Profile.userName, Profile.passWord, cts);
            }
            catch (Exception ex)
            {
                return;
                //throw;
            }
            if (result)
            {
                pixivAPI = new pixivAPI(oAuth);
                pixivUser = oAuth.User;
                this.refresh_token = pixivUser.refresh_token;
                instance = this;

                //线程下载所有排行
                new Thread(new ThreadStart(async delegate { await listview_load(50, "daily", 1, null); })).Start();
                new Thread(new ThreadStart(async delegate { await listview_load(50, "weekly", 1, null); })).Start();
                new Thread(new ThreadStart(async delegate { await listview_load(50, "monthly", 1, null); })).Start();
                new Thread(new ThreadStart(async delegate { await listview_load(50, "rookie", 1, null); })).Start();
                new Thread(new ThreadStart(async delegate { await listview_load(50, "daily_r18", 1, null); })).Start();
                new Thread(new ThreadStart(async delegate { await listview_load(50, "weekly_r18", 1, null); })).Start();
                new Thread(new ThreadStart(async delegate { await listview_load(50, "r18g", 1, null); })).Start();
                new Thread(new ThreadStart(async delegate { await listview_load(50, "original", 1, null); })).Start();
                new Thread(new ThreadStart(async delegate { await listview_load(50, "male", 1, null); })).Start();
                new Thread(new ThreadStart(async delegate { await listview_load(50, "female", 1, null); })).Start();
                new Thread(new ThreadStart(async delegate { await listview_load(50, "male_r18", 1, null); })).Start();
                new Thread(new ThreadStart(async delegate { await listview_load(50, "female_r18", 1, null); })).Start();

            }
        }
    }
}
