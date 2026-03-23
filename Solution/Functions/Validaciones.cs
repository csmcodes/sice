using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Functions
{
    public class Validaciones
    {

        public static bool valida_cedularuc(string cedula)
        {
            try
            {

                int largo = cedula.Length;
                if (largo == 10 || largo == 13)
                {
                    int digito = int.Parse(cedula.Substring(2, 1));

                    string sucursal = (largo == 13) ? cedula.Substring(10, 3) : "001";
                    string datos = cedula.Substring(0, 9);
                    string verificador = cedula.Substring(9, 1);


                    int[] coeficientes = new int[] {2, 1, 2, 1, 2, 1, 2, 1, 2 };
                    if (digito < 6)
                    {

                        int suma = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            int d = int.Parse(datos.Substring(i, 1));
                            int c = coeficientes[i];

                            int p = d * c;
                            if (p > 9)
                                p = p - 9;

                            suma += p;
                        }

                        int entero = suma / 10;
                        int proximodec = (entero * 10) + 10;

                        int sobra = proximodec - suma;
                        if (sobra == 10)
                        {
                            if (verificador == "0")
                                return true;
                            else
                                return false;
                        }
                        else
                        {
                            if (sobra.ToString() != verificador)
                                return false;
                            else
                                return true;
                        }
                    }
                    else if (digito == 6)
                    {
                        verificador = cedula.Substring(8, 1);
                        coeficientes = new int[] { 3, 2, 7, 6, 5, 4, 3, 2 };

                        int suma = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            int d = int.Parse(datos.Substring(i, 1));
                            int c = coeficientes[i];

                            int p = d * c;
                            suma += p;
                        }

                        int entero = suma / 11;
                        int residuo = suma % 11;
                        int valor = 0;
                        if (residuo>0)
                            valor = 11 - residuo;

                        if (valor.ToString() == verificador)
                            return true;
                        else
                            return false;
                    }
                    else if (digito == 9)
                    {
                        verificador = cedula.Substring(9, 1);
                        coeficientes = new int[] { 4, 3, 2, 7, 6, 5, 4, 3, 2 };
                        int suma = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            int d = int.Parse(datos.Substring(i, 1));
                            int c = coeficientes[i];

                            int p = d * c;
                            suma += p;
                        }

                        int entero = suma / 11;
                        int residuo = suma % 11;
                        int valor = 0;
                        if (residuo>0)
                            valor = 11 - residuo;
                        if (valor.ToString() == verificador)
                            return true;
                        else
                            return false;

                    }
                    else
                        return false;
                }
                else
                    return false;
                    
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        

    }
}
