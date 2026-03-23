using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using BusinessLogicLayer;

namespace Services
{
    public class Packages
    {
        public Packages()
        {
            //CONSTRUCTOR
        }


        #region Comprobante

        public static string NumeroComprobante(int empresa, int comprobante)
        {
            Comprobante comp = new Comprobante();
            comp.com_empresa = empresa;
            comp.com_empresa_key = empresa;
            comp.com_codigo = comprobante;
            comp.com_codigo_key = comprobante;
            comp = ComprobanteBLL.GetByPK(comp);
            return NumeroComprobante(comp);
        }

        public static string NumeroComprobante(Comprobante comp)
        {
            return FormatNumeroComprobante(comp.com_ctipocomid, comp.com_almacenid, comp.com_pventaid, comp.com_numero);
        }

        public static string FormatNumeroComprobante(string sigla, string almacen, string punto, int numero)
        {
            return sigla + "-" + almacen + "-" + punto + "-" + string.Format("{0:0000000}", numero);
        }




        public static int GetNextNumeroComprobante(int empresa, int periodo, int ctipocom, int almacen, int pventa)
        {
            Dtipocom dti = new Dtipocom();
            dti.dti_empresa = empresa;
            dti.dti_empresa_key = empresa;
            dti.dti_periodo = periodo;
            dti.dti_periodo_key = periodo;
            dti.dti_ctipocom = ctipocom;
            dti.dti_ctipocom_key = ctipocom;
            dti.dti_almacen = almacen;
            dti.dti_almacen_key = almacen;
            dti.dti_puntoventa = pventa;
            dti.dti_puntoventa_key = pventa;
            dti = DtipocomBLL.GetByPK(dti);
            return dti.dti_numero.Value + 1;

            /*Ctipocom  cti = new Ctipocom();
            cti.cti_empresa = empresa ;
            cti.cti_empresa_key = empresa ;
            cti.cti_codigo = ctipocom;
            cti.cti_codigo_key = ctipocom;
            cti= CtipocomBLL.GetByPK(cti);

            Almacen alm = new Almacen();
            alm.alm_empresa = empresa ;
            alm.alm_empresa_key = empresa ;
            alm.alm_codigo = almacen ;
            alm.alm_codigo_key = almacen ;
            alm = AlmacenBLL .GetByPK(alm); 
   
            Puntoventa pve = new Puntoventa();
            pve.pve_empresa = empresa ;
            pve.pve_empresa_key = empresa;
            pve.pve_almacen = almacen;
            pve.pve_almacen_key = almacen;
            pve.pve_secuencia = pventa;
            pve.pve_secuencia_key =pventa ;
            pve = PuntoventaBLL.GetByPK(pve);


            return FormatNumeroComprobante(cti.cti_id, alm.alm_id, pve.pve_id, (dti.dti_numero.Value + 1));    */

        }

        public static string GetNumeroComprobante(Comprobante comp)
        {


            Ctipocom cti = new Ctipocom();
            cti.cti_empresa = comp.com_empresa;
            cti.cti_empresa_key = comp.com_empresa;
            cti.cti_codigo = comp.com_ctipocom;
            cti.cti_codigo_key = comp.com_ctipocom;
            cti = CtipocomBLL.GetByPK(cti);

            Almacen alm = new Almacen();
            alm.alm_empresa = comp.com_empresa;
            alm.alm_empresa_key = comp.com_empresa;
            alm.alm_codigo = comp.com_almacen.Value;
            alm.alm_codigo_key = comp.com_almacen.Value;
            alm = AlmacenBLL.GetByPK(alm);

            Puntoventa pve = new Puntoventa();
            pve.pve_empresa = comp.com_empresa;
            pve.pve_empresa_key = comp.com_empresa;
            pve.pve_almacen = comp.com_almacen.Value;
            pve.pve_almacen_key = comp.com_almacen.Value;
            pve.pve_secuencia = comp.com_pventa.Value;
            pve.pve_secuencia_key = comp.com_pventa.Value;
            pve = PuntoventaBLL.GetByPK(pve);


            return FormatNumeroComprobante(cti.cti_id, alm.alm_id, pve.pve_id, comp.com_numero);

        }


        public static string GetIdPersona()
        {
            int max = PersonaBLL.GetMax("per_codigo");
            return "CUE" + string.Format("{0:0000}", max); ;
        }

        #endregion


        #region Cancelaciones



        public static Comprobante CreateCancelacion(Comprobante comp, List<Drecibo> detalle, ref string mensaje)
        {
            DateTime fecha = DateTime.Now;
            int ctipocom = 3; // SE DEBE OBTENER DE ALGUN LADO ?????
            Dtipocom dti = new Dtipocom();
            dti.dti_empresa = comp.com_empresa;
            dti.dti_empresa_key = comp.com_empresa;
            dti.dti_periodo = comp.com_periodo;
            dti.dti_periodo_key = comp.com_periodo;
            dti.dti_ctipocom = ctipocom;
            dti.dti_ctipocom_key = ctipocom;
            dti.dti_almacen = comp.com_almacen.Value;
            dti.dti_almacen_key = comp.com_almacen.Value;
            dti.dti_puntoventa = comp.com_pventa.Value;
            dti.dti_puntoventa_key = comp.com_pventa.Value;
            dti = DtipocomBLL.GetByPK(dti);
            dti.dti_numero = dti.dti_numero.Value + 1;

            Comprobante c = new Comprobante();
            c.com_empresa = comp.com_empresa;
            c.com_periodo = comp.com_periodo;
            c.com_tipodoc = Constantes.cRecibo.tpd_codigo;
            c.com_ctipocom = ctipocom;
            c.com_fecha = fecha;
            c.com_almacen = comp.com_almacen;
            c.com_pventa = comp.com_pventa;
            c.com_codclipro = comp.com_codclipro;
            c.com_tclipro = comp.com_tclipro;
            c.com_agente = comp.com_agente;
            //c.com_serie 
            c.com_numero = dti.dti_numero.Value;
            c.com_concepto = "CANCELACION";
            c.com_modulo = Packages.GetModulo(c.com_tipodoc); ;
            c.com_transacc = Packages.GetTransacc(c.com_tipodoc);
            c.com_nocontable = 0;
            c.com_estado = 1;
            c.com_descuadre = 0;
            c.com_adestino = 0;
            c.com_doctran = Packages.GetNumeroComprobante(c);

            BLL transaction = new BLL();
            transaction.CreateTransaction();
            try
            {
                transaction.BeginTransaction();

                c.com_codigo = ComprobanteBLL.InsertIdentity(transaction, c);//GUARDA CABECERA CANCELACION
                int contador = 0;
                foreach (Drecibo drec in detalle)
                {
                    drec.dfp_empresa = c.com_empresa;
                    drec.dfp_comprobante = c.com_codigo;
                    drec.dfp_secuencia = contador;
                    drec.dfp_tclipro = comp.com_tclipro;
                    drec.dfp_fecha_ven = fecha;
                    drec.dfp_ref_comprobante = comp.com_codigo;
                    DreciboBLL.Insert(transaction, drec);  //GUARDA EL RECIBO 
                    contador++;
                }




                contador = 0;
                foreach (Ddocumento doc in comp.documentos)
                {
                    Dcancelacion dca = new Dcancelacion();
                    dca.dca_empresa = doc.ddo_empresa;
                    dca.dca_comprobante = doc.ddo_comprobante;
                    dca.dca_transacc = doc.ddo_transacc;
                    dca.dca_doctran = doc.ddo_doctran;
                    dca.dca_pago = doc.ddo_pago;
                    dca.dca_comprobante_can = c.com_codigo;
                    dca.dca_secuencia = 0;
                    dca.dca_debcre = (doc.ddo_debcre == Constantes.cDebito) ? Constantes.cCredito : Constantes.cDebito;
                    dca.dca_transacc = c.com_transacc;
                    dca.dca_tipo_cambio = doc.ddo_tipo_cambio;
                    dca.dca_monto = doc.ddo_monto;
                    dca.dca_monto_ext = doc.ddo_monto;
                    DcancelacionBLL.Insert(transaction, dca);   //GUARDA EL DETALLE DE CANCELACION
                    contador++;

                    //Cambia el estado del documento que tambien se debe actualizar
                    doc.ddo_empresa_key = doc.ddo_empresa;
                    doc.ddo_comprobante_key = doc.ddo_comprobante;
                    doc.ddo_transacc_key = doc.ddo_transacc;
                    doc.ddo_doctran_key = doc.ddo_doctran;
                    doc.ddo_pago_key = doc.ddo_pago;
                    doc.ddo_cancela = dca.dca_monto;
                    doc.ddo_cancelado = 1;
                    DdocumentoBLL.Update(transaction, doc);// ACTUALIZA EL DOCUMENTO CANCELANDOLO   
                    //
                }

                DtipocomBLL.Update(transaction, dti);//ACTUALIZA LA SECUENCIA DEL COMPROBANTE

                transaction.Commit();
                mensaje = "OK";
            }
            catch
            {
                transaction.Rollback();
                mensaje = "ERROR";
            }

            return c;

        }


        #endregion

        #region Reteciones

        public static Comprobante GenRetencion(int empresa, long comprobante, ref string mensaje)
        {
            Comprobante comp = ComprobanteBLL.GetByPK(new Comprobante { com_empresa = empresa, com_empresa_key = empresa, com_codigo = comprobante, com_codigo_key = comprobante });


            comp.total = new Total();
            comp.total.tot_empresa = empresa;
            comp.total.tot_empresa_key = empresa;
            comp.total.tot_comprobante = comprobante;
            comp.total.tot_comprobante_key = comprobante;
            comp.total = TotalBLL.GetByPK(comp.total);

            comp.documentos = DdocumentoBLL.GetAll(new WhereParams("ddo_empresa ={0} and ddo_comprobante={1} and ddo_cancela=0", empresa, comprobante), "");

            return GenRetencion(comp, ref mensaje);
        }


        public static Comprobante GenRetencion(Comprobante comp, ref string mensaje)
        {
            List<Dretencion> lst = DretencionBLL.GetAll(new WhereParams("drt_empresa ={0} and drt_factura = {1} ", comp.com_empresa, comp.com_doctran), "");
            if (lst.Count > 0)
            {
                mensaje = "ERROR";
                return null;
            }

            DateTime fecha = DateTime.Now;
            int ctipocom = Constantes.cComRetencion.cti_codigo;

            Dtipocom dti = new Dtipocom();
            dti.dti_empresa = comp.com_empresa;
            dti.dti_empresa_key = comp.com_empresa;
            dti.dti_periodo = comp.com_periodo;
            dti.dti_periodo_key = comp.com_periodo;
            dti.dti_ctipocom = ctipocom;
            dti.dti_ctipocom_key = ctipocom;
            dti.dti_almacen = comp.com_almacen.Value;
            dti.dti_almacen_key = comp.com_almacen.Value;
            dti.dti_puntoventa = comp.com_pventa.Value;
            dti.dti_puntoventa_key = comp.com_pventa.Value;
            dti = DtipocomBLL.GetByPK(dti);
            dti.dti_numero = dti.dti_numero.Value + 1;


            Persona per = PersonaBLL.GetByPK(new Persona { per_empresa = comp.com_empresa, per_empresa_key = comp.com_empresa, per_codigo = comp.com_codclipro.Value, per_codigo_key = comp.com_codclipro.Value });


            Impuesto impiva = ImpuestoBLL.GetByPK(new Impuesto { imp_empresa = comp.com_empresa, imp_empresa_key = comp.com_empresa, imp_codigo = per.per_retiva.Value, imp_codigo_key = per.per_retiva.Value });
            Impuesto impfue = ImpuestoBLL.GetByPK(new Impuesto { imp_empresa = comp.com_empresa, imp_empresa_key = comp.com_empresa, imp_codigo = per.per_retfuente.Value, imp_codigo_key = per.per_retfuente.Value });


            Comprobante c = new Comprobante();
            c.com_empresa = comp.com_empresa;
            c.com_periodo = comp.com_periodo;
            c.com_tipodoc = Constantes.cRetencion.tpd_codigo;
            c.com_ctipocom = ctipocom;
            c.com_tclipro = Constantes.cProveedor;
            c.com_fecha = fecha;
            c.com_almacen = comp.com_almacen;
            c.com_pventa = comp.com_pventa;
            c.com_codclipro = comp.com_codclipro;
            c.com_agente = comp.com_agente;
            //c.com_serie 
            c.com_numero = dti.dti_numero.Value;
            c.com_concepto = "RETENCION " + per.per_apellidos + " " + per.per_nombres;
            c.com_modulo = Packages.GetModulo(c.com_tipodoc); ;
            c.com_transacc = Packages.GetTransacc(c.com_tipodoc);
            c.com_nocontable = 0;
            c.com_estado = Constantes.cEstadoProceso;
            c.com_descuadre = 0;
            c.com_adestino = 0;
            c.com_doctran = Packages.GetNumeroComprobante(c);

            BLL transaction = new BLL();
            transaction.CreateTransaction();
            try
            {
                transaction.BeginTransaction();

                c.com_codigo = ComprobanteBLL.InsertIdentity(transaction, c);//GUARDA CABECERA CANCELACION
                int contador = 0;

                c.ccomdoc = new Ccomdoc();
                c.ccomdoc.cdoc_empresa = c.com_empresa;
                c.ccomdoc.cdoc_comprobante = c.com_codigo;
                c.ccomdoc.cdoc_factura = comp.com_codigo; //LIGA LA RETENCION CON LA FACTURA
                c.ccomdoc.cdoc_nombre = per.per_apellidos + " " + per.per_nombres;
                c.ccomdoc.cdoc_direccion = per.per_direccion;
                c.ccomdoc.cdoc_telefono = per.per_telefono;
                c.ccomdoc.cdoc_ced_ruc = per.per_ciruc;
                CcomdocBLL.Insert(transaction, c.ccomdoc);

                Dretencion retiva = new Dretencion();
                retiva.drt_empresa = c.com_empresa;
                retiva.drt_comprobante = c.com_codigo;
                retiva.drt_impuesto = impiva.imp_codigo;
                //retiva.drt_cuenta= 
                retiva.drt_valor = comp.total.tot_timpuesto;
                retiva.drt_porcentaje = impiva.imp_porcentaje;
                retiva.drt_total = retiva.drt_valor * (retiva.drt_porcentaje / 100);
                retiva.drt_debcre = Constantes.cDebito;
                //retiva.drt_cuenta_transacc 
                //retiva.drt_con_codigo
                retiva.drt_factura = comp.com_doctran;
                DretencionBLL.Insert(transaction, retiva);


                Dretencion retfue = new Dretencion();
                retfue.drt_empresa = c.com_empresa;
                retfue.drt_comprobante = c.com_codigo;
                retfue.drt_impuesto = impfue.imp_codigo;
                //retfue.drt_cuenta= 
                retfue.drt_valor = comp.total.tot_subtotal + comp.total.tot_subtot_0;
                retfue.drt_porcentaje = impfue.imp_porcentaje;
                retfue.drt_total = retfue.drt_valor * (retfue.drt_porcentaje / 100);
                retfue.drt_debcre = Constantes.cDebito;
                //retfue.drt_cuenta_transacc 
                //retfue.drt_con_codigo
                retfue.drt_factura = comp.com_doctran;
                DretencionBLL.Insert(transaction, retfue);


                ////CANCELACION DEL DOCUMENTO A PARTIR DEL VALOR DE LA RETENCION
                decimal totaretencion = retiva.drt_total.Value + retfue.drt_total.Value;
                foreach (Ddocumento doc in comp.documentos)
                {
                    Dcancelacion dca = new Dcancelacion();
                    //dca.dca_empresa = doc.ddo_empresa;
                    //dca.dca_comprobante = doc.ddo_comprobante;
                    dca.dca_empresa = c.com_empresa;
                    dca.dca_comprobante = comp.com_codigo;
                    dca.dca_transacc = doc.ddo_transacc;
                    dca.dca_doctran = doc.ddo_doctran;
                    dca.dca_pago = doc.ddo_pago;
                    dca.dca_comprobante_can = c.com_codigo;
                    dca.dca_secuencia = 0;
                    dca.dca_debcre = (doc.ddo_debcre == Constantes.cDebito) ? Constantes.cCredito : Constantes.cDebito;
                    dca.dca_transacc = comp.com_transacc;
                    dca.dca_tipo_cambio = doc.ddo_tipo_cambio;
                    if (totaretencion > doc.ddo_monto.Value)
                    {
                        dca.dca_monto = doc.ddo_monto;
                        dca.dca_monto_ext = doc.ddo_monto;
                    }
                    else
                    {
                        dca.dca_monto = totaretencion;
                        dca.dca_monto_ext = totaretencion;
                    }
                    DcancelacionBLL.Insert(transaction, dca);   //GUARDA EL DETALLE DE CANCELACION

                    //Cambia el estado del documento que tambien se debe actualizar
                    doc.ddo_empresa_key = doc.ddo_empresa;
                    doc.ddo_comprobante_key = doc.ddo_comprobante;
                    doc.ddo_transacc_key = doc.ddo_transacc;
                    doc.ddo_doctran_key = doc.ddo_doctran;
                    doc.ddo_pago_key = doc.ddo_pago;
                    doc.ddo_cancela = ((doc.ddo_cancela.HasValue) ? doc.ddo_cancela.Value : 0) + dca.dca_monto;
                    if (doc.ddo_cancela >= doc.ddo_monto)
                        doc.ddo_cancelado = 1;
                    DdocumentoBLL.Update(transaction, doc);// ACTUALIZA EL DOCUMENTO CANCELANDOLO   
                    if (totaretencion > doc.ddo_monto.Value)
                        totaretencion = totaretencion - doc.ddo_monto.Value;
                    else
                        break;
                    //
                }



                DtipocomBLL.Update(transaction, dti);//ACTUALIZA LA SECUENCIA DEL COMPROBANTE

                transaction.Commit();
                mensaje = "OK";
            }
            catch
            {
                transaction.Rollback();
                mensaje = "ERROR";
            }

            return c;

        }


        #endregion


        #region Saldos
        public static Decimal SaldoCuenta(string Pv_indica, int Pn_debcre, int Pn_Nac_ext, int empresa, int cuenta, int Pn_centro, int Pn_almacen, int Pn_transacc, DateTime Pn_fecha)
        {
            int Vn_periodo = Pn_fecha.Year;
            int Vn_mes = Pn_fecha.Month;
            int Vn_dia = Pn_fecha.Day;          
            int? Vn_mes_ini = 0;
            int? Vn_mes_fin = 0;
            decimal Vn_Saldo = 0;
            decimal Vn_Saldo_ext = 0;         
            if (Pv_indica.Equals("i"))
            {
                Vn_mes_ini = 0;
                Vn_mes_fin = 0;
            }
            else if (Pv_indica.Equals("a"))
            {
                if (Pn_debcre == 3)
                {
                    Vn_mes_ini = 0;
                    Vn_mes_fin = Vn_mes - 1;
                    if (Vn_mes_fin < 0)
                        Vn_mes_fin = 0;
                }
                else
                {
                    Vn_mes_ini = null;
                    Vn_mes_fin = null;
                }
            }
            else if (Pv_indica.Equals("m"))
            {
                if (Pn_debcre == 3)
                    Vn_mes_ini = 0;
                else
                    Vn_mes_ini = Vn_mes;
                Vn_mes_fin = Vn_mes;
            }
            List<vCuentas> lst = vCuentasBLL.GetAll(new WhereParams("cue_empresa ={0} and cue_codigo={1}", empresa, cuenta), "cue_id");
            foreach (vCuentas obj in lst)
            {
                string where = "sal_mes BETWEEN {0} and {1} and sal_periodo={2} " +
                "AND (COALESCE({3},0)    = 0 OR COALESCE(sal_transacc,0) = COALESCE({3},0)) " +
                "AND (COALESCE({4},0)    = 0 OR COALESCE(sal_almacen,0)  = COALESCE({4},0)) " +
                "AND (COALESCE({5},0)      = 0 OR COALESCE(sal_centro,0)   = COALESCE({5},0)) " +
                "AND sal_cuenta       = {6} " +
                "AND sal_empresa     = {7} ";
                List<Saldo> lst2 = SaldoBLL.GetAll(new WhereParams(where, Vn_mes_ini, Vn_mes_fin, Vn_periodo, Pn_transacc, Pn_almacen, Pn_centro, obj.cue_codigo, empresa), "");
                foreach (Saldo item in lst2)
                {
                    if (Pn_debcre == 1 || Pn_debcre == 3 || Pn_debcre == 4)
                    {
                        Vn_Saldo = Vn_Saldo + item.sal_debito;
                        Vn_Saldo_ext = Vn_Saldo_ext + item.sal_debext;
                    }
                    if (Pn_debcre == 2 || Pn_debcre == 3 || Pn_debcre == 4)
                    {
                        Vn_Saldo = Vn_Saldo - item.sal_credito;
                        Vn_Saldo_ext = Vn_Saldo_ext -item.sal_creext;
                    }
                }
                var date1 = new DateTime(Pn_fecha.Year, Pn_fecha.Month, 1, Pn_fecha.Hour, Pn_fecha.Minute, Pn_fecha.Second);           
                if (Pv_indica.Equals("a"))
                {
                    string wheresaldo = " dco_cuenta      = {0} " +
                                        " AND dco_empresa     = {1} " +
                                        " AND dco_debcre      = {2} " +
                                        " and  com_fecha BETWEEN {3} and {4} " +
                                        " AND (com_codigo     = dco_comprobante " +
                                        " AND  com_empresa    = dco_empresa) " +
                                        " AND  com_estado     IN ({8},{9})  " +
                                        " AND  com_nocontable = 0 " +
                                        " AND (COALESCE({5},0)= 0 OR COALESCE(dco_almacen,0)= COALESCE({5},0)) " +
                                        " AND (COALESCE({6},0)= 0 OR COALESCE(dco_transacc,0)= COALESCE({6},0)) " +
                                        " AND (COALESCE({7},0)= 0 OR COALESCE(dco_centro,0)=COALESCE({7},0));";
                    if (Pn_debcre == 1 || Pn_debcre == 3)
                    {
                        List<vDcontable> lst3 = vDcontableBLL.GetAll(new WhereParams(wheresaldo, obj.cue_codigo, empresa, 1, date1, Pn_fecha, Pn_almacen, Pn_transacc, Pn_centro, Constantes.cEstadoPorAutorizar, Constantes.cEstadoMayorizado), "");
                        foreach (vDcontable item in lst3)
                        {
                            Vn_Saldo = Vn_Saldo + item.dco_valor_nac ?? 0; ;
                            Vn_Saldo_ext = Vn_Saldo_ext + item.dco_valor_ext ?? 0; ;
                        }
                    }
                    if (Pn_debcre == 2 || Pn_debcre == 3)
                    {
                        List<vDcontable> lst3 = vDcontableBLL.GetAll(new WhereParams(wheresaldo, obj.cue_codigo, empresa, 2, date1, Pn_fecha, Pn_almacen, Pn_transacc, Pn_centro), "");
                        foreach (vDcontable item in lst3)
                        {
                            Vn_Saldo = Vn_Saldo - item.dco_valor_nac ?? 0; ;
                            Vn_Saldo_ext = Vn_Saldo_ext - item.dco_valor_ext ?? 0; ;
                        }
                    }
                }
            }
            if (Pn_debcre == 2)
            {
                Vn_Saldo = Vn_Saldo * -1;
                Vn_Saldo_ext = Vn_Saldo_ext * -1;
            }
            if (Pn_Nac_ext == 2)
            {
                return Vn_Saldo_ext;
            }
            else
            {
                return Vn_Saldo;
            }
        }
        public static Decimal SaldoBancos(string Pv_indica, int Pn_debcre, int Pn_Nac_ext, int Pn_empresa, int Pn_banco, int Pn_almacen, int Pn_transacc, DateTime Pn_fecha)
        {
            int Vn_periodo = Pn_fecha.Year;
            int Vn_mes = Pn_fecha.Month;
            int Vn_dia = Pn_fecha.Day;
            int? Vn_mes_ini = 0;
            int? Vn_mes_fin = 0;
            decimal Vn_Saldo = 0;
            decimal Vn_Saldo_ext = 0;

            if (Pv_indica.Equals("i"))
            {
                Vn_mes_ini = 0;
                Vn_mes_fin = 0;
            }
            else if (Pv_indica.Equals("a"))
            {
                if (Pn_debcre == 3)
                {
                    Vn_mes_ini = 0;
                    Vn_mes_fin = Vn_mes - 1;
                    if (Vn_mes_fin < 0)
                        Vn_mes_fin = 0;
                }
                else
                {
                    Vn_mes_ini = null;
                    Vn_mes_fin = null;
                }
            }
            else if (Pv_indica.Equals("m"))
            {
                if (Pn_debcre == 3)
                    Vn_mes_ini = 0;
                else
                    Vn_mes_ini = Vn_mes;
                Vn_mes_fin = Vn_mes;
            }

            string where =
          "slb_empresa = {0} " +
          "AND slb_banco  = {1} " +
          "AND slb_periodo = {2} " +
          "AND slb_mes BETWEEN {3} AND {4} " +
          "AND (COALESCE({5},0) = 0 OR COALESCE(slb_almacen,0)  = COALESCE({5},0)) " +
          "AND (COALESCE({6},0) = 0 OR COALESCE(slb_transacc,0) = COALESCE({6},0)) ";
            List<Salban> lst1 = SalbanBLL.GetAll(new WhereParams(where, Pn_empresa, Pn_banco, Vn_periodo, Vn_mes_ini, Vn_mes_fin, Pn_almacen, Pn_transacc), "");
            foreach (Salban item in lst1)
            {
                if (Pn_debcre == 1 || Pn_debcre == 3 || Pn_debcre == 4)
                {
                    Vn_Saldo = Vn_Saldo + item.slb_debito??0;
                    Vn_Saldo_ext = Vn_Saldo_ext + item.slb_debext ?? 0;
                }
                if (Pn_debcre == 2 || Pn_debcre == 3 || Pn_debcre == 4)
                {
                    Vn_Saldo = Vn_Saldo - item.slb_credito ?? 0;
                    Vn_Saldo_ext = Vn_Saldo_ext - item.slb_creext ?? 0;
                }
            }
            var date1 = new DateTime(Pn_fecha.Year, Pn_fecha.Month, 1, Pn_fecha.Hour, Pn_fecha.Minute, Pn_fecha.Second);    
            if (Pv_indica.Equals("a"))
            {
                string wheresaldo = "  dban_banco       =  {0} " +
                                    "and dban_empresa     =  {1} " +
                                    "AND dban_debcre      =  {2} " +
                                    "and  com_fecha BETWEEN {3} and {4} " +
                                    "AND (COALESCE( {5},0)    = 0 OR COALESCE(cco_adestino,0)  = COALESCE( {5},0)) " +
                                    "AND (COALESCE( {6},0)    = 0 OR COALESCE(dban_transacc,0) = COALESCE( {6},0)) " +
                                    "AND (com_empresa    = dban_empresa " +
                                    "AND  com_codigo     = dban_cco_comproba " +
                                    "AND  com_estado    IN ({7},{8}) " +
                                    "AND  com_nocontable = 0 ";

                if (Pn_debcre == 1 || Pn_debcre == 3)
                {
                    List<vDcontable> lst3 = vDcontableBLL.GetAll(new WhereParams(wheresaldo, Pn_banco, Pn_empresa, 1, date1, Pn_fecha, Pn_almacen, Pn_transacc, Constantes.cEstadoPorAutorizar, Constantes.cEstadoMayorizado), "");
                    foreach (vDcontable item in lst3)
                    {
                        Vn_Saldo = Vn_Saldo + item.dco_valor_nac ?? 0; ;
                        Vn_Saldo_ext = Vn_Saldo_ext + item.dco_valor_ext ?? 0; ;
                    }
                }
                if (Pn_debcre == 2 || Pn_debcre == 3)
                {
                    List<vDcontable> lst3 = vDcontableBLL.GetAll(new WhereParams(wheresaldo, Pn_banco, Pn_empresa, 1, date1, Pn_fecha, Pn_almacen, Pn_transacc), "");
                    foreach (vDcontable item in lst3)
                    {
                        Vn_Saldo = Vn_Saldo - item.dco_valor_nac ?? 0; ;
                        Vn_Saldo_ext = Vn_Saldo_ext - item.dco_valor_ext ?? 0; ;
                    }
                }               
            }

            if (Pn_debcre == 2)
            {
                Vn_Saldo = Vn_Saldo * -1;
                Vn_Saldo_ext = Vn_Saldo_ext * -1;
            }
            if (Pn_Nac_ext == 2)
            {
                return Vn_Saldo_ext;
            }
            else
            {
                return Vn_Saldo;
            }
        }
        #endregion
        #region Autorizaciones

        public static Autpersona GetAutorizacion(Comprobante comprobante, ref string mensaje)
        {
            bool encontro = false;
            mensaje = "";
            Persona informante = new Persona();
            informante.per_empresa = comprobante.com_empresa;
            informante.per_empresa_key = comprobante.com_empresa_key;
            informante.per_codigo = 6;//DEBE OBTENERSE DE EMPRESA
            informante.per_codigo_key = 6;

            informante = PersonaBLL.GetByPK(informante);

            Ctipocom ctipocom = new Ctipocom();
            ctipocom.cti_empresa = comprobante.com_empresa;
            ctipocom.cti_empresa_key = comprobante.com_empresa;
            ctipocom.cti_codigo = comprobante.com_ctipocom;
            ctipocom.cti_codigo_key = comprobante.com_ctipocom;

            ctipocom = CtipocomBLL.GetByPK(ctipocom);

            List<Autpersona> lst = AutpersonaBLL.GetAll(new WhereParams("ape_empresa = {0} and ape_persona={1} and ape_fac1={2} and ape_fac2={3} and ape_estado=1", comprobante.com_empresa, informante.per_codigo, comprobante.com_almacenid, comprobante.com_pventaid), "ape_val_fecha");

            foreach (Autpersona item in lst)
            {
                int desde = int.Parse(item.ape_fac3);
                int hasta = int.Parse(item.ape_fact3);
                if ((comprobante.com_numero >= desde && comprobante.com_numero <= hasta) && DateTime.Compare(comprobante.com_fecha, item.ape_val_fecha.Value) < 0)
                {
                    encontro = true;
                    int faltan = int.Parse(item.ape_fact3) - comprobante.com_numero;
                    if (faltan < Constantes.cCantidadFacturas)
                        mensaje = "FALTAN " + faltan + " COMPROBANTES PARA LLEGAR A RANGO FINAL AUTORIZADO PARA EMISION DE COMPROBANTE POR EL SRI ";
                    int dias = item.ape_val_fecha.Value.Subtract(comprobante.com_fecha).Days;
                    if (dias < Constantes.cDiasAutorizacion)
                        mensaje += "FALTAN " + dias + " DIAS PARA LLEGAR A FECHA FINAL AUTORIZADA PARA EMISION DE COMPROBANTE POR EL SRI ";

                    return item;
                }
            }
            if (!encontro)
                mensaje = "No existe autorizacion vigente, revice autorizacion ";
            return null;


        }


        #endregion

        #region Modulo
        public static int GetModulo(int tipodoc)
        {
            Tipodoc td = new Tipodoc();
            td.tpd_codigo_key = tipodoc;
            td = TipodocBLL.GetByPK(td);
            return td.tpd_modulo??1;        
        }
        #endregion

        #region Transsac
        public static int GetTransacc(int tipodoc)
        {
            Tipodoc td = new Tipodoc();
            td.tpd_codigo_key = tipodoc;
            td = TipodocBLL.GetByPK(td);
            
            if( td.tpd_modulo==Constantes.cCuentasxCobrar.mod_codigo)
            {
                if(td.tpd_codigo==Constantes.cFactura.tpd_codigo)
                {
                    return 1;
                }
                if (td.tpd_codigo == Constantes.cNotacre.tpd_codigo)
                {
                    return 6;
                }
                if (td.tpd_codigo == Constantes.cNotadeb.tpd_codigo)
                {
                    return 5;
                }
            }

            if (td.tpd_modulo == Constantes.cBancos.mod_codigo)
            {
                if (td.tpd_codigo == Constantes.cDeposito.tpd_codigo)
                {
                    return 4;
                }
                if (td.tpd_codigo == Constantes.cNotaCreditoBan.tpd_codigo)
                {
                    return 3;
                }
                if (td.tpd_codigo == Constantes.cNotaDebitoBan.tpd_codigo)
                {
                    return 2;
                }
            }
            
            
            
            
            return 1;
        }
        #endregion

    }
}
