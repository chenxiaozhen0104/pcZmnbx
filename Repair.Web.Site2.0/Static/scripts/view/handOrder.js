$(".publistBottom-lists a").each(function (i, item) {
    //console.log(window.location.pathname);
    //console.log(item.pathname)
    var regExp = new RegExp(item.pathname);
    if (regExp.test(location.pathname)) {
        $(item).parent().addClass('publicLists-active');
        $(item).addClass("color-active");
    }
});


var handOrder = new Vue({
    el: "#handOrder",
    data: {
        categorys: [],
        categoryId: 0,
        brands: {},
        brandId: 0,
        level: 1,
        type: 1,
        orderName: "",
        useCompanyName: "",
        contacts: "",
        contactsPhone: "",
        address: "",
        orderNote: ""
    },
    methods: {
        init: function () {
            var self = this;
            $.get("/Server/HandOrderInfo", {}, function (res) {
                console.log(res);
                self.categorys = res.categorys;
                self.categorys.unshift({ CategoryId: '0', Name: "请选择类目" })
                self.brands = res.brands;
                self.brands.unshift({ BrandId: '0', Name: "请选择品牌" })
            })
        },
        handOrderBtn: function () {
            //开始验证
            var self = this;
    
            if (self.categoryId != 0 && self.brandId != 0) {
                if ( !(self.orderName && self.useCompanyName && self.contacts && self.contactsPhone && self.address && self.orderNote)) {
                    layer.msg('请填写完成信息', { icon: 0, shade: [0.3, '#000'], time: 2000 });

                } else {
                    $("#userUpload").ajaxSubmit({
                        url: "/Server/HandOrder",
                        type: "post",
                        data: {
                            Name: self.orderName,
                            UseCompanyName: self.useCompanyName,
                            Contacts: self.contacts,
                            ContactsPhone: self.contactsPhone,
                            Address: self.address,
                            Describe: self.orderNote,
                            CategoryId: self.categoryId,
                            BrandId: self.brandId,
                            Type: self.type,
                            Level: self.level,
                            Model:''
                        },
                        success: function (data) {
                            if (data.warning) {
                                layer.msg(data.message, { icon: 0, shade: [0.3, '#000'], time: 2000 });
                            } else if (data.result){
                                layer.msg(data.message, { icon: 1, shade: [0.3, '#000'], time: 2000 });
                                setTimeout(function () {
                                    location.reload();
                                },1000)

                            } else{
                                layer.msg(data.message, { icon: 2, shade: [0.3, '#000'], time: 2000 })
                            }
                        }
                    });
                }
            } else {
                layer.msg('请选择类目和品牌', { icon: 0, shade: [0.3, '#000'], time: 2000 });
            }

        }
    },
    created: function () {
        this.init();
    }

})


