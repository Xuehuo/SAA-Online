$("form#password").submit(function (e) {
    e.preventDefault();
    var checkstr = function (input) {
        var valid = /[\"\"\,\<\>\+\-\*\/\%\^\=\\\!\&\|\(\)\[\]\{\}\:\;\~\`\#\$]+/;
        return (valid.test(input));
    };
    var isIncSpace = function (ui) {
        var valid = /\s/;
        return (valid.test(ui));
    };
    if ($("#psnow").val().length === 0 || $("#psnew").val().length === 0 || $("#repsnew").val().length === 0) {
        msg("填写错误", " 请将表单正确填写后再提交", "error");
        return;
    }
    if (isIncSpace($("#psnow").val()) || isIncSpace($("#psnew").val()) || isIncSpace($("#repsnew").val())) {
        msg("填写错误", " 不能包含空格", "error");
        return;
    }
    if ($("#psnew").val() !== $("#repsnew").val()) {
        msg("填写错误", " 两次输入的新密码不一致", "error");
        return;
    }
    if ($("#psnew").val().length < 6 || $("#psnew").val().length > 18) {
        msg("填写错误", " 密码长度必须大于等于6且小于等于18", "error");
        return;
    }
    if (checkstr($("#psnow").val()) || checkstr($("#psnew").val()) || checkstr($("#repsnew").val())) {
        msg("填写错误", " 密码中含有非法字符", "error");
        return;
    }
    $("form#password button[type=\"submit\"]").addClass("disabled");
    $("form#password button[type=\"submit\"]").attr("disabled", "true");
    $.ajax({
        url: "user.password",
        type: "post",
        data: { password: $("#psnow").val(), newpassword: $("#psnew").val() },
        dataType: "json",
        success: function (result) {
            if (result.flag === 0) {
                msg("修改成功", "你的登录密码已更改", "success");
            }
            else if (result.flag === 2) {
                msg("修改失败", "当前提交的密码与现有密码不匹配", "error");
            }
            else {
                msg("修改失败", "请刷新重试或联系网络组", "error");
            }
            $("form#password")[0].reset();
            $("form#password button[type=\"submit\"]").removeClass("disabled");
            $("form#password button[type=\"submit\"]").removeAttr("disabled");
        },
        error: function () {
            msg("修改失败", "网络中断或服务器错误", "error");
            $("form#password button[type=\"submit\"]").removeClass("disabled");
            $("form#password button[type=\"submit\"]").removeAttr("disabled");
        }
    });
});

$("form#info").submit(function(e) {
    e.preventDefault();
    if (!/\d{11}/.test($("#phonenum").val())) {
        msg("填写错误", "手机号码填写错误", "error");
        return;
    }
    if (!/\d{2}|\d{1}/.test($("#classnum").val())) {
        msg("填写错误", "班级填写错误", "error");
        return;
    }
    if (!/\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/.test($("#email").val())) {
        msg("填写错误", "邮箱地址填写错误", "error");
        return;
    }
    $("form#info button[type=\"submit\"]").addClass("disabled");
    $("form#info button[type=\"submit\"]").attr("disabled", "true");
    $.ajax({
        url: "user.info",
        type: "post",
        data: { phone: $("#phonenum").val(), mail: $("#email").val(), classnum: $("#classnum").val() },
        dataType: "json",
        success: function (result) {
            if (result.flag === 0) {
                msg("修改成功", "你的个人信息已更改", "success");
            }
            else {
                msg("修改失败", "请刷新重试或联系网络组", "error");
            }
            $("form#info button[type=\"submit\"]").removeClass("disabled");
            $("form#info button[type=\"submit\"]").removeAttr("disabled");
        },
        error: function () {
            msg("修改失败", "网络中断或服务器错误", "error");
            $("form#info button[type=\"submit\"]").removeClass("disabled");
            $("form#info button[type=\"submit\"]").removeAttr("disabled");
        }
    });
});

function settingUnbind() {
    $.get("user.unbind");
    $.get("user.refresh");
    window.location.href = "setting";
}

function settingRefresh() {
    $.get("user.reload");
    window.location.href = "setting";
}