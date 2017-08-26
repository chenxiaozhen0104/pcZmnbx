
$(".publistBottom-lists a").each(function (i, item) {
    //console.log(window.location.pathname);
    //console.log(item.pathname)
    var regExp = new RegExp(item.pathname);
    if (regExp.test(location.pathname)) {
        $(item).parent().addClass('publicLists-active');
        $(item).addClass("color-active");
    }
})

var myChart = echarts.init(document.getElementById('orderCountEcharts'));

var weekDatas = selectTime.setTimeType(selectTime.Type.Week);
var timeType = 2;

function substr(start, end) {
    start = start.split(" ")[0]
    end = end.split(" ")[0]
    if (start == end) {
        return start
    } else {
        return start + " 至 " + end;
    }
}

function init(startTime, endTime,type) {
    $.post('/Server/ServerRepairTime?apptype=2&beginTime=' + startTime + '&endTime=' + endTime+ '&Type=' + type, {}, function (res) {
        console.log(res);
        barWithPieoption.yAxis[0].data = res.Axis;
        barWithPieoption.series[0].data = res.data;
        barWithPieoption
        barWithPieoption.title[0].subtext = substr(startTime, endTime);
        barWithPieoption.title[1].subtext = substr(startTime, endTime);
        barWithPieoption.series[1].data = res.pieArry;
        myChart.setOption(barWithPieoption);
    });
}


function serviceTimes(that, type) {
    timeType = type;
    $(that).addClass("statistics-selectActive").siblings().removeClass("statistics-selectActive");
    weekDatas = selectTime.setTimeType(type);
    init(weekDatas[0], weekDatas[1],type)
};

function servicePrevious() {
    weekDatas = selectTime.Previous();
    init(weekDatas[0], weekDatas[1], timeType);
}
function serviceNext() {
    weekDatas = selectTime.Next();
    init(weekDatas[0], weekDatas[1], timeType)

}

init(weekDatas[0], weekDatas[1],timeType);