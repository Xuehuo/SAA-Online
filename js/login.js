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
                    $.post("../mail/default.aspx?mode=submit", { advanced_login: "0", email: $("#username").val() + "@xuehuo.org", login: $("#username").val(), password: $("#password").val(), sign_me: "0" });
                    window.location.href = "dashboard";
                }
                else if (result.flag == 2) {
                    msg("凭证错误", "请更正您的用户名或密码后重试", "error");
                    $("form")[0].reset();
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
