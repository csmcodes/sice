$(document).ready(function () {

    LoadMenu("config");
    LoadStatics();
    GetUsuario();


    $("#btnsave").on("click", Save);
    $("#btncancel").on("click", Cancel);
});

function GetUsuario() {
    var obj = {};
    var usr = GetOnlineUser();
    obj["email"] = usr;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetUsuario", jsonText, 0);
}


function Valida() {

    if ($("#usrnombres").val() == "") {
        Message("error", "El campo nombres es obligatorio", "center", "top", false);
        return false;
    }

    if ($("#usrpassword").val() == "") {
        Message("error", "El campo password es obligatorio", "center", "top", false);
        return false;
    }

    if ($("#usrpassword").val() != $("#usrpasswordconf").val()) {
        Message("error", "El password no coincide", "center", "top", false);
        return false;
    }    
    return true;
}

function Save() {
    if (Valida()) {
        var obj = {};
        var usr = GetOnlineUser();
        obj["email"] = usr;
        obj["nombres"] = $("#usrnombres").val();
        obj["password"] = $("#usrpassword").val();
        var jsonText = JSON.stringify({ objeto: obj });
        CallServerMethods(webservice + "SaveUsuario", jsonText, 1);
    }
}

function Cancel() {
    window.location.href = indexpage;
}


function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            var obj = $.parseJSON(data.d);
            $("#usrname").html(obj["nombres"]);
            $("#usrrol").html("<i class='fa fa-star p-r-5 c-blue'></i>"+obj["rol"]);
            $("#usremail").html("<i class='fa fa-briefcase p-r-5 c-brown'></i>"+obj["email"]);
            $("#usrnombres").val(obj["nombres"]);            
            $("#usrpassword").val(obj["password"]);
            $("#usrpasswordconf").val(obj["password"]);

        }
        if (retorno == 1) {
            if (data.d == "OK") {
                Message("success", "Usuario actualizado correctamente", "center", "center", true);

            }
            else
                Message("error", data.d, "center", "top", false);
        }
    }
}

function MessageCompleted() {
   
    GetUsuario();
}