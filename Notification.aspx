<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Notification.aspx.cs" Inherits="NotificationPage" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ContentPlaceHolderID="PageMenu" runat="server">
    <li>
        <a href="#" onclick="notificationList()">
            <span class="glyphicon glyphicon-repeat"></span><span class="hidden-xs"> 刷新</span></a>
    </li>
    <% if (SAAO.User.Current.IsExecutive || SAAO.User.Current.IsSupervisor || SAAO.User.Current.IsGroupHeadman)
       { %>
        <li>
            <a href="#" onclick="notificationCompose()">
                <span class="glyphicon glyphicon-edit"></span><span class="hidden-xs"> 发布通知</span></a>
        </li>
    <% } %>
    <li class="dropdown">
        <a href="#" class="dropdown-toggle" data-toggle="dropdown">
            <span class="glyphicon glyphicon-filter"></span><span class="hidden-xs"> 筛选</span> <b class="caret"></b>
        </a>
        <ul class="dropdown-menu">
            <li>
                <a href="#" onclick="notificationList(1)"><span class="glyphicon glyphicon-tags"></span> 全员通知</a>
            </li>
            <li>
                <a href="#" onclick="notificationList(2)"><span class="glyphicon glyphicon-tag"></span> 组内通知</a>
            </li>
            <li class="divider"></li>
            <li>
                <a href="#" onclick="notificationList(3)"><span class="glyphicon glyphicon-file"></span> 监督报告</a>
            </li>
        </ul>
    </li>
    <li class="hidden-xs">
        <a href="#" data-toggle="modal" data-target="#helpmodal">
            <span class="glyphicon glyphicon-question-sign"></span><span class="hidden-xs">帮助</span></a>
    </li>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <ol class="breadcrumb">
        <li id="folder">全部通知</li>
    </ol>
    <div id="container"></div>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageModal" runat="server">

</asp:Content>

<asp:Content ContentPlaceHolderID="PageScript" runat="server">
    <div class="modal fade" id="notificationmodal" tabindex="-1" role="dialog" aria-hidden="true">
        <form class="modal-dialog" enctype="multipart/form-data" method="post">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <ul class="nav nav-tabs">
                        <% if (SAAO.User.Current.IsExecutive || SAAO.User.Current.IsGroupHeadman)
                           { %>
                            <li>
                                <a href="#notice" data-toggle="tab">发布通知</a>
                            </li>
                        <% }
                          if (SAAO.User.Current.IsSupervisor)
                          { %>
                            <li>
                                <a href="#report" data-toggle="tab">监督报告</a>
                            </li>
                        <% } %>
                    </ul>
                </div>
                <div class="modal-body">
                    <div class="tab-content">
                        <div class="tab-pane" id="notice">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="input-group">
                                        <div class="input-group-btn">
                                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                                                <span id="visibility">可见级别 </span><span class="caret"></span></button>
                                            <ul class="dropdown-menu">
                                                <li>
                                                    <a href="#" onclick="$('#visibility').html('组内通知 ');">组内通知 </a>
                                                </li>
                                                <li>
                                                    <a href="#" onclick="$('#visibility').html('全员通知 ');">全员通知 </a>
                                                </li>
                                            </ul>
                                        </div>
                                        <input type="text" id="noticetitle" class="form-control" placeholder="通知标题">
                                        <% if (SAAO.User.Current.IsExecutive)
                                           { %>
                                            <span class="input-group-addon">
                                            <span class="glyphicon glyphicon-flag"></span>设为重要
                                            <input type="checkbox">
                                        </span>
                                        <% } %>
                                    </div>
                                </div>
                            </div>
                            <textarea id="noticecontent" class="form-control" rows="6" placeholder="通知内容"></textarea>
                        </div>
                        <div class="tab-pane" id="report">
                            <div class="form-group">
                                <label for="reportfile">上传监督报告文档</label>
                                <p class="help-block">为避免文档排版差异,请上传pdf格式</p>
                                <input type="file" name="reportfile" id="reportfile">
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
                            <td>
                                <i class="glyphicon glyphicon-ok"></i>
                            </td>
                            <td>
                                <i class="glyphicon glyphicon-ok"></i>
                            </td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>高二全体高层</td>
                            <td>
                                <i class="glyphicon glyphicon-ok"></i>
                            </td>
                            <td>
                                <i class="glyphicon glyphicon-ok"></i>
                            </td>
                            <td></td>
                            <td>
                                <i class="glyphicon glyphicon-ok"></i>
                            </td>
                        </tr>
                        <tr>
                            <td>审计组组员</td>
                            <td></td>
                            <td></td>
                            <td>
                                <i class="glyphicon glyphicon-ok"></i>
                            </td>
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
</asp:Content>