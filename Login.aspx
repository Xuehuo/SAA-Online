<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="LoginPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no">
    <meta name="description" content="SAAO，学活在线协作平台，是由学活自主开发的工作协作平台。">
    <meta name="keywords" content="学生活动中心,深圳中学,在线协作平台,SAAO">
    <meta name="theme-color" content="#2C3E50">
    <title>SAA Online</title>
    <link href="css/login.css" rel="stylesheet">
    <link rel="icon" href="image/favicon.ico">
    <!--[if lt IE 9]>
      <script src="//cdn.bootcss.com/html5shiv/r29/html5.min.js"></script>
      <script src="//cdn.bootcss.com/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body>
    <div id="login">
        <div id="logo">
            <img src="image/logo.png" alt="SAAO">
        </div>
        <form role="form">
            <div class="form-group">
                <input type="text" class="form-control" id="username" placeholder="用户名" data-toggle="tooltip" data-placement="right" title="用户名">
            </div>
            <div class="form-group">
                <input type="password" class="form-control" id="password" placeholder="密码" data-toggle="tooltip" data-placement="right" title="初次登录请务必修改密码。若忘记密码，请联系网络组">
            </div>
            <button type="submit" class="btn btn-primary btn-block" data-toggle="tooltip" data-placement="bottom" title="会话将于浏览器关闭后结束">登录</button>
            <% if (Wechat != null) { %>
            <br>
            <div class="alert alert-success" role="alert">
                在此处登录即可将您的学活工作网络账户与微信账户（<strong><%=Wechat %></strong>）绑定。
            </div>
            <% } %>
        </form>
    </div>
    <script src="//cdn.bootcss.com/jquery/2.1.4/jquery.min.js"></script>
    <script src="//cdn.bootcss.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>
    <script src="//cdn.bootcss.com/pnotify/2.0.0/pnotify.all.min.js"></script>
    <link href="//cdn.bootcss.com/pnotify/2.0.0/pnotify.all.min.css" rel="stylesheet">
    <script src="js/login.js"></script>
</body>
</html>
