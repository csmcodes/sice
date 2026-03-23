using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Functions;

namespace BusinessObjects
{
    public class Comprobante
    {
        #region Properties

    	[Data(key = true)]
	public Int32 com_empresa { get; set; }
	[Data(originalkey = true)]
	public Int32 com_empresa_key { get; set; }
	[Data(key = true)]
	public String com_numero { get; set; }
	[Data(originalkey = true)]
	public String com_numero_key { get; set; }
	public String com_almacen { get; set; }
	public String com_pventa { get; set; }
	public String com_secuencia { get; set; }
	public String com_ruccliente { get; set; }
	public String com_nombrecliente { get; set; }
	public DateTime? com_fecha { get; set; }        
        public DateTime? com_fecharecibe { get; set; }
	public DateTime? com_fechaenvia { get; set; }
	public DateTime? com_fecharespuesta { get; set; }
	public Int32? com_estado { get; set; }
	public String com_xml { get; set; }
	public String com_pdf { get; set; }
	public Int32? com_formato { get; set; }
	public String com_email { get; set; }
	public String com_autorizacion { get; set; }
    public String com_fechaautorizacion{ get; set; }
    public Int32? com_ambiente { get; set; }
    public Int32? com_emision{ get; set; }

    public Decimal? com_subtotalnoimp { get; set; }
    public Decimal? com_subtotalimp { get; set; }
    public Decimal? com_propina { get; set; }
    public Decimal? com_total{ get; set; }
    public Decimal? com_descuento{ get; set; }
    public Decimal? com_iva{ get; set; }
    public Decimal? com_ice { get; set; }
    public String com_mensaje{ get; set; }

	public String crea_usr { get; set; }
	public DateTime? crea_fecha { get; set; }
	public String mod_usr { get; set; }
	public DateTime? mod_fecha { get; set; }

        public Int32? com_reintentos { get; set; }
        public DateTime? com_fechaultimointento { get; set; }


        [Data(nosql = true, tablaref = "empresa", camporef = "emp_estado", foreign = "com_empresa", keyref = "emp_codigo", join = "left")]
        public Int32? com_estadoempresa { get; set; }

        [Data(nosql = true, tablaref = "formato", camporef = "for_tipo", foreign = "com_empresa, com_formato", keyref = "for_empresa,for_codigo", join = "left")]
        public String com_formatonom { get; set; }


        [Data(noprop = true)]
        public string com_contribuyente { get; set; }
        [Data(noprop = true)]
        public string com_contabilidad { get; set; }
        [Data(noprop = true)]
        public string com_regimenrimpe{ get; set; }

        [Data(noprop = true)]
        public string com_tipoidcliente { get; set; }
        [Data(noprop = true)]
        public Decimal? com_subtotalsinimp { get; set; }
        [Data(noprop = true)]
        public Decimal? com_subtotal0 { get; set; }
        [Data(noprop = true)]
        public Decimal? com_subtotaliva { get; set; }


        [Data(noprop = true)]
        public Decimal? com_subtotalnoiva { get; set; }
        [Data(noprop = true)]
        public Decimal? com_subtotalextiva { get; set; }
        [Data(noprop = true)]
        public Decimal? com_porciva { get; set; }
        [Data(noprop = true)]
        public Decimal? com_iva0 { get; set; }

        [Data(noprop = true)]
        public String com_adicional1 { get; set; }
        [Data(noprop = true)]
        public String com_adicional2 { get; set; }
        [Data(noprop = true)]
        public String com_adicional3 { get; set; }
        [Data(noprop = true)]
        public String com_adicional4 { get; set; }
        [Data(noprop = true)]
        public String com_adicional5 { get; set; }
        [Data(noprop = true)]
        public String com_adicional6 { get; set; }


        [Data(noprop = true)]
        public List<String> com_adicionales { get; set; }






        [Data(noprop = true)]
        public string com_direccioncliente { get; set; }


        [Data(noprop = true)]
        public string com_direccionmatriz { get; set; }
        [Data(noprop = true)]
        public string com_direccionsucursal { get; set; }
        [Data(noprop = true)]
        public string com_telefonocliente { get; set; }

        [Data(noprop = true)]
        public string com_fechastr { get; set; }

        //Para uso de la guia de remision
        [Data(noprop = true)]
        public string com_fechainitransporte { get; set; }
        [Data(noprop = true)]
        public string com_fechafintransporte { get; set; }
        [Data(noprop = true)]
        public string com_dirpartida { get; set; }
        [Data(noprop = true)]
        public string com_placa { get; set; }



        //Para uso de la retencion
        [Data(noprop = true)]
        public string com_periodofiscal { get; set; }

        //Para uso de la nota de credito
        [Data(noprop = true)]
        public string com_rise { get; set; }
        [Data(noprop = true)]
        public string com_coddocmodificado{ get; set; }
        [Data(noprop = true)]
        public string com_numdocmodificado { get; set; }
        [Data(noprop = true)]
        public string com_fechaemisiondocsustento { get; set; }
        [Data(noprop = true)]
        public decimal? com_valormodificacion { get; set; }
        [Data(noprop = true)]
        public string com_motivo { get; set; }
        [Data(noprop = true)]
        public string com_agenteret { get; set; }
        [Data(noprop = true)]
        public string com_regimenmicro{ get; set; }



        [Data(noprop = true)]
        public List<Detalle> detalle { get; set; }
        [Data(noprop = true)]
        public List<Formapago> formas { get; set; }


        #endregion

        #region Constructors


        public  Comprobante()
        {
        }

        public  Comprobante( Int32 com_empresa,String com_numero,String com_almacen,String com_pventa,String com_secuencia,String com_ruccliente,String com_nombrecliente,DateTime com_fecha,DateTime com_fecharecibe,DateTime com_fechaenvia,DateTime com_fecharespuesta,Int32 com_estado,String com_xml,String com_pdf,Int32 com_formato,String com_email,String com_autorizacion,String com_fechaautorizacion, Int32? com_ambiente, Int32? com_emision, Decimal? com_subtotalnoimp, Decimal? com_subtotalimp, Decimal? com_propina, Decimal? com_total, Decimal? com_descuento, Decimal? com_iva, Decimal? com_ice, String com_mensaje,String crea_usr,DateTime crea_fecha,String mod_usr,DateTime mod_fecha)
        {                
    	this.com_empresa = com_empresa;
	this.com_numero = com_numero;
	this.com_almacen = com_almacen;
	this.com_pventa = com_pventa;
	this.com_secuencia = com_secuencia;
	this.com_ruccliente = com_ruccliente;
	this.com_nombrecliente = com_nombrecliente;
	this.com_fecha = com_fecha;
	this.com_fecharecibe = com_fecharecibe;
	this.com_fechaenvia = com_fechaenvia;
	this.com_fecharespuesta = com_fecharespuesta;
	this.com_estado = com_estado;
	this.com_xml = com_xml;
	this.com_pdf = com_pdf;
	this.com_formato = com_formato;
	this.com_email = com_email;
	this.com_autorizacion = com_autorizacion;
    this.com_fechaautorizacion = com_fechaautorizacion;
    this.com_ambiente = com_ambiente;
    this.com_emision = com_emision;

    this.com_subtotalnoimp = com_subtotalnoimp;
    this.com_subtotalimp = com_subtotalimp;
    this.com_total = com_total;
    this.com_propina = com_propina;
    this.com_descuento = com_descuento;
    this.com_iva = com_iva;
    this.com_ice = com_ice;
    this.com_mensaje = com_mensaje;

	this.crea_usr = crea_usr;
	this.crea_fecha = crea_fecha;
	this.mod_usr = mod_usr;
	this.mod_fecha = mod_fecha;

           
       }

        public  Comprobante(IDataReader reader)
        {
    	this.com_empresa = (Int32)reader["com_empresa"];
	this.com_numero = reader["com_numero"].ToString();
	this.com_almacen = reader["com_almacen"].ToString();
	this.com_pventa = reader["com_pventa"].ToString();
	this.com_secuencia = reader["com_secuencia"].ToString();
	this.com_ruccliente = reader["com_ruccliente"].ToString();
	this.com_nombrecliente = reader["com_nombrecliente"].ToString();
	this.com_fecha = (reader["com_fecha"] != DBNull.Value) ? (DateTime?)reader["com_fecha"] : null;
	this.com_fecharecibe = (reader["com_fecharecibe"] != DBNull.Value) ? (DateTime?)reader["com_fecharecibe"] : null;
	this.com_fechaenvia = (reader["com_fechaenvia"] != DBNull.Value) ? (DateTime?)reader["com_fechaenvia"] : null;
	this.com_fecharespuesta = (reader["com_fecharespuesta"] != DBNull.Value) ? (DateTime?)reader["com_fecharespuesta"] : null;
	this.com_estado = (reader["com_estado"] != DBNull.Value) ? (Int32?)reader["com_estado"] : null;
	this.com_xml = reader["com_xml"].ToString();
	this.com_pdf = reader["com_pdf"].ToString();
	this.com_formato = (reader["com_formato"] != DBNull.Value) ? (Int32?)reader["com_formato"] : null;
	this.com_email = reader["com_email"].ToString();
	this.com_autorizacion = reader["com_autorizacion"].ToString();
    this.com_fechaautorizacion= (reader["com_fechaautorizacion"] != DBNull.Value) ? (string)reader["com_fechaautorizacion"] : null;
    this.com_ambiente = (reader["com_ambiente"] != DBNull.Value) ? (Int32?)reader["com_ambiente"] : null;
    this.com_emision= (reader["com_emision"] != DBNull.Value) ? (Int32?)reader["com_emision"] : null;

    this.com_subtotalnoimp = (reader["com_subtotalnoimp"] != DBNull.Value) ? (Decimal?)reader["com_subtotalnoimp"] : null;
    this.com_subtotalimp = (reader["com_subtotalimp"] != DBNull.Value) ? (Decimal?)reader["com_subtotalimp"] : null;
    this.com_propina = (reader["com_propina"] != DBNull.Value) ? (Decimal?)reader["com_propina"] : null;
    this.com_total = (reader["com_total"] != DBNull.Value) ? (Decimal?)reader["com_total"] : null;
    this.com_descuento = (reader["com_descuento"] != DBNull.Value) ? (Decimal?)reader["com_descuento"] : null;
    this.com_iva= (reader["com_iva"] != DBNull.Value) ? (Decimal?)reader["com_iva"] : null;
    this.com_iva = (reader["com_ice"] != DBNull.Value) ? (Decimal?)reader["com_ice"] : null;
    this.com_mensaje= reader["com_mensaje"].ToString();
            this.com_formatonom= reader["com_formatonom"].ToString();


            this.crea_usr = reader["crea_usr"].ToString();
	this.crea_fecha = (reader["crea_fecha"] != DBNull.Value) ? (DateTime?)reader["crea_fecha"] : null;
	this.mod_usr = reader["mod_usr"].ToString();
	this.mod_fecha = (reader["mod_fecha"] != DBNull.Value) ? (DateTime?)reader["mod_fecha"] : null;
        this.com_reintentos = (reader["com_reintentos"] != DBNull.Value) ? (Int32?)reader["com_reintentos"] : null;
        this.com_fechaultimointento = (reader["com_fechaultimointento"] != DBNull.Value) ? (DateTime?)reader["com_fechaultimointento"] : null;

            this.com_estadoempresa = (reader["com_estadoempresa"] != DBNull.Value) ? (Int32?)reader["com_estadoempresa"] : null;
        }


        public Comprobante(object objeto)
        {            
            if (objeto != null)
            {
                Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
                	object com_empresa = null;
	object com_numero = null;
	object com_almacen = null;
	object com_pventa = null;
	object com_secuencia = null;
	object com_ruccliente = null;
	object com_nombrecliente = null;
	object com_fecha = null;
	object com_fecharecibe = null;
	object com_fechaenvia = null;
	object com_fecharespuesta = null;
	object com_estado = null;
	object com_xml = null;
	object com_pdf = null;
	object com_formato = null;
	object com_email = null;
	object com_autorizacion = null;
    object com_fechaautorizacion = null;
    object com_ambiente= null;
    object com_emision= null;
    object com_subtotalnoimp = null;
    object com_subtotalimp = null;
    object com_propina= null;
    object com_total= null;
    object com_descuento = null;
    object com_iva= null;
    object com_ice= null; 
             object com_mensaje = null;

	object crea_usr = null;
	object crea_fecha = null;
	object mod_usr = null;
	object mod_fecha = null;


                	tmp.TryGetValue("com_empresa", out com_empresa);
	tmp.TryGetValue("com_numero", out com_numero);
	tmp.TryGetValue("com_almacen", out com_almacen);
	tmp.TryGetValue("com_pventa", out com_pventa);
	tmp.TryGetValue("com_secuencia", out com_secuencia);
	tmp.TryGetValue("com_ruccliente", out com_ruccliente);
	tmp.TryGetValue("com_nombrecliente", out com_nombrecliente);
	tmp.TryGetValue("com_fecha", out com_fecha);
	tmp.TryGetValue("com_fecharecibe", out com_fecharecibe);
	tmp.TryGetValue("com_fechaenvia", out com_fechaenvia);
	tmp.TryGetValue("com_fecharespuesta", out com_fecharespuesta);
	tmp.TryGetValue("com_estado", out com_estado);
	tmp.TryGetValue("com_xml", out com_xml);
	tmp.TryGetValue("com_pdf", out com_pdf);
	tmp.TryGetValue("com_formato", out com_formato);
	tmp.TryGetValue("com_email", out com_email);
	tmp.TryGetValue("com_autorizacion", out com_autorizacion);
    tmp.TryGetValue("com_fechaautorizacion", out com_fechaautorizacion);
    tmp.TryGetValue("com_ambiente", out com_ambiente);
    tmp.TryGetValue("com_emision", out com_emision);


    tmp.TryGetValue("com_subtotalnoimp", out com_subtotalnoimp);
    tmp.TryGetValue("com_subtotalimp", out com_subtotalimp);
    tmp.TryGetValue("com_propina", out com_propina);
    tmp.TryGetValue("com_total", out com_total);
    tmp.TryGetValue("com_descuento", out com_descuento);
    tmp.TryGetValue("com_iva", out com_iva);
    tmp.TryGetValue("com_ice", out com_ice);
    tmp.TryGetValue("com_mensaje", out com_mensaje);

	tmp.TryGetValue("crea_usr", out crea_usr);
	tmp.TryGetValue("crea_fecha", out crea_fecha);
	tmp.TryGetValue("mod_usr", out mod_usr);
	tmp.TryGetValue("mod_fecha", out mod_fecha);


                	this.com_empresa = (Int32)Conversiones.GetValueByType(com_empresa, typeof(Int32));
	this.com_numero = (String)Conversiones.GetValueByType(com_numero, typeof(String));
	this.com_almacen = (String)Conversiones.GetValueByType(com_almacen, typeof(String));
	this.com_pventa = (String)Conversiones.GetValueByType(com_pventa, typeof(String));
	this.com_secuencia = (String)Conversiones.GetValueByType(com_secuencia, typeof(String));
	this.com_ruccliente = (String)Conversiones.GetValueByType(com_ruccliente, typeof(String));
	this.com_nombrecliente = (String)Conversiones.GetValueByType(com_nombrecliente, typeof(String));
	this.com_fecha = (DateTime?)Conversiones.GetValueByType(com_fecha, typeof(DateTime?));
	this.com_fecharecibe = (DateTime?)Conversiones.GetValueByType(com_fecharecibe, typeof(DateTime?));
	this.com_fechaenvia = (DateTime?)Conversiones.GetValueByType(com_fechaenvia, typeof(DateTime?));
	this.com_fecharespuesta = (DateTime?)Conversiones.GetValueByType(com_fecharespuesta, typeof(DateTime?));
	this.com_estado = (Int32?)Conversiones.GetValueByType(com_estado, typeof(Int32?));
	this.com_xml = (String)Conversiones.GetValueByType(com_xml, typeof(String));
	this.com_pdf = (String)Conversiones.GetValueByType(com_pdf, typeof(String));
	this.com_formato = (Int32?)Conversiones.GetValueByType(com_formato, typeof(Int32?));
	this.com_email = (String)Conversiones.GetValueByType(com_email, typeof(String));
	this.com_autorizacion = (String)Conversiones.GetValueByType(com_autorizacion, typeof(String));
    this.com_fechaautorizacion = (String)Conversiones.GetValueByType(com_fechaautorizacion, typeof(String));
    this.com_ambiente= (Int32?)Conversiones.GetValueByType(com_ambiente, typeof(Int32?));
    this.com_emision= (Int32?)Conversiones.GetValueByType(com_emision, typeof(Int32?));

    this.com_subtotalnoimp= (Decimal?)Conversiones.GetValueByType(com_subtotalnoimp, typeof(Decimal?));
    this.com_subtotalimp = (Decimal?)Conversiones.GetValueByType(com_subtotalimp, typeof(Decimal?));
    this.com_propina = (Decimal?)Conversiones.GetValueByType(com_propina, typeof(Decimal?));
    this.com_total = (Decimal?)Conversiones.GetValueByType(com_total, typeof(Decimal?));
    this.com_descuento = (Decimal?)Conversiones.GetValueByType(com_descuento, typeof(Decimal?));
    this.com_iva= (Decimal?)Conversiones.GetValueByType(com_iva, typeof(Decimal?));
    this.com_ice= (Decimal?)Conversiones.GetValueByType(com_ice, typeof(Decimal?));
    this.com_mensaje= (String)Conversiones.GetValueByType(com_mensaje, typeof(String));

	this.crea_usr = (String)Conversiones.GetValueByType(crea_usr, typeof(String));
	this.crea_fecha = (DateTime?)Conversiones.GetValueByType(crea_fecha, typeof(DateTime?));
	this.mod_usr = (String)Conversiones.GetValueByType(mod_usr, typeof(String));
	this.mod_fecha = (DateTime?)Conversiones.GetValueByType(mod_fecha, typeof(DateTime?));

                object com_reintentos = null;
                object com_fechaultimointento = null;
                tmp.TryGetValue("com_reintentos", out com_reintentos);
                tmp.TryGetValue("com_fechaultimointento", out com_fechaultimointento);
                this.com_reintentos = (Int32?)Conversiones.GetValueByType(com_reintentos, typeof(Int32?));
                this.com_fechaultimointento = (DateTime?)Conversiones.GetValueByType(com_fechaultimointento, typeof(DateTime?));

            }
        }
        #endregion

        #region Methods
        public PropertyInfo[] GetProperties()
        {
            return this.GetType().GetProperties();
        }        
        #endregion


    }
}
