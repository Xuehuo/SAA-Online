<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Setting.aspx.cs" Inherits="SettingPage" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ContentPlaceHolderID="PageMenu" runat="server">
    
</asp:Content>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <div class="panel panel-default">
        <div class="panel-heading">微信绑定</div>
        <div class="panel-body">
            <div class="col-md-6">
                <% if (SAAO.User.Current.Wechat == "") { %>
                <h4>微信绑定</h4>
                <ol>
                    <li>扫描二维码关注微信企业号（学活工作网络）</li>
                    <li>进入SAAO应用</li>
                    <li>输入用户名密码登陆</li>
                    <li>点击<a href="setting">这里</a>刷新状态</li>
                </ol>
                <% } else { %>
                <h4>微信登陆</h4>
                <p>在企业号中即可直接进入SAAO（不需要用户名密码登录）。</p>
                <p>当前已绑定微信号 <%=SAAO.User.Current.Wechat %> <button class="btn btn-danger btn-xs" onclick="settingUnbind()">解除绑定</button></p>
                <div class="checkbox">
                    <label>
                        <input type="checkbox" id="FilePush" <% if(SAAO.User.Current.FilePush==1){%>checked="checked"<%} %> />
                        新文件推送
                    </label>
                </div>
                <% } %>
            </div>
            <div class="col-md-6">
                <img src="image/qy.jpg" class="img-responsive">
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
    
</asp:Content>