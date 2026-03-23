$(document).ready(function () {

    SetIcons();
    GetMenu("Comprobante");
    GetComprobanteFromXML();



    //LoadMenu("listado");
    //LoadStatics();
    //GetCabecera();
    //LoadListado();

});

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            $("#main-content").append(data.d);
            $(".fecha").datepicker({
                dateFormat: "dd/mm/yy"
            }); //Setea campos de tipo fecha
                        
        }

    }
}


function GetComprobanteFromXML() {
    var obj = {};
    obj["com_empresa"] = GetQueryStringParams("empresa")
    obj["com_numero"] = GetQueryStringParams("numero");
    obj["crea_usr"] = GetOnlineUser();

    
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetComprobanteFromXML", jsonText, 0);
}


