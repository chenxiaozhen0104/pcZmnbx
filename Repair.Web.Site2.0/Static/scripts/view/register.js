
new Vue({
    el: '#register-content',
    data: {
        userName: "",
        regPhone: "",
        regPassword: "",
        regAgainPassword: "",
        regCode: "",
        regChecked:false
    },
    methods: {
        register: function () {
            var self = this;
            if (!self.regPhone || !self.regPassword || !self.regAgainPassword) {
                AWarnMsg('请填写完整信息');
            } else {
                if (self.regChecked) {
                    $.post("/home/Register", { username: self.userName, phone: self.regPhone, smsCode: self.regCode, password: self.regPassword }, function (res) {
                        if (res.success) {
                            ASuccess(res.success);
                        } else {
                            AWarnMsg(res.error);
                        }
                    })

                } else {
                    AWarnMsg("勾选同意并阅读");
                }
            }
        }
    },
    created: function () {
        $(function () {
            var index = 0;
            $(".login-topConten>a").click(function () {
                var indexs = $(this).index();
                if (indexs != 2) {
                    Redirect('/home/Login')
                }
            })

            //$(".login-liCommon").click(function () {
            //    index = $(this).index();
            //    $(this).addClass("login-active").siblings("li").removeClass("login-active");
            //    $(".login-centerInput").eq(index).css("display", "block").siblings(".login-centerInput").css("display", "none");
            //});

            $(".longin-focus").focus(function () {
                $(this).siblings("span").addClass("login-inconActice");
                $(this).siblings("span").children().css("color", "white");
            });
           
            $('#phoneBlur').blur(function () {
                var regText = $("#phoneBlur");
                var reg = /^[1][34578]\d{9}$/;
                if (regText.val() == "") {
                    layer.tips('请输入手机号码', '#phoneBlur', {
                        tips: 2,
                        time: 0
                    });
                } else {
                    if (!reg.test(regText.val())) {
                        layer.tips('手机号码有误', '#phoneBlur', {
                            tips: 2,
                            time: 0
                        });
                    } else {
                        layer.closeAll();
                    }
                }
            })

            $("#passwordBlur").blur(function () {
                var regTexts = $("#passwordBlur");
                if (regTexts.val() == "") {
                    layer.tips('请输入密码', '#passwordBlur', {
                        tips: 2,
                        time: 0
                    });
                } else {
                    if (regTexts.val().length > 16 || regTexts.val().length < 8) {
                        layer.tips('请输入8-16位登录密码', '#passwordBlur', {
                            tips: 2,
                            time: 0
                        });
                    } else {
                        layer.closeAll();
                    }
                }

            })

            $("#passwordBlurs").blur(function () {
                var regTexts = $("#passwordBlur");
                var regTextss = $("#passwordBlurs");
                if (regTexts.val() != regTextss.val()) {
                    layer.tips('密码不一致！', '#passwordBlurs', {
                        tips: 2,
                        time: 0
                    });
                } else {
                    layer.closeAll();
                }
                
            })

            $("#userBlur").blur(function () {
                var userName = $("#userBlur");
                if (userName.val() == "") {
                    layer.tips('请输入用户真实名！', '#userBlur', {
                        tips: 2,
                        time: 0
                    });
                } else {
                    layer.closeAll();
                }

            })

            $("#regCode").click(function () {
                var self = this;
                var time = 60;
                var yzmText = $("#phoneBlur").val();
                if (yzmText) {
                    $(self).attr('value', time + 's后重试');
                    $(self).attr("disabled", true);
                    $.post("/home/SmSVeriCode", { phone: yzmText }, function (res) {
                        if (res.success) {
                            ASuccess(res.success);
                        } else {
                            AWarnMsg(res.error);
                        }
                    });
                    var timer = setInterval(function () {
                        time--;
                        $(self).attr('value', time + 's后重试');
                        if (time == 0) {
                            clearInterval(timer);
                            $(self).attr("disabled", false)
                            $(self).attr('value', '获取验证码');
                            return;
                        };
                    }, 1000);

                } else {
                    AWarnMsg('请输入手机号码');
                }
            });

            $(".register-agreen").click(function () {
                $(".register-agreement").css("display", "block");
            })
          
            $(".icon-shanchu").click(function () {
                $(".register-agreement").css("display", "none");
            })

            $(".agreement-submit").click(function () {
                $(".register-agreement").css("display", "none");
                $(".register-checkbox2").attr("checked", 'true');
            })

        })

    }
})


