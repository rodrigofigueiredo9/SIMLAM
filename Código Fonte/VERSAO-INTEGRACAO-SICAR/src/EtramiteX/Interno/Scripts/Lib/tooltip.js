/*******************************************************************************
 jquery.mb.components
 Copyright (c) 2001-2010. Matteo Bicocchi (Pupunzi); Open lab srl, Firenze - Italy
 email: info@pupunzi.com
 site: http://pupunzi.com

 Licences: MIT, GPL
 http://www.opensource.org/licenses/mit-license.php
 http://www.gnu.org/licenses/gpl.html
 ******************************************************************************/

jQuery.fn.extend({
	everyTime: function(interval, label, fn, times, belay) {
		return this.each(function() {
			jQuery.timer.add(this, interval, label, fn, times, belay);
		});
	},
	oneTime: function(interval, label, fn) {
		return this.each(function() {
			jQuery.timer.add(this, interval, label, fn, 1);
		});
	},
	stopTime: function(label, fn) {
		return this.each(function() {
			jQuery.timer.remove(this, label, fn);
		});
	}
});

jQuery.extend({
	timer: {
		guid: 1,
		global: {},
		regex: /^([0-9]+)\s*(.*s)?$/,
		powers: {
			// Yeah this is major overkill...
			'ms': 1,
			'cs': 10,
			'ds': 100,
			's': 1000,
			'das': 10000,
			'hs': 100000,
			'ks': 1000000
		},
		timeParse: function(value) {
			if (value == undefined || value == null)
				return null;
			var result = this.regex.exec(jQuery.trim(value.toString()));
			if (result[2]) {
				var num = parseInt(result[1], 10);
				var mult = this.powers[result[2]] || 1;
				return num * mult;
			} else {
				return value;
			}
		},
		add: function(element, interval, label, fn, times, belay) {
			var counter = 0;
			
			if (jQuery.isFunction(label)) {
				if (!times) 
					times = fn;
				fn = label;
				label = interval;
			}
			
			interval = jQuery.timer.timeParse(interval);

			if (typeof interval != 'number' || isNaN(interval) || interval <= 0)
				return;

			if (times && times.constructor != Number) {
				belay = !!times;
				times = 0;
			}
			
			times = times || 0;
			belay = belay || false;
			
			if (!element.$timers) 
				element.$timers = {};
			
			if (!element.$timers[label])
				element.$timers[label] = {};
			
			fn.$timerID = fn.$timerID || this.guid++;
			
			var handler = function() {
				if (belay && this.inProgress) 
					return;
				this.inProgress = true;
				if ((++counter > times && times !== 0) || fn.call(element, counter) === false)
					jQuery.timer.remove(element, label, fn);
				this.inProgress = false;
			};
			
			handler.$timerID = fn.$timerID;
			
			if (!element.$timers[label][fn.$timerID]) 
				element.$timers[label][fn.$timerID] = window.setInterval(handler,interval);
			
			if ( !this.global[label] )
				this.global[label] = [];
			this.global[label].push( element );
			
		},
		remove: function(element, label, fn) {
			var timers = element.$timers, ret;
			
			if ( timers ) {
				
				if (!label) {
					for ( label in timers )
						this.remove(element, label, fn);
				} else if ( timers[label] ) {
					if ( fn ) {
						if ( fn.$timerID ) {
							window.clearInterval(timers[label][fn.$timerID]);
							delete timers[label][fn.$timerID];
						}
					} else {
						for ( var fn in timers[label] ) {
							window.clearInterval(timers[label][fn]);
							delete timers[label][fn];
						}
					}
					
					for ( ret in timers[label] ) break;
					if ( !ret ) {
						ret = null;
						delete timers[label];
					}
				}
				
				for ( ret in timers ) break;
				if ( !ret ) 
					element.$timers = null;
			}
		}
	}
});

if (jQuery.browser.msie)
	jQuery(window).one("unload", function() {
		var global = jQuery.timer.global;
		for ( var label in global ) {
			var els = global[label], i = els.length;
			while ( --i )
				jQuery.timer.remove(els[i], label);
		}
	});
	
	
	
/*******************************************************************************
 jquery.mb.components
 Copyright (c) 2001-2010. Matteo Bicocchi (Pupunzi); Open lab srl, Firenze - Italy
 email: info@pupunzi.com
 site: http://pupunzi.com

 Licences: MIT, GPL
 http://www.opensource.org/licenses/mit-license.php
 http://www.gnu.org/licenses/gpl.html
 ******************************************************************************/

/*
	VERSION: Drop Shadow jQuery Plugin 1.6  12-13-2007

	REQUIRES: jquery.js (1.2.6 or later)

	SYNTAX: $(selector).dropShadow(options);  // Creates new drop shadows
					$(selector).redrawShadow();       // Redraws shadows on elements
					$(selector).removeShadow();       // Removes shadows from elements
					$(selector).shadowId();           // Returns an existing shadow's ID

	OPTIONS:

		left    : integer (default = 4)
		top     : integer (default = 4)
		blur    : integer (default = 2)
		opacity : decimal (default = 0.5)
		color   : string (default = "black")
		swap    : boolean (default = false)

	The left and top parameters specify the distance and direction, in	pixels, to
	offset the shadow. Zero values position the shadow directly behind the element.
	Positive values shift the shadow to the right and down, while negative values 
	shift the shadow to the left and up.
	
	The blur parameter specifies the spread, or dispersion, of the shadow. Zero 
	produces a sharp shadow, one or two produces a normal shadow, and	three or four
	produces a softer shadow. Higher values increase the processing load.
	
	The opacity parameter	should be a decimal value, usually less than one. You can
	use a value	higher than one in special situations, e.g. with extreme blurring. 
	
	Color is specified in the usual manner, with a color name or hex value. The
	color parameter	does not apply with transparent images.
	
	The swap parameter reverses the stacking order of the original and the shadow.
	This can be used for special effects, like an embossed or engraved look.

	EXPLANATION:
	
	This jQuery plug-in adds soft drop shadows behind page elements. It is only
	intended for adding a few drop shadows to mostly stationary objects, like a
	page heading, a photo, or content containers.

	The shadows it creates are not bound to the original elements, so they won't
	move or change size automatically if the original elements change. A window
	resize event listener is assigned, which should re-align the shadows in many
	cases, but if the elements otherwise move or resize you will have to handle
	those events manually. Shadows can be redrawn with the redrawShadow() method
	or removed with the removeShadow() method. The redrawShadow() method uses the
	same options used to create the original shadow. If you want to change the
	options, you should remove the shadow first and then create a new shadow.
	
	The dropShadow method returns a jQuery collection of the new shadow(s). If
	further manipulation is required, you can store it in a variable like this:

		var myShadow = $("#myElement").dropShadow();

	You can also read the ID of the shadow from the original element at a later
	time. To get a shadow's ID, either read the shadowId attribute of the
	original element or call the shadowId() method. For example:

		var myShadowId = $("#myElement").attr("shadowId");  or
		var myShadowId = $("#myElement").shadowId();

	If the original element does not already have an ID assigned, a random ID will
	be generated for the shadow. However, if the original does have an ID, the 
	shadow's ID will be the original ID and "_dropShadow". For example, if the
	element's ID is "myElement", the shadow's ID would be "myElement_dropShadow".

	If you have a long piece of text and the user resizes the	window so that the
	text wraps or unwraps, the shape of the text changes and the words are no
	longer in the same positions. In that case, you can either preset the height
	and width, so that it becomes a fixed box, or you can shadow each word
	separately, like this:

		<h1><span>Your</span> <span>Page</span> <span>Title</span></h1>

		$("h1 span").dropShadow();

	The dropShadow method attempts to determine whether the selected elements have
	transparent backgrounds. If you want to shadow the content inside an element,
	like text or a transparent image, it must not have a background-color or
	background-image style. If the element has a solid background it will create a
	rectangular	shadow around the outside box.

	The shadow elements are positioned absolutely one layer below the original 
	element, which is positioned relatively (unless it's already absolute).

	*** All shadows have the "dropShadow" class, for selecting with CSS or jQuery.

	ISSUES:
	
		1)	Limited styling of shadowed elements by ID. Because IDs must be unique,
				and the shadows have their own ID, styles applied by ID won't transfer
				to the shadows. Instead, style elements by class or use inline styles.
		2)	Sometimes shadows don't align properly. Elements may need to be wrapped
				in container elements, margins or floats changed, etc. or you may just 
				have to tweak the left and top offsets to get them to align. For example,
				with draggable objects, you have to wrap them inside two divs. Make the 
				outer div draggable and set the inner div's position to relative. Then 
				you can create a shadow on the element inside the inner div.
		3)	If the user changes font sizes it will throw the shadows off. Browsers 
				do not expose an event for font size changes. The only known way to 
				detect a user font size change is to embed an invisible text element and
				then continuously poll for changes in size.
		4)	Safari support is shaky, and may require even more tweaks/wrappers, etc.
		
		The bottom line is that this is a gimick effect, not PFM, and if you push it
		too hard or expect it to work in every possible situation on every browser,
		you will be disappointed. Use it sparingly, and don't use it for anything 
		critical. Otherwise, have fun with it!
				
	AUTHOR: Larry Stevens (McLars@eyebulb.com) This work is in the public domain,
					and it is not supported in any way. Use it at your own risk.
*/


(function($){

	var dropShadowZindex = 1;  //z-index counter

	$.fn.dropShadow = function(options)
	{
		// Default options
		var opt = $.extend({
			left: 4,
			top: 4,
			blur: 2,
			opacity: .5,
			color: "black",
			swap: false
			}, options);
		var jShadows = $([]);  //empty jQuery collection
		
		// Loop through original elements
		this.not(".dropShadow").each(function()
		{
			var jthis = $(this);
			var shadows = [];
			var blur = (opt.blur <= 0) ? 0 : opt.blur;
			var opacity = (blur == 0) ? opt.opacity : opt.opacity / (blur * 8);
			var zOriginal = (opt.swap) ? dropShadowZindex : dropShadowZindex + 1;
			var zShadow = (opt.swap) ? dropShadowZindex + 1 : dropShadowZindex;
			
			// Create ID for shadow
			var shadowId;
			if (this.id) {
				shadowId = this.id + "_dropShadow";
			}
			else {
				shadowId = "ds" + (1 + Math.floor(9999 * Math.random()));
			}

			// Modify original element
			$.data(this, "shadowId", shadowId); //store id in expando
			$.data(this, "shadowOptions", options); //store options in expando
			jthis
				.attr("shadowId", shadowId)
				.css("zIndex", zOriginal);
			if (jthis.css("position") != "absolute") {
				jthis.css({
					position: "relative",
					zoom: 1 //for IE layout
				});
			}

			// Create first shadow layer
			bgColor = jthis.css("backgroundColor");
			if (bgColor == "rgba(0, 0, 0, 0)") bgColor = "transparent";  //Safari
			if (bgColor != "transparent" || jthis.css("backgroundImage") != "none" 
					|| this.nodeName == "SELECT" 
					|| this.nodeName == "INPUT"
					|| this.nodeName == "TEXTAREA") {		
				shadows[0] = $("<div></div>")
					.css("background", opt.color);								
			}
			else {
				shadows[0] = jthis
					.clone()
					.removeAttr("id")
					.removeAttr("name")
					.removeAttr("shadowId")
					.css("color", opt.color);
			}
			shadows[0]
				.addClass("dropShadow")
				.css({
					height: jthis.outerHeight(),
					left: blur,
					opacity: opacity,
					position: "absolute",
					top: blur,
					width: jthis.outerWidth(),
					zIndex: zShadow
				});
				
			// Create other shadow layers
			var layers = (8 * blur) + 1;
			for (i = 1; i < layers; i++) {
				shadows[i] = shadows[0].clone();
			}

			// Position layers
			var i = 1;			
			var j = blur;
			while (j > 0) {
				shadows[i].css({left: j * 2, top: 0});           //top
				shadows[i + 1].css({left: j * 4, top: j * 2});   //right
				shadows[i + 2].css({left: j * 2, top: j * 4});   //bottom
				shadows[i + 3].css({left: 0, top: j * 2});       //left
				shadows[i + 4].css({left: j * 3, top: j});       //top-right
				shadows[i + 5].css({left: j * 3, top: j * 3});   //bottom-right
				shadows[i + 6].css({left: j, top: j * 3});       //bottom-left
				shadows[i + 7].css({left: j, top: j});           //top-left
				i += 8;
				j--;
			}

			// Create container
			var divShadow = $("<div></div>")
				.attr("id", shadowId) 
				.addClass("dropShadow")
				.css({
					left: jthis.position().left + opt.left - blur,
					marginTop: jthis.css("marginTop"),
					marginRight: jthis.css("marginRight"),
					marginBottom: jthis.css("marginBottom"),
					marginLeft: jthis.css("marginLeft"),
					position: "absolute",
					top: jthis.position().top + opt.top - blur,
					zIndex: zShadow
				});

			// Add layers to container	
			for (i = 0; i < layers; i++) {
				divShadow.append(shadows[i]);
			}
			
			// Add container to DOM
			jthis.after(divShadow);

			// Add shadow to return set
			jShadows = jShadows.add(divShadow);

			// Re-align shadow on window resize
			$(window).resize(function()
			{
				try {
					divShadow.css({
						left: jthis.position().left + opt.left - blur,
						top: jthis.position().top + opt.top - blur
					});
				}
				catch(e){}
			});
			
			// Increment z-index counter
			dropShadowZindex += 2;

		});  //end each
		
		return this.pushStack(jShadows);
	};


	$.fn.redrawShadow = function()
	{
		// Remove existing shadows
		this.removeShadow();
		
		// Draw new shadows
		return this.each(function()
		{
			var shadowOptions = $.data(this, "shadowOptions");
			$(this).dropShadow(shadowOptions);
		});
	};


	$.fn.removeShadow = function()
	{
		return this.each(function()
		{
			var shadowId = $(this).shadowId();
			$("div#" + shadowId).remove();
		});
	};


	$.fn.shadowId = function()
	{
		return $.data(this[0], "shadowId");
	};


	$(function()  
	{
		// Suppress printing of shadows
		var noPrint = "<style type='text/css' media='print'>";
		noPrint += ".dropShadow{visibility:hidden;}</style>";
		$("head").append(noPrint);
	});

})(jQuery);	
	
	
	
	
/*******************************************************************************
 jquery.mb.components
 Copyright (c) 2001-2010. Matteo Bicocchi (Pupunzi); Open lab srl, Firenze - Italy
 email: info@pupunzi.com
 site: http://pupunzi.com

 Licences: MIT, GPL
 http://www.opensource.org/licenses/mit-license.php
 http://www.gnu.org/licenses/gpl.html
 ******************************************************************************/

/*
 * Name:jquery.mb.tooltip
 * Version: 1.6
 * dependencies: jquery.timers.js, jquery.dropshadow.js
*/

(function($){
  jQuery.fn.mbTooltip = function (options){
    return this.each (function () {
      this.options = {
        live:true,
        opacity : 1,
        wait:2000,
        timePerWord:70,
        cssClass:"default",
        hasArrow:true,
        imgPath:"_img/",
        hasShadow:true,
        shadowColor:"red",
        shadowLeft:1,
        anchor:"mouse", //"parent",
        shadowTop:1,
        mb_fade:200
      };
      $.extend (this.options, options);
      if (this.options.live)$("[title]").live("mouseover",function(){$(this).mbTooltip(options);});
      var ttEl=$(this).is("[title]")? $(this): $(this).find("[title]");
      var wait=this.options.wait;
      var hasShadow=this.options.hasShadow;
      var fade=this.options.mb_fade;
      var myOptions=this.options;
      $(ttEl).each(function(){
/*
        $(this).hover(function(){
          $(this).css("cursor","help");
        },function(){
          $(this).css("cursor","default");
        });
*/
        $(this).attr("tooltip", $(this).attr("title"));
        $(this).removeAttr("title");
        $(this).attr("tooltipEnable","true");
        var theEl=$(this);
        var ttCont= theEl.attr("tooltip");
        var hover=$.browser.msie?"mouseenter":"mouseover";
        $(this).bind(hover,function(e){
          if (myOptions.anchor=="mouse") $(document).mb_getXY();
          $(this).one("mouseout",function(){
            $(this).stopTime();
            $(this).deleteTooltip(hasShadow,fade);
          }).one("click",function(){
            $(this).stopTime();
            $(this).deleteTooltip(hasShadow,fade);
          });
          $(this).oneTime(wait, function() {
            if ($(this).attr("tooltipEnable")=="true")
              $(this).buildTooltip(ttCont,myOptions,e);
          });
        });
      });
    });
  };

  var mbX = 0;
  var mbY = 0;

  $.fn.extend({
    mb_getXY:function(){
      $(document).bind("mousemove", function(e) {
        mbX = e.pageX;
        mbY = e.pageY;
      });
      return {x:mbX,y:mbY};
    },
    buildTooltip: function(cont,options){
      this.options={};
      $.extend (this.options, options);
      var parent=$(this);
      $("body").append("<div id='tooltip'></div>");
      var imgUrl=this.options.imgPath+"up.png";
      $("#tooltip").html(cont);
      $("#tooltip").addClass(this.options.cssClass);
      if (this.options.hasArrow){
        $("#tooltip").prepend("<img id='ttimg' src='"+imgUrl+"'>");
        $("#ttimg").css({
          position:"absolute",
          opacity:.5
        });

        $("#ttimg").addClass("top");
      }
      $("#tooltip").css({
        position:"absolute",
        top:  this.options.anchor=="mouse"?$(document).mb_getXY().y +7:parent.offset().top+(parent.outerHeight()),
        left:this.options.anchor=="mouse"?$(document).mb_getXY().x+7:parent.offset().left,
        opacity:0
      });

      $("#tooltip").findBestPos(parent,this.options.imgPath,this.options.anchor);
      if (this.options.anchor=="mouse") $(document).unbind("mousemove");
      if (this.options.hasShadow) {
        $("#tooltip").dropShadow({left: this.options.shadowLeft, top: this.options.shadowTop, blur: 2, opacity: 0.3, color:this.options.shadowColor});
        $(".dropShadow").css("display","none");
        $(".dropShadow").mb_bringToFront();
        $(".dropShadow").fadeIn(this.options.mb_fade);
      }
      $("#tooltip").mb_bringToFront();
      $("#tooltip").fadeTo(this.options.mb_fade,this.options.opacity,function(){});
      var timetoshow=3000+cont.length*this.options.timePerWord;
      var hasShadow=this.options.hasShadow;
      var fade=this.options.mb_fade;
      $(this).oneTime(timetoshow,function(){$(this).deleteTooltip(hasShadow,fade);});
    },
    deleteTooltip: function(hasShadow,fade){
      var sel=hasShadow?"#tooltip,.dropShadow":"#tooltip";
      $(sel).fadeOut(fade,function(){$(sel).remove();});
    },
    findBestPos:function(parent,imgPath,anchor){
      var theEl=$(this);
      var ww= $(window).width()+$(window).scrollLeft();
      var wh= $(window).height()+$(window).scrollTop();
      var w=theEl.outerWidth();
      theEl.css({width:w});
      var t=((theEl.offset().top+theEl.outerHeight(true))>wh)? theEl.offset().top-(anchor!="mouse"? parent.outerHeight():0)-theEl.outerHeight()-20 : theEl.offset().top;
      t=t<0?0:t;
      var l=((theEl.offset().left+w)>ww-5) ? theEl.offset().left-(w-(anchor!="mouse"?parent.outerWidth():0)) : theEl.offset().left;
      l=l<0?0:l;
      if (theEl.offset().top+theEl.outerHeight(true)>wh){
        $("#ttimg").attr("src",imgPath+"bottom.png");
        $("#ttimg").removeClass("top").addClass("bottom");
      }
      theEl.css({width:w, top:t, left:l});
    },
    disableTooltip:function(){
      $(this).find("[tooltip]").attr("tooltipEnable","false");
    },
    enableTooltip:function(){
      $(this).find("[tooltip]").attr("tooltipEnable","true");
    }
  });

  jQuery.fn.mb_bringToFront = function(){
    var zi=99999;
    $('*').each(function() {
      if($(this).css("position")=="absolute"){
        var cur = parseInt($(this).css('zIndex'));
        zi = cur > zi ? parseInt($(this).css('zIndex')) : zi;
      }
    });
    $(this).css('zIndex',zi+=99999); 
  };

  $(function(){
    //due to a problem of getter/setter for select
    $("select[title]").each(function(){
      var selectSpan=$("<span></span>");
      selectSpan.attr("title",$(this).attr("title"));
      $(this).wrapAll(selectSpan);
      $(this).removeAttr("title");
    });
  });

})(jQuery);	
	