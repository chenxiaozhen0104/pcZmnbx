$(".navInner a").each(function (i, item) {
    //console.log(window.location.pathname);
    //console.log(item.pathname)
    var path_array = location.pathname;
    var regExp = new RegExp(item.pathname);
    if (regExp.test(path_array)) {
        $(item).addClass("home-titleActive");
    }
});

$.post("/User/GetCompanyInfo", {}, function (res) {
    if (sessionStorage._page == "1") {
        $(".commonCompany").html(res.useCompany.Name);
        $(".commonName").html(res.userName);
    }
    if (sessionStorage._page == 2) {
        $(".commonCompany").html(res.serviceCompany.Name);
        $(".commonName").html(res.userName);
    }
})

// 返回顶部dom操作；
$.fn.toTop = function (options) {
    var defaults = {
        showHeight: 150,
        speed: 1000
    };
    var options = $.extend(defaults, options);
    $("body").prepend("<div id='totop'><a>返回顶部</a></div>");
    var $toTop = $(this);
    var $top = $("#totop");
    var $ta = $("#totop a");
    $toTop.scroll(function () {
        var scrolltop = $(this).scrollTop();
        if (scrolltop >= options.showHeight) {
            $top.show();
        }
        else {
            $top.hide();
        }
    });
    $ta.hover(function () {
        $(this).addClass("cur");
    }, function () {
        $(this).removeClass("cur");
    });
    $top.click(function () {
        $("html,body").animate({ scrollTop: 0 }, options.speed);
    });
}

$(window).toTop({
    showHeight: 1000,//设置滚动高度时显示
    speed: 500 //返回顶部的速度以毫秒为单位
});



function Tips(ncontent, htmlname) {
    layer.tips(ncontent, "input[name='" + htmlname + "']", {
        time: 0
    });
}

function TipsMore(ncontent, htmlname) {
    layer.tips(ncontent, "input[name='" + htmlname + "']", {
        time: 0,
        tipsMore: true
    });
}

function TipsTop(ncontent, htmlname) {
    layer.tips(ncontent, "input[name='" + htmlname + "']", {
        time: 0,
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
    layer.msg(ncontent, { icon: 1, shade: [0.3, '#000'], time: 1000 });
}


function AError(ncontent) {
    layer.alert(ncontent, { icon: 2 });
}

function AErrorMsg(ncontent) {
    layer.msg(ncontent, { icon: 2, shade: [0.3, '#000'], time: 2000 });
}

function ASuccess(ncontent) {
    layer.alert(ncontent, { icon: 1 });
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

function LoadInfo(message){
    var title = arguments[0] != undefined ? arguments[0] : message;
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

function DialogModel()
{
    var con = arguments[0];
    
    var width = arguments[1];
    var height = arguments[2];

    layer.open({
        type: 1,
        title: false,
        closeBtn: 1,
        shadeClose: false,
        shade: [0.3, '#393D49'],
        area: [width, height],
        content: con
    });
}

function OpenUrl() {
    var url = arguments[0];
    var title = arguments[1];
    var width = (arguments[2] != undefined && arguments[2] != 0) ? arguments[2] + "px" : $(window).width() - 40 + "px";
    var height = (arguments[3] != undefined && arguments[3] != 0) ? arguments[3] + "px" : "85%";

    layer.open({
        type: 2,
        title: title,
        skin: 'demo-class',
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