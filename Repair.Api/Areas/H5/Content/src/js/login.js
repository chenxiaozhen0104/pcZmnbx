function cacheUser(res) {
    localStorage.userId = res.id + '';
    localStorage.userName = res.name;
    localStorage.userType = res.type;
    localStorage.userPhone = res.phone;
}
//localStorage.clear();

localStorage.appType = (function () {

    var reg = new RegExp("(^|&)appType=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);

    var type = r != null ? unescape(r[2]) : undefined;

    return type;
})();

var login = new Vue({
    el: '#content',
    data: {
        passwordLogin: false,
        userName: '',
        password: '',
        logo: localStorage.appType == 1 ? '../images/login_logo.png' : '../images/login_logoservice.png',
        mobile: '',
        smsCode: ''
    },
    methods: {
        login: function () {
            var index = layer.msg("登录中...", { icon: 16, time: 0, shadeClose: false });
            if (this.passwordLogin) {
                $.post('/api/user/PasswordLogin', { userName: this.userName, password: this.password }, function (res) {
                    if (res.error) {
                        layer.msg(res.error, { icon: 2, time: 2000 })
                        layer.close(index)
                    } else {
                        cacheUser(res);
                        login.userInfo();
                        //if (localStorage.appType == 2) {//用户如果是服务单位管理员或普通人员，跳转到订单列表
                        //    //location.href = "orderlist.html?target=_blank_closecurrent&appType=" + localStorage.appType
                        //} else if (localStorage.appType == 1) {
                        //    location.href = "CompanyType.html?target=_blank_closecurrent&appType=" + localStorage.appType
                        //    //location.href = "repair.html?target=_blank_closecurrent&appType=" + localStorage.appType
                        //}
                    }
                })
            } else {
                $.post('/api/user/SMSLogin', { smsCode: this.smsCode, mobile: this.mobile }, function (res) {
                    if (res.error) {
                        layer.msg(res.error, { icon: 2, time: 2000 })
                        layer.close(index)
                    } else {
                        cacheUser(res)
                        login.userInfo();
                    }
                })
            }
        },

        userInfo: function () {
            $.get("/api/user/Info", function (res) {
                if (localStorage.appType == 1) {
                    if (res.useCompany)
                        location.href = "repair.html?target=_blank_closecurrent&appType=" + localStorage.appType
                    else {
                        location.href = "CompanyType.html?target=_blank_closecurrent&appType=" + localStorage.appType
                    }
                } else {
                    if (res.serviceCompany)
                        location.href = "orderlist.html?target=_blank_closecurrent&appType=" + localStorage.appType
                    else
                        location.href = "CompanyType.html?target=_blank_closecurrent&appType=" + localStorage.appType
                }
            });
        }
    }
})