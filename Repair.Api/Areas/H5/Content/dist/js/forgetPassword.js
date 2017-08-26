var login=new Vue({el:"#content",data:{userName:"",password:"",passwordConfirm:"",mobile:"",smsCode:""},methods:{submit:function(){$.post("/api/user/forgetPassword",{mobile:this.mobile,password:this.password,smsCode:this.smsCode},function(o){o.error?layer.msg(o.error,{icon:0,time:3e3}):location.href="login.html?type="+localStorage.appType})}}});
//# sourceMappingURL=forgetPassword.js.map
