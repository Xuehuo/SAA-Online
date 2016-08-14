<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Mail.aspx.cs" Inherits="MailPage" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ContentPlaceHolderID="PageMenu" runat="server">
    <li><a href="#" onclick="mailCompose()"><span class="glyphicon glyphicon-pencil"></span><span class="hidden-xs">撰写邮件</span></a></li>
    <li><a href="../mail/webmail.aspx" target="_blank"><span class="glyphicon glyphicon-share-alt"></span><span class="hidden-xs">完整邮箱</span></a></li>
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
                    <button id="btnaddfiles" type="button" class="btn btn-primary pull-left" style="margin-right: 10px;">添加附件</button>
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
</asp:Content>

<asp:Content ContentPlaceHolderID="PageScript" runat="server">
    <script src="//cdn.bootcss.com/tinymce/4.2.0/tinymce.min.js"></script>
    <script src="//cdn.bootcss.com/tinymce/4.2.0/langs/zh_CN.js"></script>
    <script src="plugin/base64.min.js"></script>
    <script src="plugin/dropzone.min.js"></script>
</asp:Content>