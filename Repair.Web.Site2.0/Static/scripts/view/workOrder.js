
    $(".publistBottom-lists a").each(function (i, item) {
        //console.log(window.location.pathname);
        //console.log(item.pathname)
        var regExp = new RegExp(item.pathname);
        if (regExp.test(location.pathname)) {
            $(item).parent().addClass('publicLists-active');
            $(item).addClass("color-active");
        }
    });



