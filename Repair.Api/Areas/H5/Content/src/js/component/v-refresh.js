(function () {
    var beginY, threshold = 5, thresholdLog = Math.log(threshold), y = 0, dy = 50, canDrop = true
    Vue.component('v-refresh', {
        props: ['css', 'start', 'showLoading'],
        template: '<div style="position:relative" :class="css">\
         <div style="position:absolute;z-index:5;font-size:16px;width:100%;text-align:center;height:25px;" :style="tipStyle" v-html="tip"></div>\
         <div style="line-height:25px;text-align:center" v-show="showDone">加载完成</div>\
         <div @touchstart="touchstart" @touchend="touchend" @touchmove="touchmove" :style="style"><slot>暂时没有更多内容</slot></div>\
    </div>',
        data: function () {
            return {
                tip: '下拉刷新',
                showDone: false,
                showTip: false,
                tipStyle: {
                    transition: 'none',
                    clip: 'rect(25px,1000px,25px,0)'
                },
                style: {
                    transition: 'none',
                    transform: 'translate3d(0,0,0)',
                }
            }
        },
        methods: {
            touchstart: function (e) {
                beginY = e.touches[0].clientY
            },
            touchend: function (e) {
                this.style.transition = 'all 300ms'
                if (y > dy) {
                    this._start()
                    var self = this
                } else {
                    this._cancel()
                }
            },
            touchmove: function (e) {
                if (!canDrop || window.scrollY != 0) {
                    return
                }
                y = Math.max(0, (e.touches[0].clientY - beginY));
                y = threshold + (y - threshold) * thresholdLog / Math.log(y)
                this.tip = y > dy ? '↑ 松开刷新' : '↓ 下拉刷新'
                this.style.transform = 'translate3d(0,' + y + 'px,0)'
                this.tipStyle.clip = 'rect(' + Math.max(25 - y, 0) + 'px,1000px,25px,0px)'

                this.style.transition = 'none'
                this.tipStyle.transition = 'none'
            },
            _start: function () {
                canDrop = false
                if (this.showLoading) {
                    this.style.transform = 'translate3d(0,25px,0)'
                    this.tip = '<i class="iconfont icon-loading"></i>加载中...'
                }
                this.start && this.start(this._done)
            },
            _cancel: function () {
                this.tipStyle.transition = 'all 300ms'
                this.tipStyle.clip = 'rect(25px,1000px,25px,0)'
                y = 0
                canDrop = true
                this.style.transform = 'translate3d(0,0,0)'
            },
            _done: function () {
                this._cancel()
            }
        },
        created: function () {
            //document.body.ontouchmove = function (e) {
            //    e.preventDefault();
            //}
        }
    })

})()