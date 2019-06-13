using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Plugins.yande.MahuaApis;
using System;

namespace Newbe.Mahua.Plugins.yande.MahuaEvents
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
            if (context.Message.Contains("嬲") || context.Message.Contains("女管理") || context.Message.Contains("幼稚") || context.Message.Contains("东东") || context.Message.Contains("冬冬"))
            {
                bool flag = false;
                TimeSpan time = DateTime.Now - Helper.dateTimeCD;
                flag = time.TotalSeconds > Helper.timeCD;
                if (flag)
                {
                    string imageUrl = Helper.GetPath();
                    if (imageUrl == null)
                    {
                        _mahuaApi.SendGroupMessage(context.FromGroup).Text("正在获取最新图片").At(context.FromQq).Done();
                    }
                    else
                    {
                        Helper.dateTimeCD = DateTime.Now;
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
