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