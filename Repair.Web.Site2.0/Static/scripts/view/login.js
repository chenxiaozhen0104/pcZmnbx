
new Vue({
    el: '#login-content',
    data: {
        pwdPhone: "",
        pwdPassword: "",

        yzmPhone: "",
        yzmCode: "",

        fgtPhone: "",
        fgtCode: "",
        fgtPassword: "",
        fgtNewPassword: "",
        checked: false,

        userName: "",
        regPhone: "",
        regPassword: "",
        regAgainPassword: "",
        regCode: "",
        regChecked: true

    },
    methods: {
        pwdLogin: function () {
            var self = this;
            if (!self.pwdPhone || !self.pwdPassword) {
                AWarnMsg("手机号或密码不能为空");
            } else {

                //if (self.checked) {
                //    localStorage.setItem("Phone", self.pwdPhone);
                //    localStorage.setItem("Password", self.pwdPassword);
                //}
                Loading();
                $.post("/home/PwdLogin", { phone: self.pwdPhone, password: self.pwdPassword }, function (res) {
                    if (res.success) {
                        ARightMsg(res.success);
                        setTimeout(function () {
                            window.open('/User/Center', '_self');
                        }, 500)
                    } else {
                        AWarnMsg(res.error);
                    }
                })

            }
        },
        yzmLogin: function () {
            var self = this;
            if (!self.yzmPhone || !self.yzmCode) {
                AWarnMsg("手机号或验证码不能为空");
            } else {
                Loading();
                $.post("/home/SmsLogin", { phone: self.yzmPhone, smsCode: self.yzmCode }, function (res) {
                    if (res.success) {
                        ARightMsg(res.success);
                        setTimeout(function () {
                            window.open('/User/Center', '_self');
                        }, 500)
                    } else {
                        AWarnMsg(res.error);
                    }
                })
            }
        },
        fgtLogin: function () {
            var self = this;
            Loading();
            if (self.fgtPhone && self.fgtCode && self.fgtPassword && self.fgtNewPassword) {
                if (self.fgtPassword.length < 6) return;
                if (self.fgtPassword == self.fgtNewPassword) {
                    $.post("/home/ResetPassword", { phone: self.fgtPhone, smsCode: self.fgtCode, password: self.fgtPassword }, function (res) {
                        if (res.result) {
                            ARightMsg(res.message);
                        } else {
                            AWarnMsg(res.message);
                        }

                    })

                } else {
                    AWarnMsg('两次输入密码不一致');
                }
            } else {
                AWarnMsg('请填写完整信息');
            }
        },
        regAlter: function () {
            var self = this;
            var reg = /^[1][34578]\d{9}$/;
            if (reg.test(self.regPhone)) {
                layer.closeAll();
            } else {
                layer.tips('手机号码有误', '#phoneBlur', {
                    tips: 2,
                    time: 0
                });
            }
        },
        homeAlter: function (id) {
            var self = this;
            if (id == "passwordBlur") {
                if (self.regPassword.length < 6) {
                    layer.tips('密码长度不能小于6位数', '#passwordBlur', {
                        tips: 2,
                        time: 0
                    });
                } else {
                    layer.closeAll();
                };
            }
            if (id == 'fgtpassword') {
                if (self.fgtPassword.length < 6) {
                    layer.tips('密码长度不能小于6位数', '#fgtpassword', {
                        tips: 2,
                        time: 0
                    });
                } else {
                    layer.closeAll();
                };
            }
           
        },
        register: function () {
            var self = this;
            Loading();
            console.log(self.regChecked);
       
            if (self.regPhone && self.regPassword && self.regAgainPassword && self.regCode) {
                if (self.regPassword != self.regAgainPassword) {
                    AWarnMsg('两次输入密码不一致')
                } else {
                    if (self.regChecked) {
                        $.post("/Home/Register", { username: self.userName, phone: self.regPhone, smsCode: self.regCode, password: self.regPassword }, function (res) {
                            if (res.success) {
                                ARightMsg(res.success);
                                setTimeout(function () {
                                    window.open('/User/Center', '_self');
                                }, 500)
                            } else {
                                AWarnMsg(res.error);
                            }
                        })
                    } else {
                        layer.closeAll();
                        AWarnMsg("勾选同意并阅读");
                    }
                }
            } else {
                AWarnMsg('请填写完整信息');

            }
        }
    },
    created: function () {
        //var self = this;
        //self.pwdPhone = localStorage.getItem("Phone");
        //self.pwdPassword = localStorage.getItem("Password");

        $(function () {

            $(".login-liCommon").click(function () {
                var index = $(this).index();
                $(this).addClass("login-active").siblings("li").removeClass("login-active");
                $(".login-centerInput").eq(index).css("display", "block").siblings(".login-centerInput").css("display", "none");
            });
            $(".longin-focus").focus(function () {
                $(this).siblings("span").addClass("login-inconActice");
                $(this).siblings("span").children().css("color", "white");
            });
            $(".login-forgetPassword").click(function () {
                $(".login-centerForms").css("display", "block");
                $(".login-centerForm").css("display", "none");
            });

            $(".longin-focus").focus(function () {
                $(this).siblings("span").addClass("login-inconActice");
                $(this).siblings("span").children().css("color", "white");
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

