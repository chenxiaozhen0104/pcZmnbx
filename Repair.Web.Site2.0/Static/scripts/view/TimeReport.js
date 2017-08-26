
$(".publistBottom-lists a").each(function (i, item) {
    //console.log(window.location.pathname);
    //console.log(item.pathname)
    var regExp = new RegExp(item.pathname);
    if (regExp.test(location.pathname)) {
        $(item).parent().addClass('publicLists-active');
        $(item).addClass("color-active");
    }
});

var date = new Date;
var myChart = echarts.init(document.getElementById('timeEcharts'));
var year = date.getFullYear();

function init(year) {
    $.post('/Server/RepairdTimeCount?apptype=1&year=' + year , {}, function (res) {
        res.series = res.series.map(function (item) {
            var lens = item.data.length;
            for (var i = 0; i < lens; i++) {
                item.data[i] = (item.data[i] / 60).toFixed(2);
            }
            return item;
        })
        orderOption.title.text = year + '年' + '\n' + "派单响应时间";
        orderOption.legend.data = res.lengData
        orderOption.series = res.series;
        myChart.setOption(orderOption);
    });
}

init(year);

