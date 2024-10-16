$(document).ready(function(){

    AOS.init({
        easing: 'ease-in-out-sine',
        once: true
    });

    //
    // Tab-Pane change function
    var tabChange = function(){
        var tabs = $('.appProBx .appProThumb ul > li');
        var active = tabs.filter('.active');
        var next = active.next('li').length? active.next('li').find('a') : tabs.filter(':first-child').find('a');
        // Bootsrap tab show, para ativar a tab
        next.tab('show')
    }
    // Tab Cycle function
    //var tabCycle = setInterval(tabChange, 5000)
    // Tab click event handler
    $(function(){
        $('.appProBx .appProThumb ul li a').click(function(e) {
            e.preventDefault();
            clearInterval(tabCycle);
            $(this).tab('show')
            setTimeout(function(){
                tabCycle = setInterval(tabChange, 999999);
            }, 1000);
        });
    });
    //

    //
    var hoverTimer;
    $(document).on("mouseenter", "header .mainNav ul li", function() {
        var elem = this;
        hoverTimer = setTimeout(function() {
            $(elem).find('.subLevelMenu').stop(true, true).slideDown();
        }, 400);
    });
    
    $(document).on("mouseleave", "header .mainNav ul li", function() {
        clearTimeout(hoverTimer);
        $(this).find('.subLevelMenu').stop(true, true).slideUp();
    });
    
    $('.fancybox').fancybox({
        openEffect : 'elastic',
        closeEffect : 'elastic'
    });

    $('.fancyboxInline').fancybox({
        openEffect : 'elastic',
        closeEffect : 'elastic',
        padding: 0,
        wrapCSS: 'clientsPopBg'
    });

    $('.fancyboxmedia').fancybox({
        openEffect  : 'none',
        closeEffect : 'none',
        helpers : {
            media : {}
        }
    });

    /* accordian start */
    function hidden(a){
        $(a).removeClass('selected');
        $(a).parent().find('ul:first').slideUp();
    }
    function visible(b){
        $(b).parent().siblings().find('a').removeClass('selected');
        $(b).parent().parent().find('li ul:visible').slideUp();
        $(b).addClass('selected');
        $(b).parent().find('ul:first').slideDown();
    }
    function check(c){
        if($(c).parent().find('ul:first').is(':hidden')) {
        visible(c);
        }else{
        hidden(c);
        }
    }
    $(function(){
        $(".firstLevel li:has(ul) > a").click(function(){
        check($(this));
        return false;
        });
    });
    /* accordian end */

/* home banner script */
    //Function to animate slider captions 
    function doAnimations( elems ) {
        //Cache the animationend event in a variable
        var animEndEv = 'webkitAnimationEnd animationend';
        
        elems.each(function () {
            var $this = $(this),
                $animationType = $this.data('animation');
            $this.addClass($animationType).one(animEndEv, function () {
                $this.removeClass($animationType);
            });
        });
    }
    
    //Variables on page load 
    var $myCarousel = $('#carousel-example-generic, #glanceGallery, #profileVideo'),
        $firstAnimatingElems = $myCarousel.find('.item:first').find("[data-animation ^= 'animated']");
        
    //Initialize carousel 
    $myCarousel.carousel({
        interval:10000
    });
    
    //Animate captions in first slide on page load 
    doAnimations($firstAnimatingElems);
    
    //Pause carousel  
    $myCarousel.carousel('pause');
        
    //Other slides to be animated on carousel slide event 
    $myCarousel.on('slide.bs.carousel', function (e) {
        var $animatingElems = $(e.relatedTarget).find("[data-animation ^= 'animated']");
        doAnimations($animatingElems);
    }); 
/* home banner script */
    
/* home gallery script */
    $('#newsGallery').bxSlider({
        minSlides:1,
        maxSlides:3,
        moveSlides:1,
        slideWidth:320,
        pager:false,
        touchEnabled: false
    });
    
    /*$('#socialGallery').bxSlider({
        minSlides:1,
        maxSlides:3,
        moveSlides:1,
        slideWidth:351,
        pager:false,
        touchEnabled: false
    });*/

    $('#directorGallery').bxSlider({
        minSlides:1,
        maxSlides:4,
        moveSlides:1,
        slideWidth:288,
        pager:false,
        controls:true,
        touchEnabled: true,
        auto: false,
        infiniteLoop: false,
        controls: false,
        pause: 12000
    });
/* home gallery script */

/* hamburger script */
    $('#hamburger').click(function(){
		$(this).toggleClass('open');
        $(this).parents('.headerNavBx').find('.mainNav').toggleClass('mainNavOpen');
	});
/* hamburger script */

/* header search script */
    $('header .headerSearchBx .headSearchBut').click(function(){
        $(this).parents('.headerSearchBx').find('.searchInputBx').slideToggle();
    });

    $(document).click(function(){
        $(".searchInputBx").slideUp();
    });

    $(".searchInputBx, .headSearchBut").click(function(e){
        e.stopPropagation();
    });
/* header search script */ 
    
/* header bg on scroll */
    $(window).scroll(function(){
       if($(window).scrollTop() >= 20){
           $('header').addClass('bg');
       } else{
           $('header').removeClass('bg');
       }
    });

    if($(window).scrollTop() >= 20){
        $('header').addClass('bg');
    }
/* header bg on scroll */    

    $('[data-toggle="tooltip"]').tooltip();

    // mobile conversion
    $('.span_select').click(function(){
        $(this).parents('.mobiledropdown').find('.options').toggleClass('openselect');
    });
    $('.mobiledropdown').on('click','.selItem',function(){
        $(this).parents('.mobiledropdown').find('.options').toggleClass('openselect');
    });
    $('.options a').click(function(){
        $(this).parents('.mobiledropdown').find('.span_select').text("");
        if($(this).parents('.mobiledropdown').find('.selItem').length == 1){
            $(this).parents('.mobiledropdown').find('.selItem').remove();
        }
        $(this).parents('.mobiledropdown').find('.span_select').after('<span class="selItem">'+ $(this).text() +'</span>');
        $(this).parents('.mobiledropdown').find('.options').removeClass('openselect');
    });
    
});

/* timeline */
    timeline(document.querySelectorAll('.timeline'), {
        forceVerticalMode: 700,
        mode: 'horizontal',
        verticalStartPosition: 'left',
        visibleItems: 4
    });




