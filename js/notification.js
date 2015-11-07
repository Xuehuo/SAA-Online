function notificationList(folder) {
    $("#content .breadcrumb").html("<li id=\"folder\">全部通知</li>");
    if (arguments[0]) {
        $("#folder").before("<li onclick=\"notificationList()\"><a href=\"#\">返回</a></li>");
        switch (folder) {
            case 1:
                $("#folder").html("全员通知");
                break;
            case 2:
                $("#folder").html("组内通知");
                break;
            case 3:
                $("#folder").html("监督报告");
                break;
        }
    }
    else {
        folder = 0;
    }
        
    $("#container").fadeOut("fast", function () {
        $("#container").empty();
        $.ajax({
            url: "notification.list",
            type: "get",
            cache: false,
            success: function (result) {
                if (result.flag == 0) {
                    if (result.data.length > 0) {
                        for (var i = 0; i < result.data.length; i++) {
                            if (result.data[i].type == 0 && (folder == 1 || folder == 0))
                                $("#container").append("<div class=\"notificationcard\" data-id=\"" + result.data[i].ID + "\">" + (result.data[i].important > 0 ? "<img class=\"noticeflag\" src=\"image/flagged.png\" />" : "") + "<h3 class=\"noticetitle\">" + result.data[i].title + "</h3><div class=\"noticename\"><i class=\"glyphicon glyphicon-user\"></i><a> " + result.data[i].user + "</a> 发布至 全员通知</div><div class=\"noticecontent\">" + result.data[i].content + "</div><div class=\"noticedatetime\">发布时间: <a> " + result.data[i].notifyTime + "</a></div></div>");

                            else if (result.data[i].type == 1 && (folder == 2 || folder == 0))
                                $("#container").append("<div class=\"notificationcard\" data-id=\"" + result.data[i].ID + "\">" + (result.data[i].important > 0 ? "<img class=\"noticeflag\" src=\"image/flagged.png\" />" : "") + "<h3 class=\"noticetitle\">" + result.data[i].title + "</h3><div class=\"noticename\"><i class=\"glyphicon glyphicon-user\"></i><a> " + result.data[i].user + "</a> 发布至 组内通知</div><div class=\"noticecontent\">" + result.data[i].content + "</div><div class=\"noticedatetime\">发布时间: <a> " + result.data[i].notifyTime + "</a></div></div>");

                            else if (result.data[i].type == 2 && (folder == 3 || folder == 0))
                                $("#container").append("<div class=\"notificationcard reportcard\" onclick=\"notificationReport(this)\" data-id=\"" + result.data[i].ID + "\" data-report=\"" + result.data[i].reportFile + "\"><h3 class=\"noticetitle\">监督报告</h3><div class=\"noticename\"><i class=\"glyphicon glyphicon-user\"></i><a> " + result.data[i].user + "</a> 发布至 监督报告</div><div class=\"noticecontent\">" + result.data[i].content + "</div><div class=\"noticedatetime\">发布时间: <a> " + result.data[i].notifyTime + "</a></div></div>");

                        }
                    }
                    else {
                        $("#container h2").fadeIn("fast");
                    }
                }
                else if (result.flag == 3) {
                    msg("错误", "服务器错误，加载通知列表失败", "error");
                }
                if ($("#container").html() == "") {
                    $("#container").html("<h2>现在没有通知</h2>");
                }
                $("#container").fadeIn("fast");
            },
            error: function () {
                msg("初始化时出错", "网络中断或服务器错误", "error");
            }
        });
    });
}

function notificationCompose() {
    $("#notificationmodal>form")[0].reset();
    $("#notificationmodal").modal("show");
    $("#visibility").html("可见级别 ");
}

function notificationReport(obj) {
    $("#reportmodal a.btn-primary").attr("href", "notification.report.id=" + $(obj).data("report") + "&download=1");
    $("#reportframe").attr("src", "notification.report.id=" + $(obj).data("report"));
    $("#reportmodal").modal("show");
}

$("#notificationmodal>form").submit(function (e) {
    e.preventDefault();
    $("#notificationmodal>form button[type=\"submit\"]").addClass("disabled");
    $("#notificationmodal>form button[type=\"submit\"]").attr("disabled", "disabled");
    if ($("#report").css("display") == "block") {
        if ($("#reportabstract").val() == "") {
            msg("填写错误", "请将表单填写完整后重试", "error");
            $("#notificationmodal>form button[type=\"submit\"]").removeClass("disabled");
            $("#notificationmodal>form button[type=\"submit\"]").removeAttr("disabled");
            return;
        }
        var formData = new FormData($("#notificationmodal>form")[0]);
        formData.append("title", "监督报告 " + new Date().getMonth() +"月" + new Date().getDay() + "日");
        formData.append("content", $("#report textarea").val());
        formData.append("important", 0);
        formData.append("type", 2);
        $.ajax({
            url: "notification.create",
            type: "post",
            data: formData,
            contentType: false,
            processData: false,
            success: function (result) {
                if (result.flag == 0) {
                    $("#notificationmodal").modal("hide");
                    msg("发布成功", "相应邮件将很快投递", "success");
                }
                else
                    msg("发布失败", "服务器错误", "error");
                $("#notificationmodal>form button[type=\"submit\"]").removeClass("disabled");
                $("#notificationmodal>form button[type=\"submit\"]").removeAttr("disabled");
                notificationList();
            },
            error: function () {
                msg("发送失败", "服务器错误或网络中断", "error");
                $("#notificationmodal>form button[type=\"submit\"]").removeClass("disabled");
                $("#notificationmodal>form button[type=\"submit\"]").removeAttr("disabled");
            }
        });
    }
    else {
        if ($("#noticetitle").val() == "" || $("#noticecontent").val() == "" || $("#visibility").html() == "可见级别 ") {
            msg("填写错误", "请将表单填写完整后重试", "error");
            $("#notificationmodal>form button[type=\"submit\"]").removeClass("disabled");
            $("#notificationmodal>form button[type=\"submit\"]").removeAttr("disabled");
            return;
        }
        $.ajax({
            url: "notification.create",
            type: "post",
            data: { title: $("#noticetitle").val(), content: $("#noticecontent").val(), important: ($("#notificationmodal>form input[type=\"checkbox\"]").prop("checked") ? 1 : 0), type: ($("#visibility").html() == "组内通知 " ? 1 : 0) },
            dataType: "json",
            success: function (result) {
                if (result.flag == 0) {
                    $("#notificationmodal").modal("hide");
                    msg("发布成功", "相应邮件将很快投递", "success");
                }
                else
                    msg("发布失败", "服务器错误", "error");
                $("#notificationmodal>form button[type=\"submit\"]").removeClass("disabled");
                $("#notificationmodal>form button[type=\"submit\"]").removeAttr("disabled");
                notificationList();
            },
            error: function () {
                msg("发送失败", "服务器错误或网络中断", "error");
                $("#notificationmodal>form button[type=\"submit\"]").removeClass("disabled");
                $("#notificationmodal>form button[type=\"submit\"]").removeAttr("disabled");
            }
        });

    }
});
$(function () {
    $("#notificationmodal .nav-tabs li").eq(0).addClass("active");
    $($("#notificationmodal .nav-tabs li").eq(0).children("a").attr("href")).addClass("active");
    notificationList();
});