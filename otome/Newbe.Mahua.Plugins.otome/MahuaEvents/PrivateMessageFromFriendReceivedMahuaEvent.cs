using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Plugins.otome.MahuaApis;
using System;

namespace Newbe.Mahua.Plugins.otome.MahuaEvents
{
    /// <summary>
    /// 来自好友的私聊消息接收事件
    /// </summary>
    public class PrivateMessageFromFriendReceivedMahuaEvent
        : IPrivateMessageFromFriendReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public PrivateMessageFromFriendReceivedMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessFriendMessage(PrivateMessageFromFriendReceivedContext context)
        {
            if (context.Message.Equals("百合"))
            {
                _mahuaApi.SendPrivateMessage(context.FromQq).Text("1").Done();
                bool flag = false;
                TimeSpan time = DateTime.Now - Helper.dateTime;
                flag = time.TotalSeconds > Helper.timeCD;
                if (flag)
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq).Text("2").Done();
                    string imageUrl = Helper.GetPath();
                    if (imageUrl == null)
                    {
                        _mahuaApi.SendPrivateMessage(context.FromQq).Text("正在获取最新图片").Done();
                    }
                    else
                    {
                        _mahuaApi.SendPrivateMessage(context.FromQq).Text("3").Done();
                        _mahuaApi.SendPrivateMessage(context.FromQq).Text(imageUrl).Done();
                        
                        Helper.dateTime = DateTime.Now;
                        _mahuaApi.SendPrivateMessage(context.FromQq).Image(imageUrl).Done();
                        System.GC.Collect();
                    }
                }
                else
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq).Text(string.Format("正在冷却,剩余{0}秒", ((int)(Helper.timeCD - time.TotalSeconds)).ToString())).Done();
                }
                //if (time.TotalSeconds < Profile.timeCD)
                //{
                //    flag = false;
                //}
                //else
                //{
                //    flag = true;
                //    Profile.dateTime = DateTime.Now;
                //}
                //if (flag)
                //{

                //}
            }
        }
    }
}
