$(function () {
    $("div#content").css("height", $(window).height() - 50);
});
$(window).resize(function () {
    $("div#content").css("height", $(window).height() - 50);
});

scheduler.init("container", new Date(), "week");

$.ajax({
    url: "calendar.list",
    type: "get",
    cache: false,
    success: function (result) {
        if (result.flag == 0) {
            scheduler.parse(result.data, "json");
        }
        else if (result.flag == 1)
            msg("提示", "找不到近期的任务", "info");
        else if (result.flag == 3)
            msg("初始化时出错", "服务器错误", "error");
        else
            msg("初始化时出错", "服务器错误", "error");
    },
    error: function () {
        msg("初始化时出错", "网络中断或服务器错误", "error");
    }
});


scheduler.attachEvent("onEventAdded", function (id, ev) {
    update_event(ev);
});
scheduler.attachEvent("onEventChanged", function (id, ev) {
    update_event(ev);
});

function update_event(event_obj) {
    $.ajax({
        url: "calendar.update",
        type: "post",
        data: {
            event_id: event_obj.id,
            event_text: event_obj.text,
            start_date: event_obj.start_date,
            end_date: event_obj.end_date
        },
        success: function (result) {
            if (result.flag == 0) {
                if (result.data.idORerr == "event-does-not-exist") {
                    msg("更新日历事件失败", "该事件已不存在，请刷新页面", "error");
                }
                else {
                    if (event_obj.id != result.data.idORerr)
                        scheduler.changeEventId(event_obj.id, result.data.idORerr);     //ChangeID
                    scheduler.getEvent(result.data.idORerr).color = result.data.color;      //ChangeColor
                    scheduler.setCurrentView();     //Refresh
                    msg("更新日历事件成功", "数据已更新", "success");
                }
            }
            else if (result.flag == 2)
                msg("更新日历事件失败", "事件内容不能为空", "error");
            else if (result.flag == 3)
                msg("更新日历事件失败", "服务器错误", "error");
            else
                msg("更新日历事件失败", "服务器错误", "error");
        },
        error: function () {
            msg("更新日历事件失败", "网络中断或服务器错误", "error");
        }
    });
}

scheduler.attachEvent("onEventDeleted", function (id) {
    $.ajax({
        url: "calendar.delete.id=" + id,
        type: "get",
        async: false,
        success: function (result) {
            if (result.flag == 0) {
                msg("成功", "事件删除成功", "success");
            }
            else if (result.flag == 3)
                msg("删除事件时出错", "服务器错误", "error");
            else
                msg("删除事件时出错", "服务器错误", "error");
        },
        error: function () {
            msg("删除事件时出错", "网络中断或服务器错误", "error");
        }
    });
});


//Check if the new text is empty
scheduler.attachEvent("onEventSave", function (id, ev, is_new) {
    if (!ev.text) {
        msg("填写错误", " 事件内容不能为空", "error");
        return false;
    }
    else {
        return true;
    }
})