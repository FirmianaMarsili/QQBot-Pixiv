﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newbe.Mahua.Plugins.Parrot.MahuaApis
{
    public class Profile
    {
        //p站账号
        public const string userName = "";
        public const string passWord = "";

        public const uint limitCount = 1000; //限制每个排行最多下载多少,一个作者会有多张图片 如果不限制基本上可以是20g起步

        public const string ExceptionSender = "";
        //不下载某类型图片  0插图 1漫画 2动图
        public static  List<int> black = new List<int>
        {
            1            
        };

        public static Newbe.Mahua.IMahuaApi _mahuaApi
        {
            get
            {
                return Newbe.Mahua.Plugins.Parrot.MahuaEvents.PrivateMessageFromFriendReceivedMahuaEvent._mahuaApi;
            }
        }
        public static Dictionary<string, bool> Done = new Dictionary<string, bool>
        {
            ["daily"] = false,
            ["weekly"] = false,
            ["monthly"] = false,
            ["rookie"] = false,
            ["daily_r18"] = false,
            ["weekly_r18"] = false,
            ["r18g"] = false,
            ["original"] = false,
            ["male"] = false,
            ["female"] = false,
            ["male_r18"] = false,
            ["female_r18"] = false,
        };
        //保存所有图片的地址,因p站会验证,所以目前我没办法发送在线图片,只能保存在本地
        public static Dictionary<string, List<string>> path = new Dictionary<string, List<string>>
        {
            ["daily"] = new List<string>(),
            ["weekly"] = new List<string>(),
            ["monthly"] = new List<string>(),
            ["rookie"] = new List<string>(),
            ["daily_r18"] = new List<string>(),
            ["weekly_r18"] = new List<string>(),
            ["r18g"] = new List<string>(),
            ["original"] = new List<string>(),
            ["male"] = new List<string>(),
            ["female"] = new List<string>(),
            ["male_r18"] = new List<string>(),
            ["female_r18"] = new List<string>(),
        };
        public static Dictionary<string, int> currentIndex = new Dictionary<string, int>
        {
            ["daily"] = -1,
            ["weekly"] = -1,
            ["monthly"] = -1,
            ["rookie"] = -1,
            ["daily_r18"] = -1,
            ["weekly_r18"] = -1,
            ["r18g"] = -1,
            ["original"] = -1,
            ["male"] = -1,
            ["female"] = -1,
            ["male_r18"] = -1,
            ["female_r18"] = -1,
        };
    }
}
