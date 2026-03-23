$(document).ready(function () {
    SetIcons();
    GetMenu("Envio");
});


function UploadXML() {

    //var jsonText = JSON.stringify({ xml: $("#txtxml").val()});
    //CallServerMethods(webservice + "UploadComprobante", jsonText, 0);

    var obj = {};
    obj["mail"] = $("#txtmail").val();
    obj["xml"] = $("#txtxml").val();
    obj["tipo"] = $("#txttipo").val();
    if (Validar(obj)) {
        var jsonText = JSON.stringify({ objeto: obj });
        //CallServerMethods(webservice + "UploadComprobante", jsonText, 0);
        CallServerMethods(webservice + "RecibirComprobanteObj", jsonText, 0);
    }
    //CallServerMethods(webservice + "RecibirComprobanteObj", jsonText, 0);

}

function Validar(obj)
{
    
    if (obj["mail"]=="" ||obj["xml"]== undefined)
    {
        alert("Mail es obligatorio");
        return false;
    }
    if (obj["tipo"] == "" || obj["tipo"] == undefined) {
        alert("Tipo es obligatorio (1 o 2)");
        return false;
    }
    if (obj["xml"] == "" || obj["xml"] == undefined) {
        alert("XML es obligatorio");
        return false;
    }
    return true;
}

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            alert(data.d);
        }      
    }
}