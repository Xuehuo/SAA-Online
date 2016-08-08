function contactApplyFilter() {
    var filterString = "";
    if (contactFilterStr[0])
        filterString += contactFilterStr[0];
    if (contactFilterStr[1])
        filterString += contactFilterStr[1];
    if (contactFilterStr[2])
        filterString += contactFilterStr[2];
    if (filterString == "")
        filterString = "*";
    $("#container").mixItUp("multiMix", { filter: filterString });
    $("#filtermodal").modal("hide");
}

var contactFilterStr = new Array("", "", "");

function contactFilter(i,obj) {
    if ($(obj).hasClass("btn-primary")) {
        contactFilterStr[i] = "";
        $(obj).removeClass("btn-primary")
    }
    else {
        $(obj).siblings().removeClass("btn-primary");
        $(obj).addClass("btn-primary");
        contactFilterStr[i] = "." + $(obj).data("filter");
    }
}

function contactShowInfo(obj) {
    $("#contactmodal .modal-title").html($(obj).html());
    $("#contactmodal dl").children("dd").eq(0).html($(obj).data("group"));
    $("#contactmodal dl").children("dd").eq(1).html($(obj).data("job"));
    $("#contactmodal dl").children("dd").eq(2).html($(obj).data("phone"));
    $("#contactmodal dl").children("dd").eq(3).html($(obj).data("mail"));
    $("#contactmodal dl").children("dd").eq(4).html($(obj).data("class"));
    $("#contactmodal").modal("show");
}

$.ajax({
    url: "contact.list",
    type: "get",
    success: function (result) {
        if (result.flag == 0) {
            var initial = new Array(result.data.length);
            for (var i = 0; i < result.data.length; i++) {
                var classstr = "s" + result.data[i].senior + " g" + result.data[i].group + " c" + result.data[i].initial;
                var datastr = "data-job=\"" + result.data[i].jobName + "\" data-group=\"" + result.data[i].groupName + "\" data-phone=\"" + result.data[i].phone + "\" data-mail=\"" + result.data[i].mail + "\" data-class=\"" + result.data[i].grade + "(" + result.data[i].class + ")班\"";
                $("#container").append("<div onclick=\"contactShowInfo(this)\" class=\"mix " + classstr + "\" " + datastr + ">" + result.data[i].realname + "</div>");
                initial[i] = result.data[i].initial;
            }
            $("#container").mixItUp({
                callbacks: {
                    onMixStart: function () {
                        if ($("#contact #container h2").css("display") != "none") {
                            $("#contact #container h2").fadeOut("fast");
                        }
                    },
                    onMixEnd: function () {
                        if ($("#container").mixItUp("getState").fail) {
                            $("#contact #container h2").fadeIn("fast");
                        }
                    }
                },
                sort: "name:asc"
            });
        }
        else {
            msg("初始化时出错", "无法加载联系人数据，请刷新重试", "error");
        }
    },
    error: function () {
        msg("初始化时出错", "网络中断或服务器错误", "error");
    }
});
