Vue.component("v-state-search", {
    props: ["statelist"],
    template: '<div class="stateSelect border-bottom-1">\
                   <div :class="{active:active==999}" class="col-xs-4 text-center" @click="goto(0)"><span id="stateIng">进行中</span><span style="margin: 0 0 0 10px" class="glyphicon glyphicon-triangle-bottom"></span></div>\
                   <div :class="{active:active==1}" class="col-xs-4 text-center" @click="goto(1)"><span>已完成</span></div>\
                   <div :class="{active:active==2}" class="col-xs-4 text-center" @click="goto(2)"><span>全部</span></div>\
                   <div class="state-panel">\
                        <div class="row" >\
                            <div class="col-xs-4 text-center" v-for="item in statelist" @click="goto(999,item)"><span>{{item.name}}</span></div>\
                        </div>\
                   </div>\
               </div>',
    data: function () {
        return {
            active: 999
        }
    },
    methods: {
        goto: function (active, item) {

            if (active == 0) {
                this.show();
                return;
            }

            $(".state-panel").removeClass('show');
            $(".glyphicon").removeClass('checked');

            this.active = active;

            if (active == 1) {
                this.$emit('state-goto', 2048)
                return;
            }
            if (active == 2) {
                this.$emit('state-goto', 4 + 8 + 16 + 32 + 64 + 128 + 256 + 512 + 1024 + 2048)
                return;
            }
           
            if (active == 999) {
                $(".stateSelect #stateIng").html(item.name);
            }

            this.$emit('state-goto', item.val)
        },
        show: function () {
            if ($(".state-panel").hasClass("show")) {
                $(".state-panel").removeClass('show');
                $(".glyphicon").removeClass('checked');
            }
            else {
                $(".state-panel").addClass('show');
                $(".glyphicon").addClass('checked');
            }
        }
    }
});