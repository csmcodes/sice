using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public class Enums
    {
        public enum EstadoComprobante
        {
            PROCESO = 0,
            GRABADO = 1,
            ENVIADO = 2,
            RECIBIDO = 3,
            DEVUELTO = 4,
            AUTORIZADO = 5,
            NOAUTORIZADO = 6,
            ANULADO = 7,
            ELIMINADO = 8,
            DELEGADOASAPP = 9

        }

        public static string GetEstadoComprobante(int? estado)
        {
            string retorno = "";
            if (estado == (int)EstadoComprobante.PROCESO)
                retorno = EstadoComprobante.PROCESO.ToString();
            if (estado == (int)EstadoComprobante.GRABADO)
                retorno = EstadoComprobante.GRABADO.ToString();
            if (estado == (int)EstadoComprobante.ENVIADO)
                retorno = EstadoComprobante.ENVIADO.ToString();
            if (estado == (int)EstadoComprobante.RECIBIDO)
                retorno = EstadoComprobante.RECIBIDO.ToString();
            if (estado == (int)EstadoComprobante.DEVUELTO)
                retorno = EstadoComprobante.DEVUELTO.ToString();
            if (estado == (int)EstadoComprobante.AUTORIZADO)
                retorno = EstadoComprobante.AUTORIZADO.ToString();
            if (estado == (int)EstadoComprobante.NOAUTORIZADO)
                retorno = EstadoComprobante.NOAUTORIZADO.ToString();
            if (estado == (int)EstadoComprobante.ANULADO)
                retorno = EstadoComprobante.ANULADO.ToString();
            if (estado == (int)EstadoComprobante.ELIMINADO)
                retorno = EstadoComprobante.ELIMINADO.ToString();
            if (estado == (int)EstadoComprobante.DELEGADOASAPP)
                retorno = EstadoComprobante.DELEGADOASAPP.ToString();
            return retorno;
        }

        public enum AsappModo
        {
            SHADOW = 1,
            DELEGADO = 2
        }




        public enum EstadoInforme
        {
            PROCESO = 0,
            GUARDADO = 1,
            ANULADO = 9
        }

        public static string GetEstadoInforme(int estado)
        {
            string retorno = "";
            if (estado == (int)EstadoInforme.PROCESO)
                retorno = EstadoInforme.PROCESO.ToString();
            if (estado == (int)EstadoInforme.GUARDADO)
                retorno = EstadoInforme.GUARDADO.ToString();
            if (estado == (int)EstadoInforme.ANULADO)
                retorno = EstadoInforme.ANULADO.ToString();
            return retorno;
        }





        public enum EstadoRegistro
        {
            INACTIVO = 0,
            ACTIVO = 1,
            ANULADO = 9
        }

        public static string GetEstadoRegistro(int estado)
        {
            string retorno = "";
            if (estado == (int)EstadoRegistro.ACTIVO)
                retorno = EstadoRegistro.ACTIVO.ToString();
            if (estado == (int)EstadoRegistro.INACTIVO)
                retorno = EstadoRegistro.INACTIVO.ToString();
            if (estado == (int)EstadoRegistro.ANULADO)
                retorno = EstadoRegistro.ANULADO.ToString();
            return retorno;
        }


        public enum Emision
        {
            NORMAL = 1
        }

        public static string GetEmision(int? emision)
        {
            string retorno = "";
            if (emision == (int)Emision.NORMAL)
                retorno = Emision.NORMAL.ToString();
            return retorno;
        }


        public enum Ambiente
        {
            PRUEBAS = 1,
            PRODUCCIÓN = 2
        }

        public static string GetAmbiente(int? ambiente)
        {
            string retorno = "";
            if (ambiente == (int)Ambiente.PRUEBAS)
                retorno = Ambiente.PRUEBAS.ToString();
            if (ambiente == (int)Ambiente.PRODUCCIÓN)
                retorno = Ambiente.PRODUCCIÓN.ToString();
            return retorno;
        }

        public enum EstadoCorreo
        {
            INACTIVO = 0,
            ACTIVO = 1,
            ENVIADO = 2,
            ENTREGADO = 3,
            CANCELADO = 4,
            ERROR = 5,
            ANULADO = 9
        }

        public static string GetEstadoCorreo(int estado)
        {
            string retorno = "";
            if (estado == (int)EstadoCorreo.ACTIVO)
                retorno = EstadoCorreo.ACTIVO.ToString();
            if (estado == (int)EstadoCorreo.INACTIVO)
                retorno = EstadoCorreo.INACTIVO.ToString();
            if (estado == (int)EstadoCorreo.ENVIADO)
                retorno = EstadoCorreo.ENVIADO.ToString();
            if (estado == (int)EstadoCorreo.ENTREGADO)
                retorno = EstadoCorreo.ENTREGADO.ToString();
            if (estado == (int)EstadoCorreo.CANCELADO)
                retorno = EstadoCorreo.CANCELADO.ToString();
            if (estado == (int)EstadoCorreo.ERROR)
                retorno = EstadoCorreo.ERROR.ToString();
            if (estado == (int)EstadoCorreo.ANULADO)
                retorno = EstadoCorreo.ANULADO.ToString();
            return retorno;
        }

    }
}
