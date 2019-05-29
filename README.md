# QQBot-Pixiv

## Requirements:    
  .net 4.5 
## Profile:
为某沙雕群编写的一个自动爬取p站上排行榜并同过机器人发送
## References:
[pixiv_API-c-](https://github.com/xingoxu/pixiv_API-c-)\n\r
[Newbe.Mahua.Framework](https://github.com/Newbe36524/Newbe.Mahua.Framework)

## How To Use:
阅读文档[开始第一个 QQ 机器人【适用于 v1.9-v1.X】](http://www.newbe.pro/2018/06/10/Newbe.Mahua/Begin-First-Plugin-With-Mahua-In-v1.9/)进行搭建环境,因为是使用通用接口,所有可以直接生成放在各平台去运行,优先推荐cqpPro,其次是mpq,切记不要用cleverqq,我使用框架写个零功能的放进去都线程报错

将具体配置在Profile.cs里
插件会将爬取得图片下载到D:/Unity/Mikot下,因p站会验证账号,目前我没有太好的办法发送在线图片


基于MIT License

### PS:
  心血来潮的产物,代码极烂,基本不考虑重构和添加新功能,大概

