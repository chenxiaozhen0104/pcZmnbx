$(document).ready(function () {

    var Height = $(window).height() - 92;
    $(".show-panel").height(Height); 


    $(".chooseList>.col-xs-4").click(function () {
        var $t = $(this)
        var panelSelector = $(this).data('panel')
        $('.ddl').not(this).removeClass('checked');
        if ($t.hasClass('checked')) {
            $t.removeClass('checked')
            $(panelSelector).removeClass('show')
        } else {
            $t.addClass('checked')
            $('.show-panel').not(panelSelector).removeClass('show');
            $(panelSelector).addClass('show')
        }
    });

    //$(".area>span").click(function() {
    //   $(".area>span").removeClass('spanClick')
    //   $(this).addClass('spanClick');
    //   $(".ddl").eq(0).find("span").first().text($(this).html());
    //   $('.show-panel').removeClass('show');
    //});
    //$(".category>span").click(function() {
    //   $(".category>span").removeClass('spanClick')
    //   $(this).addClass('spanClick');
    //   $(".ddl").eq(1).find("span").first().text($(this).html());
    //   $('.show-panel').removeClass('show');
    //});
    // $(".brand>span").click(function() {
    //   $(".brand>span").removeClass('spanClick')
    //   $(this).addClass('spanClick');
    //   $(".ddl").eq(2).find("span").first().text($(this).html());
    //   $('.show-panel').removeClass('show');
    //});

    $(".icon-sou").click(function () {
        $(".clickSerch").addClass('moveSerch');
    });

    $(".btn,.search-back").click(function () {
        $(".clickSerch").removeClass('moveSerch');
    });


    var searchTag = {
        area: null,
        category: null,
        brand: null
    }
    var index = 1;
    var isPageLoading=false;
    function search(pageIndex,isPage) {
        $(".loading").addClass("show");
        $.get('/api/device/search', {
            searchKey: $('#txt-key').val(),
            areaId: searchTag.area,
            categoryId: searchTag.category,
            brandId: searchTag.brand,
            pageIndex: pageIndex || 1,
            pageSize:20
        }, function (res) {


            var len = res.data.length;
            var data = res.data;
            var html = "";
            for (var i = 0 ; i < len; i++) {
                html += "<a id='" + data[i].id + "' class='list-group-item row device-item'>" +
                  "<div class='col-xs-10'>" +
                      "<div class='list-group-item-heading'>" + data[i].name+"<span class='qrcode'>"+ (data[i].qrcode||"")+"</span></div>" +
                      "<div class='row list-group-item-text'>" +
                         "<p class='col-xs-8'>类目：" + data[i].category + "</p>" +
                           "<p class='col-xs-4'>品牌：" + (data[i].brand || '暂无') + "</p>" +
                      "</div>" +
                     "<div class='row list-group-item-text'>" +
                         "<p class='col-xs-8'>型号：" + (data[i].model || '暂无') + "</p>" +
                          "<p class='col-xs-4'>区域：" + data[i].area + "</p>" +  
                      "</div>" +
                      "<div class='row list-group-item-text'>" +
                         "<p class='col-xs-12'>安装地址：" + (data[i].address || '暂无')+ "</p>" +
                      "</div>" +
                  "</div>" +
                  "<div class='col-xs-2 text-center'>" +
                      (data[i].qrcode ? "" : "<div class='iconfont icon-erweima' style='font-size:30px;'></div>") +
                  "</div>" +
              "</a>"
            }
            if (!isPage) {
                $(".list-group").html(html);
                index = 1;
                isPageLoading = false;
            } else {
                isPageLoading = data.length == 0;
                $('.nodata').show();
                $(".list-group").append(html);
            }
            
            $(".loading").removeClass("show");
           
        });


    }

    search();

    $(window).scroll(function () {
       

        if ($(document).scrollTop() >= $(document).height() - $(window).height() - 20&&!isPageLoading) {
            index++;
            search(index,true);
        }
    })

    function brand() {
        $.post("/api/device/initData", function (res) {
            var len = res.area.length;
            var len1 = res.category.length;
            var len2 = res.brand.length;
            var areaText = "";
            var categoryText = "";
            var brandText = "";

            for (var i = 0 ; i < len; i++) {
                areaText += "<span class='search-tag' data-type='area' data-id='" + res.area[i].id + "'>" + res.area[i].name + "</span>"
            }
            $(".area").append(areaText);
            for (var i = 0 ; i < len1; i++) {
                categoryText += "<span class='search-tag' data-type='category' data-id='" + res.category[i].id + "'>" + res.category[i].name + "</span>"
            }
            $(".category").append(categoryText);
            for (var i = 0 ; i < len2; i++) {
                brandText += "<span class='search-tag' data-type='brand' data-id='" + res.brand[i].id + "'>" + res.brand[i].name + "</span>"
            }
            $(".brand").append(brandText);


        })
        $('.show-panel').on('click', 'span', function () {
            console.log(123)
            $('.show-panel').removeClass('show');
            var $t = $(this);
            var type = $t.data('type');
            $('.ddl-' + type).children().eq(0).text($t.text())
            searchTag[type] = $t.data('id');
            $('.ddl').removeClass('checked')
            search();
        });
    }
    brand()



    // 搜索的oninput事件;
    $("#txt-key").on("input", function () {
        search();
    })
   

    var detailId;
    $(".list-group").on('click', '.device-item', function () {
        //$('.bounceds').addClass('show');
       layer.open({
            type: 1,
            title: false,
            closeBtn: 1,
            shadeClose: false,
            shade: [0.3, '#393D49'],
            //area: [100, height],
            content: $('.bounceds')
        });
        detailId = this.id;
        $.get("/api/device/get?deviceId=" + this.id, function (res) {
            console.log(res)
            if (res) {
                $('#txtQrCode').val(res.QRCode)
                $('#txtLocation').val(res.Position)
                $(".deviceName").text(res.Name);
                $(".areaName").text(res.Area.Name);
                $(".categoryName").text(res.Category.Name);
                $(".modelName").text(res.Model);
            }
        })
    })
    $('#btn-submit').click(function () {
        var actionUrl = new LG.URL();

        var rqCode = $("#txtQrCode").val();
        if (rqCode == "" || rqCode == undefined) {
            layer.msg('二维码为空！', { icon: 2, time: 1500 });
            return;
        }
        var address = $("#txtLocation").val();


        $.ajax({
            url: "/api/device/setQRCode",
            type: "post",
            data: { "deviceId": detailId, "qrCode": rqCode, "address": address },
            success: function (obj) {
                var timeOut = 2000;
                if (obj.error) {
                    layer.alert(obj.error,{icon:0})
                }
                else {
                  
                    $('#' + detailId).find('.icon-erweima').remove()
                    layer.closeAll();
                    layer.msg("保存成功", { icon: 6, time: 1000 });
                }
            }
        });
    })

    $("#btn-close").click(function () {
        $('.bounceds').removeClass('show');
    });
    $("#btn-repair").click(function () {
        location.href = "deviceDetail.html?deviceId=" + detailId;
    })

});




function openQRCode() {
    var actionUrl = new LG.URL(ActionBaseUrl);


    actionUrl.set("target", "_openqrcode");
    window.location.href = actionUrl.url();
}

function qrCallBack(eUrl) {
    var eStr = eUrl.substr(eUrl.lastIndexOf('/') + 1);

    $("#txtQrCode").val(eStr);
}



