$(function () {
    var clone=$(".nav .images li").first().clone();
    $(".nav .images").append(clone);
    var i=0;
    var size = $(".nav .images li").length;
//            点击向左轮播
    $(".nav .btn_l").click(function () {
        i--;
        if(i==-1){
            $(".nav .images").css({left:-(size-1)*687});
            i=size-2;
        }
        $(".nav .images").stop().animate({left:-i*687}, 500);
        $(".nav .num li").eq(i).addClass("on").siblings().removeClass("on");
    });
//            点击向右轮播
    $(".nav .btn_r").click(function () {
        moveR();
    });
//            向右轮播函数
    function moveR() {
        i++;
        if(i==size){
            $(".nav .images").css({left:0});
            i=1;
        }
        $(".nav .images").stop().animate({left:-i*687}, 500);
        if(i==size-1){
            $(".nav .num li").eq(0).addClass("on").siblings().removeClass("on");
        }else {
            $(".nav .num li").eq(i).addClass("on").siblings().removeClass("on");
        }
    }
//            鼠标滑过圆点
    $(".nav .num li").hover(function () {
        var index=$(this).index();
        i=index;
        $(".nav .images").stop().animate({left:-i*687}, 500);
        $(this).addClass("on").siblings().removeClass("on");
    });
//            定时自动轮播
    var t=setInterval(function () {
        moveR();
    },3000);
//            鼠标滑过图片停止自动轮播
    $(".nav").hover(function(){
        clearInterval(t);},
            function(){
                t=setInterval(function () {
                    moveR();
                },3000)
        });

    $('.homeContOne-left li').mouseenter(function () {
        $(this).children('ul').css('display', 'block');
    });
    $('.homeContOne-left li').mouseleave(function () {
        $(this).children('ul').css('display', 'none');
    });

    $(".navInner a").each(function (i, item) {
        //console.log(window.location.pathname);
        //console.log(item.pathname)
   
        var path_array = location.pathname;
        var regExp = new RegExp(item.pathname);
        console.log(path_array);
        if (regExp.test(path_array )) {
            $(item).addClass("home-titleActive");
        }
    });


   // 返回顶部控制;
  
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

})