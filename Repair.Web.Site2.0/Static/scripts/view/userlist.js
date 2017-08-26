

var userdata = new Vue({
    el: '#userdata',
    data: {
        list: {},
        pageindex: 1,
        pagesize: 10,
        pagetotal: 0,
        searchkey: '',
    },
    methods: {
        find: function (obj) {
            var self = this;
            if (!parseInt(obj)) {
                if (obj == 'pre') {
                    if (self.pageindex == 1)
                        return;
                    else
                        self.pageindex--;
                } else if (obj == 'next') {
                    if (self.pageindex == self.pagetotal)
                        return;
                    else
                        self.pageindex++;
                }
            }
            else {
                self.pageindex = obj;
            }
          
        },
        init: function () {
            LoadInfo("查询中");
            var self = this;
            $.post("/Server/UserListData/", { search: self.searchkey, pageIndex: self.pageindex, pageSize: self.pagesize }, function (res) {
                console.log(res);
                if (res) {
                    self.list = res.list;
                    self.pagetotal = Math.ceil(res.pageTotal / self.pagesize);               
                   
                    layer.closeAll();
                } else {
                    self.list = [];
                    self.pagetotal = 0;
                    layer.closeAll();
                }
            });
        },
        doSearch: function () {
            this.init();
        }

    },
    created: function () {
        this.init();
     
    }
});
$(function () {
    $(".publistBottom-lists a").each(function (i, item) {
        //console.log(window.location.pathname);
        //console.log(item.pathname)
        var regExp = new RegExp(item.pathname);
        if (regExp.test(location.pathname)) {
            $(item).parent().addClass('publicLists-active');
            $(item).addClass("color-active");
        }
    })
})