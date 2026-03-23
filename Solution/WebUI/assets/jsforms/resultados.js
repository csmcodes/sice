$(document).ready(function () {

    LoadMenu("resultados");
    LoadStatics();
    LoadResultados();

});


function LoadResultados() {
    var obj = {};
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetResultados", jsonText, 0);
}


function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            $("#contenido").html(data.d);
        }
    }
}

function Show(codigo) {
    window.open("ver.html?codigo=" + codigo + "&open=true","Ver","");
}