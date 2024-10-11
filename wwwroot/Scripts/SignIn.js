
$(document).ready(function () {
    $('#remember_me').attr('checked', 'checked');
    $("input:text,form").attr("autocomplete", "off");
});

function alertifyMessage1(msg) {
    alertify.alert(msg);
    var alertID = document.getElementById("alertify-ok");
    setTimeout(function () {
        if (alertID) {
            alertID.click();
        }
    }, closetime)
    return false;
}

$('#loginidd').click(function (e) {
    debugger;
    if (validateLoginInputs() == false)
        return false;
    if (validateOTPLoginInputs()) {
        e.preventDefault();
        var formData = $('#LoginSubmitForm').serialize();
        $.ajax({
            type: "POST",
            url: "/Account/LoginSubmit",
            data: formData,
            headers: {
                "IsAuthenticated": $('#IsAuthenticated').val()
            },
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    if (response.authenticate) {
                        alertifyMessage1("Please enter the passcode sent to your email address");
                        $('#formpassCode').css('display', 'none');
                        $('#formOTP').css('display', 'block');
                        $('#IsAuthenticated').val('1');
                    } else {
                        if (response.resent) {
                            alertifyMessage1(response.message);
                        } else {
                            window.location.href = response.redirectUrl;
                        }
                    }
                } else {
                    alertifyMessage1(response.message);
                    $('#resendOTP').css('display', 'inline-block');
                    $('.resendOTP-height').css('display', 'block');
                }
            }, error: function (xhr, status, error) {
                alertifyMessage1(error);
            }
        });
    } else {
        var formPassCode = $('#formpassCode').is(":visible");
        if (!formPassCode) {
            e.preventDefault();
        }
        else if (formPassCode && !validateLoginInputs()) {
            e.preventDefault();
        }
    }
});

$('#resendOTP').click(function (e) {
    e.preventDefault();

    $.ajax({
        type: "POST",
        url: "/Account/ResendAuthenticationOTP",
        success: function (response) {
            if (response.success) {
                alertifyMessage1(response.message);
            }
        }, error: function (xhr, status, error) {
            alertifyMessage1(error);
        }
    });
});


const PortolotogglePassword = document.querySelector('#PortolotogglePassword');
const Portolopassword = document.querySelector('#id_password');

PortolotogglePassword.addEventListener('click', function (e) {
    // toggle the type attribute

    const type = Portolopassword.getAttribute('type') === 'password' ? 'text' : 'password';
    Portolopassword.setAttribute('type', type);

    // toggle the eye slash icon
    //this.classList.toggle('fa-eye-slash');
    if (type == "password") {
        PortolotogglePassword.src = "/images/icons/eye.gif";
    } else {

        PortolotogglePassword.src = "/images/icons/hideeye.png";
    }
});


$('#login').click(function () {
    var error = "";
    var phone = $.trim($('#id_username').val());
    var password = $.trim($('#id_password').val());
    if (phone == "") {
        error += "&#9679  Enter Mobile Number or UserName  <br>";
        $('#id_username').css('background-color', 'yellow');
    }
    if (password == "") {
        error += "&#9679  Enter Password  <br>";
        $('#id_password').css('background-color', 'yellow');
    }
    if ($('#remember_me').is(':checked')) {
        // save username and password
        localStorage.usrname = $('#id_username').val();
        localStorage.pass = $('#id_password').val();
        localStorage.chkbx = $('#remember_me').val();
    }
    else if (ViewBag.MaxFailed == true) {
        document.getElementById("login").disabled = true;
        setInterval(function () { document.getElementById("login").disabled = false; }, 30000);
    }
    else {
        localStorage.usrname = '';
        localStorage.pass = '';
        localStorage.chkbx = '';
    }
    if (error != "") {
        var lines = error.split("<br>").length;
        if (lines > 2) {
            var errormsg = "Please correct all enteries before Sign In<br>";
            errormsg += error;
            alertify.alert(errormsg);
        }
        else
            alertify.alert(error);
        return false;
    }
});

$('#ForgetPass').click(function () {
    if ($('#id_username').val().trim().indexOf("@@") !== -1)
        $('#txtEmailAddress').val($('#id_username').val());
    else
        $('#txtEmailAddress').val("")
    $('#modelRWPass').modal('show');
});

$('#cmdGetHelp,#cmdGetHelpPCode').click(function (e) {
    e.preventDefault();
    $('input[name="rblNeedHelp"][value="0"]').prop("checked", "checked");
    var id = $('#id_username').val();
    if (id.indexOf('@@') > -1)
        $('#txtPhoneNo').val(id);
    else
        $('#txtPhoneNo').val("");
    //$('#modelRwNeedHelp').modal('show');
    //$('.modal-content').draggable();
    $('.help').toggle();
});

$('#NeedHelpProceed1').click(function (e) {
    var error = "";
    var PhoneNo = $('#txtPhoneNo').val().trim();
    if (PhoneNo == "") {
        error += "&#9679 Enter Mobile Number <br>";
        $('#txtPhoneNo').css('background-color', 'yellow');
    }
    else if (PhoneNo.length != 10) {
        error += "&#9679 Please enter 10 digit Mobile Number <br>";
        $('#txtPhoneNo').css('background-color', 'yellow');
    }
    else if (PhoneNo != "" && PhoneNo.length == 10) {

        $.ajax({
            url: "/SignIn/checkExistMobileNo",
            datatype: 'json',
            data: { mobileNo: PhoneNo },
            async: false,
            success: function (data) {
                if (data == "true") {
                    window.location.href = "/SignIn/ResetPassword?mobileno=" + PhoneNo;
                }
                else {
                    $('#txtPhoneNo').val("");
                    error = "Mobile number not Exist! Please enter a Registered mobile number.";
                }
            },
            error: function (result) { }
        });
    }
    if (error != "") {
        var lines = error.split("<br>").length;
      
            var errormsg = "";
            errormsg += error;
            alertify.alert(errormsg);
            var alertID = document.getElementById('alertify-ok'); if (alertID) {
                setTimeout(function () { alertID.click(); }, 5000);
            }
            return false;
       
    }
});
function validateLoginInputs() {
    var emailReg = /^([\w-\.]+)@@((\[[0-9]{1, 3}\.[0-9]{1, 3}\.[0-9]{1, 3}\.)|(([\w-]+\.)+))([a-zA-Z]{2, 4}|[0-9]{1, 3})(\]?)$/
    var formPassCode = $('#formpassCode').is(":visible");
    var userName = $('#id_username').val();
    var passcode = $('#id_passcode').val();
    if (formPassCode) {
        var error = "";
        if (userName == "" && passcode == "") {
            error = "Enter email address and passcode";
        }
        else if (userName == "") {
            error = "Enter email address";
        }
        else if (passcode == "") {
            error = "Enter passcode";
        }
        else if (!userName.match(emailReg)) {
            error = "Invalid email address";
        }
        if (error != "") {
            alertify.alert(error);
            var alertID = document.getElementById("alertify-ok");
            setTimeout(function () {
                if (alertID) { alertID.click(); }
            }, closetime)
            return false;
        }
        return true;
    }
    else {
        return true;
    }
}


function validateOTPLoginInputs() {
    //var emailReg = /^([\w-\.]+)@@((\[[0-9]{1, 3}\.[0-9]{1, 3}\.[0-9]{1, 3}\.)|(([\w-]+\.)+))([a-zA-Z]{2, 4}|[0-9]{1, 3})(\]?)$/
    var formOTP = $('#formOTP').is(":visible");
    var userName = $('#id_username').val();
    var password = $('#id_password').val();
    var isCheckPassword = $('#id_password').is(':disabled');
    var otp = $('#id_otp').val();
    if (formOTP) {
        var error = "";
        if (userName == "" && otp == "") {
            error = "Enter email address and passcode";
        }
        else if (userName == "") {
            error = "Enter email address";
        }
        else if (!isCheckPassword && password == "") {
            error = "Enter password";
        }
        else if (otp == "") {
            error = "Enter passcode";
        }
        if (error != "") {
            alertify.alert(error);
            var alertID = document.getElementById("alertify-ok");
            setTimeout(function () {
                if (alertID) { alertID.click(); }
            }, closetime)
            return false;
        }
        return true;
    }
    else {
        return true;
    }
}



