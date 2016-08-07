<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Mail.aspx.cs" Inherits="mailPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no">
    <meta name="description" content="SAAO，学活在线协作平台，是由学活自主开发的工作协作平台。">
    <meta name="keywords" content="学生活动中心,深圳中学,在线协作平台,SAAO">
    <meta name="theme-color" content="#007480">
    <title>SAA Online</title>
    <link href="css/basic.css" rel="stylesheet">
    <link rel="icon" href="image/favicon.ico">
    <!--[if lt IE 9]>
      <script src="//cdn.bootcss.com/html5shiv/r29/html5.min.js"></script>
      <script src="//cdn.bootcss.com/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body id="mail">
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
                    <a class="navbar-brand">邮件</a>
                </div>
                <ul class="nav navbar-nav navbar-left">
                    <li><a href="#" onclick="mailCompose()"><span class="glyphicon glyphicon-pencil"></span><span class="hidden-xs">撰写邮件</span></a></li>
                    <li class="dropdown">
                        <a href="#" class="dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-folder-open"></span><span class="hidden-xs">文件夹</span> <b class="caret"></b></a>
                        <ul class="dropdown-menu">
                            <li><a href="#" data-folder="INBOX" onclick="mailFolder(this)"><span class="glyphicon glyphicon-inbox"></span>收件箱</a></li>
                            <li><a href="#" data-folder="Sent" onclick="mailFolder(this)"><span class="glyphicon glyphicon-send"></span>发件箱</a></li>
                            <li><a href="#" data-folder="Drafts" onclick="mailFolder(this)"><span class="glyphicon glyphicon-pencil"></span>草稿</a></li>
                            <li class="divider"></li>
                            <li><a href="#" data-folder="Trash" onclick="mailFolder(this)"><span class="glyphicon glyphicon-trash"></span>垃圾箱</a></li>
                        </ul>
                    </li>
                    <li><a href="../mail/webmail.aspx" target="_blank"><span class="glyphicon glyphicon-share-alt"></span><span class="hidden-xs">完整邮箱</span></a></li>
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
            <div id="maillist">
                <ol class="breadcrumb">
                    <li><%=SAAO.User.Current.Mail %></li>
                    <li>收件箱</li>
                </ol>
                <table class="table table-hover">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>发件人</th>
                            <th>主题</th>
                            <th>时间</th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                </table>
            </div>
            <div id="mailpreview">
                <ol class="breadcrumb">
                    <li><a href="#" onclick="mailList()">返回</a></li>
                    <li><a href="#" onclick="mailReply()">回复</a></li>
                    <li><a href="#" onclick="mailForward()">转发</a></li>
                    <li><a href="#" onclick="mailDelete()">删除</a></li>

                </ol>
                <div id="maildetail">
                    <dl class="dl-horizontal">
                        <dt>主题</dt>
                        <dd id="mailsubject"></dd>
                        <dt>发件人</dt>
                        <dd id="mailfrom"></dd>
                        <dt>收件人</dt>
                        <dd id="mailto"></dd>
                        <dt>时间</dt>
                        <dd id="mailtime"></dd>
                        <dt id="mailattachdt">附件</dt>
                        <dd id="mailattach"></dd>
                    </dl>
                    <hr />
                </div>
                <iframe id="mailframe" src="#" frameborder="0" onload="$(this).fadeIn('fast');"></iframe>
            </div>
        </div>
    </div>
    <div class="modal fade" id="mailmodal" tabindex="-1" role="dialog" aria-hidden="true">
        <form class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">撰写邮件</h4>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <label for="recipient" class="col-sm-1 control-label">收件人</label>
                            <div class="col-sm-11">
                                <input type="text" class="form-control" id="recipient" placeholder="收件人（用半角逗号隔开）">
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="subject" class="col-sm-1 control-label">主题</label>
                            <div class="col-sm-11">
                                <input type="text" class="form-control" id="subject" placeholder="主题">
                            </div>
                        </div>

                    </div>
                    <textarea id="mailcontent"></textarea>
                </div>
                <div class="modal-footer">
                    <!--<button type="button" class="btn btn-primary btn-xs pull-left">添加附件</button>-->
                    <p class="text-muted pull-left">暂不提供附件服务。</p>
                    <button type="button" class="btn btn-default" data-dismiss="modal">取消</button>
                    <button type="submit" class="btn btn-primary">发送</button>
                </div>
            </div>
        </form>
    </div>
    <div class="modal fade" id="helpmodal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">帮助</h4>
                </div>
                <div class="modal-body">
                    <p>此处的邮件系统功能并不完善，如需要完整的邮件系统请在顶栏中跳转。</p>
                    <p>使用此处系统之前，请先使用手机电子邮件或完整版邮件系统，避免无法正常载入文件夹。</p>
                    <p>此处邮件系统不能提供发送附件的功能，若需要该功能，请使用完整版。</p>
                    <hr>
                    <p>在手机上使用电子邮件的简要说明</p>
                    <ol>
                        <li>你需要手动设置或选择其他（IMAP/SMTP、SMTP）</li>
                        <li>收发件服务器地址均为：xuehuo.shenzhong.net</li>
                        <li>请注意端口号（IMAP 143、POP3 110、SMTP 25）</li>
                        <li>可选择SSL证书登录（端口号会变化）</li>
                    </ol>
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
    <script src="//cdn.bootcss.com/tinymce/4.2.0/tinymce.min.js"></script>
    <script src="//cdn.bootcss.com/tinymce/4.2.0/langs/zh_CN.js"></script>
    <script src="plugin/base64.min.js"></script>
    <script src="js/basic.js"></script>
    <script src="js/mail.js"></script>
</body>
</html>
