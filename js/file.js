var fileCurrent;

function fileInfo(obj) {
    var fileid = $(obj).data("id");
    fileCurrent = fileid;
    $.ajax({
        url: "file.info.id=" + fileid,
        type: "get",
        cache: false,
        success: function (result) {
            if (result.flag != 0) {
                msg("错误", "加载文件属性失败，请刷新重试", "error");
            }
            else {
                $("#filemodal dl").children("dd").eq(0).children().val(result.data.name);
                $("#filemodal dl").children("dd").eq(1).html(result.data.extension);
                $("#filemodal dl").children("dd").eq(2).html(result.data.uploadTime);
                $("#filemodal dl").children("dd").eq(3).html(fileGetAutoSize(result.data.size, 2));
                $("#filemodal dl").children("dd").eq(4).html(result.data.uploader);
                $("#filemodal dl").children("dd").eq(5).html(result.data.group);
                $("#filemodal dl").children("dd").eq(6).html(result.data.downloadCount);
                var info = $("#filemodal dl").children("dd").eq(7).children();
                info.val(result.data.info);
                info.text(info.val());
                $("#filetagbox").tagging({
                    "case-sensitive": false,
                    "edit-on-delete": false,
                    "forbidden-chars-text": "以下字符是非法的：",
                    "forbidden-chars-callback": function (text) {
                        msg("错误的标签", text, "error")
                    },
                    "no-backspace": true,
                    "no-duplicate-text": "重复的标签：",
                    "no-duplicate-callback": function (text) {
                        msg("错误的标签", text, "error")
                    }
                });
                $("#filetagbox").tagging("removeAll");
                if (result.data.tag != "")
                    for (var i = 0; i < result.data.tag.split(",").length; i++)
                        $("#filetagbox").tagging("add", result.data.tag.split(",")[i]);
                $("#filemodal .btn-group").children().removeClass("btn-primary").addClass("btn-default")
                $("#filemodal .btn-group").children().eq(result.data.permission).removeClass("btn-default").addClass("btn-primary");


                $("#file #filemodal").modal("show");
            }
        },
        error: function () {
            msg("获取文件信息失败", "网络中断或服务器错误", "error");
        }
    })
}

function fileGetTypeClass(extension) {
    var typeClass = ["|txt|doc|docx|ppt|pptx|xls|xlsx|pdf|wps|et|dps|",
        "|bmp|jpg|jpeg|png|tiff|gif|pcx|tga|exif|fpx|svg|psd|cdr|pcd|dxf|ufo|eps|ai|raw|cgm|wmf|emf|pict|",
        "|wmv|asf|asx|rm|rmvb|mpg|mpeg|mpe|3gp|mov|mp4|m4v|avi|dat|mkv|flv|vob|",
        "|rar|zip|7z|cab|arj|lzh|tar|gz|ace|uue|bz2|jar|"];

    var filetype = extension;
    var rt = "othertype";
    if (typeClass[0].indexOf("|" + extension + "|") != -1)
        rt = "doctype";
    else if (typeClass[1].indexOf("|" + extension + "|") != -1)
        rt = "imagetype";
    else if (typeClass[2].indexOf("|" + extension + "|") != -1)
        rt = "videotype";
    else if (typeClass[3].indexOf("|" + extension + "|") != -1)
        rt = "ziptype";
    return rt;
}

function fileGetAutoSize(size, roundCount) {
    var KBCount = 1024;
    var MBCount = KBCount * 1024;
    var GBCount = MBCount * 1024;
    var TBCount = GBCount * 1024;
    if (KBCount > size)
        return Math.round(size, roundCount) + "B";
    else if (MBCount > size)
        return Math.round(size / KBCount, roundCount) + "KB";
    else if (GBCount > size)
        return Math.round(size / MBCount, roundCount) + "MB";
    else if (TBCount > size)
        return Math.round(size / GBCount, roundCount) + "GB";
    else
        return Math.round(size / TBCount, roundCount) + "TB";
}

function fileDelete() {
    $.ajax({
        url: "file.delete.id=" + fileCurrent,
        type: "get",
        async: false,
        success: function (result) {
            if (result.flag == 0) {
                msg("成功", "文件删除成功", "success");
                $("#file #filemodal").modal("hide");
                fileList();
            }
            else {
                msg("错误", "文件删除失败", "error");
                $("#file #filemodal").modal("hide");
                fileList();
            }
        },
        error: function () {
            $("#file #filemodal").modal("hide");
            msg("删除文件失败", "网络中断或服务器错误", "error");
        }
    });
}

function fileDownload() {
    window.open("file.download.id=" + fileCurrent);
    var obj = $("#filemodal dl").children("dd").eq(6);
    obj.html(parseInt(obj.html()) + 1);
}

function filePermission(obj) {
    $(obj).parent().children().removeClass("btn-primary").addClass("btn-default");
    $(obj).addClass("btn-primary");
}

function fileSave() {
    if ($("#file #filemodal #filename").val().trim() != "") {
        if ($("#file #filemodal textarea").val().length < 600 && $("#file #filemodal textarea").val().trim() != "" && $("#file #filemodal textarea").val() != "在此输入对该文件的必要描述（600字以内）") {
            var tagvalid = true;
            var tags = $("#file #filetagbox").tagging("getTags");
            var tagstring = "";
            for (var i = 0; i < tags.length; i++)
                if (tags[i].length > 20)
                    tagvalid = false;
                else
                    if (i != tags.length - 1)
                        tagstring += tags[i] + ",";
                    else
                        tagstring += tags[i];
            if (tagvalid) {

                $.ajax({
                    url: "file.update.id=" + fileCurrent,
                    type: "post",
                    data: { name: $("#file #filemodal #filename").val().trim(), info: $("#file #filemodal textarea").val(), permission: parseInt($("#filemodal .btn-group").children(".btn-primary").data("per")), tag: tagstring },
                    async: false,
                    success: function (result) {
                        if (result.flag != 0) {
                            msg("错误", "更新文件信息失败，请重试", "error");
                        }
                        else {
                            msg("成功", "更新文件信息成功", "success");
                            $("#file #filemodal").modal("hide");
                        }
                        fileList();
                    },
                    error: function () {
                        msg("更新文件信息失败", "网络中断或服务器错误", "error");
                    }
                });
            }
            else {
                msg("错误", "标签的长度超过20字", "error");
            }
        }
        else {
            msg("错误", "文件描述为空或描述过长", "error");
        }
    }
    else {
        msg("错误", "文件名为空", "error");
    }
}

function fileList() {
    $("#container").fadeOut("fast", function () {
        $("#container").empty();
        $.ajax({
            url: "file.list",
            type: "get",
            cache: false,
            success: function (result) {
                if (result.flag == 0) {
                    if (result.data.length > 0) {
                        for (var i = 0; i < result.data.length; i++) {
                            $("#container").append("<table onclick=\"fileInfo(this)\" data-id=\"" + result.data[i].guid + "\"><tr class=\"filetitle\"><td>" + result.data[i].name + "</td></tr><tr class=\"filevertype\"><td><table><tr><td class=\"downloadcount\">" + result.data[i].downloadCount + "次下载" + (!result.data[i].info ? "  <strong>[需要描述]</strong>" : "") + "</td><td class=\"filetype " + fileGetTypeClass(result.data[i].extension) + "\">" + result.data[i].extension + "</td></tr></table></td></tr><tr class=\"fileowntime\"><td>" + result.data[i].uploaderName + " 于 " + result.data[i].datetime + "</td></tr></table>");
                        }
                        $("#container").fadeIn("fast");
                    }
                    else {
                        $("#container").html("<h2>现在没有文件</h2>");
                        $("#container").fadeIn("fast");
                        $("#container h2").fadeIn("fast");
                    }
                }
                else {
                    msg("错误", "加载文件列表失败，请刷新重试", "error");
                }
            },
            error: function () {
                msg("初始化时出错", "网络中断或服务器错误", "error");
            }
        });
    });
}


fileList();

$("#filemodal").on("hidden.bs.modal", function () {
    $('#filetagbox').tagging('destroy');
    //fileList();
});
$("#uploadmodal").on("hidden.bs.modal", function () {
    fileList();
});


var fileDropZone = new Dropzone("#uploadmodal", {
    url: "file.upload",
    previewTemplate: "<div class=\"file-row\"><div class=\"upfilename\"><p data-dz-name></p></div><div class=\"upfiledivtwo\"><p data-dz-size></p><p class=\"uploadstatep\"></p></div><div class=\"upfileaction\" data-dz-remove><span class=\"glyphicon glyphicon-remove\"></span><span class=\"glyphicon glyphicon-ok\"></span></div></div>",
    parallelUploads: 1,
    maxFilesize: 2048,
    uploadMultiple: true,
    clickable: "#btnaddfiles",
    init: function () {
        this.on("addedfile", function () {
            $("#btnstartupload").fadeIn();
            $("#btnclearqueue").fadeIn();
        }),
        this.on("uploadprogress", function (file, progress) {
            $(file.previewTemplate).children(".upfilename").css("background-size", progress + "%");
        }),
        this.on("sending", function (file) {
            $("#file #btnstartupload").fadeOut();
            this.options.autoProcessQueue = true;
            $(file.previewTemplate).children('.upfiledivtwo').animate({ backgroundColor: "rgb(0, 190, 254)" }, 120);
            $(file.previewTemplate).find("[data-dz-size]").css("display", "none");
            $(file.previewTemplate).find(".uploadstatep").fadeOut("fast");
            $(file.previewTemplate).find(".uploadstatep").text("上传中");
            $(file.previewTemplate).find(".uploadstatep").fadeIn("fast");
        }),
        this.on("queuecomplete", function () {
            this.options.autoProcessQueue = false;
            msg("文件上传", "上传队列已经处理完毕，请及时为新上传的文件添加标签及说明", "info");
        }),
        this.on("error", function (file, errorMessage) {
            $(file.previewTemplate).children('.upfiledivtwo').animate({ backgroundColor: "rgb(239, 127, 127)" }, 120);
            $(file.previewTemplate).children('.upfileaction').animate({ backgroundColor: "rgb(239, 127, 127)" }, 120);
            $(file.previewTemplate).find(".uploadstatep").fadeOut("fast");
            $(file.previewTemplate).find(".uploadstatep").text("出错");
            $(file.previewTemplate).find(".uploadstatep").fadeIn("fast");
            if (file.size > 2048 * 1024 * 1024) {
                msg("上传失败", "文件大于2G。请分段压缩上传或联系网络组", "error");
            }
        }),
        this.on("success", function (file) {
            $(file.previewTemplate).children('.upfiledivtwo').animate({ backgroundColor: "rgb(60,178,112)" }, 120);
            $(file.previewTemplate).children('.upfileaction').animate({ backgroundColor: "rgb(60,178,112)" }, 120);
            $(file.previewTemplate).find(".uploadstatep").fadeOut("fast");
            $(file.previewTemplate).find(".uploadstatep").text("成功");
            $(file.previewTemplate).find(".uploadstatep").fadeIn("fast");
            $(file.previewTemplate).find(".glyphicon-remove").fadeOut("fast", function () {
                $(file.previewTemplate).find(".glyphicon-ok").fadeIn("fast", function () {
                    $(file.previewTemplate).find(".glyphicon-ok").css("display", "inline-block");
                });
            });
        }),
        this.on("reset", function () {
            this.options.autoProcessQueue = false;
            setTimeout(function () {
                $("#btnstartupload").fadeOut();
                $("#btnclearqueue").fadeOut();
            }, 150);
        })
    },
    autoProcessQueue: false,
    previewsContainer: "#uploadqueue",
    dictFallbackMessage: "你的浏览器不支持拖放上传",
    dictFallbackText: "使用以下控件进行上传",
    dictFileTooBig: "文件过大！",
    dictResponseError: "服务器错误，请重试",
    dictCancelUpload: "中止",
    dictCancelUploadConfirmation: "确定要取消上传吗？",
    dictRemoveFile: "移除",
});