using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using BusinessLogicLayer;

namespace Services
{
    public class Dictionaries
    {
        public static int cod_empresa = 0;

        public static Dictionary<string, string> Empty()
        {
            return new Dictionary<string, string>();
        }


        #region Get Obj

        public static object GetObject(object objeto, string id, Type tipo)
        {
            Dictionary<string, object> tmp = (Dictionary<string, object>)objeto;
            return GetObject(tmp, id, tipo);
        }


        public static object GetObject(Dictionary<string, object> dic, string id, Type tipo)
        {
            object obj = null;
            dic.TryGetValue(id, out obj);

            if (tipo == typeof(String))
                return Functions.Conversiones.ObjectToString(obj);
            if (tipo == typeof(Int32?))
                return Functions.Conversiones.ObjectToIntNull(obj);
            if (tipo == typeof(Decimal?))
                return Functions.Conversiones.ObjectToDecimalNull(obj);
            if (tipo == typeof(DateTime?))
                return Functions.Conversiones.ObjectToDateTimeNull(obj);
            if (tipo == typeof(Int32?))
                return Functions.Conversiones.ObjectToIntNull(obj);
            if (tipo == typeof(bool?))
                return Functions.Conversiones.ObjectToBoolNull(obj);
            if (tipo == typeof(long?))
                return Functions.Conversiones.ObjectToLongNull(obj);
            if (tipo == typeof(object[]))
                return Functions.Conversiones.ObjectToObjectArray(obj);
            return obj;
        }


        #endregion


    }
}
