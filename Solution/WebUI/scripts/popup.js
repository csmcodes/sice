 
var webservice = "ws/Metodos.asmx/";

//Funciona que invoca al servidor mediante JSON
function CallServerPopup(strurl, strdata, retorno) {
    //ClearValidate();
    $.ajax({
        type: "POST",
        url: strurl,
        data: strdata,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            //if (retorno == 0)
            //    CallPersonaResult(data);
            if (retorno == 1)
                EditComprobanteResult(data);
            if (retorno == 2)
                SaveComprobanteResult(data);
            if (retorno == 3)
                EditComprobanteXMLResult(data);
            if (retorno == 4)
                SaveComprobanteXMLResult(data);
            if (retorno == "GetMails")
                GetMailsComprobanteResult(data);
            

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            var errorData = $.parseJSON(XMLHttpRequest.responseText);
            jQuery.alerts.dialogClass = 'alert-danger';
            jAlert(errorData.Message, 'Error', function () {
                jQuery.alerts.dialogClass = null; // reset to default
            });
        }

    })
}

function PopUpOk() {
    var id = $(this)[0].id.replace("btnok", "");
    if (id == "modComprobante") {
//        var data = $("#documentdata").data();
        //        SaveDocument(data["empresa"], data["proyecto"], data["nivel"], data["nivelvalor"], data["periodo"], data["resultado"], data["actividad"], data["variable"], data["indicador"])
        SaveComprobante();
        KillPopUp(id);
    }
   
}

function PopUpCancel() {
    var id = $(this)[0].id.replace("btncancel", "");
    KillPopUp(id)
}




function CallPopUp(idpopup, titulo, clase, clase1, htmlcontent) {
    KillPopUp(idpopup);    
    
    var popup = $('<div class="modal fade '+clase+'" id="' + idpopup + '" tabindex="-1"><div class="modal-dialog '+clase1+'"><div class="modal-content"> <div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button><h4 class="modal-title">' + titulo + '</h4></div><div class="modal-body">' + htmlcontent + '</div><div class="modal-footer"><button type="button"  id="btnok' + idpopup + '"  class="btn btn-success">Aceptar</button><button type="button" id="btncancel' + idpopup + '" class="btn btn-default" data-dismiss="modal">Cancelar</button></div></div></div></div>');
    $("#popups").append(popup);
    $('#' + idpopup).modal('show');
    $("#btnok" + idpopup).on("click", PopUpOk);
    $("#btncancel" + idpopup).on("click", PopUpCancel);
}


function KillPopUp(idpopup) {
    
    $('#' + idpopup).modal('hide');
    var popup = $("#" + idpopup);
    $(popup).remove();
    $('.modal-backdrop').remove();
}




//POPUP CALL EDIT COMPROBANTE

function EditComprobante(empresa, numero) {
    var obj = {};
    obj["com_empresa"] = empresa;
    obj["com_numero"] = numero;
    obj["crea_usr"] = GetOnlineUser();
    
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerPopup(webservice+ "EditComprobante", jsonText, 1);
}

var dialogcom;

function EditComprobanteResult(data) {
    if (data != "") {

        dialogcom= bootbox.dialog({
            title: 'Datos Comprobante',            
            message: data.d,
            buttons: {
                aceptar:
                {
                    label: "Aceptar",
                    className: "btn-primary",
                    callback: function () {
                        SaveComprobante();
                    }
                },
                cancelar:
                {
                    label: "Cancelar",
                    className: "btn-default",
                }
            }
        });



        CallPopUp("modComprobante", "Datos Comprobante", "", "", data.d);
    }
}


function SaveComprobante() {

    var obj = {};
    obj["com_empresa"] = $("#editcomprobante").data("empresa");
    obj["com_numero"] = $("#txtclave_e").val();
    obj["com_email"] = $("#txtmail_e").val();
    obj["com_estado"] = $("#cmbestado_e").val();
    obj["mod_usr"] = GetOnlineUser();
    obj["mod_fecha"] = new Date($.now());
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerPopup(webservice + "SaveComprobante", jsonText, 2);
}

function SaveComprobanteResult(data) {
    if (data.d != "") {
        var obj = $.parseJSON(data.d);
        EndEdit(obj);
    }
}




function EditComprobanteXML(empresa, numero) {
    var obj = {};
    obj["com_empresa"] = empresa;
    obj["com_numero"] = numero;
    obj["crea_usr"] = GetOnlineUser();

    var jsonText = JSON.stringify({ objeto: obj });
    CallServerPopup(webservice + "EditComprobanteXML", jsonText, 3);
}

var dialogxml;

function EditComprobanteXMLResult(data) {
    if (data != "") {

        dialogxml = bootbox.dialog({
            title: 'XML Comprobante',
            size: 'large',
            message: data.d,
            buttons: {
                    aceptar:
                    {
                        label: "Aceptar",
                        className: "btn-primary",
                        callback: function () {
                            var obj = {};
                            obj["arc_empresa"] = $("#editcomprobante").data("empresa");
                            obj["arc_numero"] = $("#editcomprobante").data("numero");
                            obj["arc_xml"] = $("#txtxml").val();
                            obj["mod_usr"] = GetOnlineUser();
                            obj["mod_fecha"] = new Date($.now());
                            var jsonText = JSON.stringify({ objeto: obj });
                            CallServerPopup(webservice + "SaveComprobanteXML", jsonText, 4);
                        }
                    },
                    cancelar:
                    {
                        label: "Cancelar",
                        className: "btn-default",
                    }
                }
            });

        
    }
}

function SaveComprobanteXMLResult(data)
{
    if (data.d == "OK") {
        bootbox.alert("XML guardado exitosamente...");
        dialogxml.modal("hide");
    }
    else
    {
        bootbox.alert(data.d);
    }
}



///////////////////VER MAILS ENVIADOS/////////////////////

function GetMailsComprobante(empresa, numero) {
    var obj = {};
    obj["com_empresa"] = empresa;
    obj["com_numero"] = numero;
    obj["crea_usr"] = GetOnlineUser();

    var jsonText = JSON.stringify({ objeto: obj });
    CallServerPopup(webservice + "GetMailsComprobante", jsonText, "GetMails");
}


function GetMailsComprobanteResult(data) {
    if (data != "") {
        var obj = $.parseJSON(data.d);

        bootbox.dialog({
            title: obj[0],
            size: 'large',
            message: obj[1],
            buttons: {
                cancelar:
                {
                    label: "Aceptar",
                    className: "btn-default",
                }
            }
        });


    }
}