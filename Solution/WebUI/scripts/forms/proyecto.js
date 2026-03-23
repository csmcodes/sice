var Proyecto = function () {

    return {

        //main function to initiate the module
        init: function () {
            $("#save").on("click", Save)
            GetEmpresas();
            

        }

    };

} ();


function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            $("#selinstitucion").append(data.d);
            $("#selinstitucion").on("change", GetProyectos);
            $("#selinstitucion").change();
        }
        if (retorno == 1) {
            $("#tdproyectos tbody").html(data.d);
        }
        if (retorno == 2) {
            $("#tdproyectos tbody").html("");
            $('#basic').modal('hide');
            GetProyectos();
        }
        if (retorno == 3) {
            var obj = $.parseJSON(data.d);
            SetObj(obj);
            $('#basic').modal('show');

        }
        if (retorno == 4) {
            $("#tdperiodos tbody").html(data.d);

        }
        if (retorno == 5) {
            $("#tdniveles tbody").html(data.d);

        }
    }
}

function GetEmpresas() {

    var obj = {};
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetEmpresasSelec", jsonText, 0);

}


function GetProyectos() {

    var obj = {};
    obj["pro_empresa"] = $("#selinstitucion").val();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetProyectos", jsonText, 1);

}

function GetPeriodosProyecto() {
    var obj = {};
    obj["per_empresa"] = $("#config").data("empresa");
    obj["per_proyecto"] = $("#config").data("codigo");
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetPeriodosProyecto", jsonText, 4);
}

function GetNivelesProyecto() {
    var obj = {};
    obj["niv_empresa"] = $("#config").data("empresa");
    obj["niv_proyecto"] = $("#config").data("codigo");
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetNivelesProyecto", jsonText, 5);
}



function Save() {
    var obj = GetObj();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "SaveProyecto", jsonText, 2);
}



function CleanForm() {
    $("#txtcodigo").val("");
    $("#txtnombre").val("");
    $("#txtdescripcion").val("");
    $("#txtinicio").val("");
    $("#chkactivo").attr("checked", true);
}

function GetObj() {
    var obj = {};
    obj["pro_codigo"] = $("#txtcodigo").val();
    obj["pro_empresa"] = $("#selinstitucion").val();
    obj["pro_nombre"] = $("#txtnombre").val();
    obj["pro_descripcion"] = $("#txtdescripcion").val();
    obj["pro_inicio"] =  $("#txtinicio").val();
    obj["pro_estado"] = GetCheckValue($("#chkactivo"));
    obj["crea_usr"] = "admin";
    obj["crea_fecha"] = '01/12/2014';
    return obj;
}

function SetObj(obj) {
    CleanForm();
    $("#txtcodigo").val(obj["pro_codigo"]);
    $("#txtdescripcion").val(obj["pro_descripcion"]);
    $("#txtnombre").val(obj["pro_nombre"]);
    $("#txtinicio").val(GetDateValue(obj["pro_inicio"]));
    $("#chkactivo").attr("checked", SetCheckValue(obj["pro_estado"]));
}

function AddNew() {
    CleanForm();
    $('#basic').modal('show');
}





function Edit(obj) {
    var row = obj.parentNode.parentNode;

    var obj1 = {};
    obj1["pro_empresa"] = $(row).data("empresa");
    obj1["pro_codigo"] = $(row).data("codigo");
    var jsonText = JSON.stringify({ objeto: obj1 });
    CallServerMethods(webservice + "GetProyecto", jsonText, 3);
}

function Config(obj) {
    var row = obj.parentNode.parentNode;

    $("#config").data("empresa", $(row).data("empresa"));
    $("#config").data("codigo", $(row).data("codigo"));
    $("#proyectodata").html("<strong>Proyecto " + row.cells[1].innerText + "</strong><br>" + row.cells[2].innerText);

    $("#listado").addClass("hide");
    $("#config").removeClass("hide");

    GetPeriodosProyecto();
    GetNivelesProyecto();
}

function CloseConfig() {
    $("#config").addClass("hide");
    $("#listado").removeClass("hide");
}