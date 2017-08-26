$(function () {
    $("#AjaxClose").click(function () {
        parent.layer.closeAll();
    });

    $("#AjaxForm").keydown(function (u) {
        var t = (window.event) ? event.keyCode : u.keyCode;
        if (t == 32) {
            return false;
        } else {
            if (t == 13) {
                ToSubmit();
            }
        }
        return true;
    });

    $("#AjaxSubmit").click(function () {
        ToSubmit();
    });
});

function ToSubmit() {
    if ($("#AjaxSubmit").attr('disabled') == false || $("#AjaxSubmit").attr('disabled') == undefined) {
        $("#AjaxSubmit").attr('disabled', true);
        $("#AjaxForm").ajaxSubmit(GetOptions());
    }
}

//ajax
function GetOptions() {
    var index;
    var options = {
        type: "post",
        dataType: "json",
        beforeSubmit: function () {
            var result = AjaxSubmitValidate();
            if (result) {
                index = Loading();
            } else {
                $("#AjaxSubmit").attr('disabled', false);
            };
            return result;
        },
        success: function (data) {
            CloseLoad(index);
            switch (data.ErrCode) {
                case -3:
                    AMessage(data.ErrMsg);
                    break;
                case -2:
                    ShowError(data.HtmlId, data.ErrMsg);
                    break;
                case -1:
                    AError("<font color='red'>" + data.ErrMsg + "</font>");
                    break;
                case 0:
                    if (data.ReturnUrl != "")
                        AlertUrlRight(data.ErrMsg, data.ReturnUrl, false);
                    else
                        AlertRight(data.ErrMsg, false);
                    break;
                case 200:
                    AlertRight(data.ErrMsg, true);
                    break;
                default:
                    AError(data.msg);
                    break;
            }
        },
        complete: function () {
            $("#AjaxSubmit").attr('disabled', false);
        },
        error: function (xhr, status, error) {
            CloseLoad(index);
            AError("网络错误，请联系客服解决！");
        }
    };
    return options;
}

function GetAjaxOptions(url, data) {
    var index;
    var options = {
        url: url,
        data: data,
        dataType: "json",
        beforeSubmit: function () {
            index = Loading();
            return true;
        },
        success: function (data) {
            switch (data.ErrCode) {
                case -3:
                    AMessage(data.ErrMsg);
                    break;
                case -2:
                    ShowError(data.HtmlId, data.ErrMsg);
                    break;
                case -1:
                    AError("<font color='red'>" + data.ErrMsg + "</font>");
                    break;
                case 0:
                    if (data.ReturnUrl != "")
                        AlertUrlRight(data.ErrMsg, data.ReturnUrl, false);
                    else
                        AlertRight(data.ErrMsg, false);
                    break;
                case 200:
                    AlertRight(data.ErrMsg, true);
                    break;
                default:
                    AError(data.msg);
                    break;
            }
        },
        complete: function () {
            CloseLoad(index);
        },
        error: function (xhr, status, error) {
            AError("网络错误，请联系客服解决！");
        }
    };
    return options;
}

//分页
function PageInfo(page) {
    var url = window.location.href;
    if (page != null) {
        if (url.indexOf("?") > 0) {
            if (url.indexOf("page") > 0) {
                url = url.split("?")[0] + setParam("page", page);
            } else {
                url = url + "&page=" + page;
            }
        } else {
            url = url + "?page=" + page;
        }
        window.location.href = url;
    }
}

function setParam(param, value) {
    var query = location.search.substring(1);
    var p = new RegExp("(^|)" + param + "=([^&]*)(|$)");
    if (p.test(query)) {
        var firstParam = query.split(param)[0];
        var secondParam = query.split(param)[1];
        if (secondParam.indexOf("&") > -1) {
            var lastPraam = secondParam.split("&")[1];
            return '?' + firstParam + param + '=' + value + '&' + lastPraam;
        } else {
            if (firstParam) {
                return '?' + firstParam + param + '=' + value;
            } else {
                return '?' + param + '=' + value;
            }
        }
    } else {
        if (query == '') {
            return '?' + param + '=' + value;
        } else {
            return '?' + query + '&' + param + '=' + value;
        }
    }
}