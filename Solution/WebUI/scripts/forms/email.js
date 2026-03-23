/****  Summernote Editor  ****/

$(document).ready(function () {

    SetIcons();
    GetMenu("Comprobantes");
    /****  CKE Editor  ****/
    if ($('.cke-editor').length && $.fn.ckeditor) {
        $('.cke-editor').each(function () {
            $(this).ckeditor();
        });
    }

    $("#send").on("click",SendMail)


    //LoadMenu("listado");
    //LoadStatics();
    //GetCabecera();
    //LoadListado();

})


function SendMail() {
    var obj = {};
    obj["to"] = $("#to").val();
    obj["asunto"] = $("#asunto").val();
    obj["empresa"] = GetOnlineCompany();
    obj["body"] = $("#editor1").val();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "SendMail", jsonText, 0);
}


function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            alert(data.d);
        }
      

    }
}