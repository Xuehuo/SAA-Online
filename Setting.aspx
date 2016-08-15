<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Setting.aspx.cs" Inherits="SettingPage" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ContentPlaceHolderID="PageMenu" runat="server">
    
</asp:Content>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <div class="panel panel-default">
        <div class="panel-heading">账号登陆</div>
        <div class="panel-body">
            <div class="col-md-6" style="display: none">
                <h4>微信绑定</h4>
                <ol>
                    <li>关注学活订阅号（SMS_SAA）</li>
                    <li>发送<code>xsfpdkd24ad80a4114</code>到订阅号</li>
                    <li>订阅号回复绑定成功</li>
                    <li>点击<a href="#">这里</a>刷新状态</li>
                </ol>
                <h4>微信登陆</h4>
                <ol>
                    <li>向订阅号发送<code>saao</code>（不区分大小写）</li>
                    <li>订阅号回复一个一次性的登录链接</li>
                    <li>打开链接（打开即自动登录），在微信浏览器中使用SAAO</li>
                </ol>
                <p>当前已绑定微信号<code>test</code> <button class="btn btn-danger btn-xs">解除绑定</button>
                </p>
            </div>
            <div class="col-md-6">
                <h4>登陆链接</h4>
                <p>在此处可获得一个永久性的登录链接，您可以将其保存为桌面上的快捷方式或者浏览器书签以便快速登陆。为保安全，在生成该链接前需要您设置一个四位<code>PIN</code>，在使用该链接登录时，只需在密码栏输入4位的<code>PIN</code>即可。需要特别注意的是，这个<code>PIN</code>不会被存储在服务器上，丢失后可重新生成，之前生成的链接不会失效，若不慎泄露链接，请修改密码。
                </p>
                <form class="form-inline">
                    <div class="form-group">
                        <label for="pin">PIN</label>
                        <input type="text" class="form-control" id="pin" placeholder="PIN" maxlength="4" style="width: 60px">
                    </div>
                    <div class="form-group">
                        <label for="pwd">密码</label>
                        <input type="password" class="form-control" id="pwd" placeholder="密码">
                    </div>
                    <a href="#" class="btn btn-default" onclick="settingGenerateLink(this);return false;">生成</a>
                </form>
            </div>
        </div>
    </div>
    <div class="panel panel-default">
        <div class="panel-heading">修改密码</div>
        <div class="panel-body">
            <form class="form-horizontal" id="password">
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
                <div class="form-group">
                    <div class="col-sm-offset-2 col-sm-7">
                        <button type="submit" class="btn btn-default">确定修改</button>
                    </div>
                </div>
            </form>
        </div>
    </div>
    <div class="panel panel-default">
        <div class="panel-heading">个人信息</div>
        <div class="panel-body">
            <form class="form-horizontal" id="info">
                <div class="form-group">
                    <label class="col-sm-2 control-label">姓名</label>
                    <div class="col-sm-10">
                        <p class="form-control-static"><%=SAAO.User.Current.Realname %></p>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-2 control-label">用户名</label>
                    <div class="col-sm-10">
                        <p class="form-control-static" id="username"><%=SAAO.User.Current.Username %></p>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-2 control-label">职务</label>
                    <div class="col-sm-10">
                        <p class="form-control-static"><%=SAAO.Organization.Current.GetGroupName(SAAO.User.Current.Group) + SAAO.Organization.Current.GetJobName(SAAO.User.Current.Job) %></p>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-2 control-label" for="phonenum">手机号码</label>
                    <div class="col-sm-7">
                        <input placeholder="手机号码" class="form-control" type="text" id="phonenum" value="<%=SAAO.User.Current.Phone %>" maxlength="11">
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-2 control-label" for="email">私人邮箱</label>
                    <div class="col-sm-7">
                        <input placeholder="私人邮箱" class="form-control" type="text" id="email" value="<%=SAAO.User.Current.Mail %>">
<%--                        <div class="checkbox">
                            <label>
                                <input type="checkbox" disabled="disabled"> 将工作邮箱的邮件转发到私人邮箱
                            </label>
                        </div>--%>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-2 control-label" for="classnum">班级</label>
                    <div class="col-sm-7">
                        <div class="input-group">
                            <div class="input-group-addon">高<%=SAAO.User.Current.Senior == 2 ? "二" : "一" %></div>
                            <input placeholder="班级" class="form-control" type="text" id="classnum" value="<%=SAAO.User.Current.Class %>" maxlength="2">
                            <div class="input-group-addon">班</div>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-sm-offset-2 col-sm-7">
                        <button type="submit" class="btn btn-default">保存更改</button>
                    </div>
                </div>
            </form>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageModal" runat="server">

</asp:Content>

<asp:Content ContentPlaceHolderID="PageScript" runat="server">
    <script src="plugin/aes.js"></script>
    <script src="plugin/sha256.js"></script>
</asp:Content>