$(document).ready(function () {


    LoadMenu("listado");
    LoadStatics();
    GetCabecera();
    //LoadListado();

});

function GetCabecera() {
    var obj = {};
    var usr = GetOnlineUser();
    obj["email"] = usr;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetCabeceraListado", jsonText, 0);
}


function LoadListado() {
    var obj = {};
    var usr = $(".selectpicker").val();  
    obj["email"] = usr;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetListado", jsonText, 1);
}

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            $("#contenidocabecera").html(data.d);
            $(".selectpicker").on("change", LoadListado);
            $("#btnadd").on("click", AddNew);
            LoadListado();
        }
        if (retorno == 1)
            $("#contenidodetalle").html(data.d);
        if (retorno == 2)
            LoadListado();
    }
}

function Edit(codigo) {
    window.open("partidos.html?codigo=" + codigo, "", ""); 
}
function Show(codigo) {
    window.open("partidos.html?codigo=" + codigo + "&open=true", "", "");
}



function AddNew() {
    var obj = {};
    var usr = $(".selectpicker").val();  
    obj["email"] = usr;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "AddNew", jsonText, 2);
}