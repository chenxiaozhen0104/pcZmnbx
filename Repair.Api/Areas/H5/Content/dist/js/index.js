function openQRCode(){var e=new LG.URL(ActionBaseUrl);e.set("target","_openqrcode"),window.location.href=e.url()}function qrCallBack(e){var a=e.substr(e.lastIndexOf("/")+1);$("#txtQrCode").val(a)}$(document).ready(function(){function e(e,a){$(".loading").addClass("show"),$.get("/api/device/search",{searchKey:$("#txt-key").val(),areaId:o.area,categoryId:o.category,brandId:o.brand,pageIndex:e||1,pageSize:20},function(e){for(var t=e.data.length,o=e.data,i="",n=0;n<t;n++)i+="<a id='"+o[n].id+"' class='list-group-item row device-item'><div class='col-xs-10'><div class='list-group-item-heading'>"+o[n].name+"<span class='qrcode'>"+(o[n].qrcode||"")+"</span></div><div class='row list-group-item-text'><p class='col-xs-8'>类目："+o[n].category+"</p><p class='col-xs-4'>品牌："+(o[n].brand||"暂无")+"</p></div><div class='row list-group-item-text'><p class='col-xs-8'>型号："+(o[n].model||"暂无")+"</p><p class='col-xs-4'>区域："+o[n].area+"</p></div><div class='row list-group-item-text'><p class='col-xs-12'>安装地址："+(o[n].address||"暂无")+"</p></div></div><div class='col-xs-2 text-center'>"+(o[n].qrcode?"":"<div class='iconfont icon-erweima' style='font-size:30px;'></div>")+"</div></a>";a?(c=0==o.length,$(".nodata").show(),$(".list-group").append(i)):($(".list-group").html(i),s=1,c=!1),$(".loading").removeClass("show")})}function a(){$.post("/api/device/initData",function(e){for(var a=e.area.length,t=e.category.length,o=e.brand.length,s="",c="",i="",n=0;n<a;n++)s+="<span class='search-tag' data-type='area' data-id='"+e.area[n].id+"'>"+e.area[n].name+"</span>";$(".area").append(s);for(var n=0;n<t;n++)c+="<span class='search-tag' data-type='category' data-id='"+e.category[n].id+"'>"+e.category[n].name+"</span>";$(".category").append(c);for(var n=0;n<o;n++)i+="<span class='search-tag' data-type='brand' data-id='"+e.brand[n].id+"'>"+e.brand[n].name+"</span>";$(".brand").append(i)}),$(".show-panel").on("click","span",function(){console.log(123),$(".show-panel").removeClass("show");var a=$(this),t=a.data("type");$(".ddl-"+t).children().eq(0).text(a.text()),o[t]=a.data("id"),$(".ddl").removeClass("checked"),e()})}var t=$(window).height()-92;$(".show-panel").height(t),$(".chooseList>.col-xs-4").click(function(){var e=$(this),a=$(this).data("panel");$(".ddl").not(this).removeClass("checked"),e.hasClass("checked")?(e.removeClass("checked"),$(a).removeClass("show")):(e.addClass("checked"),$(".show-panel").not(a).removeClass("show"),$(a).addClass("show"))}),$(".icon-sou").click(function(){$(".clickSerch").addClass("moveSerch")}),$(".btn,.search-back").click(function(){$(".clickSerch").removeClass("moveSerch")});var o={area:null,category:null,brand:null},s=1,c=!1;e(),$(window).scroll(function(){$(document).scrollTop()>=$(document).height()-$(window).height()-20&&!c&&(s++,e(s,!0))}),a(),$("#txt-key").on("input",function(){e()});var i;$(".list-group").on("click",".device-item",function(){layer.open({type:1,title:!1,closeBtn:1,shadeClose:!1,shade:[.3,"#393D49"],content:$(".bounceds")}),i=this.id,$.get("/api/device/get?deviceId="+this.id,function(e){console.log(e),e&&($("#txtQrCode").val(e.QRCode),$("#txtLocation").val(e.Position),$(".deviceName").text(e.Name),$(".areaName").text(e.Area.Name),$(".categoryName").text(e.Category.Name),$(".modelName").text(e.Model))})}),$("#btn-submit").click(function(){var e=(new LG.URL,$("#txtQrCode").val());if(""==e||void 0==e)return void layer.msg("二维码为空！",{icon:2,time:1500});var a=$("#txtLocation").val();$.ajax({url:"/api/device/setQRCode",type:"post",data:{deviceId:i,qrCode:e,address:a},success:function(e){e.error?layer.alert(e.error,{icon:0}):($("#"+i).find(".icon-erweima").remove(),layer.closeAll(),layer.msg("保存成功",{icon:6,time:1e3}))}})}),$("#btn-close").click(function(){$(".bounceds").removeClass("show")}),$("#btn-repair").click(function(){location.href="deviceDetail.html?deviceId="+i})});
//# sourceMappingURL=index.js.map
