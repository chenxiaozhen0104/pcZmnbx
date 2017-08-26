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

    $(".right-nav-text").click(function () {
        $(this).addClass("yellowBackground").siblings(".right-nav-text").removeClass("yellowBackground");
    })
})

var order = new Vue({
    el: '#OrderListvue',
    data: {
        list: [],//订单数据
        usecompanydic: [],//报修公司信息
        devicedic: [],//设备信息，
        userDic: [],//报修人信息
        serveuserdic: [],//维修人信息
        piclist: [],//图片,
        state: 4 + 8 + 16 + 32 + 64 + 128 + 256 + 512 + 1024 + 2048 + 4096,
        pageindex: 1,
        pagesize: 10,
        pagetotal: 0,
        searchkey: '',
        statenum:[]
    },
    methods: {
        //获取维修人员
        getrepairuser: function (id) {
            var result = {};
            for (var i in this.serveuserdic) {
                if (id == i) {
                    result = this.serveuserdic[i];
                    break;
                }
            }
            return result;
        },
        //获取保修人员
        getUser: function (id) {
            var result = {};
            for (var i in this.userDic) {
                if (id == i) {
                    result = this.userDic[i];
                    break;
                }
            }
            return result;
        },
        getDevice: function (id) {
            var result = {};
            for (var i in this.devicedic) {
                if (id == i) {
                    result = this.devicedic[i];
                    break;
                }
            }
            return result;
        },
        getImgurl: function (id) {
            var result = [];
            for (var i in this.piclist) {
                if (id == this.piclist[i].OuterId) {
                    result.push(this.piclist[i]);
                }
            }
            return result;

        },

        getType: function (obj) {
            if (obj == 1)
                return "需要维修";
            else
                return "保养";
        },
        getLevel: function (obj) {
            if (obj == 1)
                return "普通工单";
            else
                return "加急工单";
        },
        getNum: function (o) {
            if (o) {
                var a = 0;
                for (var i in this.statenum) {
                    if (o & this.statenum[i].State)
                        a = a + this.statenum[i].Num;
                }
                return a;
            } else {
                var a = 0;
                this.statenum.filter(function (obj) {
                    a = a + obj.Num;
                });
                return a;
            }
        },
        init: function () {
            var self = this;
            try {
                LoadInfo("查询中");
                $.post("/Server/WxGetOrderStateNum/", {}, function (res) {
                    self.statenum = res;

                });
                $.post("/Server/OrderListData/", { state: self.state, search: self.searchkey, pageIndex: self.pageindex, pageSize: self.pagesize }, function (res) {
                    if (res) {
                        console.log(res);
                        self.list = res.MainOrderData;
                        self.devicedic = res.DeviceDic;
                        self.usecompanydic = res.UseCompanyDic;
                        self.userDic = res.UserDic;
                        self.serveuserdic = res.ServeUserDic;
                        self.piclist = res.PicList;
                        self.pagetotal = res.pageTotal;
                        layer.closeAll();
                    } else {
                        self.list = [];
                        self.pagetotal = 0;
                        layer.closeAll();
                    }
                    document.documentElement.scrollTop = document.body.scrollTop = 0;
                });

              
            }
            catch (err) {
                layer.closeAll();
            }  
        },
        find: function (obj) {
            var self = this;
            if (!parseInt(obj)) {
                if (obj == 'pre') {
                    if (self.pageindex == 1)
                        return;
                    else
                        self.pageindex--;
                } else if (obj == 'next') {
                    if (self.pageindex == self.pagetotal)
                        return;
                    else
                        self.pageindex++;
                }
            }
            else {
                self.pageindex = obj;
            }
            this.resetInit();
            this.init();
        },
        stateFun: function (obj) {
            this.resetInit();
            this.state = obj;
            this.init();
        },
        searchFun: function () {
            this.resetInit();
            this.init();
        },
        resetInit: function () {
            self.list = [];
            self.devicedic = [];
            self.usecompanydic = [];
            self.userDic = [];
            self.serveuserdic = [];
            self.piclist = [];
            self.pagetotal =0;
        },
        getState: function (state) {
                switch (state) {
                    case 8:
                        return "待派单";
                        break;
                    case 16:
                        if (sessionStorage._page == "1") {
                            return "待服务";
                        };
                        if (sessionStorage._page == "2") {
                            return "已派单";
                        }
                        break;
                    case 32:
                        return "工作中";
                        break;
                    case 64:
                        return "未确认";
                        break;
                    case 128:
                        return "未解决";
                        break;
                    case 256:
                        return "已取消";
                        break;
                    case 512:
                        return "已关闭";
                        break;
                    case 1024 || 2048:
                        return "已完成";
                        break;
                    case 2048:
                        return "已完成";
                        break;
                    case 4096:
                        return "已转单";
                        break;
                    default:
                        return "";
                }

        }
    },
    created: function () {
        this.init();
    }

});
