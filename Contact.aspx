<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Contact.aspx.cs" Inherits="ContactPage" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ContentPlaceHolderID="PageMenu" runat="server">
    <li><a href="#" data-toggle="modal" data-target="#filtermodal"><span class="glyphicon glyphicon-filter"></span><span class="hidden-xs">筛选</span></a></li>
    <li><a href="#" data-toggle="modal" data-target="#helpmodal"><span class="glyphicon glyphicon-question-sign"></span><span class="hidden-xs">帮助</span></a></li>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <div id="container">
        <h2>没有找到联系人</h2>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageModal" runat="server">
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
                                <button type="button" class="btn btn-default" data-filter="s1" onclick="contactFilter(0, this)">高一</button>
                                <button type="button" class="btn btn-default" data-filter="s2" onclick="contactFilter(0, this)">高二</button>
                            </div>
                        </div>
                        <div class="form-group">
                            <label>组别</label>
                            <div class="btn-group" role="group">
                                <% for (int i = 0; i < SAAO.Organization.Current.Structure.Select("[group] IS NOT NULL").Length; i++)
                                   { %>
                                    <button type="button" class="btn btn-default" data-filter="g<%= i %>" onclick="contactFilter(1, this)"><%= SAAO.Organization.Current.GetGroupName(i) %></button>
                                <% } %>
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
</asp:Content>

<asp:Content ContentPlaceHolderID="PageScript" runat="server">
    <script src="plugin/jquery.mixitup.min.js"></script>
</asp:Content>