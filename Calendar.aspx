<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Calendar.aspx.cs" Inherits="CalendarPage" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ContentPlaceHolderID="PageMenu" runat="server">
    <li><a href="#" data-toggle="modal" data-target="#helpmodal"><span class="glyphicon glyphicon-question-sign"></span><span class="hidden-xs">帮助</span></a></li>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <div id="container" class="dhx_cal_container">
        <div class="dhx_cal_navline">
            <div class="dhx_cal_prev_button">&nbsp;</div>
            <div class="dhx_cal_next_button">&nbsp;</div>
            <div class="dhx_cal_today_button"></div>
            <div class="dhx_cal_date"></div>
            <div class="dhx_cal_tab" name="day_tab"></div>
            <div class="dhx_cal_tab" name="week_tab"></div>
            <div class="dhx_cal_tab" name="month_tab"></div>
        </div>
        <div class="dhx_cal_header"></div>
        <div class="dhx_cal_data"></div>
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
                    <p>在日历上拖动以新建事件</p>
                    <p>在日历上拖动已有的事件以修改其时间</p>
                    <p>双击事件或使用事件旁的按钮可修改事件内容</p>
                    <p>使用删除按钮以删除事件</p>
                    <p><strong>在事件内容的首二字指定组别（也可指定全体），如：“设计：工作服终稿”“全体：XXXX”；不同组别的事件颜色不同，未指明组别的事件为灰色。</strong></p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageScript" runat="server">
    <script src="plugin/dhtmlxScheduler/dhtmlxscheduler.js"></script>
    <link href="plugin/dhtmlxScheduler/dhtmlxscheduler_flat.css" rel="stylesheet" />
</asp:Content>