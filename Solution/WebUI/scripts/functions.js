//Archivo:          functions.js
//Descripción:      Contiene Funciones generales 
//Desarrollador:    Cristhian Sanmartin M.
//Fecha:            Agosto  2013
//2013. Gestión Tecnológica GTEC Cía. Ltda. Todos los derechos reservados/// <reference path="functions.js" />



//Es necesario referencias a la siguientes librerias y estilos
//<link rel="stylesheet" href="assets/plugins/jnotify/jNotify.jquery.css">
//<script src="assets/plugins/jnotify/jNotify.jquery.min.js"></script>
//<script src="assets/js/notifications.js"></script>
//horizpos = right, left, center
//vertipos = top, center, bottom
//overlay = true, false

function FastMessage(tipo, mensaje) {
    Message(tipo, mensaje, null, null, null);
}

function MessageFull(tipo, mensaje, horizpos, vertipos, overlay, tiempo) {
    /**** INFO MESSAGE TYPE ****/
    if (tipo == 'info') {
        jNotify(
                mensaje, {
                    HorizontalPosition: (horizpos != null) ? horizpos : "right",
                    VerticalPosition: (vertipos != null) ? vertipos : "bottom",
                    ShowOverlay: (overlay != null) ? overlay : false,
                    TimeShown: (tiempo != null) ? tiempo : 5000,
                    OpacityOverlay: $(this).data("opacity") ? $(this).data("opacity") : 0.5,
                    MinWidth: $(this).data("min-width") ? $(this).data("min-width") : 250
                });
    }

    /**** SUCCESS MESSAGE TYPE ****/
    else if (tipo == 'success') {
        jSuccess(
               mensaje, {
                   HorizontalPosition: (horizpos != null) ? horizpos : "right",
                   VerticalPosition: (vertipos != null) ? vertipos : "bottom",
                   ShowOverlay: (overlay != null) ? overlay : false,
                   TimeShown: (tiempo != null) ? tiempo : 3000,
                   OpacityOverlay: $(this).data("opacity") ? $(this).data("opacity") : 0.5,
                   MinWidth: $(this).data("min-width") ? $(this).data("min-width") : 250,
                   onClosed: function () { // added in v2.0
                       try {
                           MessageClose();
                       }
                       catch (err) {
                       }
                   },
                   onCompleted: function () { // added in v2.0
                       try {
                           MessageCompleted();
                       }
                       catch (err) {
                       }
                   }
               });
    }

    /**** ERROR MESSAGE TYPE ****/
    else if (tipo == 'error') {
        jError(
                mensaje, {
                    HorizontalPosition: (horizpos != null) ? horizpos : "right",
                    VerticalPosition: (vertipos != null) ? vertipos : "bottom",
                    ShowOverlay: (overlay != null) ? overlay : false,
                    TimeShown: (tiempo != null) ? tiempo : 2000,
                    OpacityOverlay: $(this).data("opacity") ? $(this).data("opacity") : 0.5,
                    MinWidth: $(this).data("min-width") ? $(this).data("min-width") : 250
                });
    }
}

function Message(tipo, mensaje, horizpos, vertipos, overlay) {
    MessageFull(tipo, mensaje, horizpos, vertipos, overlay, null);
}
///////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////STORAGE FUNCTIONS ////////////////////////////////////

function ClearStorage() {
    sessionStorage.clear();
    localStorage.clear();
}

function SetStorage(obj, id, persistent) {
    var jsonText = JSON.stringify(obj);
    if (persistent)
        localStorage[id] = jsonText;
    else
        sessionStorage[id] = jsonText;
}

function GetStorage(id) {
    var jsonText = sessionStorage.getItem(id);
    if (jsonText == null)
        jsonText = localStorage.getItem(id);
    var obj = $.parseJSON(jsonText);
    return obj;
}

////////////////////////////////////////////////////////////////////////////


///////////////////////////FUNCIONES BASICAS ///////////////////////////////////////

function GetBooleanCheckValue(obj) {
    var valor = GetCheckValue(obj)
    if (valor == 1)
        return true;
    else
        return false;
    //return null;
}

function GetCheckValue(obj) {
    if ($(obj).length > 0) {
        if ($(obj)[0].checked)
            return 1;
        else
            return 0;
    }
    //return null;
}

function SetCheckValue(value) {
    if (value != null) {
        if (value.toString() == "1")
            return true;
    }
    return false;
    //return null;
}

function GetDateValue(value) {
    if (value != null) {
        var date = new Date(parseInt(value.substr(6)));
        return date.toLocaleString();
        //return eval(value.slice(1, -1));
    }
    return "";
}

function GetDateStringValue(value) {
    if (value != null) {
        var date = new Date(parseInt(value.substr(6)));
        var twoDigitMonth = date.getUTCMonth() + 1 + ""; if (twoDigitMonth.length == 1) twoDigitMonth = "0" + twoDigitMonth;
        var twoDigitDate = date.getUTCDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
        var currentDate = twoDigitDate + "/" + twoDigitMonth + "/" + date.getUTCFullYear();

        return currentDate;
        //return eval(value.slice(1, -1));
    }
    return "";
}

function complete3(event) {
    var id = event.target.id;
    var value = pad($("#" + id).val(), 3)
    $("#" + id).val(value);
}
function complete9(event) {
    var id = event.target.id;
    var value = pad($("#" + id).val(), 9)
    $("#" + id).val(value);
}
function pad(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}

function CurrencyFormatted(amount) {
    var i = parseFloat(amount);
    if (isNaN(i)) { i = 0.00; }
    var minus = '';
    if (i < 0) { minus = '-'; }
    i = Math.abs(i);
    i = parseInt((i + .005) * 100);
    i = i / 100;
    s = new String(i);
    if (s.indexOf('.') < 0) { s += '.00'; }
    if (s.indexOf('.') == (s.length - 2)) { s += '0'; }
    s = minus + s;
    return s;
}

function DateFormatted(d) {
    var day = d.getDate();
    var month = d.getMonth() + 1;
    var year = d.getFullYear();
    if (day < 10) {
        day = "0" + day;
    }
    if (month < 10) {
        month = "0" + month;
    }
    var date = day + "/" + month + "/" + year;
    return date;
}

function CleanString(s) {

    s = s.replace(/[\r\n]+/, '');
    s = s.trim();
    return s;


}


function sleep(milliseconds) {
    var start = new Date().getTime();
    for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
            break;
        }
    }
}


//////CARGA EL MENU/////////////////////////////////////////////////

function LoadMenu(menu) {
    var usr = GetOnlineUser();
    var obj = {};
    obj["email"] = usr;
    obj["menu"] = menu;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods("ws/Metodos.asmx/GetMenu", jsonText, "MENU");
}


function LoadStatics() {
    var jsonText = JSON.stringify({});
    CallServerMethods("ws/Metodos.asmx/GetStatics", jsonText, "STATICS");
}

/////////CARGA ICONOS///////////////////////////////////////
function SetIcons() {

    var empresa = GetOnlineCompany();
    if (empresa == "1") {//INDIAN MOTOS
        $(".navbar-brand").html(" <a href='#'><img src='images/inmot.png' alt=''  height='26'></a>");
    }
    if (empresa == "2") {//KARNATAKA
        $(".navbar-brand").html(" <a href='#'><img src='images/karnataka.png' alt=''height='26'></a>");
    }
    if (empresa == "3") {//VYCAST
        $(".navbar-brand").html(" <a href='#'><img src='images/vycast.png' alt=''  height='26'></a>");
    }
}



//////OBTIENE LOS VALORES DEL QUERY STRING///////////////////////////

function GetQueryStringParams(sParam) {
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return sParameterName[1];
        }
    }
}



