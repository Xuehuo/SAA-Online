<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Contact.aspx.cs" Inherits="contactPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no">
    <meta name="description" content="SAAO，学活在线协作平台，是由学活自主开发的工作协作平台。">
    <meta name="keywords" content="学生活动中心,深圳中学,在线协作平台,SAAO">
    <meta name="theme-color" content="#085E08">
    <title>SAA Online</title>
    <link href="css/basic.css" rel="stylesheet">
    <link rel="icon" href="image/favicon.ico">
    <!--[if lt IE 9]>
      <script src="//cdn.bootcss.com/html5shiv/r29/html5.min.js"></script>
      <script src="//cdn.bootcss.com/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body id="contact">
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
                <div class="navbar-header"><a class="navbar-brand">联系人</a></div>
                <ul class="nav navbar-nav navbar-left">
                    <li><a href="#" data-toggle="modal" data-target="#filtermodal"><span class="glyphicon glyphicon-filter"></span><span class="hidden-xs">筛选</span></a></li>
                    <li><a href="#" data-toggle="modal" data-target="#helpmodal"><span class="glyphicon glyphicon-question-sign"></span><span class="hidden-xs">帮助</span></a></li>
                </ul>
                <ul class="nav navbar-nav navbar-right">
                    <li class="dropdown"><a href="#" class="dropdown-toggle" data-toggle="dropdown"><%=SAAO.User.Current.realname %> <b class="caret"></b></a>
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
            <div id="container">
                <h2>没有找到联系人</h2>
            </div>
        </div>
    </div>
    <div class="modal fade" id="filtermodal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button><h4 class="modal-title">筛选</h4>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <label>年级</label>
                            <div class="btn-group" role="group">
                                <button type="button" class="btn btn-default" data-filter="s1" onclick="contactFilter(0,this)">高一</button>
                                <button type="button" class="btn btn-default" data-filter="s2" onclick="contactFilter(0,this)">高二</button>
                            </div>
                        </div>
                        <div class="form-group">
                            <label>组别</label>
                            <div class="btn-group" role="group">
                                <% for (int i = 0; i < SAAO.Organization.Current.structure.Select("[group] IS NOT NULL").Length; i++) { %>
                                <button type="button" class="btn btn-default" data-filter="g<%=i %>" onclick="contactFilter(1,this)"><%=SAAO.Organization.Current.GetGroupName(i) %></button>
                                <%} %>
                            </div>
                        </div>
                        <div class="form-group">
                            <label>姓</label>
                            <div class="btn-group btn-group-xs" role="group">
                                <button type="button" class="btn btn-default" data-filter="c1" onclick="contactFilter(2,this)">A</button>
                                <button type="button" class="btn btn-default" data-filter="c2" onclick="contactFilter(2,this)">B</button>
                                <button type="button" class="btn btn-default" data-filter="c3" onclick="contactFilter(2,this)">C</button>
                                <button type="button" class="btn btn-default" data-filter="c4" onclick="contactFilter(2,this)">D</button>
                                <button type="button" class="btn btn-default" data-filter="c5" onclick="contactFilter(2,this)">E</button>
                                <button type="button" class="btn btn-default" data-filter="c6" onclick="contactFilter(2,this)">F</button>
                                <button type="button" class="btn btn-default" data-filter="c7" onclick="contactFilter(2,this)">G</button>
                                <button type="button" class="btn btn-default" data-filter="c8" onclick="contactFilter(2,this)">H</button>
                                <button type="button" class="btn btn-default" data-filter="c9" onclick="contactFilter(2,this)">I</button>
                                <button type="button" class="btn btn-default" data-filter="c10" onclick="contactFilter(2,this)">J</button>
                                <button type="button" class="btn btn-default" data-filter="c11" onclick="contactFilter(2,this)">K</button>
                                <button type="button" class="btn btn-default" data-filter="c12" onclick="contactFilter(2,this)">L</button>
                                <button type="button" class="btn btn-default" data-filter="c13" onclick="contactFilter(2,this)">M</button>
                                <button type="button" class="btn btn-default" data-filter="c14" onclick="contactFilter(2,this)">N</button>
                                <button type="button" class="btn btn-default" data-filter="c15" onclick="contactFilter(2,this)">O</button>
                                <button type="button" class="btn btn-default" data-filter="c16" onclick="contactFilter(2,this)">P</button>
                                <button type="button" class="btn btn-default" data-filter="c17" onclick="contactFilter(2,this)">Q</button>
                                <button type="button" class="btn btn-default" data-filter="c18" onclick="contactFilter(2,this)">R</button>
                                <button type="button" class="btn btn-default" data-filter="c19" onclick="contactFilter(2,this)">S</button>
                                <button type="button" class="btn btn-default" data-filter="c20" onclick="contactFilter(2,this)">T</button>
                                <button type="button" class="btn btn-default" data-filter="c21" onclick="contactFilter(2,this)">U</button>
                                <button type="button" class="btn btn-default" data-filter="c22" onclick="contactFilter(2,this)">V</button>
                                <button type="button" class="btn btn-default" data-filter="c23" onclick="contactFilter(2,this)">W</button>
                                <button type="button" class="btn btn-default" data-filter="c24" onclick="contactFilter(2,this)">X</button>
                                <button type="button" class="btn btn-default" data-filter="c25" onclick="contactFilter(2,this)">Y</button>
                                <button type="button" class="btn btn-default" data-filter="c26" onclick="contactFilter(2,this)">Z</button>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">取消</button>
                    <button type="button" class="btn btn-primary" onclick="contactApplyFilter()">确定</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="contactmodal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button><h4 class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal">
                        <dl class="dl-horizontal">
                            <dt>组别</dt>
                            <dd></dd>
                            <dt>职务</dt>
                            <dd></dd>
                            <dt>电话</dt>
                            <dd></dd>
                            <dt>邮箱</dt>
                            <dd></dd>
                            <dt>班级</dt>
                            <dd></dd>
                        </dl>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">确定</button>
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
                    <p>若需更改资料请联系网络组</p>
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
    <script src="plugin/jquery.mixitup.min.js"></script>
    <script src="js/basic.js"></script>
    <script src="js/contact.js"></script>
</body>
</html>
