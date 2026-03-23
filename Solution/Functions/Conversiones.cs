using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Functions
{
    public class Conversiones
    {

        public static string LogicToString(int? valor)
        {
            if (valor.HasValue)
            {
                if (valor.Value == 1)
                    return "SI";
            }
            return "NO";
        }
    
        public static object GetValueByType(object valor, Type type)
        {
            if (type != typeof(string))
            {
                if (valor != null)
                {
                    type = Nullable.GetUnderlyingType(type) ?? type;
                    try
                    {
                        return Convert.ChangeType(valor, type);
                    }
                    catch
                    {
                        return Activator.CreateInstance(type);
                    }
                }
                else
                    return Activator.CreateInstance(type);
            }
            else
                return valor;
        }

        public static decimal GetDecimal(string valor)
        {
            decimal valordecimal = 0;
            decimal.TryParse(valor.Replace('.', ','), out valordecimal);
            return valordecimal;
        }


        


        public static int? ObjectToIntNull(object valor)
        {
            if (valor != null)
            {
                int entero;
                if (int.TryParse(valor.ToString(), out entero))
                    return entero;

            }
            return null;

        }

        public static decimal? ObjectToDecimalNull(object valor)
        {
            if (valor != null)
            {
                decimal dec;
                if (decimal.TryParse(valor.ToString().Replace(".", ","), out dec))
                    return dec;

            }
            return null;

        }

        public static DateTime? ObjectToDateTimeNull(object valor)
        {
            if (valor != null)
            {
                DateTime dt;
                if (DateTime.TryParse(valor.ToString(), out dt))
                    return dt;

            }
            return null;

        }
        public static String ObjectToString(object valor)
        {
            if (valor != null)
                return valor.ToString();

            return null;

        }

        public static long? ObjectToLongNull(object valor)
        {
            if (valor != null)
            {
                long dec;
                if (long.TryParse(valor.ToString().Replace(".", ","), out dec))
                    return dec;

            }
            return null;

        }
        public static bool? ObjectToBoolNull(object valor)
        {
            if (valor != null)
            {
                bool boo;
                if (bool.TryParse(valor.ToString(), out boo))
                    return boo;

            }
            return null;

        }
        public static string GetDateString(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToShortDateString();
            }
            else
                return "";

        }

        public static object[] ObjectToObjectArray(object obj)
        {
            if (obj != null)
                return (object[])obj;
            return null;
        }

        public static string NumeroALetras(string num)
        {
            string res, dec = "";
            Int64 entero;
            int decimales;
            double nro;

            try
            {
                nro = Convert.ToDouble(num);
            }
            catch
            {
                return "";
            }

            entero = Convert.ToInt64(Math.Truncate(nro));
            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));

            //dec = " " + decimales.ToString() + "/100";

            if (decimales > 0)
            {
                dec = " CON " + NumeroALetras(decimales.ToString()).Replace("DÓLARES","").Trim() + " CENTAVOS";
            }

            res = NumeroALetras(Convert.ToDouble(entero)) + " DÓLARES " + dec;
            return res;
        }

        private static string NumeroALetras(double value)
        {
            string Num2Text = "";
            value = Math.Truncate(value);

            if (value == 0) Num2Text = "CERO";
            else if (value == 1) Num2Text = "UNO";
            else if (value == 2) Num2Text = "DOS";
            else if (value == 3) Num2Text = "TRES";
            else if (value == 4) Num2Text = "CUATRO";
            else if (value == 5) Num2Text = "CINCO";
            else if (value == 6) Num2Text = "SEIS";
            else if (value == 7) Num2Text = "SIETE";
            else if (value == 8) Num2Text = "OCHO";
            else if (value == 9) Num2Text = "NUEVE";
            else if (value == 10) Num2Text = "DIEZ";
            else if (value == 11) Num2Text = "ONCE";
            else if (value == 12) Num2Text = "DOCE";
            else if (value == 13) Num2Text = "TRECE";
            else if (value == 14) Num2Text = "CATORCE";
            else if (value == 15) Num2Text = "QUINCE";
            else if (value < 20) Num2Text = "DIECI" + NumeroALetras(value - 10);
            else if (value == 20) Num2Text = "VEINTE";
            else if (value < 30) Num2Text = "VEINTI" + NumeroALetras(value - 20);
            else if (value == 30) Num2Text = "TREINTA";
            else if (value == 40) Num2Text = "CUARENTA";
            else if (value == 50) Num2Text = "CINCUENTA";
            else if (value == 60) Num2Text = "SESENTA";
            else if (value == 70) Num2Text = "SETENTA";
            else if (value == 80) Num2Text = "OCHENTA";
            else if (value == 90) Num2Text = "NOVENTA";

            else if (value < 100) Num2Text = NumeroALetras(Math.Truncate(value / 10) * 10) + " Y " + NumeroALetras(value % 10);
            else if (value == 100) Num2Text = "CIEN";
            else if (value < 200) Num2Text = "CIENTO " + NumeroALetras(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = NumeroALetras(Math.Truncate(value / 100)) + "CIENTOS";

            else if (value == 500) Num2Text = "QUINIENTOS";
            else if (value == 700) Num2Text = "SETECIENTOS";
            else if (value == 900) Num2Text = "NOVECIENTOS";
            else if (value < 1000) Num2Text = NumeroALetras(Math.Truncate(value / 100) * 100) + " " + NumeroALetras(value % 100);
            else if (value == 1000) Num2Text = "MIL";
            else if (value < 2000) Num2Text = "MIL " + NumeroALetras(value % 1000);
            else if (value < 1000000)
            {
                Num2Text = NumeroALetras(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0) Num2Text = Num2Text + " " + NumeroALetras(value % 1000);
            }

            else if (value == 1000000) Num2Text = "UN MILLON";
            else if (value < 2000000) Num2Text = "UN MILLON " + NumeroALetras(value % 1000000);
            else if (value < 1000000000000)
            {
                Num2Text = NumeroALetras(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000) * 1000000);
            }
            else if (value == 1000000000000) Num2Text = "UN BILLON";
            else if (value < 2000000000000) Num2Text = "UN BILLON " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            else
            {
                Num2Text = NumeroALetras(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }

            return Num2Text;
        }

    }
}
