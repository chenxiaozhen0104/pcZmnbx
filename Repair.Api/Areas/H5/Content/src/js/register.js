var login = new Vue({
    el: '#content',
    data: {
        password: '',
        passwordConfirm: '',
        mobile: '',
        logo: localStorage.appType == '1' ? '../images/login_logo.png' : '../images/login_logoservice.png',
        smsCode: '',
        RealName: ''
    },
    methods: {
        register: function () {
            if (!this.RealName) {
                layer.msg("请输入用户名", { icon: 0 })
                return;
            }
            if (this.password != this.passwordConfirm) {
                layer.msg("密码前后输入输入不一致", { icon: 0 })
                return;
            }
            $.post('/api/user/register', {
                password: this.password,
                mobile: this.mobile,
                smsCode: this.smsCode,
                realName: this.RealName
            }, function (res) {              
                if (res.error) {
                    layer.alert(res.error, { icon: 0 })
                } else {
                    layer.msg("注册成功", { icon: 4, time: 2000 })
                    setTimeout(function () {
                        location.href = "CompanyType.html?appType=" + localStorage.appType;
                        //location.href = "login.html?appType=" + localStorage.appType;
                    }, 2000)
                }
            })
        }
    }
})