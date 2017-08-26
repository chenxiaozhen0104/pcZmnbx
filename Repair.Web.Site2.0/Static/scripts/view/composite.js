
$(".publistBottom-lists a").each(function (i, item) {
    //console.log(window.location.pathname);
    //console.log(item.pathname)
    var regExp = new RegExp(item.pathname);
    if (regExp.test(location.pathname)) {
        $(item).parent().addClass('publicLists-active');
        $(item).addClass("color-active");
    }
})






var com = new Vue({
    el: '#statistics-time',
    data: {
        userNameArr: [],
        orderIng: [],
        ordered: [],
        orderUnComplated: [],
        timeArr: []
    },
    methods: {
        init: function (beginTime, endTime) {
             LoadInfo("查询中...");
             $(".serviceForm>h3").html(substr(beginTime, endTime) + "人员工单情况");
             var self = this;
            ; $.post("/Server/CompositeData", { appType: 1, beginTime: beginTime, endTime: endTime }, function (res) {
                if (res) {
                    self.userNameArr = res.userNmaeArr;
                    self.orderIng = res.orderIng;
                    self.ordered = res.ordered;
                    self.orderUnComplated = res.orderUnComplated;
                    self.timeArr = res.timeArr;
                }
                layer.closeAll();
            })
        },
        getworktime: function (v) {
            for (var i in this.timeArr) {
                if (this.timeArr[i].Key == v) {
                    return this.timeArr[i].Value.toFixed(2);
                }
            }
            return 0
        },
        getorderIng: function (v) {
            var t = 0;
            for (var i in this.orderIng) {
                if (this.orderIng[i].Key == v) {
                    t += this.orderIng[i].Value;
                }
            }
            return t;
        },
        getordered: function (v) {
            var t = 0;
            for (var i in this.ordered) {
                if (this.ordered[i].Key == v) {
                    t += this.ordered[i].Value;
                }
            }
            return t;
        },
        getUnComplated: function (v) {
            var t = 0;
            for (var i in this.UnComplated) {
                if (this.UnComplated[i].Key == v) {
                    t += this.UnComplated[i].Value;
                }
            }
            return t;
        }

    },
    created: function () {

    }
});


var datas = selectTime.setTimeType(selectTime.Type.Day);

function substr(start, end) {
    start = start.split(" ")[0]
    end = end.split(" ")[0]
    if (start == end) {
        return start
    } else {
        return start + " 至 " + end;
    }
}


com.init(datas[0], datas[1]);
$(".serviceForm>h3").html(substr(datas[0], datas[1]) + "人员工单情况");
    
function serviceTimes(that, type) {
    $(that).addClass("statistics-selectActive").siblings().removeClass("statistics-selectActive");
    datas = selectTime.setTimeType(type);
    com.init(datas[0], datas[1]);
};

function servicePrevious() {
    datas = selectTime.Previous();
    com.init(datas[0], datas[1]);
}
function serviceNext() {
    datas = selectTime.Next();
    com.init(datas[0], datas[1]);
}


