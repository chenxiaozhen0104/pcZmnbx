Vue.component('btn-verify-code', {
    props: ['mobile'],
    template: ' <a v-bind:class="{disabled:btnSMSdisabled}" v-on:click="getSMSCode" class="btn-sms-code">{{ SmsCodeText }}</a>',
    data: function () {
        return {
            btnSMSdisabled: false,
            SmsCodeText: '获取验证码'
        }
    },
    methods: {
        getSMSCode: function () {
            var timeOut = 60
            var t = this;
            if (t.btnSMSdisabled) {
                return;
            }
            t.btnSMSdisabled = true
            t.SmsCodeText = timeOut + "s 后重试"
            var cid = setInterval(function () {
                timeOut--;
                t.SmsCodeText = timeOut + "s 后重试"
                if (timeOut == 0) {
                    clearInterval(cid)
                    t.SmsCodeText = "获取验证码"
                    t.btnSMSdisabled = false
                }
            }, 1000)
            $.get('/home/SmSVeriCode',
                { mobile: this.mobile },
                function (res) {
                    if (res.error) {
                        t.SmsCodeText = "获取验证码"
                        t.btnSMSdisabled = false
                        clearInterval(cid)
                        layer.alert("发送失败,请检测手机号是否有误", { icon: 0 })
                    }
                })
            this.$emit('send-sms-code', this.user)
        }
    }
})