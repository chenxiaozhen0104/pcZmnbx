Vue.component("timeline-item",{props:["line"],template:'<li v-bind:class=line.First><i class="node-icon"></i>                    <span class="time">{{line.CreateTime}}</span>                    <span class="txt">{{line.Content}}</span>               </li>'}),Vue.component("v-timeline",{props:["orderid"],template:'<div class="track-rcol">                            <div class="track-list">                                    <ul>                                        <timeline-item v-for="item in timeline" :line="item"></timeline-item>                                    </ul>                            </div>                           </div>',data:function(){return{timeline:[]}},created:function(){var e=this;$.get("/api/order/timeline",{orderId:this.orderid},function(i){i.error?reject(i.error):e.timeline=i})}});
//# sourceMappingURL=v-timeline.js.map