
/*
* 
* -----------------------
* This plugin depends on iCheck plugin for checkbox and radio inputs
*
* @type plugin
* @usage $("#todo-widget").todolist( options );
*/
(function ($) {

    'use strict';

    $(document).
        on("click", ".spinner .btn", function (e) {
            if (e) e.preventDefault();
            var isAdd = $(this).find("i").hasClass("fa-caret-up");
            var input = $(this).closest("div").parent().find("input");
            input.val(parseInt(input.val()) + (isAdd ? 1 : -1));
            input.trigger("change");
        });

}(jQuery));

/*
* 系统开关
* -----------------------
* This plugin depends on iCheck plugin for checkbox and radio inputs
*
* @type plugin
* @usage $("#todo-widget").todolist( options );
*/
(function ($) {

    'use strict';

    $(document).
        on("click", ".sys-switch a", function (e) {
            if (e) e.preventDefault();
            var pThis = $(this);
            var icon = pThis.find("i");
            var span = pThis.find("span");

            $.get(pThis.attr("href"), function (rt) {
                if (rt) {
                    icon.removeClass("fa-toggle-off").addClass("fa-toggle-on");
                    span.removeClass("text-danger").addClass("text-success");
                    span.text("已开启");
                } else {
                    icon.removeClass("fa-toggle-on").addClass("fa-toggle-off");
                    span.removeClass("text-success").addClass("text-danger");
                    span.text("立即开启");
                }
            }, "json");
        });
}(jQuery));

(function ($) {
    'use strict';
    $(document)
        .on("mouseover", ".rate-start i", function () {
            var pThis = $(this);
            for (var cur = pThis.next() ; cur != null && cur.length != 0; cur = cur.next()) {
                cur.removeClass("fa-star");
                cur.addClass("fa-star-o");
            }
            for (var cur = pThis; cur != null && cur.length != 0; cur = cur.prev()) {
                cur.removeClass("fa-star-o");
                cur.addClass("fa-star");
            }
        })
        .on("mouseout", ".rate-start", function (e) {
            var pDiv = $(this);
            var pThis = pDiv.children()[pDiv.data("value")];
            $(pThis).mouseover();
        })
        .on("click", ".rate-start i", function (e) {
            var pThis = $(this);
            var pDiv = $(pThis.parent());

            //调用ajax
            $.ajax({
                type: "GET",
                url: pDiv.data("url") + '?level=' + pThis.index(),
                cache: false,
                dataType: "json",
                success: function (val) {
                    //设置值
                    pDiv.data("value", pThis.index());
                    pThis.mouseover();
                }
            });
        });

    //启动日期选择控件
    $(".daterange-frame").each(function (i, e) {
        var frm = $(e);

        var dayRange = 365;
        if (frm.data("range"))
            dayRange = frm.data("range");

        var dateRange1 = new pickerDateRange('daterange', {
            isTodayValid: true,
            startDate: frm.find("#TimeStart").val(),
            endDate: frm.find("#TimeEnd").val(),
            startDateId: "TimeStart",
            endDateId: "TimeEnd",
            needCompare: true,
            defaultText: ' 至 ',
            autoSubmit: true,
            theme: 'ta',
            calendars: 2,
            //时间范围控制需要以下两个参数同时设置
            dayRangeMax: dayRange,
            //monthRangeMax  该属性值为0时，可以选择限制日期选择范围是多少天
            monthRangeMax: 0
        });
    });
}(jQuery));

/*
* 页面帮助配置
*
*/
(function ($) {
    'use strict';
    $(document)
        .on("click", "[data-toggle=pg-help-swtich]", function (e) {
            var pThis = $(this);
            var id = pThis.data("id");

            $.ajax({
                type: "GET",
                url: "/ajax/pgswitch/" + id,
                cache: false,
                dataType: "json"
            });

        })
        .on("click", "[data-toggle=pg-help-swtich-set]", function (e) {
            var pThis = $(this);
            var id = pThis.data("id");
            var val = pThis.data("val");

            $.ajax({
                type: "GET",
                url: "/ajax/PgSwitchSet/" + id + '?val=' + val,
                cache: false,
                dataType: "json"
            });

        })
        .on("click", "[data-toggle=pg-help-swtich-clear]", function (e) {
            $.ajax({
                type: "GET",
                url: "/ajax/PgSwitchClear",
                cache: false,
                dataType: "json"
            });
        })
        .on("click", "[data-toggle=expand]", function (e) {
            if (e) e.preventDefault();
            var pThis = $(this);
            var bExpand = pThis.text() == "展开";

            pThis.text(bExpand ? "收起" : "展开");

            var target = $(pThis.data("target"));
            target.each(function () {
                if (bExpand)
                    $(this).show();
                else
                    $(this).hide();
            });
        })
        .on("click", "[data-skin]", function (e) {
            var pThis = $(this);
            var skin = pThis.data("skin");
            $("body").addClass(skin);
            $.ajax({
                type: "GET",
                url: "/ajax/UserCfgSet/main_skin?set=" + skin,
                cache: false,
                dataType: "json"
            });
        });
}(jQuery));


/*
* 翻页控件
*
*/
(function ($) {
    'use strict';
    $(document)
        //按页码翻页
        .on('click', 'a[data-pagger]', function (e) {
            e.preventDefault();

            var pThis = $(this);

            var target = pThis.data("target").split(',');
            var form = $("#" + target[0]);
            var curEle = $("#" + target[1]);

            curEle.val(pThis.data("pagger"));

            form.submit();
        })
        .on('blur', 'input[data-target=pagger]', function (e) {
            e.preventDefault();

            var pThis = $(this);

            var target = pThis.data("target").split(',');
            var form = $("#" + target[0]);
            var curEle = $("#" + target[1]);

            curEle.val(pThis.val());
            var url = pThis.data("targeturl");
            if (url != "") {
                form.attr('action', url);
            }
            form.submit();
        })
        .on('keypress', 'input[data-target=pagger]', function (e) {
            if (e.keyCode == "13") {
                e.preventDefault();
                var pThis = $(this);

                var target = pThis.data("target").split(',');
                var form = $("#" + target[0]);
                var curEle = $("#" + target[1]);

                curEle.val(pThis.val());
                var url = pThis.data("targeturl");
                if (url != "") {
                    form.attr('action', url);
                }
                form.submit();
            }
        });

}(jQuery));

var index;

function startLoding() {
    index = Loading();
}

function stopLoading() {
         layer.close(index);
}

/*
* Ajax 表单
* 1、 将 表单的数据 打包，然后将请求结果填入到Html节点
*       form[data-toggle=ajax-form] 
*/
(function($) {
    'use strict';

    $(document)
        .on('submit', 'form[data-toggle=ajax-form]', function(e) {
            e.preventDefault();
            var pThis = $(this);
            var url = pThis.attr("action");
            var data = pThis.serialize();

            $.ajax({
                url: url,
                type: "POST",
                data: data,
                success: function(result) {
                    $(pThis.data("target")).html(result);
                },
                beforeSend: startLoding,
                complete: stopLoading,
                dataType: "html"
            });
        })
        .on("submit", "form[data-toggle=ajax-partial]", function(e) {
            if (e) e.preventDefault();
            var pThis = $(this);
            var url = $(this).attr("action");
            var target = $(this).data("target");
            if (pThis.attr("enctype") == "multipart/form-data") {
               var data = new FormData(pThis[0]);

                $.ajax({
                    url: url,
                    type: 'POST',
                    data: data,
                    async: true,
                    cache: false,
                    contentType: false,
                    processData: false,

                    beforeSend: startLoding,
                    complete: stopLoading,

                    success: function(result) {
                        $(target).replaceWith(result);
                    },
                    error: function(returndata) {
                        //alert(returndata);
                    }
                });
            } else {
                var data = pThis.serialize();
                $.ajax({
                    url: url,
                    type: "POST",
                    data: data,
                    success: function (result) {
                        $(target).replaceWith(result);
                    },
                    beforeSend: startLoding,
                    complete: stopLoading,
                    dataType: "html"
                });
            }
        })
        .on("click", "a[data-toggle=ajax-partial]", function(e) {
            if (e) e.preventDefault();
            var url = $(this).attr("href");
            var target = $(this).data("target");
            $.ajax({
                url: url,
                type: "GET",
                cache: false,
                dataType: "html",
                beforeSend: startLoding,
                complete: stopLoading,
                success: function(resp) {
                    $(target).replaceWith(resp);
                }
            });
        });
}(jQuery));

(function($) {
    'use strict';

    $(document).on("click", ".mult-condition label", function(e) {
        if (e != null) e.stopPropagation();
        var pThis = $(this);
        if (pThis.hasClass("bg-aqua-active"))
            pThis.removeClass("bg-aqua-active");
        else
            pThis.addClass("bg-aqua-active");

        //需要延时处理
        setTimeout(function() {
            pThis.closest("form").submit();
        }, 20)
    });


}(jQuery));


(function($) {
    $(document).on('click', '[data-toggle=confirm-request]', function(e) {

        e.preventDefault();

        var pThis = $(this);

        confirmDlg($(this).data(), function (confirm) {

            if (!confirm) return;

            $(pThis.data('target')).modal({
                remote: pThis.attr("href")
            });

        });
    });


    function confirmDlg(data, callback) {
        var html = ' <div class="modal-header">' +
            '<h4 class="modal-title" id="myModalLabel">' + (data.title||"操作") + '</h4></div>' +
            '<div class="modal-body">' + (data.content || "确认操作") + '</div>' +
            '<div class="modal-footer">' +
            '<button type="button" class="btn btn-default" data-dismiss="confirm-cancel">' + (data.cancelText || "取消") + '</button>' +
            '<button type="button" class="btn btn-primary" data-dismiss="confirm-ok">'+ (data.confirmText || "保存") +'</button></div>';

        $("#remoteDlgSm").find(".modal-content").html(html);
        $("#remoteDlgSm").modal({
            keyboard: false,
            backdrop: false
        });

        $(document).on('click', '[data-dismiss=confirm-ok]', function() {
            off();
            callback(true);
        }).on('click', '[data-dismiss=confirm-cancel]', function() {
            off();
            callback(false);
        });

        function off() {
            $("#remoteDlgSm").modal("hide");
            $(document)
                .off('click', '[data-dismiss=confirm-ok]')
                .off('click', '[data-dismiss=confirm-cancel]');
        }

    }


}(jQuery));


(function ($) {
    $(document).on("click", "[data-toggle=dlg-selector]", function () {
        var pThis = $(this);

        var dlgFrm = pThis.closest(".modal");
        var dlgData = dlgFrm.data('bs.modal');

        dlgData.dlgValue = pThis.data("value");
    })
   .on("click", "[data-toggle=dlg-comfirm]", function () {
       var pThis = $(this);
       var dlgFrm = pThis.closest(".modal");
       var dlgData = dlgFrm.data('bs.modal');

       var target = $(dlgData.options.setField);
       target.val(dlgData.dlgValue);
       target.change();

       dlgData.dlgValue = null;
       dlgFrm.modal("hide");
   })
   .on("click", "[data-toggle=dlg-cencel]", function () {
       var pThis = $(this);
       var dlgFrm = pThis.closest(".modal");
       var dlgData = dlgFrm.data('bs.modal');
       dlgData.dlgValue = null;
       dlgFrm.modal("hide");
   });
}(jQuery));



(function ($) {
    $("#remoteDlg").on("hidden.bs.modal", function () {
        $(this).find('.modal-content').empty();
        $(this).removeData("bs.modal");
    });
    $("#remoteDlgLg").on("hidden.bs.modal", function () {
        $(this).find('.modal-content').empty();
        $(this).removeData("bs.modal");
    });
    $("#remoteDlgSm").on("hidden.bs.modal", function () {
        $(this).find('.modal-content').empty();
        $(this).removeData("bs.modal");
    });

    $(function () {
        if (top.location !== self.location) {
            top.location.href = self.location.href;
        }
    });

}(jQuery));
