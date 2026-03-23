
var stop = false;

$(document).ready(function () {
    var d = new Date();
    var time = d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
    var time = d.toLocaleTimeString();
    $("#txthasta").val(convertDate(d));
    //$("#txthasta").val(d.getDate() + "/" + (d.getMonth()+1) + "/" + d.getFullYear());

    var di = new Date();
    di.setDate(di.getDate() - 30);
    $("#txtdesde").val(convertDate(di));
    //$("#txtdesde").val(di.getDate() + "/" + (di.getMonth()+1) + "/" + di.getFullYear());
   // run();
});

function convertDate(inputFormat) {
    function pad(s) { return (s < 10) ? '0' + s : s; }
    var d = new Date(inputFormat);
    return [pad(d.getDate()), pad(d.getMonth() + 1), d.getFullYear()].join('/');
}

function ControlRobot() {
    var texto = $("#btncontrol").val();
    if (texto == "Correr") {
        $("#btncontrol").val("Parar");
        stop = false;
        CheckComprobantes();
    }
    else {
        $("#btncontrol").val("Correr");
        stop = true;        
    }
}




function run() {
    var tiempo = parseInt($("#txttime").val());
    //myVar = setTimeout(mensaje, tiempo);    
    myVar = setTimeout(CheckComprobantes, tiempo);    


}

function mensaje() {
    $("#datos").prepend($.now().toString() + " <br>");
    run();
}

function CheckComprobantes() {
    var obj = {};    
    //obj["com_empresa"] = $("#cmbempresa").val();
    obj["com_fecha"] = $("#txtdesde").val();
    obj["crea_fecha"] = $("#txthasta").val();
    //obj["com_estado"] = $("#cmbestado").val();
    obj["com_ambiente"] = $("#txtambiente").val();
    obj["top"]= $("#txttop").val();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "CheckComprobantes", jsonText, 0);
}

function DevComprobantes() {
    var obj = {};
    //obj["com_empresa"] = $("#cmbempresa").val();
    obj["com_fecha"] = $("#txtdesde").val();
    obj["crea_fecha"] = $("#txthasta").val();
    //obj["com_estado"] = $("#cmbestado").val();
    obj["com_ambiente"] = $("#txtambiente").val();
    obj["top"] = $("#txttop").val();
    obj["sleep"] = $("#txttime").val
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "DevueltoComprobantes", jsonText, 1);
}
function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            $("#datos").prepend(data.d);
            if (!stop)
                run();
        }
        if (retorno == 1)
        {
            $("#datos").prepend(data.d);
        }
    }
}