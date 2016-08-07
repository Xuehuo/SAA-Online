<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="dashboardPage" %>

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
    <link href="css/basic.css" rel="stylesheet">
    <link rel="icon" href="image/favicon.ico">
    <!--[if lt IE 9]>
      <script src="//cdn.bootcss.com/html5shiv/r29/html5.min.js"></script>
      <script src="//cdn.bootcss.com/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body id="dashboard">
    <div id="menu">
        <div id="logo">
            <img src="image/logo.png" alt="SAAO">
        </div>
        <ul>
            <li><a href="dashboard"><span>仪表盘</span><span class="glyphicon glyphicon-home"></span></a></li>
            <li><a href="notification"><span>通知</span><span class="glyphicon glyphicon-comment"></span></a></li>
            <li><a href="mail"><span>邮件</span><span class="glyphicon glyphicon-envelope"></span></a></li>
            <li><a href="file"><span>文件</span><span class="glyphicon glyphicon-folder-open"></span></a></li>
            <li><a href="calendar"><span>日历</span><span class="glyphicon glyphicon-calendar"></span></a></li>
            <li><a href="contact"><span>联系人</span><span class="glyphicon glyphicon-user"></span></a></li>
        </ul>
    </div>
    <div id="workspace">
        <nav id="topbar" class="navbar navbar-default" role="navigation">
            <div class="container-fluid">
                <div class="navbar-header">
                    <a class="navbar-brand">仪表盘</a>
                </div>
                <ul class="nav navbar-nav navbar-right">
                    <li class="dropdown"><a href="#" class="dropdown-toggle" data-toggle="dropdown"><%=SAAO.User.Current.Realname %> <b class="caret"></b></a>
                        <ul class="dropdown-menu">
                            <li><a href="#" data-toggle="modal" data-target="#passwordmodal">修改密码</a></li>
                            <li class="divider"></li>
                            <li><a href="#" onclick="userLogout()">注销</a></li>
                        </ul>
                    </li>
                </ul>
            </div>
        </nav>
        <div id="content">
            <div class="panel panel-default">
                <div class="panel-heading" id="grouphead">
                </div>
                <table class="table">
                    <thead>
                        <tr>
                            <th>今天开始</th>
                            <th>正在进行</th>
                            <th>今天结束</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <ol id="taskbegin"></ol>
                            </td>
                            <td>
                                <ol id="taskdoing"></ol>
                            </td>
                            <td>
                                <ol id="tasktodo"></ol>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <div class="modal fade" id="passwordmodal" tabindex="-1" role="dialog" aria-hidden="true">
        <form class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">修改密码</h4>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <label class="col-sm-2 control-label" for="psnow">当前密码</label>
                            <div class="col-sm-7">
                                <input placeholder="现在的密码" class="form-control" type="password" id="psnow">
                                <p class="help-block">在这里输入你现在的密码</p>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-2 control-label" for="psnew">新密码</label>
                            <div class="col-sm-7">
                                <input placeholder="新密码" class="form-control" type="password" id="psnew">
                                <p class="help-block">在这里输入你的新密码，长度在6-18个字符，不包含空格或中文字</p>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-2 control-label" for="repsnew">重复密码</label>
                            <div class="col-sm-7">
                                <input placeholder="再次输入你的新密码" class="form-control" type="password" id="repsnew">
                                <p class="help-block">请在这里再次输入你设定的新密码</p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">取消</button>
                    <button type="submit" class="btn btn-primary">确定修改</button>
                </div>
            </div>
        </form>
    </div>
    <script src="//cdn.bootcss.com/jquery/2.1.4/jquery.min.js"></script>
    <script src="//cdn.bootcss.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>
    <script src="//cdn.bootcss.com/pnotify/2.0.0/pnotify.all.min.js"></script>
    <link href="//cdn.bootcss.com/pnotify/2.0.0/pnotify.all.min.css" rel="stylesheet">
    <script src="js/basic.js"></script>
    <script src="js/dashboard.js"></script>
</body>
</html>

