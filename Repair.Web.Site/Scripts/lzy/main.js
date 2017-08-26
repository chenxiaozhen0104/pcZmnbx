$('.menu li').mouseenter(function(){
    $(this).children('ul').css('display','block');
  });
$('.menu li').mouseleave(function(){
    $(this).children('ul').css('display','none');
  });
$('.submenu').mouseenter(function(){
	$(this).parent('.menu li').addClass('ahover');
})
$('.submenu').mouseleave(function(){
	$(this).parent('.menu li').removeClass('ahover');
})