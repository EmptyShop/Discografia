$("#Password").val("");
$.each($(".passwordField"), function (index, value) { $(value).hide(); });

$("#cambiarPassword").click(function () {
    if (!$(this).is(":checked")) {
        $("#Password").val("");
        $("#PasswordConfirm").val("");
    }
    $.each($(".passwordField"), function (index, value) { $(value).toggle(); });
})