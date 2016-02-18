$.ajax({
    url: "dashboard.list",
    type: "get",
    dataType: "json",
    success: function (result) {
        if (result.flag == 0) {
            $("#grouphead").text(result.data.group + " 任务");
            for (var i = 0; i < result.data.begin.length; i++)
                $("#taskbegin").append("<li>" + result.data.begin[i].title + "</li>");

            for (var i = 0; i < result.data.doing.length; i++)
                $("#taskdoing").append("<li>" + result.data.doing[i].title + "</li>");

            for (var i = 0; i < result.data.todo.length; i++)
                $("#tasktodo").append("<li>" + result.data.todo[i].title + "</li>");
            if (result.data.begin.length == 0)
                $("#taskbegin").parent().html("无");
            if (result.data.doing.length == 0)
                $("#taskdoing").parent().html("无");
            if (result.data.todo.length == 0)
                $("#tasktodo").parent().html("无");

        }
        else {
            msg("初始化时出错", "无法加载任务数据，请刷新重试", "error");
        }
    },
    error: function () {
        msg("初始化时出错", "网络中断或服务器错误", "error");
    }
});