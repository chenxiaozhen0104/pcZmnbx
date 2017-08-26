Vue.component("btn-verify-code",{props:["mobile"],template:' <a v-bind:class="{disabled:btnSMSdisabled}" v-on:click="getSMSCode" class="btn-sms-code">{{ SmsCodeText }}</a>',data:function(){return{btnSMSdisabled:!1,SmsCodeText:"获取验证码"}},methods:{getSMSCode:function(){var e=60,t=this;if(!t.btnSMSdisabled){t.btnSMSdisabled=!0,t.SmsCodeText=e+"s 后重试";var s=setInterval(function(){e--,t.SmsCodeText=e+"s 后重试",0==e&&(clearInterval(s),t.SmsCodeText="获取验证码",t.btnSMSdisabled=!1)},1e3);$.get("/api/common/sendverifycode",{mobile:this.mobile},function(e){e.error&&(t.SmsCodeText="获取验证码",t.btnSMSdisabled=!1,clearInterval(s),layer.alert("发送失败,请检测手机号是否有误",{icon:0}))}),this.$emit("send-sms-code",this.user)}}}});
//# sourceMappingURL=btn-verify-code.js.map