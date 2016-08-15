function msg(title, text, type) {
    new PNotify({
        title: title,
        text: text,
        type: type,
        shadow: false
    });
}
$("button").tooltip();
$("form").submit(function (e) {
    e.preventDefault();
    if ($("#username").val() == "" || $("#password").val() == "")
        msg("填写错误", " 请将表单填写完整后再提交", "error");
    else {
        $("form").fadeOut("fast");
        $.ajax({
            url: "user.login",
            type: "post",
            data: { username: $("#username").val(), password: $("#password").val() },
            dataType: "json",
            success: function (result) {
                if (result.flag == 0) {
                    window.location.href = "dashboard";
                }
                else if (result.flag == 2) {
                    if ($("form").data("pin"))
                        requestPIN("输入的PIN不正确，请重试");
                    else {
                        msg("凭证错误", "请更正您的用户名或密码后重试", "error");
                        $("form")[0].reset();
                    }
                }
                else if (result.flag == 3) {
                    msg("系统错误", "请刷新重试或联系技术部网络组", "error");
                }
                $("form").fadeIn("fast");
            },
            error: function () {
                msg("系统错误", "网络中断或服务器错误", "error");
                $("form").fadeIn("fast");
            }
        });
    }
});

function getUrlPara(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

if (getUrlPara("username") && getUrlPara("cipher") && getUrlPara("hash")) {
    $("#username").val(getUrlPara("username"));
    $("form").data("pin", "using");
    requestPIN("请输入您设置的PIN以完成登陆");
}

function requestPIN(str) {
    $("form").css("opacity", 0);
    $("#username").val(getUrlPara("username"));
    var pin = prompt(str);
    if (pin) {
        $("#password").val(Aes.Ctr.decrypt(getUrlPara("cipher"), pin + getUrlPara("hash"), 256));
        $("form").submit();
    }
}
