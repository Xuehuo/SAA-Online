<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Calendar.aspx.cs" Inherits="CalendarPage" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ContentPlaceHolderID="PageMenu" runat="server">
    
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

</asp:Content>

<asp:Content ContentPlaceHolderID="PageScript" runat="server">
    <script src="plugin/dhtmlxScheduler/dhtmlxscheduler.js"></script>
    <link href="plugin/dhtmlxScheduler/dhtmlxscheduler_flat.css" rel="stylesheet"/>
</asp:Content>