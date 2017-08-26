var login = new Vue({
    el: '#content',
    data: {
        userName: '',
        password: '',
        passwordConfirm: '',
        mobile: '',
        smsCode: ''
    },
    methods: {
        submit: function () {
            $.post('/api/user/forgetPassword', {
                mobile: this.mobile,
                password: this.password,
                smsCode: this.smsCode
            }, function (res) {
                if (res.error) {
                    layer.msg(res.error, { icon: 0, time: 3000 })
                } else {
                    location.href = "login.html?type=" + localStorage.appType;
                }
            })
        }
    }
})