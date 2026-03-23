$(document).ready(function () {
    ClearStorage();
    $("#btnlogin").on("click", Login);

    var empresa = GetQueryStringParams("empresa");    
    /*if (empresa == "inmot") {//INDIAN MOTOS
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
    }*/

    if (empresa == "transortiz") {//TRANSPORTES ORTIZ
        $(".login-logo").html(" <a href='#'><img src='images/tortiz.png' alt='' width='300'></a>");
        SetOnlineCompany("16", false);
    }

    if (empresa == "carlogistica") {//CARLOGISTICA
        $(".login-logo").html(" <a href='#'><img src='images/carlogistica.png' alt='' width='300'></a>");
        SetOnlineCompany("13", false);
    }

    /*if (empresa == "cercom") {//CERCOM
        $(".login-logo").html(" <a href='#'><img src='images/cercom.png' alt='' width='300'></a>");
        SetOnlineCompany("12", false);
    }


    if (empresa == "brysear") {//BRYSEAR
        $(".login-logo").html(" <a href='#'><img src='images/logobrysear.png' alt='' width='300'></a>");
        SetOnlineCompany("23", false);
    }

    if (empresa == "jemarketing") {//JYE
        $(".login-logo").html(" <a href='#'><img src='images/logoada.jpg' alt='' width='300'></a>");
        SetOnlineCompany("24", false);
    }*/

    if (empresa == "redesk") {//REDESK
        $(".login-logo").html(" <a href='#'><img src='images/redesk.jpg' alt='' width='300'></a>");
        SetOnlineCompany("21", false);
    }
    /*if (empresa == "tmc") {//CERCOM
        $(".login-logo").html(" <a href='#'><img src='images/tmc.jpg' alt='' width='100'></a>");
        SetOnlineCompany("20", false);
    }
    if (empresa == "inmosur") {//INMOSUR
        $(".login-logo").html(" <a href='#'><img src='images/inmosur.jpg' alt='' width='100'></a>");
        SetOnlineCompany("26", false);
    }
    if (empresa == "ivc") {//IVC VEHICOMERCIAL
        $(".login-logo").html(" <a href='#'><img src='images/vehicomercial.jpeg' alt='' width='100'></a>");
        SetOnlineCompany("28", false);
    }*/
    if (empresa == "bp") {//BANCO DEL PERNO
        $(".login-logo").html(" <a href='#'><img src='images/logobp.png' alt='' width='100'></a>");
        SetOnlineCompany("29", false);
    }
    if (empresa == "corpbp") {//CORPORACION BANCO DEL PERNO
        $(".login-logo").html(" <a href='#'><img src='images/logobp.png' alt='' width='100'></a>");
        SetOnlineCompany("30", false);
    }



    if (empresa == "") {//Administrador
        SetOnlineCompany("", false);
    }


});




function Login() {

    var obj = {};
    obj["usr_id"] = $(".user").val();
    obj["usr_password"] = $(".password").val();
    obj["uxe_usuario"] = $(".user").val();
    obj["uxe_empresa"] = GetOnlineCompany();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "Login", jsonText, 0);
}

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            if (data.d == "OK") {
                SetOnlineUser($(".user").val(), GetBooleanCheckValue($("#chkmantener")));
                window.location.href = indexpage;
            }            
            else
                Message("error", data.d, "center", "top", false);
            
        }
    }
}
