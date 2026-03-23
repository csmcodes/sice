
var open = true;

$(document).ready(function () {

    LoadMenu("listado");
    LoadStatics();
    var stropen = GetQueryStringParams("open");
    if (stropen != null) {
        if (stropen == "true")
            open = true;
        else
            open = false;
        if (open)
            $("#btnsave").hide();
    }
    GetCabecera();
    LoadPartidos();

    $("#btnver").on("click", Ver);

    $("#btnsave").on("click", Close);
    $("#btnprint").on("click", Print);

    $("#btnupdate").on("click", Update);
    var usrid = GetOnlineUser();
    if (usrid == "csanmartin@gtec.com.ec")
        $("#btnupdate").show();
    else
        $("#btnupdate").hide();

});



function Ver() {
    if (open) {
        open = false;
        $("#btnver").html("<i class='fa fa-toggle-up'></i> Cerrar todos");        
        $('.panel-collapse').collapse('show');
        $('a[data-toggle="collapse"]').removeClass("collapsed");

    }
    else {
        open = true;
        $("#btnver").html("<i class='fa fa-toggle-down'></i> Abrir todos");
        $('.panel-collapse').collapse('hide');
        $('a[data-toggle="collapse"]').addClass("collapsed");
    }

        

    /*$('#collapse-init').click(function () {
        if (active) {
            active = false;
            $('.panel-collapse').collapse('show');
            $('.panel-title').attr('data-toggle', '');
            $(this).text('Enable accordion behavior');
        } else {
            active = true;
            $('.panel-collapse').collapse('hide');
            $('.panel-title').attr('data-toggle', 'collapse');
            $(this).text('Disable accordion behavior');
        }
    });*/

    //$("#accordion2").html("");
    //LoadPartidos(true);
}

function GetCabecera() {
    var obj = {};
    obj["codigo"] = GetQueryStringParams("codigo");
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetCabecera", jsonText, 3);
}

function LoadPartidos(open){
    var obj = {};
    obj["codigo"] = GetQueryStringParams("codigo");    
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetALLPartidos", jsonText, 0);
}

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            $("#accordion2").html(data.d);

            Posiciones();
            Ver();
        }
        if (retorno == 1) {
            if (data.d != "OK")
                Message("error", data.d, "center", "top", false);
        }
        if (retorno == 2) {
            if (data.d == "OK") {
                Message("success", "Juego cerrado correctamente", "center", "center", true);                
               
            }
            else
                Message("error", data.d, "center", "top", false);

        }
        if (retorno == 3) {
            var obj = $.parseJSON(data.d);
            if (obj["pagada"].toString() == "1")
                $("#pagar").hide();
            if (obj["estado"].toString() == "1") {
                $("#recor").show();
                $("#closed").hide();
            }
            if (obj["estado"].toString() == "2") {
                $("#recor").hide();
                $("#closed").show();
            }
        }
        if (retorno == 4) {
            location.reload();
        }
    }
}


function MessageCompleted() {
    window.location.href = "listado.html";
}


function Posiciones() {
    PosicionesGrupo("A");
    PosicionesGrupo("B");
    PosicionesGrupo("C");
    PosicionesGrupo("D");
    PosicionesGrupo("E");
    PosicionesGrupo("F");
    PosicionesGrupo("G");
    PosicionesGrupo("H");
    PosicionesGrupo("OCTAVOS");
    //PosicionesGrupo("CUARTOS");
    //PosicionesGrupo("SEMI");
    //PosicionesGrupo("FINAL");
}

function sortTable(htmltable) {

    var rows = $(htmltable.rows).get();
    rows.sort(function (a, b) {

        var PA = parseInt($(a).children('td.puntos').text());
        var PB = parseInt($(b).children('td.puntos').text());

        var GDA = parseInt($(a).children('td.gd').text());
        var GDB = parseInt($(b).children('td.gd').text());

        if (PA > PB) {
            return -1;
        }

        if (PA < PB) {
            return 1;
        }

        if (PA == PB) {
            if (GDA > GDB) {
                return -1
            }
            else if (GDA < GDB)
                return 1;
            else
                return -1;                
        }
        return 0;

    });
    $.each(rows, function (index, row) {
        $(htmltable).children('tbody').append(row);
    });
}


function SaveChange(row, grupo) {
    PosicionesGrupo(grupo);

    var obj = {}
    obj["cabecera"] = GetQueryStringParams("codigo");
    obj["partido"] = row.id;


    //obj["resultado"] = 1;
    obj["scorelocal"] = $(row).find(".scorelocal").val();
    obj["scorevisitante"] = $(row).find(".scorevisitante").val();
    obj["puntos"] = 0;
    obj["estado"] = 1;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "SaveChanges", jsonText, 1);

}

function Print() {
    $("#logoprint").show();
    $("#accordion2").printThis();
    $("#logoprint").hide();
}

function Close() {
    if (confirm("Los cambios realizados permanecen incluso si abandona el sitio, una vez finalizado el juego no se podrá modificar... ¿Desea continuar?")) {
        var obj = {}
        obj["codigo"] = GetQueryStringParams("codigo");
        var jsonText = JSON.stringify({ objeto: obj });
        CallServerMethods(webservice + "CloseCabecera", jsonText, 2);
    }

}

function PosicionesGrupo(grupo) {

    if (grupo == "A" || grupo == "B" || grupo == "C" || grupo == "D" || grupo == "E" || grupo == "F" || grupo == "G" || grupo == "H") {
        var htmlequipos = $("#grupo" + grupo).find("table.posiciones")[0];
        $(htmlequipos).find(".puntos").html("0");
        $(htmlequipos).find(".gd").html("0");
        var htmltable = $("#grupo" + grupo).find("table.partidos")[0];
        for (var r = 1; r < htmltable.rows.length; r++) {
            var equipolocal = $(htmltable.rows[r]).find(".equipolocal").data("equipo");
            var equipovisitante = $(htmltable.rows[r]).find(".equipovisitante").data("equipo");
            var scorelocal = $(htmltable.rows[r]).find(".scorelocal").val();
            var scorevisitante = $(htmltable.rows[r]).find(".scorevisitante").val();

            var puntosvisitante = 0;
            var puntoslocal = 0;

            if (parseInt(scorelocal) > parseInt(scorevisitante))
                puntoslocal = 3;
            else if (parseInt(scorelocal) < parseInt(scorevisitante))
                puntosvisitante = 3;
            else {
                puntoslocal = 1;
                puntosvisitante = 1;
            }



            for (var i = 1; i < htmlequipos.rows.length; i++) {
                var equipo = $(htmlequipos.rows[i]).find(".equipo").data("equipo");
                var puntos = $(htmlequipos.rows[i]).find(".puntos");
                var gd = $(htmlequipos.rows[i]).find(".gd");
                if (equipo == equipolocal) {
                    var total = parseInt(puntos.html()) + puntoslocal;
                    puntos.html(total);
                    var totalgd = parseInt(gd.html()) + parseInt(scorelocal) - parseInt(scorevisitante);
                    gd.html(totalgd);
                }
                if (equipo == equipovisitante) {
                    var total = parseInt(puntos.html()) + puntosvisitante;
                    puntos.html(total)
                    var totalgd = parseInt(gd.html()) - parseInt(scorelocal) + parseInt(scorevisitante);
                    gd.html(totalgd);
                }

            }

        }
        sortTable(htmlequipos);
        GetValoresGrupo("OCTAVOS");
        PosicionesGrupo("OCTAVOS");
        
    }
    if (grupo == "OCTAVOS") {
        GetValoresGrupo("CUARTOS");
        PosicionesGrupo("CUARTOS");
    }
    if (grupo == "CUARTOS") {
        GetValoresGrupo("SEMI");
        PosicionesGrupo("SEMI");
    }
    if (grupo == "SEMI") {
        GetValoresGrupo("FINAL");        
    }

}





function GetPosicionGrupo(posicion, grupo) {


    var htmlequipos = $("#grupo" + grupo).find("table.posiciones")[0];


    var row = htmlequipos.rows[parseInt(posicion)];
    return $(row).find(".equipo").html(); // ("img") + " " + $(row).find(".equipo").data("equipo");



}

function GetResultadoPartido(resultado, partido) {
    var equipolocal = $("#" + partido).find(".equipolocal");
    var equipovisitante = $("#" + partido).find(".equipovisitante");

    var scorelocal = $("#" + partido).find(".scorelocal").val();
    var scorevisitante = $("#" + partido).find(".scorevisitante").val();

    var g = "";
    var p = "";
    if (scorelocal > scorevisitante) {
        g = equipolocal.html();
        p = equipovisitante.html();
    }
    else {
        p = equipolocal.html();
        g = equipovisitante.html();
    }

    if (resultado == "G")
        return g;
    if (resultado == "P")
        return p;
    
}


function GetValoresGrupo(grupo) { 

    var htmltable = $("#grupo"+grupo).find("table")[0];
    for (var r = 1; r < htmltable.rows.length; r++) {
        var equipolocal = $(htmltable.rows[r]).find(".equipolocal");        
        equipolocal.html(GetValor(equipolocal.data("equipo")));
        var equipovisitante = $(htmltable.rows[r]).find(".equipovisitante");
        equipovisitante.html(GetValor(equipovisitante.data("equipo")));        
    }
}


function GetValor(parametro) {

    var arrayparametro = new Array();

    arrayparametro = parametro.split(',');

    if (arrayparametro.length > 1) {

        if (arrayparametro[0] == "G")//OBTIENE LA POSICION DE UN GRUPO
        {
            return GetPosicionGrupo(arrayparametro[1], arrayparametro[2]);
        }
        if (arrayparametro[0] == "P")//OBTIENE EL RESULTADO DE UN PARTIDO
        {
            return GetResultadoPartido(arrayparametro[1], arrayparametro[2]);
        }
    }

    return parametro;

}


function Update() {


    var detalle = new Array();
    
    var htmltable = $("#grupoOCTAVOS").find("table.partidos")[0];
    for (var r = 1; r < htmltable.rows.length; r++) {

        var obj = {};
        obj["cabecera"] = GetQueryStringParams("codigo");
        obj["partido"] = htmltable.rows[r].id;
        obj["equipolocal"] = $(htmltable.rows[r]).find(".equipolocal").text();
        obj["equipovisitante"] = $(htmltable.rows[r]).find(".equipovisitante").text();
        detalle[detalle.length] = obj;

    }
   
    htmltable = $("#grupoCUARTOS").find("table.partidos")[0];
    for (var r = 1; r < htmltable.rows.length; r++) {

        var obj = {};
        obj["cabecera"] = GetQueryStringParams("codigo");
        obj["partido"] = htmltable.rows[r].id;
        obj["equipolocal"] = $(htmltable.rows[r]).find(".equipolocal").text();
        obj["equipovisitante"] = $(htmltable.rows[r]).find(".equipovisitante").text();
        detalle[detalle.length] = obj;

    }

    htmltable = $("#grupoSEMI").find("table.partidos")[0];
    for (var r = 1; r < htmltable.rows.length; r++) {

        var obj = {};
        obj["cabecera"] = GetQueryStringParams("codigo");
        obj["partido"] = htmltable.rows[r].id;
        obj["equipolocal"] = $(htmltable.rows[r]).find(".equipolocal").text();
        obj["equipovisitante"] = $(htmltable.rows[r]).find(".equipovisitante").text();
        detalle[detalle.length] = obj;

    }

    htmltable = $("#grupoFINAL").find("table.partidos")[0];
    for (var r = 1; r < htmltable.rows.length; r++) {

        var obj = {};
        obj["cabecera"] = GetQueryStringParams("codigo");
        obj["partido"] = htmltable.rows[r].id;
        obj["equipolocal"] = $(htmltable.rows[r]).find(".equipolocal").text();
        obj["equipovisitante"] = $(htmltable.rows[r]).find(".equipovisitante").text();
        detalle[detalle.length] = obj;

    }

    var jsonText = JSON.stringify({ objeto: detalle });
    CallServerMethods(webservice + "UpdateDetalles", jsonText, 4);
}