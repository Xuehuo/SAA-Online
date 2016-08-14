<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="File.aspx.cs" Inherits="FilePage" %>
<%@ MasterType TypeName="MasterPage" %>

<asp:Content ContentPlaceHolderID="PageMenu" runat="server">
    <li><a href="#" onclick="fileList()"><span class="glyphicon glyphicon-repeat"></span><span class="hidden-xs">刷新</span></a></li>
    <li><a href="#" data-toggle="modal" data-target="#uploadmodal"><span class="glyphicon glyphicon-cloud-upload"></span><span class="hidden-xs">上传</span></a></li>
    <li><a href="#" data-toggle="modal" data-target="#helpmodal"><span class="glyphicon glyphicon-question-sign"></span><span class="hidden-xs">帮助</span></a></li>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <div id="container">
        <h2>现在没有文件</h2>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageModal" runat="server">
    <div class="modal fade" id="filemodal" tabindex="-1" role="dialog" aria-hidden="true">
        <form class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button><h4 class="modal-title">属性</h4>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal">
                        <dl class="dl-horizontal">
                            <dt>文件名</dt>
                            <dd>
                                <input class="form-control" id="filename" type="text" maxlength="50">
                            </dd>
                            <dt>拓展名</dt>
                            <dd></dd>
                            <dt>上传时间</dt>
                            <dd></dd>
                            <dt>大小</dt>
                            <dd></dd>
                            <dt>上传者</dt>
                            <dd></dd>
                            <dt>组</dt>
                            <dd></dd>
                            <dt>下载次数</dt>
                            <dd></dd>
                            <dt>描述</dt>
                            <dd>
                                <textarea id="fileinfo" maxlength="600" placeholder="在此输入对该文件的必要描述（600字以内）"></textarea>
                            </dd>
                            <dt>标签</dt>
                            <dd>
                                <div class="tagging" id="filetagbox">
                                    <input class="type-zone" contenteditable="true">
                                </div>
                            </dd>
                            <dt>权限级别</dt>
                            <dd>
                                <div class="btn-group btn-group-sm" role="group">
                                    <button type="button" data-per="0" onclick="filePermission(this)" class="btn btn-default">全部</button>
                                    <button type="button" data-per="1" onclick="filePermission(this)" class="btn btn-default">仅本组</button>
                                    <button type="button" data-per="2" onclick="filePermission(this)" class="btn btn-default">仅高二</button>
                                    <button type="button" data-per="3" onclick="filePermission(this)" class="btn btn-default">仅高层</button>
                                </div>
                                <p class="text-muted">
                                    权限包括查看、修改、删除。<br>
                                    上传者拥有文件所有权。
                                </p>
                            </dd>
                        </dl>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger pull-left" onclick="fileDelete()">删除</button>
                    <button type="button" class="btn btn-success" onclick="fileDownload()">下载</button>
                    <button type="button" class="btn btn-primary" onclick="fileSave()">保存</button>
                </div>
            </div>
        </form>
    </div>
    <div class="modal fade" id="uploadmodal" tabindex="-1" role="dialog" aria-hidden="true">
        <form class="modal-dialog" id="Uploadform">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button><h4 class="modal-title">上传文件</h4>
                </div>
                <div class="modal-body">
                    <div class="form-horizontal">
                        <div id="uploadbtndiv">
                            <button type="button" class="btn btn-primary" id="btnaddfiles">选择文件</button>
                            <button type="button" class="btn btn-default" id="btnstartupload" onclick="fileDropZone.processQueue()">开始上传</button>
                            <button type="button" class="btn btn-default" id="btnclearqueue" onclick="fileDropZone.removeAllFiles(true);$('#btnstartupload').fadeOut();$('#btnclearqueue').fadeOut();">清空队列</button>
                        </div>
                        <div class="table table-striped" id="uploadqueue">
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
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
                    <p>多文件功能已取消</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageScript" runat="server">
    <script src="plugin/dropzone.min.js"></script>
    <script src="plugin/base64.min.js"></script>
    <script src="plugin/tagging.min.js"></script>
</asp:Content>