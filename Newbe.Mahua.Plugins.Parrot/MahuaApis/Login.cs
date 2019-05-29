using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Plugins.Parrot.MahuaEvents;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        private async Task listview_load(int per_page, string mode,int page = 1, List<pixivIllust> illustbeforeList = null)
        {
            
            if (Login.instance == null)
            {
                //GroupMessageReceivedMahuaEvent._mahuaApi.SendPrivateMessage(context.FromQq).Text("未登陆").Done();
                return;
            }
            CancellationTokenSource CancelTokenSource = new CancellationTokenSource();
            JObject jd = null;
            try
            {
               //mahuaApi.SendPrivateMessage(context.FromQq).Text("开始获取缩略").Done();
                jd = await Login.instance.pixivAPI.rankingAsync("all", mode, page, per_page, 
                    null, CancelTokenSource
                    /*GroupMessageReceivedMahuaEvent._mahuaApi, context*/);
            }
            catch (Exception ex)
            {
                //GroupMessageReceivedMahuaEvent._mahuaApi.SendPrivateMessage(context.FromQq).Text(ex.ToString()).Done();
                throw;
            }
            //GroupMessageReceivedMahuaEvent.total = (int)jd["pagination"]["pages"];
            //GroupMessageReceivedMahuaEvent._mahuaApi.SendPrivateMessage(context.FromQq).Text("共几页" + 
            //    GroupMessageReceivedMahuaEvent.total).Done();
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
                //mahuaApi.SendPrivateMessage(context.FromQq).Text("正在下载" + page).Done();
                //GroupMessageReceivedMahuaEvent.page = page;
                if (Profile._mahuaApi != null)
                {
                    Profile._mahuaApi.SendPrivateMessage(Profile.ExceptionSender).Text(mode + "     " + page).Done();
                }                
                await listview_load(50, mode,page + 1, illustbeforeList);
            }
            else
            {
                //mahuaApi.SendPrivateMessage(context.FromQq).Text("正在下载原图").Done();
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
                        //GroupMessageReceivedMahuaEvent._mahuaApi.SendPrivateMessage(context.FromQq).Text(ex.ToString()).Done();
                        //throw;
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
                    if (Profile.path[mode].Count <= Profile.limitCount && !Profile.black.Contains((int)items.Type))
                    {
                        CancelTokenSource = new CancellationTokenSource();                       
                        foreach (var item in items.OriginalURL)
                        {                            
                            var task_imagedownload = Login.instance.pixivAPI.DownloadFileAsync(string.Format("D:/Unity/Mikot/{0}/", mode), item, null, CancelTokenSource);
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
                                 Profile.path[mode].Add(imagepath);                                                         
                            }                      
                        }
                    }
                    
                }          
            }


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
