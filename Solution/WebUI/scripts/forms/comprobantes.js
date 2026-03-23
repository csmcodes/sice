$(document).ready(function () {

    GetEmpresas();

    $("#cmbempresa").on("change", LoadComprobantes);
    $("#txtfecha").on("change", LoadComprobantes);
    $("#txtnumero").on("change", LoadComprobantes);
    $("#txtcliente").on("change", LoadComprobantes);
    

});

function GetEmpresas() {

    var obj = {};
    //    obj["emp_codigo"] = $("#txtcodigo").val();
    //    obj["emp_ruc"] = $("#txtruc").val();
    //    obj["emp_nombre"] = $("#txtnombre").val();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetEmpresas", jsonText, 3);

}

function LoadComprobantes() {

    var obj = {};
    obj["com_empresa"] = $("#cmbempresa").val();
    obj["com_fecha"] = $("#txtfecha").val();
    obj["com_secuencia"] = $("#txtnumero").val();
    obj["com_nombrecliente"] = $("#txtcliente").val();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "LoadComprobantes", jsonText, 0);

}

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            $("#content").html(data.d);
        }
        if (retorno == 1) {
            alert(data.d);
            LoadComprobantes();
        }
        if (retorno == 2) {
            alert(data.d);
        }
        if (retorno == 3) {
            $("#cmbempresa").html(data.d);
            LoadComprobantes();
        }
    }
}

function Respuesta(clave, tipo) {

    window.open("respuesta.htm?clave=" + clave+"&tipo="+tipo, "", "toolbar=no,location=no,directories=no,status=yes,menubar=no,scrollbars=yes,resizable=yes,width=800,height=700,top=50,left=50");
}


function Reenviar(empresa, clave) {

    var obj = {};
    obj["empresa"] = empresa;
    obj["clave"] = clave;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "EnviarComprobante", jsonText, 1);
}

function Resetear(empresa, clave) {

    var obj = {};
    obj["empresa"] = empresa;
    obj["clave"] = clave;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "ResetComprobante", jsonText, 1);
}


function Verificar(clave) {

    var jsonText = JSON.stringify({ clave: clave });
    CallServerMethods(webservice + "VerificarComprobante", jsonText, 1);
}


function RIDE(empresa, clave) {

    window.open("wfPrintComprobante.aspx?empresa=" + empresa + "&clave=" + clave+"&generar=true");
}


function Mail(empresa, clave) {

    var obj = {};
    obj["empresa"] = empresa;
    obj["clave"] = clave;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "MailComprobante", jsonText, 2);
}