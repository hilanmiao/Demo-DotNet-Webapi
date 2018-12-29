# Demo-DotNet-Webapi
简易的webapi模板，.net webapi identity + swagger，基础的增删改查实例。

- [x] 基础功能：使用**.net webapi identity、swagger**构建
- [x] 基础功能：Swagger基础配置
- [x] 基础功能：Swagger文件上传配置
- [x] 基础功能：Swagger配置输入 Authorization 头部参数
- [x] 基础功能：Nuget安装Microsoft.Owin.Cors，解决跨域问题
- [x] 基础功能：统一返回格式以及例外处理
- [x] 业务功能：返回json键值统一为小写，解决json null问题，解决时间格式化问题
- [x] 业务功能：EF初始化数据
- [x] 业务功能：NLog记录日志
- [x] 业务功能：增删改查基础实例
- [x] 业务功能：上传文件实例

![Alt text](https://raw.githubusercontent.com/hilanmiao/Demo-DotNet-Webapi/master/Screen/screen1.jpg)

#### 软件架构
    "Microsoft.AspNet.WebApi": "5.2.3",
    "EntityFramework" : "6.1.3",
    "Microsoft.AspNet.Identity" : "2.2.1",
    "Microsoft.AspNet.Cors" : "5.0.0",
    "NLog" : "4.5.6",
    "Swagger.Net" : "0.5.5",

#### 安装教程

``` bash
使用VS 2015或更高版本软件打开，修改web.config文件的数据库连接，初次启动会自动初始化部分数据。

```

#### 使用说明

正常启动后，使用浏览器打开[http://localhost:55930/swagger](http://localhost:55930/swagger)。UploadFiles文件下有简单的开发说明和使用说明。

#### 参与贡献

1. Fork 本项目
2. 新建 Feat_xxx 分支
3. 提交代码
4. 新建 Pull Request