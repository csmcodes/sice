using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessObjects
{
   

    public class WhereParams
    {        
       

        public string where { get; set; }
        public object[] valores { get; set; }
        
        public WhereParams()
        {
            this.where = "";
            this.valores = new object[0];
        }

        public WhereParams(string where, params object[] valores)
        {
            this.where = where;
            this.valores = valores;
        }

        public WhereParams(params object[] valores)
        {
            this.where = "";
            this.valores = valores;
        }


        /*public WhereParams(string campos, string operador, object valor1)
        {
            this.campos = campos;
            this.operador = operador;
            this.valor1 = valor1;
            this.valor2 = null;
        }

        public WhereParams(string campos, string operador, object valor1, object valor2)
        {
            this.campos = campos;
            this.operador = operador;
            this.valor1 = valor1;
            this.valor2 = valor2;
        }*/


      

        

        

    }
}
