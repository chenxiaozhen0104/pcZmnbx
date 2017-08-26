$(".publistBottom-lists a").each(function (i, item) {
    //console.log(window.location.pathname);
    //console.log(item.pathname)
    var regExp = new RegExp(item.pathname);
    if (regExp.test(location.pathname)) {
        $(item).parent().addClass('publicLists-active');
        $(item).addClass("color-active");
    }
});



var equipmentData = new Vue({
    el: '#equipmentLists',
    data: {
        devicelist: [],
        imgList: [],
        areas: [],
        brands: [],
        categorys: [],
        areaId: '0',
        brandsId: '0',
        categoryId: '0',
        manufacturersNames: [],
        manufacturersName: '暂无',
        pageindex: 1,
        pagesize: 10,
        searchkey: '',
        AssetsId: '',
        QRCode: '',
        Name: '',
        Model: '',
        Position: '',
        Longitude: '',
        Dimension: '',
        BuyTime: '',
        WarrantyTime: '',
        checkboxName: '',

        Note: '',
        DeviceId: '',
        pagetotal: 0,

        addExcel: "",
        item1: {},
        item2: {},
        item3: {},
        item4: {},
        cateArry1: [],
        cateArry2: [],
        cateArry3: [],
        cateArry4: [],
        selectItem: {},

        selectCheckBoxArr: []
    },
    methods: {

        init: function () {
            var self = this;
            $.post("/Device/EquipmentList", { search: self.searchkey, pageIndex: self.pageindex, pageSize: self.pagesize }, function (res) {

                layer.closeAll();
                self.pagetotal = Math.ceil(res.count / self.pagesize);
                self.devicelist = res.list.filter(function (item) {
                    item.selectCheck = false;
                    return true;
                });
                self.imgList = res.imgList;
                document.documentElement.scrollTop = document.body.scrollTop = 0;
            })
        },
        setSelectCheck: function (obj) {
            var self = this;
            if (obj.selectCheck) {
                obj.selectCheck = false;
                for (var i = 0; i < self.selectCheckBoxArr.length; i++) {
                    if (self.selectCheckBoxArr[i] == obj.DeviceId) {
                        self.selectCheckBoxArr.splice(i, 1);
                        break;
                    }
                }
            }
            else {
                self.selectCheckBoxArr.push(obj.DeviceId);
                obj.selectCheck = true;
            }
        },
        updateCate() {
            var self = this;
            var arr = [];
            self.selectCheckBoxArr.filter(function (item) {
                arr.push(item);
            });
            $.post("/Device/UpdateCategoryId", { deviceIdArr: arr, CategoryId: self.selectItem.CategoryId }, function (res) {
                console.log(res);
                layer.msg(res.success, { icon: 0, time: 1000 });
                setTimeout(function () {
                    layer.closeAll();
                    window.location.reload();
                }, 1500)
                self.selectCheckBoxArr = [];
                
            })
        },
        getChildCate(item) {
            if (item && item.hasOwnProperty('LevelDeep')) {
                var self = this;
                this.selectItem = item;
            }

        },
        getBaseInfo: function () {
            var self = this;

            $.post("/Device/GetBaseInfo", {}, function (res) {
                self.areas = res.areas;
                self.areas.unshift({ AreaId: "0", Name: "请选择区域" });

                self.brands = res.brands;
                self.brands.unshift({ BrandId: "0", Name: "请选择品牌" });

                self.categorys = res.categorys;
                self.cateArry1 = res.categorys.filter(function (item) {
                    return item.LevelDeep == 1;
                })
                self.cateArry2 = res.categorys.filter(function (item) {
                    return item.LevelDeep == 2;
                })
                self.cateArry3 = res.categorys.filter(function (item) {
                    return item.LevelDeep == 3;
                })
                self.cateArry4 = res.categorys.filter(function (item) {
                    return item.LevelDeep == 4;
                })
                self.categorys.unshift({ CategoryId: "0", Name: "请选择类目" });

            })
        },

        doSearch: function () {
            var self = this;
            self.init();
            Loading();
        },
        chooseManu: function (brand) {
            var self = this;

            if (brand) {
                self.manufacturersNames = self.brands.filter(function (item) {
                    if (brand == item.BrandId) {
                        return item.Manufacturer && item.Manufacturer.Name ? item.Manufacturer.Name : '暂无';
                    }
                });

                self.manufacturersName = self.manufacturersNames[0].Manufacturer && self.manufacturersNames[0].Manufacturer.Name ? self.manufacturersNames[0].Manufacturer.Name : '暂无';
            } else {
                self.brandsId = "0";
            }
        },
        addEquipment: function () {
            layer.open({
                type: 1,
                title: false,
                closeBtn: 0,
                area: ['center', 'center'],
                skin: 'layui-layer-nobg',
                shadeClose: true,
                content: $('.equipment-addCont')
            });
        },

        deviceDelete: function (id) {

            AComfig("删除后将无法恢复！确定要删除该设备信息吗？", function () {

            })
        },
        

        deviceDeleteAll: function () {
            var self = this;
            if (self.selectCheckBoxArr.length > 0) {
                layer.open({
                    type: 1,
                    title: false,
                    closeBtn: 0,
                    area: ['center', 'center'],
                    skin: 'layui-layer-nobg',
                    shadeClose: true,
                    content: $('.amendCategory')
                })
            } else {
                layer.msg("请选择需要修改类目的设备", { icon: 0, time: 1500 });
            }
        },
        deviceEditor: function (item) {
            var self = this;
            if (item == "0") {
                $(".equipment-editorTitle>h3").html("新增设备");
                self.AssetsId = "";
                self.QRCode = "";
                self.Name = "";
                self.Model = "";
                self.Position = "";
                self.DeviceId = "";
                self.brandsId = "0";
                self.areaId = "0";
                self.categoryId = "0";
                self.Note = "";
                self.Longitude = "";
                self.Dimension = "";
                self.BuyTime = "";
                self.WarrantyTime = "";
                self.manufacturersName = "暂无";

            } else {

                $(".equipment-editorTitle>h3").html("[编辑]" + item.DeviceId + " — " + item.Name);

                self.AssetsId = item.AssetsId;
                self.QRCode = item.QRCode;
                self.Name = item.Name;
                self.Model = item.Model;
                self.Position = item.Position;
                self.DeviceId = item.DeviceId;
                self.areaId = item.AreaId ? item.AreaId : "0";
                self.brandsId = item.BrandId ? item.BrandId : "0";
                self.categoryId = item.CategoryId ? item.CategoryId : "0";
                self.Longitude = item.Longitude;
                self.Dimension = item.Dimension;
                self.Note = item.Note;

                self.chooseManu(item.BrandId);

                self.BuyTime = self.initData(item.BuyTime);
                self.WarrantyTime = self.initData(item.WarrantyTime);

                document.querySelector("#buyTime").value = self.BuyTime;
                document.querySelector("#warrantyTime").value = self.WarrantyTime;

            }


            layer.open({
                type: 1,
                title: false,
                closeBtn: 0,
                area: ['center', 'center'],
                skin: 'layui-layer-nobg',
                shadeClose: true,
                content: $('.equipment-editorCont')
            });
        },
        initData: function (obj) {
            if (!obj) {
                return "";
            }
            obj = parseInt(obj.replace(/\D/igm, ""));
            var data = new Date(obj);
            var sepater = "-";
            var month = data.getMonth() + 1;
            var strData = data.getDate();
            if (month >= 0 && month < 10) {
                month = "0" + month;
            }
            if (strData >= 0 && strData < 10) {
                strData = "0" + strData
            }
            var currentData = data.getFullYear() + sepater + month + sepater + strData;

            return currentData;

        },
        excelSubmit: function () {
            var self = this;
            var excelUrl = $(".chagePhoto-input").val();
            if (!excelUrl) {
                layer.msg("上传文件不能为空", { icon: 0, time: 1000 });
                return;
            }
            Loading();
            $("#excelUpload").ajaxSubmit({
                url: "/Device/ImportExcel",
                type: "post",
                data: {
                },
                success: function (data) {
                    if (data.result) {
                        layer.msg(data.message, { icon: 0, time: 1000 });
                        $(".chagePhoto-input").val("");
                        self.closeAlter();
                    } else {
                        layer.msg(data.message, { icon: 0, time: 1000 });
                    }
                }

            });
            return false;

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
            self.init();
            Loading();
        },

        deviceClose: function () {
            layer.closeAll();
        },
        getUrl: function (obj) {
            var self = this;
            var result = '';
            for (var i in self.imgList) {
                if (self.imgList[i].OuterId == obj) {
                    result = self.imgList[i].Url;
                    break;
                }
            }
            return result;
        },
        submitBtn: function () {
            var self = this;
            if (self.AssetsId && self.Position && self.Name) {
                self.brandsId = self.brandsId == "0" ? "" : self.brandsId;
                self.categoryId = self.categoryId == "0" ? "" : self.categoryId;
                self.areaId = self.areaId == "0" ? "" : self.areaId;

                $.post("/Device/Add", {
                    AssetsId: self.AssetsId,
                    QRCode: self.QRCode,
                    Name: self.Name,
                    Model: self.Model,
                    BrandId: self.BrandId,
                    AreaId: self.AreaId,
                    CategoryId: self.CategoryId,
                    Position: self.Position,
                    Longitude: self.Longitude,
                    Dimension: self.Dimension,
                    BuyTime: self.BuyTime,
                    WarrantyTime: self.WarrantyTime,
                    Note: self.Note,
                    DeviceId: self.DeviceId

                }, function (res) {
                    self.brandsId = "0";
                    self.categoryId = "0";
                    self.areaId = "0";

                    if (res.error == "false") {
                        layer.msg(res.message, { icon: 0, time: 2000 });
                        self.closeAlter();

                    } else {
                        if (res.result) {
                            layer.msg(res.message, { icon: 1, time: 2000 });
                            self.closeAlter();
                            setTimeout(function () {
                                self.init();
                            }, 1500)

                        } else {
                            layer.msg(res.message, { icon: 2, time: 2000 });
                            self.closeAlter();
                        }
                    }
                })

            } else {
                AWarnMsg("资产、设备名称及安装地址为必填项！");
            }
        },
        closeAlter: function () {
            setTimeout(function () {
                layer.closeAll();
            }, 1000)
        },
        removeExcel: function () {
            $(".chagePhoto-input").val("");
        }
    },

    created: function () {
        this.init();
        this.getBaseInfo();
    }
});



function fileChange(target) {
    //var fileSize = 0;
    //if (!target.files) {
    //    var filePath = target.value;
    //    var filesSystem = new ActiveXObject("Scripting.FileSystemObject");
    //    var file = filesSystem.GetFile(filePath);
    //    fileSize = file.Size;
    //} else {
    //    fileSize = target.files[0].size;
    //}


    //var size = fileSize / 1024;
    //if (size > 2000) {
    //    layer.msg("附件不能大于2M", { icon: 0, time: 1000 });
    //    target.value = '';
    //    return;
    //}
    var name = target.value;

    var fileName = name.substring(name.lastIndexOf('.') + 1).toLowerCase();

    if (!fileName) {
        return;
    }
    if (fileName != 'xls' && fileName != 'xlsx') {
        layer.msg("请选择Excel文件上传", { icon: 0, time: 1000 });
        target.value = '';
        return;
    } else {
        $(".chagePhoto-input").val(name);
    }
}

