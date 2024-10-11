(function ($) {
    $.fn.pageMe = function (opts) {
        var $this = this,
            defaults = {
                perPage: 50,
                showPrevNext: false,
                hidePageNumbers: false,
                pageNo:0,
            },
            settings = $.extend(defaults, opts);

        var listElement = $this;
        var perPage = settings.perPage;
        var pageNo = settings.pageNo;
        var children = listElement.children();
        var pager = $('.pager');

        if (typeof settings.childSelector != "undefined") {
            children = listElement.find(settings.childSelector);
        }

        if (typeof settings.pagerSelector != "undefined") {
            pager = $(settings.pagerSelector);
        }

        var numItems = children.length;
        var numPages = Math.ceil(numItems / perPage);

        pager.data("curr", 0);

        if (settings.showPrevNext) {
            $('<li><a title="First page" href="javascript:void(0);" class="first_page">&#171</a></li>').appendTo(pager);
            $('<li><a title="Previos page" href="javascript:void(0);" class="prev_link">&lt</a></li>').appendTo(pager);
        }

        var curr = 0;
        // Added class and id in li start
        while (numPages > curr && (settings.hidePageNumbers == false)) {
            $('<li id="pg' + (curr + 1) + '" class="pg"><a href="javascript:void(0);" class="page_link">' + (curr + 1) + '</a></li>').appendTo(pager);
            curr++;
        }
        // Added class and id in li end
        $('#pg1').addClass("active")

        if (settings.showPrevNext) {
            $('<li><a title="Next page" href="javascript:void(0);" class="next_link">&gt</a></li>').appendTo(pager);
            $('<li><a title="Last page" href="javascript:void(0);" class="last_page">&#187</a></li>').appendTo(pager);
        }
        curr = 0;
        $('.pg').hide();
        while (curr < 10) {
            $('#pg' + (curr + 1)).show();
            curr++;
        }
        pager.find('.page_link:first').addClass('active');
        pager.find('.prev_link').hide();
        pager.find('.first_page').hide();
        if (numPages <= 1) {
            pager.find('.next_link').hide();
            pager.find('.last_page').hide();
            $('#pg1').hide();
            $("#pagedata").hide();
        }
        pager.children().eq(1).addClass("active");

        children.hide();
        children.slice(0, perPage).show();
        if (numPages > 1) {
            $("#pagedata").text("Page 1 of " + numPages + ", items 1 to " + perPage + " of " + numItems);
            $("#pagedata").show();
        }
        pager.find('li .page_link').click(function () {
            var clickedPage = $(this).html().valueOf() - 1;
            goTo(clickedPage, perPage);
            return false;
        });
        pager.find('li .prev_link').click(function () {
            previous();
            return false;
        });
        pager.find('li .next_link').click(function () {
            next();
            return false;
        });
        pager.find('li .first_page').click(function () {
            goTo(0);
        });
        pager.find('li .last_page').click(function () {
            goTo(numPages-1);
        });
        function previous() {
            var goToPage = parseInt(pager.data("curr")) - 1;
            goTo(goToPage);
        }

        function next() {
            goToPage = parseInt(pager.data("curr")) + 1;
            goTo(goToPage);

        }
        if (pageNo > 0)
            goTo(pageNo);

        function goTo(page) {
            var startAt = page * perPage,
                endOn = startAt + perPage;

            // Added few lines from here start
            if (numPages > 10 && page > 5) {
                $('.pg').hide();
                var count = 0;
                var start = 0;
                if (numPages > page + 5) {
                    start = page - 4;
                }
                else {
                    start = numPages - 9;
                }
                while (count < 10) {
                    $("#pg" + (start+count)).show();
                    count++;
                }
            }
            else {
                $('.pg').hide();
                var count = 0;
                var max = numPages >= 10 ? 10 : numPages;
                while (count < max) {
                    $("#pg" + (count+1)).show();
                    count++;
                }
            }
            var currpg = $("#pg" + (page + 1)).show();
            currpg.addClass("active").siblings().removeClass("active");
            if (parseInt(page) + 1 == numPages) {
                $("#pagedata").text("Page " + (parseInt(page) + parseInt(1)).toString() + " of " + numPages + ", items " + (parseInt(startAt)+1) + " to " + numItems + " of " + numItems);
            }
            else {
                $("#pagedata").text("Page " + (parseInt(page) + parseInt(1)).toString() + " of " + numPages + ", items " + (parseInt(startAt) + 1) + " to " + endOn + " of " + numItems);
            }
            $("#pagedata").show();

            
            currpg.addClass("active").siblings().removeClass("active");
            // Added few lines till here end

            children.css('display', 'none').slice(startAt, endOn).show();

            if (page >= 1) {
                pager.find('.prev_link').show();
                pager.find('.first_page').show();
            } else {
                pager.find('.prev_link').hide();
                pager.find('.first_page').hide();
            }

            if (page < (numPages - 1)) {
                pager.find('.next_link').show();
                pager.find('.last_page').show();
            } else {
                pager.find('.next_link').hide();
                pager.find('.last_page').hide();
            }

            pager.data("curr", page);
        }
    }
})(jQuery);