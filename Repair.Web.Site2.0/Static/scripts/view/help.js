$(function () {
    $(".publistBottom-lists a").each(function (i, item) {
        //console.log(window.location.pathname);
        //console.log(item.pathname)
        var regExp = new RegExp(item.pathname);
        if (regExp.test(location.pathname)) {
            $(item).parent().addClass('publicLists-active');
            $(item).addClass("color-active");
        }
    });

    $(".icon-chevron-up").click(function () {
        if ($(this).hasClass("iconDown")) {
            $(this).removeClass("iconDown");
            $(this).parent().siblings("ul").hide(500);
        } else {
            $(this).addClass("iconDown");
            $(this).parent().siblings("ul").show(500);
        }
    });

    $(".about-tab a").click(function () {
        $(this).parent("li").addClass("liActive").siblings("li").removeClass("liActive");
    })

})
