var login=new Vue({el:"#content",data:{name:"",address:"",userName:"",phone:""},methods:{register:function(){$.post("/api/user/UseCompanyRegister",{name:this.name,position:this.address,contact:this.userName,phone:this.phone},function(e){e.error?layer.alert(e.error,{icon:0}):layer.alert("申请已提交，请耐心等待审核",{icon:0})})}}});
//# sourceMappingURL=register.company.js.map
