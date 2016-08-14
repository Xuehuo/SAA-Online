<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="DashboardPage" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ContentPlaceHolderID="PageMenu" runat="server">
    
</asp:Content>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <div class="panel panel-default">
        <div class="panel-heading" id="grouphead">
        </div>
        <table class="table">
            <thead>
            <tr>
                <th>今天开始</th>
                <th>正在进行</th>
                <th>今天结束</th>
            </tr>
            </thead>
            <tbody>
            <tr>
                <td>
                    <ol id="taskbegin"></ol>
                </td>
                <td>
                    <ol id="taskdoing"></ol>
                </td>
                <td>
                    <ol id="tasktodo"></ol>
                </td>
            </tr>
            </tbody>
        </table>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageModal" runat="server">

</asp:Content>

<asp:Content ContentPlaceHolderID="PageScript" runat="server">

</asp:Content>