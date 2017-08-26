Vue.component("v-state-search",{props:["statelist"],template:'<div class="stateSelect border-bottom-1">                   <div :class="{active:active==999}" class="col-xs-4 text-center" @click="goto(0)"><span id="stateIng">进行中</span><span style="margin: 0 0 0 10px" class="glyphicon glyphicon-triangle-bottom"></span></div>                   <div :class="{active:active==1}" class="col-xs-4 text-center" @click="goto(1)"><span>已完成</span></div>                   <div :class="{active:active==2}" class="col-xs-4 text-center" @click="goto(2)"><span>全部</span></div>                   <div class="state-panel">                        <div class="row" >                            <div class="col-xs-4 text-center" v-for="item in statelist" @click="goto(999,item)"><span>{{item.name}}</span></div>                        </div>                   </div>               </div>',data:function(){return{active:999}},methods:{goto:function(t,s){return 0==t?void this.show():($(".state-panel").removeClass("show"),$(".glyphicon").removeClass("checked"),this.active=t,1==t?void this.$emit("state-goto",2048):2==t?void this.$emit("state-goto",4092):(999==t&&$(".stateSelect #stateIng").html(s.name),void this.$emit("state-goto",s.val)))},show:function(){$(".state-panel").hasClass("show")?($(".state-panel").removeClass("show"),$(".glyphicon").removeClass("checked")):($(".state-panel").addClass("show"),$(".glyphicon").addClass("checked"))}}});
//# sourceMappingURL=v-state.search.js.map
