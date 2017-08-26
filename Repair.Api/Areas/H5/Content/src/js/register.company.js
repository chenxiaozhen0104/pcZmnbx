var login = new Vue({
    el: '#content',
    data: {
        name: '',
        address: '',
        userName: '',
        phone: '',
    },
    methods: {
        register: function () {
            $.post('/api/user/UseCompanyRegister', {
                name: this.name,
                position: this.address,
                contact: this.userName,
                phone: this.phone
            }, function (res) {
                if (res.error) {
                    layer.alert(res.error,{icon:0})
                } else {
                   layer.alert("申请已提交，请耐心等待审核",{icon:0})
                }
            })
        }
    }
})