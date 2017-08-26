Vue.component('v-footer', {
    props: ['active'],
    template: '<ul class="v-footer">\
        <li :class="{active:active==0,show:appType==1}" @click="goto(0)"><i class="iconfont icon-dingdanjilu"></i>报修</li>\
        <li :class="{active:active==1,show:true}" @click="goto(1)"><i class="iconfont icon-jibenxinxi"></i>工单</li>\
        <li :class="{active:active==2,show:appType==1}" @click="goto(2)"><i class="iconfont icon-company"></i>设备</li>\
        <li :class="{active:active==3,show:true}" @click="goto(3)"><i class="iconfont icon-user"></i>我</li></ul>',
    data: function () {

        var reg = new RegExp("(^|&)appType=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);

        var type = r != null ? unescape(r[2]) : undefined;


        localStorage.appType = type || localStorage.appType;

        return {
            appType: localStorage.appType
        }
    },
    methods: {
        goto: function (i) {
            var pages = ["repair.html", "orderlist.html", "device.html", "my.html"]
            location.href = pages[i]

            this.$emit('footer-tab-click', this.user)
        }
    }
})

Vue.component('v-header', {
    props: ['showBack', 'backLink', 'showReload'],
    template: '<nav class="v-header">\
        <a v-show="showBack" @click="back" class="iconfont icon-fanhui btn-back" style="position:absolute;left:15px;color:white;text-decoration:none;"></a>\
        <slot>啄木鸟</slot>\
        <a v-show="showReload" @click="reload" style="position:absolute;right:15px;color:white;text-decoration:none;">刷新</a>\
    </nav>',
    methods: {
        reload: function () {
            location.href = location.href.replace("_blank_closecurrent", "");;

            //href = location.href.replace("_blank_closecurrent", "");
        },
        back: function (i) {
            if (this.backLink) {
                location.href = this.backLink
            } else {
                history.back()
            }

            this.$emit('header-back', this.user)
        }
    }
})
