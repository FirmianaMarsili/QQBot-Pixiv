using Newbe.Mahua.Logging;
using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Plugins.yande.MahuaApis;
using System;

namespace Newbe.Mahua.Plugins.yande.MahuaEvents
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
            if (context.Message.Contains("嬲"))
            {
                bool flag = false;
                TimeSpan time = DateTime.Now - Helper.dateTimeCD;
                flag = time.TotalSeconds > Helper.timeCD;
                if (flag)
                {
                    string imageUrl = Helper.GetPath();
                    if (imageUrl == null)
                    {
                        _mahuaApi.SendPrivateMessage(context.FromQq).Text("正在获取最新图片").Done();
                    }
                    else
                    {

                        Helper.dateTimeCD = DateTime.Now;
                        _mahuaApi.SendPrivateMessage(context.FromQq).Image(imageUrl).Done();
                        System.GC.Collect();
                    }
                }
                else
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq).Text(string.Format("正在冷却,剩余{0}秒", ((int)(Helper.timeCD - time.TotalSeconds)).ToString())).Done();
                }           
            }
        }
    }
}
