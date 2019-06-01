using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua.Plugins.Parrot.MahuaApis;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Newbe.Mahua.Plugins.Parrot.MahuaEvents
{
    /// <summary>
    /// 来自好友的私聊消息接收事件
    /// </summary>
    public class PrivateMessageFromFriendReceivedMahuaEvent
        : IPrivateMessageFromFriendReceivedMahuaEvent
    {
        public static IMahuaApi _mahuaApi;
        public static Login Login = null;
        public static List<string> path = new List<string>();
        public static CancellationTokenSource CancelTokenSource = new CancellationTokenSource();
        public static string mode = "male_r18";
        public static int page = 1;
        public static int total = -1;
        public static bool isDone = false;
        public static string date = null;
        public static int currentIndex = -1;
        public PrivateMessageFromFriendReceivedMahuaEvent(IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessFriendMessage(PrivateMessageFromFriendReceivedContext context)
        {           
            ///* --和群一样的功能 解开注释可以使用
            try
            {
                if (context.Message.Contains("mode_"))
                {
                    string key = context.Message.Replace("mode_", null);
                    _mahuaApi.SendPrivateMessage(context.FromQq).Text(string.Format("key:{0}", key)).Done();
                    if (Profile.Done.ContainsKey(key))
                    {
                        _mahuaApi.SendPrivateMessage(context.FromQq).Text(string.Format("包含key:{0}", key)).Done();
                        Dictionary<string, bool> tmpDone = new Dictionary<string, bool>(Profile.Done);
                        foreach (var item in tmpDone)
                        {
                            if (item.Key != key)
                            {
                                _mahuaApi.SendPrivateMessage(context.FromQq).Text(string.Format("key为false:{0}", item.Key)).Done();
                                Profile.Done[item.Key] = false;
                            }
                            else
                            {
                                _mahuaApi.SendPrivateMessage(context.FromQq).Text(string.Format("key为true:{0}", item.Key)).Done();
                                if (Profile.path[item.Key].Count <= 0)
                                {
                                    Profile.Done[item.Key] = false;
                                    try
                                    {
                                        _mahuaApi.SendPrivateMessage(context.FromQq).Text("路径正在加载,无法切换").Done();
                                    }
                                    catch (Exception ex)
                                    {                                  
                                    }

                                }
                                else
                                {
                                    Profile.Done[item.Key] = true;
                                    try
                                    {
                                        _mahuaApi.SendPrivateMessage(context.FromQq).Text(string.Format("已切换到{0}模式,发送\"不够色!\"or \"不够色！\" or \"不够色\"获取", key)).Done();
                                    }
                                    catch (Exception ex)
                                    {                                     
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            _mahuaApi.SendPrivateMessage(context.FromQq).Text(string.Format("不存在{0}模式", key)).Done();
                        }
                        catch (Exception ex)
                        {                          
                        }

                    }
                }
                else if (context.Message == ("关闭"))
                {
                    Dictionary<string, bool> tmpDone = new Dictionary<string, bool>(Profile.Done);
                    foreach (var item in tmpDone.Keys)
                    {
                        Profile.Done[item] = false;
                    }
                    try
                    {
                        _mahuaApi.SendPrivateMessage(context.FromQq).Text("已关闭").Done();
                    }
                    catch (Exception ex)
                    {                       
                    }

                }


                else if (context.Message == ("不够色！") || context.Message == ("不够色!") || context.Message == ("不够色"))
                {
                    bool flag = true;
                    if (flag)
                    {
                        if (Login.instance == null)
                        {
                            try
                            {
                                _mahuaApi.SendPrivateMessage(context.FromQq).Text("未开启").Done();
                            }
                            catch (Exception ex)
                            {                               
                            }

                        }
                        else
                        {
                            string key = null;
                            Dictionary<string, bool> tmpDone = new Dictionary<string, bool>(Profile.Done);
                            foreach (var item in tmpDone)
                            {
                                if (item.Value)
                                {
                                    key = item.Key;
                                    break;
                                }
                            }
                            if (key == null)
                            {
                                try
                                {
                                    _mahuaApi.SendPrivateMessage(context.FromQq).Text("未开启图片").Done();
                                }
                                catch (Exception ex)
                                {                                   
                                }
                            }
                            else
                            {
                                Profile.currentIndex[key]++;
                                int index = Profile.currentIndex[key];
                                var path = Profile.path[key];
                                if (index >= path.Count)
                                {
                                    try
                                    {
                                        _mahuaApi.SendPrivateMessage(context.FromQq).Text("目前无最新").Done();
                                    }
                                    catch (Exception ex)
                                    {                                        
                                    }

                                }
                                else
                                {
                                    string imagePath = path[index];
                                    if (File.Exists(imagePath))
                                    {
                                        try
                                        {
                                            _mahuaApi.SendPrivateMessage(context.FromQq).Image(imagePath).Done();
                                        }
                                        catch (Exception ex)
                                        {                                          
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            _mahuaApi.SendPrivateMessage(context.FromQq).Text("正在冷却").Done();
                        }
                        catch (Exception ex)
                        {                          
                        }
                    }


                }
            }
            catch (Exception ex)
            {
              
                _mahuaApi.SendPrivateMessage(context.FromQq).Text(ex.ToString()).Done();
               
            }
          //*/

        }
    }
}
