
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
    $.post('/Server/AreaCount?apptype=1&beginTime=' + startTime + '&endTime=' + endTime, {}, function (res) {
        console.log(res)
        barOption.title.text = "区域统计报表 单位(个)";
        barOption.title.subtext = substr(startTime, endTime);
        barOption.title.left = "center";
        barOption.title.itemGap = -5;
        barOption.xAxis.data = res.yAxis;
        barOption.series[0].data = res.seriesData
        myChart.setOption(barOption);
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

