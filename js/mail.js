function mailFolder() {
    mailList();
    $("#maillist tbody").empty();
    $.ajax({
        url: "mail.list.folder=INBOX",
        type: "get",
        success: function (result) {
            if (result.flag != 3) {
                for (var i = 0; i < result.data.length; i++) {
                    var mailLabel;
                    switch (result.data[i].flag) {
                        case 1:
                        case 8:
                        case 65:
                        default:
                            mailLabel = "<span class=\"glyphicon glyphicon-envelope\" style=\"color: #EEE\"></span> ";
                            //read?
                            break;
                        case 96:
                            mailLabel = "<span class=\"glyphicon glyphicon-envelope\"></span> ";
                    }
                    if (result.data[i].attachcount != 0)
                        mailLabel += "<span class=\"glyphicon glyphicon-paperclip\"></span> ";
                    $("#maillist tbody").append("<tr data-id=\"" + result.data[i].id + "\" onclick=\"mailDisplay(this)\"><td> " + mailLabel + "</td><td>" + result.data[i].from + "</td><td>" + result.data[i].subject + " <small>" + result.data[i].thumb + "</small></td><td>" + result.data[i].time + "</td></tr>");
                }
            }
            else {
                msg("初始化时出错", "服务器错误", "error");
            }

        },
        error: function () {
            msg("初始化时出错", "网络中断或服务器错误", "error");
        }
    })
}

var mailCurrentID;

function mailList() {
    $("#mailpreview").fadeOut("fast", function () { $("#maillist").fadeIn("fast"); });
    mailCurrentID = -1;
}

function mailDisplay(obj) {
    var mailid = $(obj).data("id");
    mailCurrentID = mailid;
    $(obj).children("td").children("span.glyphicon-envelope").css("color", "#EEE");
    $("#maillist").fadeOut("fast", function () {
        $("#mailframe").css("display", "none");
        $.ajax({
            url: "mail.info.id=" + mailid,
            type: "get",
            dataType: "json",
            success: function (result) {
                if (result.flag == 0) {
                    $("#mailsubject").html(result.data.subject);
                    $("#mailfrom").html(result.data.from.Name + " <small>" + result.data.from.Mail + "</small>");
                    $("#mailto").empty();
                    for (var i = 0; i < result.data.to.length; i++)
                        $("#mailto").append(result.data.to[i].Name + " <small>" + result.data.to[i].Mail + "</small> ");
                    $("#mailtime").html(result.data.time);
                    if (result.data.attachcount == 0) {
                        $("#mailattach").css("display", "none");
                        $("#mailattachdt").css("display", "none");
                    }
                    else {
                        $("#mailattach").css("display", "block");
                        $("#mailattachdt").css("display", "block");
                        $("#mailattach").html(result.data.attachcount + " 个 ");
                        for (var i = 0; i < result.data.attachcount; i++)
                            $("#mailattach").append("<a href=\"mail.attachment.id=" + result.data.id + "&index=" + (i + 1) + "\">" + result.data.attachment[i].filename + "</a> ");
                    }
                    $("#mailframe").attr("src", "mail.display.id=" + result.data.id);
                    $("#mailpreview").css("opacity", 0);
                    $("#mailpreview").css("display", "block");
                    $("#mailframe").css("height", window.innerHeight - $("#maildetail hr").offset().top - 5);
                    $("#mailpreview").animate({ "opacity": 1 }, "fast");
                }
                else if (result.flag == 2) {
                    msg("加载出错", "访问被拒绝", "error");
                }
                else if (result.flag == 3) {
                    msg("加载出错", "服务器错误", "error");
                }
            },
            error: function () {
                msg("加载出错", "网络中断或服务器错误", "error");
            }
        });
    });
}

function mailCompose() {
    $("#mailmodal>form")[0].reset();
    $("#mailmodal").modal("show");
}

function mailReply() {
    $("#mailmodal>form")[0].reset();
    $("#subject").val("RE:" + $("#mailsubject").html());
    $("#recipient").val($("#mailfrom small").html());
    $("#mailmodal").modal("show");
}

function mailForward() {
    var mailContent = "<br>---------- Forwarded message ---------<br>";
    //mailContent += $("#maildetail").html();
    mailContent += "From: " + $("#mailfrom").html() + "<br>";
    mailContent += "Date: " + $("#mailtime").html() + "<br>";
    mailContent += "Subject: " + $("#mailsubject").html() + "<br>";
    mailContent += "To: " + $("#mailto").html() + "<br><br>";
    $.get($("#mailframe").attr("src"), function (data, status) {
        mailContent += data;
        $("#mailmodal>form")[0].reset();
        tinyMCE.activeEditor.setContent(mailContent);
        $("#subject").val("FW:" + $("#mailsubject").html());
        $("#mailmodal").modal("show");
    });
}

function mailDelete() {
    $.get("mail.delete.id=" + mailCurrentID, function () {
        $("tr[data-id=" + mailCurrentID + "]").remove();
    });
}


mailFolder();

tinymce.init({
    //language: "zh_CN",
    selector: "#mailcontent",
    theme: "modern",
    plugins: [
        "advlist autolink lists link image charmap print preview hr anchor pagebreak",
        "searchreplace wordcount visualblocks visualchars code fullscreen",
        "insertdatetime media nonbreaking save table contextmenu directionality",
        "emoticons paste textcolor"
    ],
    menubar: false,
    toolbar_items_size: "small",
});

$(window).resize(function () {
    $("#content").css("height", $(window).height() - 50);
    $("#maillist").css("height", $(window).height() - 50);
    $("#mailpreview").css("height", $(window).height() - 50);
    $("#mailframe").css("height", $(window).height() - $("#maildetail hr").offset().top - 5);
});

$("#mailmodal>form").submit(function (e) {
    e.preventDefault();
    $("#mailmodal>form button[type=\"submit\"]").addClass("disabled");
    $("#mailmodal>form button[type=\"submit\"]").attr("disabled", "disabled");
    $.ajax({
        url: "mail.send",
        type: "post",
        data: { to: $("#recipient").val(), subject: $("#subject").val(), content: Base64.encode(tinyMCE.activeEditor.getContent()) },
        dataType: "json",
        success: function (result) {
            if (result.flag == 0) {
                $("#mailmodal").modal("hide");
                msg("发送成功", "邮件将很快投递", "success");
            }
            else
                msg("发送失败", "服务器错误", "error");
            $("#mailmodal>form button[type=\"submit\"]").removeClass("disabled");
            $("#mailmodal>form button[type=\"submit\"]").removeAttr("disabled");
        },
        error: function () {
            msg("发送失败", "服务器错误或网络中断", "error");
            $("#mailmodal>form button[type=\"submit\"]").removeClass("disabled");
            $("#mailmodal>form button[type=\"submit\"]").removeAttr("disabled");
        }
    });
});

var mailDropzone = new Dropzone("#maildropzone",
{
    previewTemplate: "<div class=\"tag pull-left\"><span data-dz-name></span><a class=\"tag-i\" role=\"button\" data-dz-remove>×</a></div>",
    url: "mail.upload",
    clickable: "#btnaddfiles",
    parallelUploads: 1,
    maxFilesize: 2048,
    uploadMultiple: true,
    autoProcessQueue: false
});