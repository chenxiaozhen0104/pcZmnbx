var joinData = new Vue({
     el: '#container',
    data: {
        isShow: false,
        checked: true,
        isClick:true,
        serviceList: [],
        checkedNames: "",
        chooseType:"2",
        selectType: "2",
        serviceKey: "",
        companyList: [],
        companyId: "",

        useCompanyId:'',
        ownerCompany: "",
        ownerPosition: "",
        ownerContact: "",
        ownerPhone: "",

        serviceCompanyId:'',
        IsManufacturer: "0",
        serviceCompany: "",
        servicePosition: "",
        serviceContact: "",
        servicePhone: "",
        userState: '',
        serUserState: '',
     },
    methods: {
        getServiceList: function () {
            var self = this;
            $.get("/Join/GetCategoryList", {}, function (res) {
                self.serviceList = res;
            })
        },
        serviceBlur: function () {
            var self = this;
            var reg = /^[1][34578]\d{9}$/;
            if (!reg.test(self.servicePhone)) {
                layer.tips('手机号码有误', '#servicePhone', {
                    tips: 2,
                    time: 0
                });
            } else {
                layer.closeAll();
                self.isClick = false;
            };

          

        },
        ownerBlur: function () {
            var self = this;
            var reg = /^[1][34578]\d{9}$/;
            if (!reg.test(self.ownerPhone)) {
                layer.tips('手机号码有误', '#phoneBlur' , {
                    tips: 2,
                    time: 0
                });
            } else {
                layer.closeAll();
            };
        },

        fileChange: function () {
            var fileStr = $(".file").val();
            $(".chagePhoto-input").val(fileStr);
        },
        searchCompany: function () {
            var self = this;
            self.companyList = [];

            $.get("/Join/SearchCompany", {
                keywords: self.serviceKey,
                type: self.selectType
            }, function (res) {
              
                if (res.length!==0) {
                    self.companyList = res;
                    self.isShow = false;
                } else {
                    self.companyList = [];
                    self.isShow = true;
                }
               
            })
        },
        addCompanyBtn: function () {
            var self = this;
            if ((self.selectType == "2" && (self.serUserState == 3 || self.serUserState == 2)) || (self.selectType == "1" && (self.userState == 3 || self.userState == 2))) {
                if (self.companyId) {
                    if (self.checked) {
                        Loading();
                        $.post("/Join/JoinCompany", {
                            id: self.companyId,
                            type: self.selectType
                        }, function (res) {
                            console.log(res);
                            if (res.result) {
                                ARightMsg(res.message);
                            } else {
                                AWarnMsg(res.message)
                            }
                        })
                    } else {
                        AWarnMsg("请选择同意啄木鸟网站服务协议！")
                    }
                } else {
                    AWarnMsg("请搜索需要加入的企业！")
                }
            } else {
                AWarnMsg("企业信息已存在，不可重复加入！")
            }
        },
        createOwnerBtn: function () {
            var self = this;
            if (self.userState == 3 || self.userState == 2) {
                if (self.ownerCompany && self.ownerPosition && self.ownerContact && self.ownerPhone) {
                    if (self.checked) {
                        $.post("/Join/userCompanyRegister", {
                            Name: self.ownerCompany,
                            Position: self.ownerPosition,
                            Contact: self.ownerContact,
                            Phone: self.ownerPhone,
                            UseCompanyId: ""
                        }, function (res) {
                            if (res.result) {
                                layer.msg(res.message, { icon: 1, shade: [0.3, '#000'], time: 1500 });
                                setTimeout(function () {
                                    location.href = "/Server/OrderList?type=1"
                                }, 1500)
                            } else {
                                AWarnMsg(res.message)
                            }
                        })

                    } else {

                        AWarnMsg("请选择同意啄木鸟网站服务协议！")
                    }
                } else {
                    AWarnMsg("请填写完成信息！")
                }
            } else {
                AWarnMsg("当前状态不可以注册公司信息！")
            }
        },
        createServiceBtn: function () {
            var self = this;
            if (self.serUserState == 3 || self.serUserState == 2) {
                $('input:checkbox[name=serviceList]:checked').each(function (i, item) {
                    self.checkedNames += $(this).val() + ","
                });
                if (self.serviceCompany && self.servicePosition && self.serviceContact && self.servicePhone) {
                    if (self.checked) {
                        $("#joinUpload").ajaxSubmit({
                            url: "/Join/serverCompanyRegister",
                            type: "post",
                            data: {
                                Name: self.serviceCompany,
                                Position: self.servicePosition,
                                Contact: self.serviceContact,
                                Phone: self.servicePhone,
                                Categorys: self.checkedNames,
                                IsManufacturer: self.IsManufacturer,
                                ServiceCompanyId: self.serviceCompanyId
                            },
                            success: function (data) {
                                if (data.result) {
                                    layer.msg(data.message, { icon: 1, shade: [0.3, '#000'], time: 1500 });
                                    setTimeout(function () {
                                        location.href = "/Server/OrderList?type=2"
                                    }, 1500)

                                } else {
                                    AWarnMsg(data.message);
                                }
                            }
                        });
                    } else {
                        AWarnMsg("请选择同意啄木鸟网站服务协议！")
                    }

                } else {

                    AWarnMsg("单位名称，地址，联系人及电话为必填项！")
                }
            } else {
                AWarnMsg("当前状态不可以注册公司信息！")
            }
            
          
        },
        getJoinData() {
            var self = this;
            $.get("/Join/GetJoinInfo", {}, function (res) {
                if (res.ServiceCompany) {
                    self.IsManufacturer = res.ServiceCompany.IsManufacturer,
                        self.serviceCompany = res.ServiceCompany.Name,
                        self.servicePosition = res.ServiceCompany.Position,
                        self.serviceContact = res.ServiceCompany.Contact,
                        self.servicePhone = res.ServiceCompany.Phone,
                        self.serviceCompanyId = res.ServiceCompany.ServiceCompanyId,
                        self.checkedNames = res.ServiceCompany.Categorys,
                        $('input:checkbox[name=serviceList]').each(function (i, item) {
                            if (self.checkedNames.indexOf($(this).val()) != -1) {
                                $(this).attr("checked", true);
                            }

                        });
                };
                if (res.UseCompany) {
                    self.ownerCompany = res.UseCompany.Name,
                        self.ownerPosition = res.UseCompany.Position,
                        self.ownerContact = res.UseCompany.Contact,
                        self.ownerPhone = res.UseCompany.Phone,
                        self.useCompanyId = res.UseCompany.UseCompanyId
                }
                self.userState = res.UserState;
                self.serUserState = res.SerUserState;
             
            });
        }
    },
    created: function () {
        var self = this;
        self.getServiceList();
       //查看是否有申请过
        self.getJoinData();
       
    }
});





$(function () {


    function circleProgress(obj, value, average, num, str, colors) {
        var canvas = document.getElementById(obj);
        var context = canvas.getContext('2d');
        var _this = $(canvas),
            value = Number(value),// 当前百分比,数值
            average = Number(average),// 平均百分比
            color = "",// 进度条、文字样式
            maxpercent = 100,//最大百分比，可设置
            c_width = _this.width(),// canvas，宽度
            c_height = _this.height();// canvas,高度
        // 判断设置当前显示颜色
        if (value == maxpercent) {
            color = "#29c9ad";
        } else if (value > average) {
            color = colors;
        } else {
            color = "#ff6100";
        }
        // 清空画布
        context.clearRect(0, 0, c_width, c_height);
        // 画初始圆
        context.beginPath();
        // 将起始点移到canvas中心
        context.moveTo(c_width / 2, c_height / 2);
        // 绘制一个中心点为（c_width/2, c_height/2），半径为c_height/2，起始点0，终止点为Math.PI * 2的 整圆
        context.arc(c_width / 2, c_height / 2, c_height / 2, 0, Math.PI * 2, false);
        context.closePath();
        context.fillStyle = '#dedede'; //填充颜色
        context.fill();
        // 绘制内圆
        context.beginPath();
        context.strokeStyle = color;
        context.lineCap = 'square';
        context.closePath();
        context.fill();
        context.lineWidth = 10.0;//绘制内圆的线宽度

        function draw(cur) {
            // 画内部空白  
            context.beginPath();
            //context.moveTo(24, 24);
            context.arc(c_width / 2, c_height / 2, c_height / 2 - 10, 0, Math.PI * 2, true);
            context.closePath();
            context.fillStyle = 'rgba(245,245,245,1)';  // 填充内部颜色
            context.fill();
            // 画内圆
            context.beginPath();
            // 绘制一个中心点为（c_width/2, c_height/2），半径为c_height/2-5不与外圆重叠，
            // 起始点-(Math.PI/2)，终止点为((Math.PI*2)*cur)-Math.PI/2的 整圆cur为每一次绘制的距离
            context.arc(c_width / 2, c_height / 2, c_height / 2 - 5, -(Math.PI / 2), ((Math.PI * 2) * cur) - Math.PI / 2, false);
            context.stroke();
            //在中间写字  
            context.font = "bold 18pt Arial";  // 字体大小，样式
            context.fillStyle = "#333333";  // 颜色
            context.textAlign = 'center';  // 位置
            context.textBaseline = 'middle';
            context.moveTo(c_width / 2, c_height / 2);  // 文字填充位置
            context.fillText(num, c_width / 2, c_height / 2 - 20);
            context.fillText(str, c_width / 2, c_height / 2 + 20);
        }
        // 调用定时器实现动态效果
        var timer = null, n = 0;
        function loadCanvas(nowT) {
            timer = setInterval(function () {
                if (n > nowT) {
                    clearInterval(timer);
                } else {
                    draw(n);
                    n += 0.01;
                }
            }, 15);
        }
        loadCanvas(value / 100);
        timer = null;
    };
    circleProgress("joinService", 54, 50, 171, "服务商", "#365352");
    circleProgress("joinUser", 88, 50, 475, "用户", "#ac4e02");



    $('.createCompany input:radio[name="playRoles"]').change(function () {
        var c = $(this).val();
        if (c == 2) {
            $(".createService").css("display", "block");
            $(".createOwner").css("display", "none");
        } else {
            $(".createService").css("display", "none");
            $(".createOwner").css("display", "block");
        }

    });

    //$('.joinCompany input:radio[name="playRole"]').change(function () {
    //    var j = $(this).val();
    //    console.log(j);
    //    if (j == 1) {
    //        $(".joinService").css("display", "block");
    //        $(".joinOwner").css("display", "none");
    //    } else {
    //        $(".joinService").css("display", "none");
    //        $(".joinOwner").css("display", "block");
    //    }

    //});

    $(".joinTab").click(function () {
        $(this).addClass("joinTab-active").siblings(".joinTab").removeClass("joinTab-active");
        var index = $(this).index();
        if (index == 1) {
            $(".createCompany").show();
            $(".joinCompany").hide();
        } else {
            $(".createCompany").hide();
            $(".joinCompany").show();
        }
    });



})

