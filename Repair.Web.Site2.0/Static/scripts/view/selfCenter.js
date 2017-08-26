var center=new Vue({
    el: ".content",
    data: {
        btnCodeText: '获取验证码',
        btnCodeDisabled: false,
        password: "",
        newPwd: "",
        reNewPwd: "",
        code: "",
        imgUrl: "",
        itemsYear: [],
        itemsMonth: [],
        itemsDay: [],
        selected1: "",
        selected2: "",
        selected3: "", 
        selectedAll:"",
        dataArr: {},
        photo: "",
        checkedSex: ""
    },
    methods: {
        //更改密码
        pwdSubmit: function () {
            var self = this;
            if (self.password && self.newPwd && self.reNewPwd) {
                $.post("/User/ResetPwd", { oldPwd: self.password, pwd: self.newPwd }, function (data) {
                    console.log(data);
                    if (data.error) {
                        AWarnMsg(data.error);
                    } else {
                        ARightMsg("密码修改成功");
                        self.password = "";
                        self.newPwd = "";
                        self.reNewPwd = "";
                    }
                })
            } else {
                AWarnMsg("请填写完整信息！");
            }

        },
        //判断日
        chooseDay: function () {
            this.itemsDay = [];
            if (((this.selected1 % 400 == 0) || (this.selected1 % 100 != 0 && this.selected1 % 4 == 0)) && this.selected2 == 2) {
             
                for (var i = 1; i <= 29; i++) {
                    var days = { day: i < 10 ? "0" + i : i };
                    this.itemsDay.push(days)
                }
            } else if (((this.selected1 % 400 != 0) || (this.selected1 % 100 != 0 && this.selected1 % 4 != 0)) && this.selected2 == 2) {
              
                for (var i = 1; i <= 28; i++) {
                    var days = { day: i < 10 ? "0" + i : i };
                    this.itemsDay.push(days)
                }
            } else if (this.selected2 == 1 || this.selected2 == 3 || this.selected2 == 5 || this.selected2 == 7 || this.selected2 == 8 || this.selected2 == 10 || this.selected2 == 12) {
              
                for (var i = 1; i <= 31; i++) {
                    var days = { day: i < 10 ? "0" + i : i };
                    this.itemsDay.push(days)
                }
            } else {
               
                for (var i = 1; i <= 30; i++) {
                    var days = { day: i < 10 ? "0" + i : i };
                    this.itemsDay.push(days)
                }
            }
        },
        //选择年时判断日
        chooseYear: function () {
            this.chooseDay()
        },
        chooseMonth: function () {
            this.chooseDay()
        },
        //个人资料提交
        infoSubmit: function () {
            $.post("/User/Edit",
                {
                    Address: this.dataArr.address,
                    BirthDate: this.selected1 + "-" + this.selected2 + "-" + this.selected3,
                    RealName: this.dataArr.realname,
                    NickName: this.dataArr.nickname,
                    Phone: this.dataArr.phone,
                    Email: this.dataArr.email,
                    UserId: this.dataArr.userid,
                    Sex: this.checkedSex
                }, function (res) {
                    console.log(res);
                    if (res.error) {
                        AWarnMsg(res.error);
                        
                    } else {
                        ARightMsg(res.success);
                    }
                })
        },
        init: function () {
            //获取个人数据
            var self = this;
            $.get("/user/get", {},
                function (data) {
                    console.log(data);
                    if (data.error) {
                        AWarnMsg(data.error);
                    } else {
                        
                        self.dataArr = data;
                        self.imgUrl = self.dataArr.imgurl;
                        self.checkedSex = data.sex == null ? "男" : data.sex;
                        if (self.imgUrl)
                        {
                            var prevDiv = document.getElementById('preview');
                            prevDiv.innerHTML = '<img src="' + self.imgUrl+ '"/>';
                        }
                        if (self.dataArr.birthdate) {
                            var attr = self.dataArr.birthdate.split('-');
                            self.selected1 = attr[0]
                            self.selected2 = attr[1]
                            self.selected3 = attr[2]
                        }
                    }
                });
        }
    },
    created: function () {
       
        this.init();
        //年
        for (var i = 1900; i <= 2017; i++) {
            var years = { year: i };
            this.itemsYear.push(years)
        }
        //月
        for (var j = 1; j <= 12; j++) {
            var months = { month: j<10?"0"+j:j };
            this.itemsMonth.push(months)
        }
        //日
        this.chooseDay();
    }
})

//点击切换
//$(function () {
    var hash = location.hash;
    if (hash) {
        tab(hash.match(/\d+/)[0]);
    } else {
        $(".center-change").eq(0).css("display", "block");
        $(".center-changeTitle").eq(0).addClass("highLight");
        $(".center-changeTitle:first .bottom-text").css("color", "#fba02a");
    }
    $('.center-changeTitle').click(function () {
        tab($(this).index());
    });
    function tab(index) {
        $('.center-change').siblings('.center-change').hide().end().eq(index).show();
        $(".center-changeTitle .bottom-text").eq(index).css("color", "#fba02a").parent().siblings().children(".center-changeTitle .bottom-text").css("color", "#333");
        $(".center-changeTitle").eq(index).addClass("highLight").siblings().removeClass("highLight");
    }
$(function () {
    //上传图片   
    $(".a-upload").change(function () {
       
        var file = document.getElementsByClassName("file")[0];
        var prevDiv = document.getElementById('preview');
       

        if (file.files && file.files[0]) {
            var reader = new FileReader();
            reader.onload = function (evt) {
                prevDiv.innerHTML = '<img src="' + evt.target.result + '"/>';
            }
            reader.readAsDataURL(file.files[0]);
        } else{
            prevDiv.innerHTML = '<div class="img" style="filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=scale,src=\'' + file.value + '\'"></div>';
        }
        $(".chagePhoto-input").val($(".file").val())
        $(".changePhoto-img").attr("src", $(".file").val());

    })
    //判断邮箱格式
    $("#Email").keyup(function () {
        var pattern = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
        if (pattern.test(($("#Email").val()))) {
            $("#Email").next(".warning").html("正确");
            $("#Email").next(".warning").addClass("sure");
        } else {
            $("#Email").next(".warning").html("邮箱格式不正确");
            $("#Email").next(".warning").removeClass("sure");
        }
        if ($("#Email").val() == "") {
            $("#Email").next(".warning").html("");
        }
    })
    //密码字符长度判断
    $('#newPwd').keyup(function () {
        if ($("#newPwd").val() == "") {
            $("#pwdWarning").html("密码不能为空");
            return false;
        } else if ($("#newPwd").val().length >= 6 && $("#newPwd").val().length <= 18) {
            $("#newPwd").next(".warning").html("正确");
            $("#newPwd").next(".warning").addClass("sure");
        } else if ($("#newPwd").val().length < 6 || $("#newPwd").val().length > 18) {
            $("#newPwd").next(".warning").html("请输入6-18个字符");
            $("#newPwd").next(".warning").removeClass("sure");
            return false;
        }
    });
    //前后密码是否一致
    $('#reNewPwd').keyup(function () {
        if ($("#newPwd").val() == $("#reNewPwd").val()) {
            $("#reNewPwd").next(".warning").html("正确");
            $("#reNewPwd").next(".warning").addClass("sure");
        } else {
            $("#reNewPwd").next(".warning").html("前后密码输入不一致");
            $("#reNewPwd").next(".warning").removeClass("sure");
        }
    });
})

function subimtBtn() {
    var options = {
        url: '/user/UploadImage',
        type: 'post',
        success: function (data) {
            if (data.success == 0) {
                ARightMsg("头像上传成功");        
            } else {
                AWarnMsg("头像上传失败");
            }
        }
    };
    $("#userUpload").ajaxSubmit(options);
}
window.onload = function () {
    var tabid = location.href.hash;
    if (tabid && tabid.length > 0) {
        var tab = document.querySelector(tabid);
        tab.onclick();
    }
}