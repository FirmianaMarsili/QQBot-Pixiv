using Newbe.Mahua.MahuaEvents;
using System;
using System.Collections.Generic;
using Newbe.Mahua.Plugins.Parrot.MahuaApis;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Newbe.Mahua.Plugins.Parrot.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMessageReceivedMahuaEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
     

        public GroupMessageReceivedMahuaEvent(IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            try
            {
                if (context.FromQq == Profile.ExceptionSender)
                {
                    if (context.Message.Contains("mode_"))
                    {
                        string key = context.Message.Replace("mode_", null);                       
                        if (Profile.Done.ContainsKey(key))
                        {
                            Dictionary<string, bool> tmpDone = new Dictionary<string, bool>(Profile.Done);
                            foreach (var item in tmpDone)
                            {
                                if (item.Key != key)
                                {
                                    Profile.Done[item.Key] = false;
                                }
                                else
                                {
                                    if (Profile.path[item.Key].Count <= 0)
                                    {
                                        Profile.Done[item.Key] = false;
                                        try
                                        {
                                            _mahuaApi.SendGroupMessage(context.FromGroup).Text("路径正在加载,无法切换").Done();
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
                                            _mahuaApi.SendGroupMessage(context.FromGroup).Text(string.Format("已切换到{0}模式,发送\"不够色!\"or \"不够色！\" or \"不够色\"获取", key)).Done();
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
                                _mahuaApi.SendGroupMessage(context.FromGroup).Text(string.Format("不存在{0}模式", key)).Done();
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
                            _mahuaApi.SendGroupMessage(context.FromGroup).Text("已关闭").Done();
                        }
                        catch (Exception ex)
                        {                          
                        }

                    }
                }
                else
                {
                    if (context.Message == ("不够色！") || context.Message == ("不够色!") || context.Message == ("不够色"))
                    {                        
                        bool flag = true;
                        if (flag)
                        {
                            if (Login.instance == null)
                            {
                                try
                                {
                                    _mahuaApi.SendGroupMessage(context.FromGroup).Text("未开启").At(context.FromQq).Done();
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
                                        _mahuaApi.SendGroupMessage(context.FromGroup).Text("未开启图片").At(context.FromQq).Done();
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
                                            _mahuaApi.SendGroupMessage(context.FromGroup).Text("目前无最新").At(context.FromQq).Done();
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
                                                _mahuaApi.SendGroupMessage(context.FromGroup).Image(imagePath).At(context.FromQq).Done();
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
                                _mahuaApi.SendGroupMessage(context.FromGroup).Text("正在冷却").At(context.FromQq).Done();
                            }
                            catch (Exception ex)
                            {                                
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {              
               _mahuaApi.SendGroupMessage(context.FromGroup).Text(ex.ToString()).At(Profile.ExceptionSender).Done();              
            }
         
        }
   
    }
}
