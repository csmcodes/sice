$(document).ready(function () {
    GetEmpresas();
    $("#cmbempresa").on("change", LoadAutomataData);
});

function LoadAutomataData() {
    var obj = {};
    obj["empresa"] = $("#cmbempresa").val();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetAutomataData", jsonText, 0);
}

function GetAutomataLogs(ejecucionId) {
    var obj = {};
    obj["ejecucion"] = ejecucionId;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetAutomataLogs", jsonText, 1);
}

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            $("#content").html(data.d);
        }
        if (retorno == 1) {
            $("#logs").html(data.d);
        }
        if (retorno == 3) {
            $("#cmbempresa").html(data.d);
            LoadAutomataData();
        }
    }
}
