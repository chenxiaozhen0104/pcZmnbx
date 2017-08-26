$(".publistBottom-lists a").each(function (i, item) {
        //console.log(window.location.pathname);
        console.log(item.pathname+"bb")
        var regExp = new RegExp(item.pathname);
        if (regExp.test(location.pathname)) {
            $(item).parent().addClass('publicLists-active');
            $(item).addClass("color-active");
        }
    })

    var myChart = echarts.init(document.getElementById('orderEcharts'));

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

    function init(startTime, endTime) {
        $.post('/Server/RepartUserCount?apptype=2&beginTime=' + startTime + '&endTime=' + endTime, {}, function (res) {
            console.log(res);
            res.result[0].label.normal.position = "insideTopLeft"
            res.result[0].label.normal.offset = [0, 10]
            res.result[0].label.normal.formatter = function (a) { return a.value > 0 ? a.value : "" };

            res.result[1].label.normal.position = "insideTop"
            res.result[1].label.normal.offset = [0, 10]
            res.result[1].label.normal.formatter = function (a) { return a.value > 0 ? a.value : "" };

            res.result[2].label.normal.position = "insideTopRight"
            res.result[2].label.normal.offset = [0, 10]
            res.result[2].label.normal.formatter = function (a) { return a.value > 0 ? a.value : "" };


            ordersOption.title.text = "工单数量统计 单位(个)";
            ordersOption.title.subtext = substr(startTime, endTime);
            ordersOption.title.itemGap = -5;
            ordersOption.legend.data = res.legend;
            ordersOption.xAxis.data = res.userNmaeArr;
            ordersOption.series = res.result;
            myChart.setOption(ordersOption);
        });
    }


    function serviceTimes(that, type) {
        $(that).addClass("statistics-selectActive").siblings().removeClass("statistics-selectActive");
        datas = selectTime.setTimeType(type);
        init(datas[0], datas[1])
    };

    function servicePrevious() {
        datas = selectTime.Previous();
        init(datas[0], datas[1])
    }
    function serviceNext() {
        datas = selectTime.Next();
        init(datas[0], datas[1])

    }


    init(datas[0], datas[1]);