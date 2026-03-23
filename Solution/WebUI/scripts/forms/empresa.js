var Empresa = function () {

    return {

        //main function to initiate the module
        init: function () {
            if (!jQuery().dataTable) {
                return;
            }
            $("#saveempresa").on("click", SaveEmpresa)
            GetEmpresas();

        }

    };

} ();






function GetEmpresas() {

    var obj = {};
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetEmpresas", jsonText, 0);

}

function SaveEmpresa() {
    var obj = GetEmpresaObj();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "SaveEmpresa", jsonText, 1);
}

function CleanForm() {
    $("#txtcodigo").val("");
    $("#txtruc").val("");
    $("#txtnombre").val("");
    $("#chkactivo").attr("checked", true);
}

function GetEmpresaObj() {
    var obj = {};
    obj["emp_codigo"] = $("#txtcodigo").val();
    obj["emp_ruc"] = $("#txtruc").val();
    obj["emp_nombre"] = $("#txtnombre").val();
    obj["emp_estado"] = GetCheckValue($("#chkactivo"));
    obj["crea_usr"] = "admin";
    obj["crea_fecha"] = '01/12/2014';
    return obj;
}

function SetEmpresaObj(obj) {
    CleanForm();
    $("#txtcodigo").val(obj["emp_codigo"]);
    $("#txtruc").val(obj["emp_ruc"]);
    $("#txtnombre").val(obj["emp_nombre"]);
    $("#chkactivo").attr("checked", SetCheckValue(obj["emp_estado"]));
}

function AddNew() {
    CleanForm();
    $('#basic').modal('show');
}


function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {            
            $("#tdempresas").append(data.d);
            SetSelection();
                 
        }
        if (retorno == 1) {
            $("#tdempresas tbody").html("");
            $('#basic').modal('hide');
            GetEmpresas();
        }
        if (retorno == 2) {
            var obj = $.parseJSON(data.d);
            SetEmpresaObj(obj);
            $('#basic').modal('show');
            
        }
    }
}


function EditEmpresa(codigo) {

    var obj = {};
    obj["emp_codigo"] = codigo;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetEmpresa", jsonText, 2);
}

function SetSelection() {
    $('#tdempresas tbody').on('click', 'tr', function () {

        var codigo = $(this).data("codigo");
        EditEmpresa(codigo);
    });
}