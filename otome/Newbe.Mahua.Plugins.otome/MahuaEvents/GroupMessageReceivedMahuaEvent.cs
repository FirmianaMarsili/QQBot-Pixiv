using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Plugins.otome.MahuaApis;
using System;

namespace Newbe.Mahua.Plugins.otome.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMessageReceivedMahuaEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupMessageReceivedMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            if (context.Message.Equals("嘤嘤嘤"))
            {

                bool flag = false;
                TimeSpan time = DateTime.Now - Helper.dateTime;
                flag = time.TotalSeconds > Helper.timeCD;
                if (flag)
                {
                    string imageUrl = Helper.GetPath(_mahuaApi);
                    if (imageUrl == null)
                    {
                        _mahuaApi.SendPrivateMessage(context.FromQq).Text("正在获取最新图片").Done();
                    }
                    else
                    {
                        Helper.dateTime = DateTime.Now;
                        _mahuaApi.SendGroupMessage(context.FromGroup).Image(imageUrl).At(context.FromQq).Done();
                        System.GC.Collect();
                    }
                }
                else
                {
                    _mahuaApi.SendGroupMessage(context.FromGroup).Text(string.Format("正在冷却,剩余{0}秒", ((int)(Helper.timeCD - time.TotalSeconds)).ToString())).At(context.FromQq).Done();
                }            
            }
        }
    }
}
