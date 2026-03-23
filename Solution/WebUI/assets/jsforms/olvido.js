$(document).ready(function () {
    
    $("#btnsend").on("click", Reset);


  
});




function Reset() {

    var obj = {};
    obj["email"] = $("#email").val();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "ResetPassword", jsonText, 0);
}

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            if (data.d == "OK") {
                Message("success", "Su nueva contraseña ha sido enviada a la dirección de correo", "center", "top", true);
            }
            else
                Message("error", data.d, "center", "top", false);
            
        }
    }
}