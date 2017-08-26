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
    //$(".nav-bind").each(function (i, item) {
    //    $(item).addClass("")
    //})
})
//数组是否包含某个元素
Array.prototype.contains = function (needle) {
    for (i in this) {
        if (this[i] == needle) return true;
    }
    return false;
}
Array.prototype.remove = function (val) {
    var index = this.indexOf(val);
    if (index > -1) {
        this.splice(index, 1);
    }
};
var sercerList = new Vue({
    el: '.public-right',
    data: {
        pageindex: 1,
        pagesize: 50,
        pagetotal: 0,
        bindlist: [],
        areaList: [],
        brandList: [],
        categoryArr: [],
        deviceList: [],
        joinCompany: [],
        selectItemArr: {
            deviceArr: [],
            categoryArr: [],
            areaArr: [],
            manufacturerArr: []
        },
        deviceSelectServiceCompany: 0,
        cateSelectServiceCompany: 0,
        areaSelectServiceCompany: 0,
        keys:"",

    },
    methods: {
        //获取已经选择的设备信息
        getSelectItem(data) {
            var self = this;
            for (var i in data) {
                if (data[i].BindingType == 2) {
                    self.selectItemArr.deviceArr.push(data[i].Content);
                }
                else if (data[i].BindingType == 1) {
                    self.selectItemArr.categoryArr.push(data[i].Content);
                } else if (data[i].BindingType == 4) {
                    self.selectItemArr.areaArr.push(data[i].Content);
                } else if (data[i].BindingType == 8) {
                    self.selectItemArr.manufacturerArr.push(data[i].Content);

                }
            }
        },
        //获取绑定的类型
        getBindType(v) {
            if (v == 1)
                return '绑定类目'
            else if (v == 2)
                return '绑定设备'
            else if (v == 4)
                return '绑定区域'
            else if (v == 8)
                return '绑定厂商'
            else
                return '未知'
        },
        //获取绑定的名称
        getBindName(t, v) {
            var self = this;
            if (t == 1) {
                for (var i in self.categoryArr) {
                    if (self.categoryArr[i].CategoryId == v)
                        return self.categoryArr[i].Name;
                }
            } else if (t == 2) {
                for (var i in self.deviceList) {
                    if (self.deviceList[i].DeviceId == v)
                        return self.deviceList[i].Name;

                }
            } else if (t == 4) {
                for (var i in self.areaList) {
                    if (self.areaList[i].AreaId == v)
                        return self.areaList[i].Name;

                }
            }
        },
        //获取品牌和厂家
        getManufacturer(v) {
            if (v) {
                for (var i in this.brandList) {
                    if (this.brandList[i].BrandId == v) {
                        return this.brandList[i];
                    }
                }
            } else {
                return '暂无';
            }
        },
        //获取设备信息
        getDeviceInfo(v) {
            for (var i in this.deviceList) {
                if (v == this.deviceList[i].DeviceId)
                    return this.deviceList[i];
            }
        },
        //获取类目信息
        getCategory(v) {
            for (var i in this.categoryArr) {
                if (this.categoryArr[i].CategoryId == v) {
                    return this.categoryArr[i].Name;
                }
            }
        },
        //获取区域信息
        getArea(v) {
            for (var i in this.areaList) {
                if (this.areaList[i].AreaId == v) {
                    return this.areaList[i].Name;
                }
            }
        },
        //添加
        addItem(t, v) {
            if (t == 2) {
                if (!this.selectItemArr.deviceArr.contains(v)) {
                    this.selectItemArr.deviceArr.unshift(v);
                }
            } else if (t == 1) {
                if (!this.selectItemArr.categoryArr.contains(v)) {
                    this.selectItemArr.categoryArr.unshift(v);
                }
            } else if (t == 4) {
                if (!this.selectItemArr.areaArr.contains(v)) {
                    this.selectItemArr.areaArr.unshift(v);
                }
            }
            
        },
        //移除
        removeItem(t, v) {
            if (t == 2) {
                this.selectItemArr.deviceArr.remove(v);
            } else if (t == 1) {
                this.selectItemArr.categoryArr.remove(v);
            } else if (t == 4) {
                this.selectItemArr.areaArr.remove(v);
            }
        },

        //json时间格式化
        jsonDateFormat: function (jsonDate) {
            try {
                var date = new Date(parseInt(jsonDate.replace("/Date(", "").replace(")/", ""), 10));
                var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
                var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
                return date.getFullYear() + "-" + month + "-" + day;
            } catch (ex) {
                return "";
            }
        },
        //初始化
        initData: function () {
            LoadInfo("查询中");
            var self = this;
            console.log(self.keys);
            $.get("/BindService/ServerList", { keys: self.keys, pageIndex: self.pageindex, pageSize: self.pagesize}, function (res) {
                    console.log(res);
                    self.bindlist = res.bindlist,
                    self.areaList = res.areaList,
                    self.bindlist = res.bindlist,
                    self.brandList = res.brandList,
                    self.categoryArr = res.categoryArr,
                    self.deviceList = res.deviceList,
                    self.joinCompany = res.joinCompany
                    self.pagetotal = res.total;
                    self.getSelectItem(self.bindlist);
                    layer.closeAll();
                });
        },
        //搜索查询
        search: function () {
            var self = this;
            self.initData();
        },
        //提交设备绑定
        submitBind: function (t) {
            LoadInfo("执行中");
            var self = this;
            if (t == 2) {
                if (self.deviceSelectServiceCompany != 0) {
                    var arr = [];
                    arr.push(this.deviceSelectServiceCompany);
                    $.post("/BindService/BindingDevice", { deviceArr: this.selectItemArr.deviceArr, bindingType: 2, serviceCompanyArr: arr }, function (res) {
                        //self.close();
                        ARightMsg(res);
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);

                    });
                } else {
                    AWarnMsg("请选择单位");
                }
            } else if (t == 1) {
                if (self.cateSelectServiceCompany != 0) {
                    var arr = [];
                    arr.push(this.deviceSelectServiceCompany);
                    $.post("/BindService/BindingCategory", { categoryArr: this.selectItemArr.categoryArr, bindingType: 1, serviceCompanyId: self.cateSelectServiceCompany }, function (res) {
                        //self.close();
                        ARightMsg(res);
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);
                    });
                } else {
                    AWarnMsg("请选择单位");
                }
            } else if (t == 4) {
                if (self.areaSelectServiceCompany != 0) {
                    var arr = [];
                    arr.push(this.deviceSelectServiceCompany);
                    $.post("/BindService/BindingArea", { areaArr: this.selectItemArr.areaArr, bindingType: 4, serviceCompanyId: self.areaSelectServiceCompany }, function (res) {
                        //self.close();
                        ARightMsg(res);
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);

                    });
                } else {
                    AWarnMsg("请选择单位");
                }
            }
        
        },
        bindDevice: function () {
            layer.open({
                type: 1,
                title: false,
                closeBtn: 0,
                area: ['center', 'center'],
                skin: 'layui-layer-nobg',
                shadeClose: true,
                content: $('.alert-bindDevice')
            });
        },
        bindCategory: function () {
            layer.open({
                type: 1,
                title: false,
                closeBtn: 0,
                area: ['center', 'center'],
                skin: 'layui-layer-nobg',
                shadeClose: true,
                content: $('.alert-bindCategory')
            });
        },
        bindArea: function () {
            layer.open({
                type: 1,
                title: false,
                closeBtn: 0,
                area: ['center', 'center'],
                skin: 'layui-layer-nobg',
                shadeClose: true,
                content: $('.alert-bindArea')
            });
        },
        close: function () {
            layer.closeAll();
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
            this.initData();
            //this.init();
        },
    },
    created: function () {
        var self = this;
        self.initData();
    }
})