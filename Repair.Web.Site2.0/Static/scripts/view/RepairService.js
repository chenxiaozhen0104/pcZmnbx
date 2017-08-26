$(function () {
    //var i = $(".more").index();
    $(".more").click(function () {
        if ($(this).hasClass("clickMore")) {
            $(this).eq($(this).index()).html("收起" + "<i class='iconfont icon-chevron-up'></i>");
            $(this).eq($(this).index()).removeClass("clickMore");
            $(this).eq($(this).index()).parent().siblings().children(".hidden").toggle();
        } else {
            $(this).eq($(this).index()).html("更多" + "<i class='iconfont icon-chevron-down-copy'></i>");
            $(this).eq($(this).index()).addClass("clickMore");
            $(this).eq($(this).index()).parent().siblings().children(".hidden").toggle();
        }
    })
    $(".middle-text").click(function () {
        $(this).css("color", "#fba02a").siblings().css("color", "#333");
    })
    
    //设备切换
    var hash = location.hash;
    if (hash) {
        tab(hash.match(/\d+/)[0]);
    } else {
        $(".menu-details").eq(0).css("display", "block");
        $(".nav-title").eq(0).css("border-bottom", "2px solid #fba02a");
        $(".nav-title").eq(0).children(".nav-text").css("color", "#fba02a");
        $(".details-menu").eq(0).css("display", "block");
    }
    $('.nav-title').click(function () {
        tab($(this).index());
    });
    function tab(index) {
        $('.menu-details').siblings('.menu-details').hide().end().eq(index).show();
        $('.details-menu').siblings('.details-menu').hide().end().eq(index).show();
        $(".nav-title").eq(index).children(".nav-text").css("color", "#fba02a").parents().siblings().children(".nav-text").css("color", "#333");
        $(".nav-title").eq(index).css("border-bottom", "2px solid #fba02a").siblings().css("border-bottom", "");
    }
})
//var cook = new Vue({
//    data: {
//        cook: false
//    }
//})