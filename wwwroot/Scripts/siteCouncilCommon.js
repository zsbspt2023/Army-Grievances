$(document).ready(function () {
    $('.numeric').keypress(function (event) {
        if (event.which != 8 && isNaN(String.fromCharCode(event.which))) {
            event.preventDefault();
        }
    });
    $('.numericLength3').keypress(function (event) {
        //console.log(event.which);
        if (parseInt(event.target.value) > 99) {
            event.preventDefault();
        }
    });
    $('.numericLength3').on('change', function () {
        if ($(this).val() === "") {
            $(this).val("1");
        }
        if (parseInt($(this).val()) <= 0) {
            $(this).val("1");
        }
    });
    $('.numericLength7').keypress(function (event) {
        //console.log(event.which);
        if (parseInt(event.target.value) > 999999) {
            event.preventDefault();
        }
    });
   
  
});
$(document).ready(function () {
    var uri = window.location.toString();
    if (uri.indexOf("?") > 0) {
        var clean_uri = uri.substring(0, uri.indexOf("?"));
        window.history.replaceState({}, document.title, clean_uri);
    }
});
