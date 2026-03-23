//Archivo:          server.js
//Descripción:      Contiene los metodos para conectarse con el servidor y traer datos de BD
//Desarrollador:    Cristhian Sanmartin M.
//Fecha:            Mayo  2014
//2014. Gestión Tecnológica GTEC Cía. Ltda. Todos los derechos reservados

//Funciona que invoca al servidor mediante JSON


var webservice = "ws/Metodos.asmx/";

function CallServerMethods(strurl, strdata, retorno) {


    $.ajax({
        type: "POST",
        url: strurl,
        data: strdata,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            try {
                if (retorno == "MENU") {
                    $("#main-menu").html(data.d);
                }
                else if (retorno == "STATICS") {
                    var obj = $.parseJSON(data.d);
                    //$("#numusuarios").html(obj[0]);
                    $("#acumulado").html(obj[1]);
                    //$("#juegosconf").html(obj[2]);
                }
                else
                    ServerResult(data, retorno);
            }
            catch (err) {
                alert(err.Message);
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            var errorData = $.parseJSON(XMLHttpRequest.responseText);
            alert(errorData.Message);            
        }

    })
}


function GetMenu(menuoption) {

    var obj = {};

    obj["uxe_usuario"] = GetOnlineUser();
    obj["uxe_empresa"] = GetOnlineCompany();
    obj["path"] = menuoption;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetMenu", jsonText, "MENU");
}