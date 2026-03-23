using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogicLayer;
using BusinessObjects;
using Functions;
using System.Web.Script.Serialization;
using System.Globalization;

namespace Services
{
    public static class Constantes
    {

        #region Propiedades

        #region Parametros

        public static int cEmpresa { get; set; }

        public static string cValorIVA
        {
            get
            {
                return GetParameter("valoriva");

            }
        }

        public static string cMailFrom
        {
            get
            {
                return "info@siac.com.ec";
            }

        }

        public static string cMailDominio
        {
            get
            {
                return "";
            }

        }

        public static string cMailPassword
        {
            get
            {
                return IsExternal() ? "dULKbsxL" : "cristian90";

                //return "csmGM1207";
                //return "cristian90";
            }

        }


        public static string cMailRequiereAut
        {
            get
            {
                return IsExternal() ? "1" : "0";
            }
        }

        public static string cMailServer
        {
            get
            {
                return IsExternal() ? "pro.turbo-smtp.com" : "siac.com.ec";
                //return "siac.com.ec";
                //return "mail.gtec.com.ec";
            }
        }

        public static string cMailUsarSSL
        {
            get
            {
                return IsExternal() ? "0" : "1";
            }
        }

        public static string cMailUsuario
        {
            get
            {
                //return "cristhian.san.martin99@gmail.com";
                return "csanmartin@gtec.com.ec";
            }
        }

        public static string cMailPort
        {
            get
            {
                return IsExternal() ? "587" : "25";
                //return "26";
            }
        }



        #endregion

        

        #region Estados Comprobante

        /// <summary>
        /// Valor = 0
        /// </summary>
        public static int cEstadoProceso
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Valor = 1
        /// </summary>
        public static int cEstadoDevuelta
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Valor = 2
        /// </summary>
        public static int cEstadoRecibido
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Valor =3
        /// </summary>
        public static int cEstadoEnviado
        {
            get
            {
                return 3;
            }
        }

        /// <summary>
        /// Valor = 4
        /// </summary>
        public static int cEstadoAutorizado
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        /// Valor = 5
        /// </summary>
        public static int cEstadoNoAutorizado
        {
            get
            {
                return 5;
            }
        }


        /// <summary>
        /// Valor = 9
        /// </summary>
        public static int cEstadoEliminado
        {
            get
            {
                return 9;
            }
        }

        #endregion

        #region Perfiles        
        public static string cPerfilAdministrador
        {
            get
            {
                return "admin";
            }
        }
        public static string cPerfilEmpresa
        {
            get
            {
                return "company";
            }
        }
        public static string cPerfilUsuario
        {
            get
            {
                return "user";
            }
        }

        #endregion


        #endregion

        #region Metodos

        public static string GetParameter(string id)
        {
            Parametro par = ParametroBLL.GetByPK(new Parametro { par_empresa = 1, par_empresa_key = 1, par_id = id, par_id_key = id });            
            return par.par_valor;
        }


        public static Parametro GetParameterObj(string id)
        {
            return ParametroBLL.GetByPK(new Parametro { par_empresa = 1, par_empresa_key = 1, par_id = id, par_id_key = id });            
        }

        public static bool IsExternal()
        {
            string external = System.Configuration.ConfigurationSettings.AppSettings["externalsmtp"].ToString();
            if (external == "yes")
                return true;
            
            return false;            
        }

        public static decimal GetValorIVA(DateTime? fecha)
        {
            if (!fecha.HasValue)
                fecha = DateTime.Now;
            if (fecha.Value == DateTime.MinValue)
                fecha = DateTime.Now;


            var serializer = new JavaScriptSerializer();
            List<ValorIVA> lst = serializer.Deserialize<List<ValorIVA>>(cValorIVA);

            foreach (ValorIVA item in lst)
            {
                DateTime desde = DateTime.Parse(item.desde, new CultureInfo("es-EC"));
                DateTime hasta = DateTime.Parse(item.hasta, new CultureInfo("es-EC"));

                //if (DateTime.Compare(desde, fecha.Value) >= 0 && DateTime.Compare(fecha.Value, hasta) <= 0)

                if (fecha >= desde && fecha <= hasta)
                    return item.valor;
            }
            return 12;

        }

        #endregion
    }
}
