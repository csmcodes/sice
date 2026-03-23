using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebUI
{
    public class Enums
    {
        public enum EstadoComprobante
        {
            Proceso =0,
            Firmado = 1,            
            Enviado =2,
            Recibido=3,
            Devuelto = 4,            
            Autorizado = 5,
            NoAutorizado = 6,
            Eliminado=7
        }

        public static string GetEstadoComprobante(int estado)
        {
            string retorno = "";
            if (estado == (int)EstadoComprobante.Proceso)
                retorno = EstadoComprobante.Proceso.ToString();
            if (estado == (int)EstadoComprobante.Firmado)
                retorno = EstadoComprobante.Firmado.ToString();
            if (estado == (int)EstadoComprobante.Enviado)
                retorno = EstadoComprobante.Enviado.ToString();
            if (estado == (int)EstadoComprobante.Recibido)
                retorno = EstadoComprobante.Recibido.ToString();
            if (estado == (int)EstadoComprobante.Devuelto)
                retorno = EstadoComprobante.Devuelto.ToString();
            if (estado == (int)EstadoComprobante.Autorizado)
                retorno = EstadoComprobante.Autorizado.ToString();
            if (estado == (int)EstadoComprobante.NoAutorizado)
                retorno = EstadoComprobante.NoAutorizado.ToString();
            if (estado == (int)EstadoComprobante.Eliminado)
                retorno = EstadoComprobante.Eliminado.ToString();            
            return retorno;
        }

        public enum EstadoEmpresa
        {
            Inactiva=0,
            Activa=1,
            Suspensa=3,
            Eliminada=9

        }

        public static string GetEstadoEmpresa(int estado)
        {
            string retorno = "";
            if (estado == (int)EstadoEmpresa.Activa)
                retorno = EstadoEmpresa.Activa.ToString();
            if (estado == (int)EstadoEmpresa.Inactiva)
                retorno = EstadoEmpresa.Inactiva.ToString();
            if (estado == (int)EstadoEmpresa.Suspensa)
                retorno = EstadoEmpresa.Suspensa.ToString();
            if (estado == (int)EstadoEmpresa.Eliminada)
                retorno = EstadoEmpresa.Eliminada.ToString();
            
            return retorno;
        }

        public enum FuncionEmpresa
        {
            Online = 0,
            Offline = 1

        }

        public static string GetFuncionEmpresa(int funcion)
        {
            string retorno = "";
            if (funcion == (int)FuncionEmpresa.Online)
                retorno = FuncionEmpresa.Online.ToString();
            if (funcion == (int)FuncionEmpresa.Offline)
                retorno = FuncionEmpresa.Offline.ToString();
            return retorno;
        }


    }
}
