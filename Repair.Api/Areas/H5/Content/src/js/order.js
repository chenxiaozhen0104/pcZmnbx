
var order = order || {};

order.common = {
    orderType: {
        /// <summary>
        /// 未知
        /// </summary>
        0: { description: "未知" },
        /// <summary>
        /// 需要维修
        /// </summary>
        1: { description: "需要维修" },
        /// <summary>
        /// 保养
        /// </summary>
        2: { description: "需要保养" }
    },
    orderLevel: {
        /// <summary>
        /// 未知
        /// </summary>
        0: { description: "未知" },
        /// <summary>
        /// 普通工单
        /// </summary>
        1: { description: "普通工单" },
        /// <summary>
        /// 加急工单
        /// </summary>
        2: { description: "加急工单" }
    },
    orderState: {
        /// <summary>
        /// 用户显示"新工单"
        /// </summary>
        Created: 4,
        /// <summary>
        /// 用户显示"派单中" 维修管理员显示"待派单"
        /// </summary>
        Sending: 8,
        /// <summary>
        /// 用户显示"已派单" 维修管理员显示"已派单" 维修人员显示"待服务"
        /// </summary>
        Sended: 16,
        /// <summary>
        /// 用户显示"工作中" 维修管理员显示"工作中" 维修人员显示"工作中"
        /// </summary>
        Working: 32,
        /// <summary>
        /// 用户显示"未确认" 维修管理员显示"未确认" 维修人员显示"未确认"
        /// </summary>
        Worked: 64,
        /// <summary>
        /// 用户显示"未解决" 维修管理员显示"未解决" 维修人员显示"未解决"
        /// </summary>
        Unsolved: 128,
        /// <summary>
        /// 用户显示"已取消" 维修管理员显示"已取消" 维修人员显示"已取消"
        /// </summary>
        Cancel: 256,
        /// <summary>
        /// 用户显示"已关闭" 维修管理员显示"已关闭" 维修人员显示"已关闭"
        /// </summary>
        Close: 512,
        /// <summary>
        /// 用户显示"已确认" 维修管理员显示"已完成" 维修人员显示"已完成"
        /// </summary>
        Confirm: 1024,
    },
    userType: {
        /// <summary>
        /// 服务单位用户
        /// </summary>
        SvcCompanyUser: 1,
        /// <summary>
        /// 使用单位用户
        /// </summary>
        UseCompanyUser: 2,
        /// <summary>
        /// 服务单位用户管理员
        /// </summary>
        SvcCompanyUserAdmin: 4,
        /// <summary>
        /// 使用单位用户管理员
        /// </summary>
        UseCompanyUserAdmin: 8,
        /// <summary>
        /// 超级管理员
        /// </summary>
        SuperAdmin: 16,
        /// <summary>
        /// 公共用户
        /// </summary>
        Common: 32
    },
    appType:
    {
        /// <summary>
        /// 报修
        /// </summary>
        Use: 1,
        /// <summary>
        /// 维修
        /// </summary>
        Service: 2
    },
    stateUse: {
        8: { description: "派单中", css: "bg_green", btnContent: "取消", func: "order.common.cancel()" },
        16: { description: "已派单", css: "bg_green", btnContent: "取消", func: "order.common.cancel()" },
        32: { description: "工作中", css: "bg_green" },
        64: { description: "未确认", css: "bg_red", btnContent: "确认完成", func: "order.common.confirmDialog()" },
        128: { description: "未解决", css: "bg_red" },
        256: { description: "已取消", css: "bg_gray" },
        512: { description: "已关闭", css: "bg_gray" },
        //1024: { description: "未评论", css: "bg_orange", btnContent: "评论", func: "order.common.commentDialog()" },
        1024: { description: "已完成", css: "bg_blue" },
        2048: { description: "已完成", css: "bg_blue" }
    },
    stateSvc: {
        8: { description: "待派单", css: "bg_orange", },
        16: { description: "待服务", css: "bg_orange", btnContent: "开始工作", func: "order.common.openQrCode('work')" },
        32: { description: "工作中", css: "bg_green", btnContent: "工作结束", func: "order.common.openQrCode('worked')" },
        64: { description: "未确认", css: "bg_orange" },
        128: { description: "未解决", css: "bg_red", btnContent: "开始工作", func: "order.common.openQrCode('work')" },
        256: { description: "已取消", css: "bg_gray" },
        512: { description: "已关闭", css: "bg_gray" },
        1024: { description: "已完成", css: "bg_gray" },
        2048: { description: "已完成", css: "bg_gray" }
    },
    stateSvcAdmin: {
        8: { description: "待派单", css: "bg_orange", btnContent: "派单", func: "order.common.dispatchDialog()" },
        16: { description: "已派单", css: "bg_blue" },
        32: { description: "工作中", css: "bg_green" },
        64: { description: "未确认", css: "bg_orange" },
        128: { description: "未解决", css: "bg_red" },
        256: { description: "已取消", css: "bg_gray" },
        512: { description: "已关闭", css: "bg_gray" },
        1024: { description: "已完成", css: "bg_gray" },
        2048: { description: "已完成", css: "bg_gray" }
    },
    //参数获取
    httpParam: new LG.URL(),
    //订单Id
    orderId: function () { return order.common.httpParam.get("id") },
    //用户类型
    uType: function () { return parseInt(localStorage.userType); },
    //用户Id
    userId: function () { return parseInt(localStorage.userId); },
    //app类型
    aType: function () { return parseInt(localStorage.appType); },
    //工单级别
    getOrderLevel: function (val) {
        return order.common.orderLevel[val];
    },
    //工单类型
    getOrderType: function (val) {
        return order.common.orderType[val];
    },
    //用状态信息
    getState: function (val, suid) {

        switch (order.common.aType()) {
            case order.common.appType.Use:
                return order.common.stateUse[val];
            case order.common.appType.Service:
                if (suid == order.common.userId()) {
                    return order.common.stateSvc[val];
                }
                if (order.common.uType() & order.common.userType.SvcCompanyUserAdmin) {
                    return order.common.stateSvcAdmin[val];
                }
                else {
                    return order.common.stateSvc[val];
                }
            default:
                return order.common.stateUse[val];
        }
    },
    completeCallBack: function (XHR, TS) {
        layer.closeAll();

        if (TS != "success") {
            AErrorMsg("服务器繁忙，请稍后再试试");
        } else {
            var obj = $.parseJSON(XHR.responseText);

            if (obj.success) {
                ARightMsg("执行成功")
            }
            else {
                AErrorMsg("服务器繁忙，请稍后再试试");
            }
        }

        setTimeout(function () {
            window.location.reload();
        }, 2000)
    },
    cancel: function () {
        //不显示选择类型
        $(".radio-inline").css("display", "none");
        //设置选中值
        $("#workedDialog input:radio[name='workedState'][value='Cancel']").attr("checked", true);
        //弹框
        DialogModel($("#workedDialog"), "92%", "38%");
    },
    work: function () {
        AComfig("确定提交？", function () {

            Loading();

            $.ajax({
                url: "/api/order/working",
                type: "get",
                data: { "orderId": order.common.orderId() },
                complete: order.common.completeCallBack
            })
        })
    },
    openQrCode: function (type) {
        //order.common.wordedDialog();
        //order.common.work();
        window.location.href = "http://action?target=_openqrcode&type=" + type;
    },
    wordedDialog: function () {
        //弹框
        DialogModel($("#workedDialog"), "92%", "43%");
    },
    worked: function () {

        var description = $("#workedDialog #description").val();

        var serviceUserId = $("#serviceUserId").val();

        var state = $("#workedDialog input:radio[name='workedState']:checked").val();

        AComfig((!description ? "您未输入信息," : "") + "确定提交？", function () {

            Loading();

            $.ajax({
                url: "/api/order/submitstate",
                type: "get",
                data: { "orderId": order.common.orderId(), "description": description, "state": state, "level": 5, "toUserId": serviceUserId, appType: localStorage.appType },
                complete: order.common.completeCallBack
            })
        })
    },
    dispatchDialog: function () {
        DialogModel($("#v-user"), "92%", "50%");
    },
    dispatch: function (user) {
        AComfig("确定派单给[" + user.name + "]？", function () {

            Loading();

            $.ajax({
                url: "/api/order/dispatch",
                type: "get",
                data: { "orderId": order.common.orderId(), "serviceUserId": user.id, "serviceUserName": user.name, "serviceUserPhone": user.phone },
                complete: order.common.completeCallBack
            })
        })
    },
    confirmDialog: function () {
        //不显示选择类型
        $(".radio-inline").css("display", "none");
        //设置选中值
        $("#workedDialog input:radio[name='workedState'][value='Confirm']").attr("checked", true);
        //弹框
        DialogModel($("#workedDialog"), "92%", "38%");
    },
    commentDialog: function () {
        //不显示选择类型
        $(".radio-inline").css("display", "none");

        $(".level").css("display", "block");
        $("#contentTitle").html("评论内容：");

        //设置选中值
        $("#workedDialog input:radio[name='workedState'][value='UseComment']").attr("checked", true);
        //弹框
        DialogModel($("#workedDialog"), "92%", "40%");
    },
    getstatelist: function () {
        switch (order.common.aType()) {
            case order.common.appType.Use:
                return [{ name: "派单中", val: 8 },
                { name: "已派单", val: 16 },
                { name: "工作中", val: 32 },
                { name: "未确认", val: 64 },
                { name: "未评论", val: 1024 },
                { name: "进行中", val: 8 + 16 + 32 + 64 + 1024 }];
            case order.common.appType.Service:
                return [{ name: "待派单", val: 8 },
                { name: "待服务", val: 16 },
                { name: "工作中", val: 32 },
                { name: "未确认", val: 64 },
                { name: "未解决", val: 128 },
                { name: "进行中", val: 8 + 16 + 32 + 64 + 128 + 1024 }];
            default:
                return [];
        }
    },
    //转单
    transferOrder: function () {
        alert("转单");
    }
}
//列表
order.paged = {
    // 请求地址
    url: "/api/order/search",
    // 页码初始化
    pageIndex: 1,
    //当前状态
    curState: 4 + 8 + 16 + 32 + 64 + 1024,
    // 页码初始化
    pagePage: 20,
    // 允许分页
    allowPaged: true,
    //vue模组
    vueModule: new Vue({
        el: "#orderList",
        data: {
            items: [],
            statelist: order.common.getstatelist()
        },
        methods: {
            search: function (done) {
                order.paged.search(1, order.paged.curState, done);
            },
            stateGoto: function (val) {
                order.paged.curState = val
                order.paged.search(1, order.paged.curState);
            }
        },
        filters: {
            stateDescription: function (val, suid) {
                return order.common.getState(val, suid).description;
            },
            stateCss: function (val, suid) {
                return order.common.getState(val, suid).css;
            },
            typeDescription: function (val) {
                return order.common.getOrderType(val).description;
            },
            levelDescription: function (val) {
                return order.common.getOrderLevel(val).description;
            }
        }
    }),
    // 检索
    search: function (pIndex, state, done) {
        var index = Loading();
        $.ajax({
            url: order.paged.url,
            type: "post",
            data: { "state": order.paged.curState, "atype": order.common.aType(), "pageIndex": pIndex, 'pageSize': order.paged.pagePage },
            success: function (obj) {
                //首次加载直接赋值，这是因为防止下拉刷新带来的追加行为

                if (pIndex < 2) {
                    order.paged.vueModule.items = [].concat(obj.data);
                }
                else {
                    order.paged.vueModule.items = order.paged.vueModule.items.concat(obj.data);
                }
                if (obj.data.length < order.paged.pagePage) {
                    order.paged.allowPaged = false;

                    $("#footerWran").html("<div class='row text-center'>没有工单咯</div>")
                }
            },
            complete: function () {
                CloseLoad(index);
                done && done();
            }
        })
    },
    // 上滑分页
    paged: function () {
        $(window).scroll(function () {
            if (!order.paged.allowPaged) {
                return false;
            }
            if ($(window).scrollTop() >= ($(document).height() - $(window).height())) {

                // 翻页+1
                order.paged.pageIndex = parseInt(order.paged.pageIndex) + 1;

                // 查询
                order.paged.search(order.paged.pageIndex);
            }
        });
    },
    opendetails: function (id) {

        window.location.href = "orderdetails.html?id=" + id;
    }
};

//详情
order.details = {
    // 请求地址
    url: "/api/order/get",
    //vue模组
    vueModule: new Vue({
        el: "#order",

        data: {
            items: []
        },
        methods: {
            selectUser: function (user) {
                order.common.dispatch(user);
            },
            search: function (done) {
                order.details.search(done);
            },

            getSubmitFunc: function (index, state, suid) {
                return order.common.getState(state, suid).func[index];
            },
            getSubmitContent: function (index, state, suid) {
                return order.common.getState(state, suid).btnContent[index];
            }

        },
        filters: {
            stateDescription: function (val, suid) {
                return order.common.getState(val, suid).description;
            },
            stateCss: function (val, suid) {
                return order.common.getState(val, suid).css;
            },
            typeDescription: function (val) {
                return order.common.getOrderType(val).description;
            },
            levelDescription: function (val) {
                return order.common.getOrderLevel(val).description;
            },
            submitCss: function (val, suid) {
                var result = order.common.getState(val, suid);

                if (result.btnContent) {
                    return "submitBlock";
                }
                return "submitNone";
            },
            submitContent: function (val, suid) {
                return order.common.getState(val, suid).btnContent;
            },
            submitFunc: function (val, suid) {
                return order.common.getState(val, suid).func;
            }
        }
    }),
    // 检索
    search: function (done) {
        var index = Loading();
        $.ajax({
            url: order.details.url,
            type: "post",
            data: { "id": order.common.orderId() },
            success: function (obj) {
                order.details.vueModule.items = [].concat(obj);
            },
            complete: function () {
                CloseLoad(index);
                done && done();
            }
        })
    }
};

function qrCallBack(eUrl, type) {
    var eStr = eUrl.substr(eUrl.lastIndexOf('/') + 1);

    if (eStr != $("input[name=QRCode]").val()) {
        AWarnMsg("二维码不正确" + eStr);
    }
    else {
        if (type == "work") {
            order.common.work();
        }
        else if (type == "worked") {
            order.common.wordedDialog();
        }
        else {
            AWarnMsg("操作错误");
        }
    }
}

window.order = order;