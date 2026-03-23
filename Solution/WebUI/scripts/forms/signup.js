$(document).ready(function () {
    ClearStorage();
    $("#btnsignup").on("click", SignUp);

    var empresa = GetQueryStringParams("empresa");
    if (empresa == "inmot") {//INDIAN MOTOS
        $(".login-logo").html(" <a href='#'><img src='images/inmot.png' alt='' width='300'></a>");
        SetOnlineCompany("1", false);
    }
    if (empresa == "karnataka") {//KARNATAKA
        $(".login-logo").html(" <a href='#'><img src='images/karnataka.png' alt='' width='300'></a>");
        SetOnlineCompany("2", false);
    }
    if (empresa == "vycast") {//VYCAST
        $(".login-logo").html(" <a href='#'><img src='images/vycast.png' alt='' width='300'></a>");
        SetOnlineCompany("3", false);
    }
    if (empresa == "newtire") {//NEWTIRE
        $(".login-logo").html(" <a href='#'><img src='images/newtire.png' alt='' width='300'></a>");
        SetOnlineCompany("4", false);
    }
    if (empresa == "cumple") {//EDUARDO CORSINO PALACIOS
        $(".login-logo").html(" <a href='#'><img src='images/cumple.png' alt='' width='150'></a>");
        SetOnlineCompany("5", false);
    }
    if (empresa == "importadora") {//EDUARDO CORSINO PALACIOS
        $(".login-logo").html(" <a href='#'><img src='images/cumple.png' alt='' width='150'></a>");
        SetOnlineCompany("6", false);
    }
    if (empresa == "smunoz") {//SANTIAGO MUÑOZ
        $(".login-logo").html(" <a href='#'><img src='images/smunoz.png' alt='' width='300'></a>");
        SetOnlineCompany("8", false);
    }
    if (empresa == "mduran") {//MONI DURAN
        $(".login-logo").html(" <a href='#'><img src='images/logomoni.png' alt='' width='300'></a>");
        SetOnlineCompany("9", false);
    }
    if (empresa == "imppalacios") {//IMPORTADORA EDUARDO PALACIOS
        $(".login-logo").html(" <a href='#'><img src='images/cumple2.png' alt='' width='300'></a>");
        SetOnlineCompany("10", false);
    }
    if (empresa == "tecnocyclo") {//TECNOCYCLO
        $(".login-logo").html(" <a href='#'><img src='images/tecnocyclo.jpg' alt='' width='300'></a>");
        SetOnlineCompany("11", false);
    }
    if (empresa == "reencandina") {//REENCANDINA
        $(".login-logo").html(" <a href='#'><img src='images/reencandina.png' alt='' width='300'></a>");
        SetOnlineCompany("12", false);
    }

    if (empresa == "transortiz") {//TRANSPORTES ORTIZ
        $(".login-logo").html(" <a href='#'><img src='images/tortiz.png' alt='' width='300'></a>");
        SetOnlineCompany("16", false);
    }

    if (empresa == "carlogistica") {//CARLOGISTICA
        $(".login-logo").html(" <a href='#'><img src='images/carlogistica.png' alt='' width='300'></a>");
        SetOnlineCompany("13", false);
    }

    if (empresa == "cercom") {//CERCOM
        $(".login-logo").html(" <a href='#'><img src='images/cercom.png' alt='' width='300'></a>");
        SetOnlineCompany("12", false);
    }

        
    if (empresa == "") {//Administrador
        SetOnlineCompany("", false);
    }


});



function SignUp() {

    var obj = {};

    obj["usr_id"] = $("#ciruc").val();
    obj["usr_nombres"] = $("#nombres").val();
    obj["usr_mail"] = $("#email").val();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "SignUp", jsonText, 0);
}

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            if (data.d != "ERROR") {
                Message("info", data.d, "center", "top", false);
            }
            else
                Message("error", data.d, "center", "top", false);

        }
    }
}
