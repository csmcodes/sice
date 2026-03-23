$(document).ready(function () {

    LoadMenu("mails");
    LoadStatics();
    $("#btnsend").on("click", Send);

});

function Send() {

    var obj = {};
    obj["subject"] = $("#asunto").val();
    obj["body"] = $("#bodymes").val();
    obj["usuario"] = $("#to").val();
    obj["estado"] = $("#estado").val();
    obj["pagada"] = $("#pagada").val();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "EnviarMails", jsonText, 0);
}



function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            //LoadListado();
        }
    }
}

