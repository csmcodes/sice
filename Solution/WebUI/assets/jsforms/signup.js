$(document).ready(function () {

    $("#btnsignup").on("click", SignUp);



});

function Valida() {

    if ($("#nombres").val() == "") {
        Message("error", "El campo nombres es obligatorio", "center", "top", false);
        return false;
    }
    if ($("#email").val() == "") {
        Message("error", "El campo email es obligatorio", "center", "top", false);
        return false;
    }
    if ($("#password").val() == "") {
        Message("error", "El campo password es obligatorio", "center", "top", false);
        return false;
    }

    if ($("#password").val() != $("#passwordconf").val()) {
        Message("error", "El password no coincide", "center", "top", false);
        return false;
    }
    if (GetBooleanCheckValue($("#terminos")) == false) {
        Message("error", "Debe aceptar terminos y condiciones", "center", "top", false);
        return false;
    }
    return true;
}

function SignUp() {
    
    if (Valida()) {
        var obj = {};
        obj["nombres"] = $("#nombres").val();
        obj["email"] = $("#email").val();
        obj["password"] = $("#password").val();

        var jsonText = JSON.stringify({ objeto: obj });
        CallServerMethods(webservice + "SignUp", jsonText, 0);
    }
}

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            if (data.d == "OK") {
                Message("success", "Registro completo, por favor ingrese", "center", "top", true);
                
            }
            else
                Message("error", data.d, "center", "top", false);

        }
        if (retorno == 1) {
            if (data.d == "OK") {
                SetOnlineUser($("#email").val(), false);
                window.location.href = indexpage;
            }
            else
                Message("error", data.d, "center", "top", false);

        }
    }
}

function MessageCompleted() {
    Login();
}

function Login() {

    var obj = {};
    obj["email"] = $("#email").val();
    obj["password"] = $("#password").val();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "Login", jsonText, 1);
}

