$(document).ready(function () {

    LoadMenu("set");
    LoadStatics();    

    $("#btnsave").on("click", Save);
    $("#btncancel").on("click", Cancel);
    $("#btnoctavos").on("click", Octavos);
    $("#btncuartos").on("click", Cuartos);
    $("#btnsemi").on("click", Semi);
    $("#btnfinal").on("click", Final);    


});


function Save() {
        var obj = {};
        var usr = GetOnlineUser();
        obj["numero"] = $("#numero").val();
        obj["scorelocal"] = $("#scorelocal").val();
        obj["scorevisitante"] = $("#scorevisitante").val();
        var jsonText = JSON.stringify({ objeto: obj });
        CallServerMethods(webservice + "SavePartido", jsonText, 0);
}

function Cancel() {
    window.location.href = indexpage;
}


function Octavos() {
    var obj = {};
    var jsonText = JSON.stringify({});
    //var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "ErroresOctavos", jsonText, 1);
}


function Cuartos() {
    var obj = {};
    var jsonText = JSON.stringify({});
    //var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "ActualizarCuartos", jsonText, 1);
}


function Semi() {
    var obj = {};
    var jsonText = JSON.stringify({});
    //var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "ActualizarSemi", jsonText, 1);
}

function Final() {
    var obj = {};
    var jsonText = JSON.stringify({});
    //var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "ActualizarFinal", jsonText, 1);
}




function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {         
            if (data.d == "OK") {
                Message("success", "Datos actualizados correctamente", "center", "center", true);

            }
            else
                Message("error", data.d, "center", "top", false);
        }
    }
}

function MessageCompleted() {

    GetUsuario();
}