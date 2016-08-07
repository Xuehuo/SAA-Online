<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Notification.aspx.cs" Inherits="notificationPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no">
    <meta name="description" content="SAAO，学活在线协作平台，是由学活自主开发的工作协作平台。">
    <meta name="keywords" content="学生活动中心,深圳中学,在线协作平台,SAAO">
    <meta name="theme-color" content="#98A000">
    <title>SAA Online</title>
    <link href="css/basic.css" rel="stylesheet">
    <link rel="icon" href="image/favicon.ico">
    <!--[if lt IE 9]>
      <script src="//cdn.bootcss.com/html5shiv/r29/html5.min.js"></script>
      <script src="//cdn.bootcss.com/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body id="notification">
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
                    <a class="navbar-brand">通知</a>
                </div>
                <ul class="nav navbar-nav navbar-left">
                    <li><a href="#" onclick="notificationList()"><span class="glyphicon glyphicon-repeat"></span><span class="hidden-xs"> 刷新</span></a></li>
                    <% if (SAAO.User.Current.IsExecutive || SAAO.User.Current.IsSupervisor || SAAO.User.Current.IsGroupHeadman)
                        { %>
                    <li><a href="#" onclick="notificationCompose()"><span class="glyphicon glyphicon-edit"></span><span class="hidden-xs"> 发布通知</span></a></li>
                    <%} %>
                    <li class="dropdown">
                        <a href="#" class="dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-filter"></span><span class="hidden-xs"> 筛选</span> <b class="caret"></b></a>
                        <ul class="dropdown-menu">
                            <li><a href="#" onclick="notificationList(1)"><span class="glyphicon glyphicon-tags"></span> 全员通知</a></li>
                            <li><a href="#" onclick="notificationList(2)"><span class="glyphicon glyphicon-tag"></span> 组内通知</a></li>
                            <li class="divider"></li>
                            <li><a href="#" onclick="notificationList(3)"><span class="glyphicon glyphicon-file"></span> 监督报告</a></li>
                        </ul>
                    </li>
                    <li class="hidden-xs"><a href="#" data-toggle="modal" data-target="#helpmodal"><span class="glyphicon glyphicon-question-sign"></span><span class="hidden-xs">帮助</span></a></li>
                </ul>
                <ul class="nav navbar-nav navbar-right">
                    <li class="dropdown">
                        <a href="#" class="dropdown-toggle" data-toggle="dropdown"><%=SAAO.User.Current.Realname %> <b class="caret"></b></a>
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
            <ol class="breadcrumb">
                <li id="folder">全部通知</li>
            </ol>
            <div id="container"></div>
        </div>
    </div>
    <div class="modal fade" id="notificationmodal" tabindex="-1" role="dialog" aria-hidden="true">
        <form class="modal-dialog" enctype="multipart/form-data" method="post">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <ul class="nav nav-tabs">
                        <%if (SAAO.User.Current.IsExecutive || SAAO.User.Current.IsGroupHeadman) { %>
                        <li><a href="#notice" data-toggle="tab">发布通知</a></li>
                        <%} if (SAAO.User.Current.IsSupervisor) { %>
                        <li><a href="#report" data-toggle="tab">监督报告</a></li>
                        <%} %>
                    </ul>
                </div>
                <div class="modal-body">
                    <div class="tab-content">
                        <div class="tab-pane" id="notice">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="input-group">
                                        <div class="input-group-btn">
                                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown"><span id="visibility">可见级别 </span><span class="caret"></span></button>
                                            <ul class="dropdown-menu">
                                                <li><a href="#" onclick="$('#visibility').html('组内通知 ');">组内通知 </a></li>
                                                <li><a href="#" onclick="$('#visibility').html('全员通知 ');">全员通知 </a></li>
                                            </ul>
                                        </div>
                                        <input type="text" id="noticetitle" class="form-control" placeholder="通知标题">
                                        <%if (SAAO.User.Current.IsExecutive) { %>
                                        <span class="input-group-addon">
                                            <span class="glyphicon glyphicon-flag"></span>设为重要
                                            <input type="checkbox">
                                        </span>
                                        <%} %>
                                    </div>
                                </div>
                            </div>
                            <textarea id="noticecontent" class="form-control" rows="6" placeholder="通知内容"></textarea>
                        </div>
                        <div class="tab-pane" id="report">
                            <div class="form-group">
                                <label for="exampleInputFile">上传监督报告文档</label>
                                <p class="help-block">为避免文档排版差异,请上传pdf格式</p>
                                <input type="file" name="reportfile">
                            </div>
                            <p class="help-block">监督报告概要将显示在通知区主页</p>
                            <textarea name="reportabstract" class="form-control" rows="2" placeholder="监督报告概要"></textarea>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">取消</button>
                    <button type="submit" class="btn btn-primary">发布</button>
                </div>
            </div>
        </form>
    </div>
    <div class="modal fade" id="reportmodal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">监督报告</h4>
                </div>
                <div class="modal-body">
                    <iframe src="#" id="reportframe"></iframe>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
                    <a href="#" target="_blank" class="btn btn-primary">下载</a>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="helpmodal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">帮助</h4>
                </div>
                <div class="modal-body">
                    <p>通知系统并不完善，如有问题请联系技术部。</p>
                    <p>若要查看监督报告，请点击监督报告的卡片。</p>
                    <hr>
                    <p>发布通知权限</p>
                    <table class="table table-condensed">
                        <caption></caption>
                        <thead>
                            <tr>
                                <th>职务</th>
                                <th>发布组内通知</th>
                                <th>发布全员通知</th>
                                <th>发布监督报告</th>
                                <th>设置重要通知</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>高一各组组长</td>
                                <td><i class="glyphicon glyphicon-ok"></i></td>
                                <td><i class="glyphicon glyphicon-ok"></i></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>高二全体高层</td>
                                <td><i class="glyphicon glyphicon-ok"></i></td>
                                <td><i class="glyphicon glyphicon-ok"></i></td>
                                <td></td>
                                <td><i class="glyphicon glyphicon-ok"></i></td>
                            </tr>
                            <tr>
                                <td>审计组组员</td>
                                <td></td>
                                <td></td>
                                <td><i class="glyphicon glyphicon-ok"></i></td>
                                <td></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
                </div>
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
    <script src="//mozilla.github.io/pdf.js/build/pdf.js"></script>
    <script src="js/basic.js"></script>
    <script src="js/notification.js"></script>
</body>
</html>

