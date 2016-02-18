$.ajax({
    url: "calendar.list",
    type: "get",
    cache: false,
    success: function (result) {
        if (result.flag == 0)
            $("#container").fullCalendar({
                editable: false,
                height: $(window).height() - 70,
                eventBackgroundColor: "#E64C3C",
                eventBorderColor: "#EEE",
                allDayDefault: true,
                handleWindowResize: false,
                events: result.data
            });
        else if(result.flag == 1)
            msg("提示", "找不到近期的任务", "info");
        else if(result.flag == 3)
            msg("初始化时出错", "服务器错误", "error");
        else
            msg("初始化时出错", "服务器错误", "error");
    },
    error: function () {
        msg("初始化时出错", "网络中断或服务器错误", "error");
    }
});