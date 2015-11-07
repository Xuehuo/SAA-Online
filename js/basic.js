$(function () {
    $("div#menu").css("height", $(window).height());
});
$(window).resize(function () {
    $("div#menu").css("height", $(window).height());
});

function msg(title, text, type) {
    new PNotify({
        title: title,
        text: text,
        type: type, //info success error
        shadow: false,
        mouse_reset: false
    });
}

function userLogout() {
    $.get("../mail/default.aspx?mode=logout");
    $.ajax({
        url: "user.logout",
        type: "get",
        success: function (data) {
            window.location.href = "login";
        },
        error: function (data) {
            msg("系统错误", "您可以通过关闭浏览器退出，未保存的更改将会丢失", "error");
            setTimeout('window.location.href="login"', 3000);
        }
    });
}

$("#passwordmodal>form").submit(function (e) {       //更改密码
    e.preventDefault();
    var Checkstr = function (input) {
        var valid = /[\"\"\,\<\>\+\-\*\/\%\^\=\\\!\&\|\(\)\[\]\{\}\:\;\~\`\#\$]+/;
        return (valid.test(input));
    };
    var isIncSpace = function (ui) {
        var valid = /\s/;
        return (valid.test(ui));
    };
    var checklength = function (strlength) {
        if (strlength >= 6 && strlength <= 18)
            return true;
        else
            return false;
    };
    $("#changepassword button[type=\"submit\"]").addClass("disabled");
    $("#changepassword button[type=\"submit\"]").attr("disabled", "true");
    if ($("#psnow").val() == "" || $("#psnew").val() == "" || $("#repsnew").val() == "") {
        msg("填写错误", " 请将表单正确填写后再提交", "error");
    }
    else if (isIncSpace($("#psnow").val()) == true || isIncSpace($("#psnew").val()) == true || isIncSpace($("#repsnew").val()) == true) {
        msg("填写错误", " 不能包含空格", "error");
    }
    else if (checklength($("#psnew").val().length) == false || checklength($("#repsnew").val().length) == false) {
        msg("填写错误", " 密码长度必须大于等于6且小于等于18", "error");
    }
    else if (Checkstr($("#psnow").val()) == true || Checkstr($("#psnew").val()) == true || Checkstr($("#repsnew").val()) == true) {
        msg("填写错误", " 密码中含有非法字符", "error");
    }
    else if ($("#psnew").val() != $("#repsnew").val()) {
        msg("填写错误", " 两次输入的新密码不一致", "error");
    }
    else {
        $.ajax({
            url: "user.password",
            type: "post",
            data: { password: $("#psnow").val(), newpassword: $("#psnew").val() },
            dataType: "json",
            success: function (data) {
                if (data == 1) {
                    msg("修改成功", "你的登录密码已更改", "success");
                    $("#passwordmodal").modal("hide");
                }
                else if (data == 2) {
                    msg("修改失败", "当前提交的密码与现有密码不匹配", "error");
                }
                else {
                    msg("修改失败", "请刷新重试或联系网络组", "error");
                }
            },
            error: function (data) {
                msg("修改失败", "网络中断或服务器错误", "error");
            }
        });
    }
    $("#changepassword button[type=\"submit\"]").removeClass("disabled");
    $("#changepassword button[type=\"submit\"]").removeAttr("disabled");
});