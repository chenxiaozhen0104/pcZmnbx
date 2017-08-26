// 一、报修端统计

// 区域，类目
barOption = {
    title: {
        text: '',
        subtext: '',
        textStyle: {
            fontSize: 12,
        }
    },
    tooltip: {
        trigger: 'axis',
        axisPointer: {
            type: 'shadow'
        }
    },
    legend: {
        data: []
    },
    grid: {
        left: '1%',
        right: '5%',
        bottom: '3%',
        containLabel: true
    },
    xAxis: {
        type: 'value',
    },
    yAxis: {

        type: 'category',
        axisLabel: {
            interval: 0,
            rotate: 45
        },
        nameTextStyle: {
            fontSize: 10
        },
        data: []
    },
    series: [
        {
            name: '',
            type: 'bar',
            barWidth: '90%',
            label: {
                normal: {
                    show: true,
                    position: 'insideRight',
                },
            },
            data: []
        }
    ]
};

// 派单响应时间
orderOption = {
    title: {
        text: '',
        textStyle: {
            fontSize: 12,
        },
        top: -10
    },
    tooltip: {
        trigger: 'axis'
    },
    legend: {
        data: [],
        orient: 'vertical',
        padding: 0,
        itemGap: 0,
        textStyle: {
            fontSize: 12,
        }
    },
    grid: {
        top: "17%",
        left: "11%"
    },
    xAxis: [
        {
            type: 'category',
            boundaryGap: [0, 0.01],
            axisPointer: {
                type: 'shadow'
            },
            axisLabel: {
                interval: 0,
                rotate: -45,

            },
            data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
        }
    ],
    yAxis: [
        {
            type: 'value',
            name: '单位(小时)',

        }
    ],
    series: [
    ]
};


// 二、维修端
// 工作时长统计
serviceOption = {
    title: {
        text: '',
        textStyle: {
            fontSize: 12,
        }
    },
    tooltip: {
        trigger: 'axis',
        axisPointer: {
            type: 'shadow'
        }
    },
    legend: {
        data: []

    },
    grid: {
        left: '1%',
        right: '5%',
        bottom: '3%',
        containLabel: true
    },
    xAxis: {
        type: 'value'
    },
    yAxis: {
        type: 'category',
        data: []
    },
    series: [
        {
            name: '',
            type: 'bar',
            barWidth: '40%',
            label: {
                normal: {
                    show: true,
                    position: 'insideRight',
                },
            },
            data: []
        }
    ]
};



// 订单数量 按人员
ordersOption = {
    title: {
        text: '',
        textStyle: {
            fontSize: 12,
        },
        top: '13'
    },
    tooltip: {
        trigger: 'axis',
        axisPointer: {            // 坐标轴指示器，坐标轴触发有效
            type: 'shadow'        // 默认为直线，可选为：'line' | 'shadow'
        }
    },
    legend: {
        top: "-10",
        padding:8,
        data: []
    },
    grid: {
        left: '3%',
        right: '5%',
        bottom: '3%',
        containLabel: true
    },
    xAxis: {
        type: 'value'
    },
    yAxis: {
        type: 'category',
        axisLabel: {
            interval: 0,
            rotate: 45
        },
        nameTextStyle: {
            fontSize: 10
        },
        data: []
    },
    series: []
};




// 订单数量,完成率统计 按时间
var waterMarkText = '';
var canvas = document.createElement('canvas');
var ctx = canvas.getContext('2d');
canvas.width = canvas.height =380;
ctx.textAlign = 'center';
ctx.textBaseline = 'middle';
ctx.globalAlpha = 0.08;
ctx.font = '20px Microsoft Yahei';
ctx.translate(50, 50);
ctx.rotate(-Math.PI / 4);
ctx.fillText(waterMarkText, 0, 0);

barWithPieoption = {
    backgroundColor: {
        type: 'pattern',
        image: canvas,
        repeat: 'repeat'
    },
    tooltip: {
        trigger: 'item',  
    },
   
    title: [{
        text: '维修量 单位(个)',
        x: '5%',
        itemGap:-5,
        textStyle: {
            fontSize: 12
        }
    }, {
        text: '完成情况 单位(%)',
        x: '75%',
        textAlign: 'center',
        itemGap: -5,
        textStyle: {
            fontSize: 12
        }
    }],
    grid: [{
        top: 50,
        width: '50%',
        bottom: '10%',
        left: 10,
        containLabel: true
    }],
    xAxis: [{
        type: 'value',
        splitLine: {
            show: false
        }
    }],
    yAxis: [{
        type: 'category',
        data: [],
        axisLabel: {
            interval: 0,
            rotate: 30
        },
        splitLine: {
            show: false
        }
    }],
    series: [{
        type: 'bar',
        stack: 'chart',
        z: 3,
        label: {
            normal: {
                position: 'insideRight',
               offset:[0,-7],
                show: true,
                formatter:function (a) { return a.value > 0 ? a.value : "" }
            }
        },
        data: []
    }, {
        type: 'pie',
        z:5,
        //label: {
        //    normal: {
        //        position: 'outside',
        //        formatter: "{b} : {c} ({d}%)"
        //    }
        //},
        radius: [0, '20%'],
        center: ['68%', '45%'],
        tooltip:{
            formatter: "{a}{b} : {c} ({d}%)"
        },
        data: []
    }]
};

