$(document).ready(function () {

    SetIcons();
    GetMenu("Comprobantes");
    GetCabecera();



    //LoadMenu("listado");
    //LoadStatics();
    //GetCabecera();
    //LoadListado();

});


function GetCabecera() {
    var obj = {};
    obj["uxe_usuario"] = GetOnlineUser();
    obj["uxe_empresa"] = GetOnlineCompany();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetCabeceraListado", jsonText, 0);
}


function GetObj() {
    var obj = {};
    if ($("#cmbempresa").length > 0)
        obj["com_empresa"] = $("#cmbempresa").val();
    else
        obj["com_empresa"] = GetOnlineCompany();
    //obj["com_empresa"] = $("#cmbempresa").val();    
    //obj["com_fecha"] = $("#txtdesde").val();

    obj["crea_fecha"] = $("#txtdesde").val();
    obj["mod_fecha"] = $("#txthasta").val();

    obj["com_almacen"] = $("#txtalmacen").val();
    obj["com_pventa"] = $("#txtpventa").val();
    obj["com_secuencia"] = $("#txtnumero").val();
    obj["com_nombrecliente"] = $("#txtcliente").val();
    if ($("#cmbestado").val() != "")
        obj["com_estado"] = $("#cmbestado").val();
    if ($("#cmbambiente").val() != "")
        obj["com_ambiente"] = $("#cmbambiente").val();

    if ($("#cmbformato").val() != "")
        obj["com_formato"] = $("#cmbformato").val();

    obj["crea_usr"] = GetOnlineUser();

    obj["falta"] = GetBooleanCheckValue($("#chkfalta"));

    return obj;
}


function LoadComprobantes() {

    var obj = GetObj();
    var jsonText = JSON.stringify({ objeto: obj });        
    CallServerMethods(webservice + "LoadComprobantes", jsonText, 1);
}

function LoadComprobantesSecuencia() {

    var obj = GetObj();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "LoadComprobantesSecuencia", jsonText, 1);
}

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            $("#contenidocabecera").html(data.d);
            $(".filter").on("change", LoadComprobantes);
            $(".fecha").datepicker({
                dateFormat: "dd/mm/yy"
            }); //Setea campos de tipo fecha


            if ($('.pickadate').length && $.fn.pickadate) {
                $('.pickadate').each(function () {
                    $(this).pickadate({
                        format: 'dd/mm/yyyy'
                    });
                });
            }           

            LoadComprobantes();
        }
        if (retorno == 1) {
            $("#contenidodetalle").html(data.d);
            $("#tdcomprobantes").find("th").on("click", OrderByColumn);
            ShowTotales();
        }
        if (retorno == 2) {
            alert(data.d);
            LoadComprobantes();
        }
        if (retorno == 3) {
            alert(data.d);
        }
        if (retorno == 4)//GEstion Comprobantes ALL
        {
            alert(data.d);
            LoadComprobantes();
        }
      
    }
}


function GetXML(clave)
{
    window.open("wfGetXML.aspx?clave=" + clave, "", "toolbar=no,location=no,directories=no,status=yes,menubar=no,scrollbars=yes,resizable=yes,width=800,height=700,top=50,left=50");
}

function Respuesta(clave, tipo) {

    window.open("respuesta.htm?clave=" + clave + "&tipo=" + tipo, "", "toolbar=no,location=no,directories=no,status=yes,menubar=no,scrollbars=yes,resizable=yes,width=800,height=700,top=50,left=50");
}



function Reenviar(empresa, clave) {

    var obj = {};
    obj["empresa"] = empresa;
    obj["clave"] = clave;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "EnviarComprobante", jsonText, 2);
}

function Resetear(empresa, clave) {

    var obj = {};
    obj["empresa"] = empresa;
    obj["clave"] = clave;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "ResetComprobante", jsonText, 2);
}


function Verificar(clave) {

    var jsonText = JSON.stringify({ clave: clave });
    CallServerMethods(webservice + "VerificarComprobante", jsonText, 2);
}


function RIDE(empresa, clave) {

    window.open("wfPrintComprobante.aspx?empresa=" + empresa + "&clave=" + clave + "&generar=true");
}


function Mail(empresa, clave) {

    var obj = {};
    obj["empresa"] = empresa;
    obj["clave"] = clave;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "MailComprobante", jsonText, 3);
}


function BuscarAutorizacion(empresa, clave) {

    var obj = {};
    obj["empresa"] = empresa;
    obj["clave"] = clave;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "BuscarAutorizacion", jsonText, 2);
}

function GetMensaje(empresa, clave) {

    var obj = {};
    obj["empresa"] = empresa;
    obj["clave"] = clave;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "GetMensaje", jsonText, 2);
}

function Eliminar(empresa, clave) {

    var obj = {};
    obj["empresa"] = empresa;
    obj["clave"] = clave;
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "Eliminar", jsonText, 2);
}

function OrderByColumn() {
    //alert($(this).index());
    var order = $(this).data("order");
    if (order != null && order != undefined) {
        if (order == 1)
            order = -1;
        else
            order = 1;
    }
    else
        order = 1;

    $("#tdcomprobantes>thead>tr").find("i").remove();
    
    if (order == 1)
        $(this).append("<i class='fa fa-sort-asc'></i>");
    else
        $(this).append("<i class='fa fa-sort-desc'></i>");
    $(this).data("order",order);
    sortTable("tdcomprobantes", order, $(this).index());

}

//  sortTable(f,n)
//  f : 1 ascending order, -1 descending order
//  n : n-th child(<td>) of <tr>
function sortTable(table, f, n) {
    var rows = $('#'+table+' tbody  tr').get();
    rows.sort(function (a, b) {

        // get the text of n-th <td> of <tr> 
        var A = $(a).children('td').eq(n).text().toUpperCase();
        var B = $(b).children('td').eq(n).text().toUpperCase();
        if ($.isNumeric(A.replace(',', '.')) && $.isNumeric(B.replace(',', '.'))) {
            A = parseFloat($(a).children('td').eq(n).text().replace(',', '.'));
            B = parseFloat($(b).children('td').eq(n).text().replace(',', '.'));
        }

        if (A < B) {
            return -1 * f;
        }
        if (A > B) {
            return 1 * f;
        }
        return 0;
    });

    $.each(rows, function (index, row) {
        $('#'+table).children('tbody').append(row);
    });
}

function Faltantes() {

    var desde = $("#txtdesde").val(); //.datepicker("getDate");
    var hasta = $("#txthasta").val(); //.datepicker("getDate");
    var almacen = $("#txtalmacen").val();
    var pventa = $("#txtpventa").val();
    var secuencia = $("#txtnumero").val();
    var cliente = $("#txtcliente").val();
    var ambiente = $("#cmbambiente").val();
    var empresa;
    if ($("#cmbempresa").length > 0)
        empresa = $("#cmbempresa").val();
    else
        empresa = GetOnlineCompany();
    
    
    var url = "./reports/wfReportPrint.aspx?report=FALTANTES&empresa=" + empresa + "&parameter1=" + desde + "&parameter2=" + hasta + "&parameter3=" + almacen + "&parameter4=" + pventa + "&parameter5=" + secuencia + "&parameter6=" + cliente+ "&parameter7=" + ambiente;
    var feautures = "top=0,left=0,width='+(screen.availWidth)+',height ='+(screen.availHeight)+',toolbar=0 ,location=0,directories=0,status=0,menubar=0,resizable=yes,scrolling=yes,scrollbars=yes";
    window.open(url, "Reporte", feautures);

}


function ShowTotales() {
    $("#sidebar-charts").empty();
    var table = "tdcomprobantes";
    var rows = $('#' + table + '>tbody>tr');
    var cantidadcom = 0;
    var valorcom = 0;
    $.each(rows, function (index, row) {
        var estado = row.cells[4].innerText;
        var totestado = $("#tot" + estado);
        if (totestado.length == 0) {
            totestado = $('<div id="tot' + estado + '" data-cantidad="0" data-valor="0" class="sidebar-charts-inner">' +
                                '<div class="sidebar-charts-left">' +
                                    '<div class="sidebar-chart-title">' + estado + '</div>' +
                                    '<div class="sidebar-chart-number">0</div>' +
                                '</div>' +
                            '</div><hr class="divider">');
            $("#sidebar-charts").append(totestado);
        }

        var cantidad = parseInt($(totestado).data("cantidad"));
        var valor = parseFloat($(totestado).data("valor"));

        cantidad++;
        valor += parseFloat(row.cells[6].innerText.replace(',', '.'));
        cantidadcom++;
        valorcom += parseFloat(row.cells[6].innerText.replace(',', '.'));
        $(totestado).data("cantidad", cantidad);
        $(totestado).data("valor", valor);
        $(totestado).find(".sidebar-chart-title").html(estado + " #" + cantidad + " regs.");
        $(totestado).find(".sidebar-chart-number").html("$" + (Math.round(valor * 100) / 100));
        //$('#' + table).children('tbody').append(row);
    });

    var totcom = $('<div id="totcomprobantes" class="sidebar-charts-inner">' +
                                '<div class="sidebar-charts-left">' +
                                    '<div class="sidebar-chart-title">Comprobantes #'+cantidadcom+' regs.</div>' +
                                    '<div class="sidebar-chart-number">$' + (Math.round(valorcom * 100) / 100) + '</div>' +
                                '</div>' +
                            '</div>');
    $("#sidebar-charts").append(totcom);


}


function Edit(empresa, clave) {

    window.location = "comprobante.htm?empresa=" + empresa + "&numero=" + clave;
//    var obj = {};
//    obj["empresa"] = empresa;
//    obj["clave"] = clave;
//    var jsonText = JSON.stringify({ objeto: obj });
//    CallServerMethods(webservice + "EnviarComprobante", jsonText, 2);
}


function EndEdit(obj) {
    LoadComprobantes();
}


function EditFromXML(empresa, clave) {
    window.location = "comprobante.htm?empresa=" + empresa + "&numero=" + clave;
}



//Nuevas opciones para control masivo

function GestionComprobantesAll(opcion)
{
    if (confirm("¿Esta seguro que desea continuar?")) {
        var obj = GetObj();
        obj["opcion"] = opcion;
        var jsonText = JSON.stringify({ objeto: obj });
        CallServerMethods(webservice + "GestionComprobantesAll", jsonText, 4);
    }
}