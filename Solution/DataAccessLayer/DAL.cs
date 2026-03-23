using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using SqlDataBase;
using System.Data;

namespace DataAccessLayer
{
    public enum Provider
    {
        None = 0,
        SqlServer = 1,
        PostgreSQL = 2,
        Oracle = 3        

    }

    public class DAL
    {

        public SqlDataBase.TransactionManager transaccion { get; set; }
        public SqlDataBasePG.TransactionManager transaccionpg { get; set; }

        public DAL()
        {
        }

        public void CreateTransaction()
        {
            if (GetProvider() ==Provider.SqlServer)
                transaccion = new TransactionManager();
            else if (GetProvider() == Provider.PostgreSQL)
                transaccionpg = new SqlDataBasePG.TransactionManager(); 
        }

        public void BeginTransaction()
        {
            if (GetProvider() == Provider.SqlServer)
                transaccion.BeginTransaction();
            else if (GetProvider() == Provider.PostgreSQL)
                transaccionpg.BeginTransaction();  
        }

        public void Commit()
        {
            if (GetProvider() == Provider.SqlServer)
                transaccion.Commit();
            else if (GetProvider() == Provider.PostgreSQL)
                transaccionpg.Commit();  
        }

        public void Rollback()
        {
            if (GetProvider() == Provider.SqlServer)
                transaccion.Rollback();
            else if (GetProvider() == Provider.PostgreSQL)
                transaccionpg.Rollback();  
        }

        public static Provider GetProvider()
        {
            string valor = System.Configuration.ConfigurationSettings.AppSettings["provider"].ToString();
            if (valor.IndexOf(Provider.SqlServer.ToString()) >= 0)
                return Provider.SqlServer;
            else if (valor.IndexOf(Provider.PostgreSQL.ToString()) >= 0)
                return Provider.PostgreSQL;
            else if (valor.IndexOf(Provider.Oracle.ToString()) >= 0)
                return Provider.Oracle;
            else
                return Provider.None; 

        }
    }

}
