# Pixiv 
# 长时间未进行测试,效果未知

## Requirements:    
  .net 4.5 <br>
  Newtonsoft.Json.dll
## Description:
为某沙雕群编写的一个自动爬取[p站](https://www.pixiv.net/)上排行榜并同过机器人发送
<br>
~~**p站api已经失效,有时间会重制**~~<br>
**已经修复登录问题,可以正常使用**
## References:

[pixiv_API-c-](https://github.com/xingoxu/pixiv_API-c-)  
[Newbe.Mahua.Framework](https://github.com/Newbe36524/Newbe.Mahua.Framework)

## Usage:
阅读文档[开始第一个 QQ 机器人【适用于 v1.9-v1.X】](http://www.newbe.pro/2018/06/10/Newbe.Mahua/Begin-First-Plugin-With-Mahua-In-v1.9/)进行搭建环境,因为是使用通用接口,所以可以直接生成放在各平台去运行,优先推荐cqpPro,其次是mpq

具体配置在Profile.cs里
插件会将爬取得图片下载到D:/Pixiv/Mikot下,因p站会验证账号,目前我没有太好的办法发送在线图片,如果支持文件流可以发送在线

~~todo:如果保存在本地可以在byte[]后多加一个0,去改变图片原有的md5,可以在一定程度上去跳过已经被腾讯封禁的p站图片~~

需要科学上网,不然软件无法连接p站


<br><br><br><br><br>
**==============================================================**
**==============================================================**
<br><br><br><br>


# otome

## Description:
获取[百合居](http://otome.me/)月排行榜

## Usage:
和pixiv一样的规矩<br>不过会记录上次发送到那一页那一个 文件生成在D:/otome/page.txt



<br><br><br><br>
**==============================================================**
**==============================================================**
<br><br><br><br>

# yande

## Description: 
获取[yande](https://yande.re/post)里自定义搜索的图片

## Usage:
照旧<br>文件生成在D:/yande下<br>发送的时间会很长,可以考虑下载到本地然后像pixiv一样发送


<br><br><br><br>

**==============================================================**
**==============================================================**
<br><br><br><br>

# telegra
## Requirements:    
  .net 4.5 <br>
  Newtonsoft.Json.dll<br>
  System.Drawing.dll
 ## Description:
  获取telegram里的[匿名博客](https://telegra.ph)里天音莉莉的系列图集,因其发表人的链接我查找了一下已经失效故不在放出
  ## Usage:
   照旧<br>文件生成在D:/telegra下,因链接需要代理所以无法发送在线图片,会根据期数生成相对应文件夹,并下载图片包存在里面,然后通过本地链接去发送,如果是发送到telegram里可以直接发送链接<br>需要使用代理,否则无法访问<br>因为是在电报知道其中某一期后通过向前推去尝试获取第一期,结果代码短暂的跑到上一年只查到了[第三期](https://telegra.ph/%E6%B6%A9%E5%9B%BEtime-No3-08-29),所以从第三期开始发送<br>**注意:因博主没有按一天一期,有时鸽了,有时高产,所以我偷懒了，用try catch捕捉,只要失败就会从下一天开始,成功就期数加1,请确保自己网络没有问题或者没有其他bug,否则会出现一直从错误的时间开始访问,可以自行更改**
   <br><br><br><br><br>
   
# PS:

  心血来潮的产物,代码极烂,基本不考虑重构,大概
  
  基于MIT License

