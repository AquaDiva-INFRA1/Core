﻿
$(document).ready(function ()
{
	resetAllTelerikIconTitles();
	truncateTitle();
	console.log("on document ready");

});

function resetAllTelerikIconTitles()
{
	$('.t-arrow-first, ' +
	  '.t-arrow-prev,' +
	  '.t-arrow-next,' +
	  '.t-arrow-last,' +
	  '.t-arrow-up,' +
	  '.t-arrow-down,' +
	  '.t-arrow-up-small,' +
	  '.t-arrow-down-small,' +
	  '.t-filter,'+
	  '.t-group-delete,'+
	  '.t-close,'+
	  '.t-icon-calendar'
	  ).each(function ()
	  {
		$(this).empty();
	});
}

/*Truncate Title*/
function truncateTitle()
{
	
	$('.bx-trunc-child').each(function ()
	{

		//$(this).trunk8();
		//if (!$(this).attr("title") == true) { 
		var n = $(".bx-trunc-parent").width();
		var text = $(this).text();

		//Link Breiter als/ oder gleich breit Container
		if ($(this).width() >= n)
		{
			$(this).width(n);

			//get text from title or text
			if ($(this).attr("title") != null)
				t = $(this).attr("title");
			else
				t = $(this).text();

			var nt = t.split(" ");
			var ntLast = nt.pop();
			$(this).trunk8(
			{
				fill: "..." + ntLast
			});

		}
	    //Link kürzer als Container
		else if (text.indexOf(".") != -1 || text == '' || text == null)
		{

			var l = $(this).text().length;
		    if (l == 0)
			{
				l = 1;
			}

			var w = $(this).width();
		    if (w == 0)
			{
				w = 1;
			}

			var m = w / l;

			m = Math.round(m).toFixed(0);

			var t = $(this).attr("title");
			var maxWidth = t.length * m;
			maxWidth = maxWidth + 20;

			if (maxWidth >= n)
			{
				$(this).width(n);
			}
			else
			{

				$(this).width(maxWidth);
			}

			var nt = t.split(" ");
			var ntLast = nt.pop();


			$(this).trunk8(
				{
					fill: "..." + ntLast
				});
		}


	});

}

function addTooltips() {
	$(".t-grid > table > tbody > tr > td , .t-grid > table > thead > tr > th").each(function ()
	{
        var $this = $(this);
        var text = $this.text();
        $this.attr("title", text);
    });
}

/**
 * TELERIK EXTENTIONS
 */


$(".t-grid").load(function () {

    $(".t-grid th").each(function () {

        var element = $(this);
        var div = $(document.createElement("div"));
        div.addClass("bx-header-title");
        div.css({ "overflow": "hidden", "text-overflow": "ellipsis", "float": "left" });

        var filter = element.find(".t-grid-filter");
        filter.css("float", "right");


        div.width((element.innerWidth() - filter.outerWidth() - 5));

        var a = element.find("a");
        div.html(a);

        element.prepend(div);


    });
});

$(".t-grid").change(function () {

    $(".t-grid th").each(function () {

        var element = $(this);
        var div = $(document.createElement("div"));
        div.addClass("bx-header-title");
        div.css({ "overflow": "hidden", "text-overflow": "ellipsis", "float": "left" });

        var filter = element.find(".t-grid-filter");
        filter.css("float", "right");


        div.width((element.innerWidth() - filter.outerWidth() - 5));

        var a = element.find("a");
        div.html(a);

        element.prepend(div);


    });
});



/*List*/

$(".bx-list > li").click(function ()
{
	$(this).parent().find(".selected").removeClass("selected");
    if ($(this).hasClass("selected"))
	{
		$(this).removeClass("selected");
	}
	else
	{
		$(this).addClass("selected");
	}
});
$(".bx-list-multi >li").click(function ()
{
	//$(this).parent().find(".selected").removeClass("selected")

	if ($(this).hasClass("selected"))
	{
		$(this).removeClass("selected");
	}
	else
	{
		$(this).addClass("selected");
	}
});
  


/* jQuery Validation Extension - CheckBox */
if (jQuery.validator) {
    // Checkbox Validation
    jQuery.validator.addMethod("checkrequired", function (value, element, params) {
        var checked = false;
        checked = $(element).is(':checked');
        return checked;
    }, '');
    if (jQuery.validator.unobtrusive) {
        jQuery.validator.unobtrusive.adapters.addBool("checkrequired");
    }
}
