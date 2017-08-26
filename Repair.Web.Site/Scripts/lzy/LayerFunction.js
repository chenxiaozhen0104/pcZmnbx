function Tips(ncontent, htmlname) {
    layer.tips(ncontent, "input[name='" + htmlname + "']", {
        time: 2000
    });
}

function TipsMore(ncontent, htmlname) {
    layer.tips(ncontent, "input[name='" + htmlname + "']", {
        time: 2000,
        tipsMore: true
    });
}

function TipsTop(ncontent, htmlname) {
    layer.tips(ncontent, "input[name='" + htmlname + "']", {
        time: 2000,
        tips: 1
    });
}

function ShowError(htmlId, error) {
    Tips(error, htmlId);
}

//警告
function AWarn(ncontent) {
    layer.alert(ncontent, { icon: 0 });
}

function AWarnMsg(ncontent) {
    layer.msg(ncontent, { icon: 0, shade: [0.3, '#000'], time: 2000 });
}

function ARight(ncontent) {
    layer.alert(ncontent, { icon: 1 });
}

function ARightMsg(ncontent) {
    layer.msg(ncontent, { icon: 1, shade: [0.3, '#000'], time: 2000 });
}


function AError(ncontent) {
    layer.alert(ncontent, { icon: 2 });
}

function AErrorMsg(ncontent) {
    layer.msg(ncontent, { icon: 2, shade: [0.3, '#000'], time: 2000 });
}

function ASuccess(ncontent) {
    layer.alert(ncontent, { icon: 6 });
}

function AMessage(ncontent) {
    layer.msg(ncontent, { icon: 2, shade: [0.3, '#000'], time: 2000 });
}

function AlertUrlError(content, url, isParent) {
    layer.msg(content, { icon: 2, shade: [0.3, '#000'], time: 2000 }, function () { isParent ? parent.location.href = url : window.location.href = url; });
}

function AlertUrl(content, url, isParent) {
    layer.msg(content, { time: 2000 }, function () { isParent ? parent.location.href = url : window.location.href = url; });
}

function AlertUrlRight(content, url, isParent) {
    layer.msg(content, { icon: 1, shade: [0.3, '#000'], time: 2000 }, function () { isParent ? parent.location.href = url : window.location.href = url; });
}

function AlertRight(content, isParent) {
    layer.msg(content, { icon: 1, shade: [0.3, '#000'], time: 2000 }, function () { isParent ? parent.location.reload() : window.location.reload(); });
}

function AlertClose(content) {
    layer.msg(content, { time: 2000 }, function () { parent.layer.closeAll(); });
}

//确认框
function AComfig(ncontent, yesFunction) {
    layer.open({
        content: ncontent,
        shade: [0.3, '#393D49'],
        btn: ['确定', '取消'],
        yes: function (index, layero) { //或者使用btn1
            CloseLoad(index);
            yesFunction();
        },
        cancel: function (index) { //或者使用btn2
            CloseLoad(index);
        }
    });
}

//load
function Loading() {
    var title = arguments[0] != undefined ? arguments[0] : '数据提交中，请稍候...';
    var index = layer.msg(title, {
        icon: 16,
        time: 0,
        skin: 'demo-class',
        shadeClose: false,
        shade: 0.5
    });
    return index;
}

function CloseLoad(index) {
    layer.close(index);
}

//添加
function Add() {
    OpenUrl(arguments[0], arguments[1], arguments[2], arguments[3]);
}

function OpenUrl() {
    var url = arguments[0];
    var title = arguments[1];
    var width = (arguments[2] != undefined && arguments[2] != 0) ? arguments[2] + "px" : $(window).width() - 40 + "px";
    var height = (arguments[3] != undefined && arguments[3] != 0) ? arguments[3] + "px" : "85%";

    layer.open({
        type: 2,
        title: title,
        skin: 'layui-layer-rim',
        shadeClose: false,
        shade: 0.5,
        area: [width, height],
        content: url
    });
}

function CloseFrame() {
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.close(index);
}

function GetUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return decodeURI(r[2]); return "";
}

function Redirect(href) {
    window.location.href = href;
}

//带输入的确定框
function APrompt(ncontent, yesFunction) {
    layer.prompt(
        {
            title: ncontent,
            formType: 2
        },
        function (str) {
            if (str) {
                CloseLoad(index);
                yesFunction(str);
            };
        });

}
