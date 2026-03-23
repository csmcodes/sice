using System;
using System.Collections.Generic;
using BusinessLogicLayer;
using BusinessObjects;

namespace AutomataSRI
{
    public enum AccionSRI
    {
        REENVIAR,
        VERIFICAR,
        ALERTAR,
        IGNORAR
    }

    public class ClasificadorMensajeSRI
    {
        private readonly List<SriReglaMensaje> _reglas;

        private ClasificadorMensajeSRI(List<SriReglaMensaje> reglas)
        {
            _reglas = reglas;
        }

        public static ClasificadorMensajeSRI CargarDesdeDB()
        {
            List<SriReglaMensaje> reglas = SriReglaMensajeBLL.GetAll(
                new WhereParams("srm_activo = {0}", true),
                "srm_orden ASC");
            return new ClasificadorMensajeSRI(reglas);
        }

        public AccionSRI Clasificar(string mensaje, int estado)
        {
            if (string.IsNullOrEmpty(mensaje))
                return AccionSRI.ALERTAR;

            string mensajeUpper = mensaje.ToUpper();

            foreach (SriReglaMensaje regla in _reglas)
            {
                if (regla.srm_estado.HasValue && regla.srm_estado.Value != estado)
                    continue;

                if (mensajeUpper.Contains(regla.srm_patron.ToUpper()))
                {
                    switch (regla.srm_accion.ToUpper())
                    {
                        case "REENVIAR":  return AccionSRI.REENVIAR;
                        case "VERIFICAR": return AccionSRI.VERIFICAR;
                        case "IGNORAR":   return AccionSRI.IGNORAR;
                        default:          return AccionSRI.ALERTAR;
                    }
                }
            }

            return AccionSRI.ALERTAR;
        }
    }
}
