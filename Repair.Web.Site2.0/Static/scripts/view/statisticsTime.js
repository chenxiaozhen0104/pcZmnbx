
    $(".publistBottom-lists a").each(function (i, item) {
        //console.log(window.location.pathname);
        //console.log(item.pathname)
        var regExp = new RegExp(item.pathname);
        if (regExp.test(location.pathname)) {
            $(item).parent().addClass('publicLists-active');
            $(item).addClass("color-active");
        }
    });
   
  
    var myChart = echarts.init(document.getElementById('timeEcharts'));

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
        $.post('/Server/WorkTime?apptype=2&beginTime=' + startTime + '&endTime=' + endTime, {}, function (res) {
            var lens = res.seriesdata[0].data.length;
            for (var i = 0; i < lens; i++) {
                res.seriesdata[0].data[i] = (res.seriesdata[0].data[i] / 60).toFixed(2);
            }
            serviceOption.title.text = "人员工时统计 单位(小时)";
            serviceOption.title.subtext = substr(startTime, endTime);
            serviceOption.xAxis.data = res.Axis;
            serviceOption.title.left = "center";
            serviceOption.title.itemGap = -5;
            serviceOption.series[0].data = res.seriesdata[0].data;
            myChart.setOption(serviceOption);
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
   
