function mailFolder() {
    mailList();
    $("#maillist tbody").empty();
    $.ajax({
        url: "mail.list",
        type: "get",
        success: function (result) {
            if (result.flag != 3) {
                for (var i = 0; i < result.data.length; i++) {
                    var mailLabel = "<span class=\"glyphicon glyphicon-envelope\" style=\"color: #EEE\"></span> ";
                    if (result.data[i].attachcount != 0)
                        mailLabel += "<span class=\"glyphicon glyphicon-paperclip\"></span> ";
                    $("#maillist tbody").append(
                        "<tr data-id=\"" + result.data[i].GUID + "\" onclick=\"mailDisplay(this)\">" +
                            "<td> " + mailLabel + "</td>" +
                            "<td>" + result.data[i].sender + "</td>" +
                            "<td>" + result.data[i].subject + "</td>" +
                            "<td>" + result.data[i].datetime + "</td>" +
                        "</tr>");
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
    $("#maillist").fadeOut("fast", function () {
        $("#mailframe").css("display", "none");
        $.ajax({
            url: "mail.info.id=" + mailid,
            type: "get",
            dataType: "json",
            success: function (result) {
                if (result.flag == 0) {
                    $("#mailsubject").html(result.data.Subject.split(",")[0]);
                    $("#mailfrom").html(result.data.Sender);
                    $("#mailto").html(result.data.Recipient);
                    $("#mailtime").html(result.data.Datetime);
                    if (result.data.Attachment.length == 0) {
                        $("#mailattach").css("display", "none");
                        $("#mailattachdt").css("display", "none");
                    }
                    else {
                        $("#mailattach").css("display", "block");
                        $("#mailattachdt").css("display", "block");
                        $("#mailattach").html(result.data.Attachment.length + " 个 ");
                        for (var i = 0; i < result.data.Attachment.length; i++)
                            $("#mailattach").append("<a href=\"mail.attachment.id=" + result.data.Attachment[i].id + "&name=" + result.data.Attachment[i].name + "\">" + Base64.decode(result.data.Attachment[i].name) + "</a> ");
                    }
                    $("#mailframe").html(result.data.BodyPlain);
                    $("#mailpreview").css("opacity", 0);
                    $("#mailpreview").css("display", "block");
                    $("#mailframe").css("height", window.innerHeight - $("#maildetail hr").offset().top - 5);
                    $("#mailframe").css("display", "block");
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


mailFolder();

$(window).resize(function () {
    $("#content").css("height", $(window).height() - 50);
    $("#maillist").css("height", $(window).height() - 50);
    $("#mailpreview").css("height", $(window).height() - 50);
    $("#mailframe").css("height", $(window).height() - $("#maildetail hr").offset().top - 5);
});
