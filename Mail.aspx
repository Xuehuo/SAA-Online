<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Mail.aspx.cs" Inherits="MailPage" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ContentPlaceHolderID="PageMenu" runat="server">
    <%if (Session["wechat"] == null) {%>
    <li><a href="#" onclick="mailCompose()"><span class="glyphicon glyphicon-pencil"></span><span class="hidden-xs">撰写邮件</span></a></li>
    <%} %>
    <li class="hidden-xs"><a href="#" data-toggle="modal" data-target="#helpmodal"><span class="glyphicon glyphicon-question-sign"></span><span class="hidden-xs">帮助</span></a></li>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <div id="maillist">
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
            <li>
                <a href="#" onclick="mailList()">返回</a>
            </li>
            <li>
                <a href="#" onclick="mailReply()">回复</a>
            </li>
            <li>
                <a href="#" onclick="mailForward()">转发</a>
            </li>
            <li>
                <a href="#" onclick="mailDelete()">删除</a>
            </li>

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
            <hr/>
        </div>
        <iframe id="mailframe" src="#" frameborder="0" onload="$(this).fadeIn('fast');"></iframe>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageModal" runat="server">
    <div class="modal fade" id="mailmodal" tabindex="-1" role="dialog" aria-hidden="true">
        <form class="modal-dialog modal-lg dropzone">
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
                <div class="modal-footer" id="maildropzone">
                    <button id="btnaddfiles" type="button" class="btn btn-primary pull-left" style="margin-right: 10px; display: none">添加附件</button>
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
                    <strong>邮件系统维护中，暂时无法使用。</strong>
                    <ol>
                        <li>此处邮件仅包含收件箱、无附件的邮件撰写；</li>
                        <li>如需要完整的邮件系统请在顶栏点击完整邮箱；</li>
                        <li>使用微信登陆会导致无法发件和无法跳转到完整邮箱，如需使用，请注销后使用用户名密码登陆；</li>
                        <li>在此处转发（回复）带有附件的邮件并不会保留其附件，如需转发附件，请使用完整邮箱。</li>
                    </ol>
                    <p></p>
                    <hr>

                    <p>在 Windows 10 使用邮件系统的指引</p>
                    <ol>
                        <li>打开邮件应用，点击 <kbd>账户</kbd>，点击 <kbd>添加账户</kbd>，选择 <kbd>高级设置</kbd>，选择 <kbd>Internet 电子邮件</kbd></li>
                        <li>电子邮件地址填写 <kbd><%=SAAO.User.Current.Username %>@xuehuo.org</kbd></li>
                        <li>用户名填写 <kbd><%=SAAO.User.Current.Username %></kbd></li>
                        <li>密码填写SAAO的登陆密码（邮件系统与SAAO密码相同）</li>
                        <li>账户名可随意填写，如<kbd>学活</kbd></li>
                        <li>“使用此名称发送你的邮件”填写您希望在发件时显示的名称，如<kbd>学生活动中心 <%=SAAO.User.Current.Realname %></kbd></li>
                        <li>传入电子邮件服务器填写<kbd>xuehuo.shenzhong.net</kbd></li>
                        <li>账户类型<kbd>POP3</kbd>和<kbd>IMAP4</kbd>都可，建议选择后者</li>
                        <li>传出(SMTP)电子邮件服务器填写<kbd>xuehuo.shenzhong.net</kbd></li>
                        <li>勾选 传出服务器要求身份验证</li>
                        <li>勾选 发送邮件时使用同一用户名和密码</li>
                        <li>建议勾选 需要用于传入电子邮件的 SSL</li>
                        <li>建议勾选 需要用于传出电子邮件的 SSL</li>
                        <li>点击登录即可完成设置（若在过程中遇到问题，请联系网络组）</li>
                    </ol>
                    <p>在 Android 使用邮件系统的指引</p>
                    <ol>
                        <li>打开<kbd>电子邮件</kbd>应用，在电子邮件地址输入 <kbd><%=SAAO.User.Current.Username %>@xuehuo.org</kbd></li>
                        <li><kbd>手动设置</kbd>，<kbd>个人(IMAP)</kbd>，密码填写SAAO的登陆密码（邮件系统与SAAO密码相同），下一步</li>
                        <li>将“服务器”改为<kbd>xuehuo.shenzhong.net</kbd></li>
                        <li>安全类型选择<kbd>SSL/TLS</kbd>（若出错，则改为<kbd>SSL/TLS（接受所有证书）</kbd>），端口<kbd>993</kbd>，下一步</li>
                        <li>SMTP服务器改为<kbd>xuehuo.shenzhong.net</kbd></li>
                        <li>安全类型选择<kbd>SSL/TLS</kbd>（若出错，则改为<kbd>SSL/TLS（接受所有证书）</kbd>），端口<kbd>465</kbd>，下一步</li>
                        <li>选择合适的同步频率（建议选择每小时以上避免过高耗电），下一步</li>
                        <li>“您的姓名”填写您希望在发件时显示的名称，如<kbd>学生活动中心 <%=SAAO.User.Current.Realname %></kbd>，下一步</li>
                        <li>设置完成（若在过程中遇到问题，请联系网络组）</li>
                    </ol>
                    
                    <table class="table table-condensed">
                        <caption>邮件服务器</caption>
                        <thead>
                        <tr>
                            <th>#</th>
                            <th>SMTP</th>
                            <th>IMAP</th>
                            <th>POP3</th>
                        </tr>
                        </thead>
                        <tbody>
                        <tr>
                            <td>服务器地址</td>
                            <td colspan="3">
                                xuehuo.shenzhong.net
                            </td>
                        </tr>
                        <tr>
                            <td>端口（无加密）</td>
                            <td>25</td>
                            <td>143</td>
                            <td>110</td>
                        </tr>
                        <tr>
                            <td>端口（SSL）</td>
                            <td>465、994</td>
                            <td>993</td>
                            <td>995</td>
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
</asp:Content>

<asp:Content ContentPlaceHolderID="PageScript" runat="server">
    <script src="//cdn.bootcss.com/tinymce/4.2.0/tinymce.min.js"></script>
    <script src="//cdn.bootcss.com/tinymce/4.2.0/langs/zh_CN.js"></script>
    <script src="plugin/base64.min.js"></script>
    <script src="plugin/dropzone.min.js"></script>
</asp:Content>