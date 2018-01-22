<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Mail.aspx.cs" Inherits="MailPage" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ContentPlaceHolderID="PageMenu" runat="server">
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
        <div id="mailframe"></div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageModal" runat="server">
    <div class="modal fade" id="helpmodal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">帮助</h4>
                </div>
                <div class="modal-body">
                    <strong>现已尝试使用新版的邮件系统；新版系统不包含发件功能。</strong>
                    <strong>如需取回旧版邮件系统的邮件，请联系网络组。</strong>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageScript" runat="server">
    <script src="plugin/base64.min.js"></script>
</asp:Content>