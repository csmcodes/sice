using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Functions
{
    public class Formatos
    {
        public static string CurrencyFormat(decimal? valor)
        {
            if (valor.HasValue)
            {


                //string number = valor.ToString().Replace(",", ".");

                //int punto = number.IndexOf(".");
                //int cant = number

                //int count= number.Substring(number.IndexOf(".") + 1).Length;
                
                //if (count>2)
                //{
                //    return string.Format("{0:0.000000}", valor).Replace(",", ".");
                //}
                //else
                    return string.Format("{0:0.00####}", valor).Replace(",", ".");
            }
            return "0.00";
        }

        public static string CurrencyFormat1(decimal? valor)
        {
            if (valor.HasValue)
            {
                return valor.Value.ToString("0.00");
                //return string.Format("{0:0.00}", valor).Replace(",", ".");
            }
            return "0.00";
        }

        public static string BalanceFormat(decimal? valor)
        {
            if (valor.HasValue)
            {
                return string.Format(new CultureInfo("en-US"), "{0:N2}", valor);
            }
            return "0.00";
        }


        public static string getMod11(string clave48)
        {

            int largo = clave48.Length;
            if (largo == 48)
            {
                int[] coeficientes = new int[] { 7, 6, 5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
                int suma = 0;
                for (int i = 0; i < 48; i++)
                {
                    int d = int.Parse(clave48.Substring(i, 1));
                    int c = coeficientes[i];
                    int p = d * c;
                    suma += p;
                }

                int residuo = suma % 11;
                int digito = 11 - residuo;

                return clave48 + digito.ToString();
            }
            return "";



        }


        public static string FillLeft(string texto, string caracter, int cantidad)
        {
            string fill = "";
            int desde = texto.Length;
            for (int i = desde; i < cantidad; i++)
            {
                fill += caracter;
            }
            return fill + texto;


        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }


        public static string HorarioHora(DateTime? fecha)
        {
            if (fecha.HasValue)
            {
                string dia = FirstCharToUpper(fecha.Value.ToString("dddd", new CultureInfo("es-ES"))).Substring(0, 3);
                string hora = fecha.Value.ToString("HH:mm");
                return dia + " " + hora;
            }
            else
                return "";
        }

        public static string GetDia(DateTime? fecha)
        {
            if (fecha.HasValue)
            {
                return FirstCharToUpper(fecha.Value.ToString("dddd", new CultureInfo("es-ES")));
            }
            else
                return "";
        }
        public static string GetHora(DateTime? fecha)
        {
            if (fecha.HasValue)
            {
                return fecha.Value.ToString("HH:mm");
            }
            else
                return "";
        }

        public static string GetTime(TimeSpan? tiempo)
        {
            if (tiempo.HasValue)
                return String.Format("{0:00}:{1:00}:{2:00}", (tiempo.Value.Hours + tiempo.Value.Days * 24), tiempo.Value.Minutes, tiempo.Value.Seconds);
            //return tiempo.Value.ToString("hh\\:mm\\:ss", new CultureInfo("es-ES"));
            return "";
        }

    }
}
