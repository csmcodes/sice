using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace SqlDataBase
{
    public enum SentenceType
    {
        Insert = 1,
        InsertIdentity=2,
        Update = 3,
        Delete = 4,
        GetAll = 5,
        GetAllTop = 6,
        GetByPK = 7,
        Select = 8,
        GetAllByPage = 9,
        GetRecordCount =10,
        GetMax = 11,
        GetById = 12,
        GetSum =13,
        DeleteAll = 14

        
        

    }

    public class Sentences
    {

        protected static string prefix = "@";        
        protected static string[] abecedario = {"a","b","c","d","f","g","h"};

        public Sentences()
        {
        }


        public static DataTable CreaTablaReferencias()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("tabla", typeof(string)));
            dt.Columns.Add(new DataColumn("campos", typeof(string)));
            dt.Columns.Add(new DataColumn("joins", typeof(string)));
            return dt;
        }

        public static string CreaJoin(string tabla, string tablaref, string foreign, string keyref, string join)
        {
            string[] arrayforeign = foreign.Split(',');
            string[] arraykeyref= keyref.Split(',');

            string sql = "";
            for (int i = 0; i < arrayforeign.Length; i++)
            {
                sql += ((sql != "") ? " AND " : "") + tabla + "." + arrayforeign[i] + "=" + tablaref + "." + arraykeyref[i];
            }
            if (string.IsNullOrEmpty(join))
                join = "INNER";
            return join + " JOIN " + tablaref + " ON " + sql;
 
        }

        public static void AgregaReferencia(string tabla, string tablaref, string campo, string camporef, string foreign, string keyref, string join , DataTable dt)
        {
            if (!string.IsNullOrEmpty(tablaref))
            {
                bool existe = false;
                foreach (DataRow row in dt.Rows)
                {
                    if (row["tabla"].ToString() == tablaref)
                    {
                        existe = true;
                        row["campos"] = row["campos"].ToString() + ", " + tablaref + "." + camporef + " " + campo;
                    }
                }
                if (!existe)
                {
                    DataRow row = dt.NewRow();
                    row["tabla"] = tablaref;
                    row["campos"] = tablaref + "." + camporef + " " + campo;
                    row["joins"] = CreaJoin(tabla, tablaref, foreign, keyref, join);
                    dt.Rows.Add(row);
                }
            }
        }

        public static string GetSqlCamposReferencia(DataTable dt)
        {
            string sql ="";
            foreach (DataRow row in dt.Rows)
            {
                sql += "," + row["campos"].ToString();
            }
            return sql;
            
        }


        public static string GetSqlJoinsReferencia(DataTable dt)
        {
            string sql = " ";
            foreach (DataRow row in dt.Rows)
            {
                sql += " " + row["joins"].ToString();
            }
            return sql;

        }

        public static string GetSentence(SentenceType tipo, PropertyInfo[] properties, string tablename)
        {
            string sql = "";
            DataTable dt = CreaTablaReferencias(); 
            if (tipo == SentenceType.Insert || tipo == SentenceType.InsertIdentity )
            {

                sql = "INSERT INTO " + tablename + " (";
                string sql1 = "";
                string sql2 = "";

                foreach (PropertyInfo property in properties)
                {
                    bool add = true;
                    bool addkey = false;
                    object[] attributes = property.GetCustomAttributes(typeof(Data), true);
                    if (attributes.Length > 0)
                    {
                        Data data = (Data)attributes[0];
                        if (data.auto || data.originalkey || data.nosql || data.noprop)
                            add = false;                        
                    }
                    if (add)
                    {
                        sql1 += ((sql1 != "") ? "," : "") + property.Name;
                        sql2 += ((sql2 != "") ? "," : "") + prefix + property.Name;
                    }

                }

                sql += sql1 + ") VALUES (" + sql2 + ")";
                if (tipo == SentenceType.InsertIdentity)
                    sql += ";SELECT SCOPE_IDENTITY()";
            }
            else if (tipo == SentenceType.Update)
            {
                sql = "UPDATE " + tablename + " SET ";
                string sql1 = "";
                string sql2 = "";
                foreach (PropertyInfo property in properties)
                {

                    bool add = true;
                    bool addkey = false;
                    object[] attributes = property.GetCustomAttributes(typeof(Data), true);
                    if (attributes.Length > 0)
                    {
                        Data data = (Data)attributes[0];
                        if (data.originalkey)
                        {
                            add = false;
                            //addkey = true;
                        }
                        if (data.key && data.auto)
                            add = false;
                        if (data.key)
                            addkey = true;
                        if (data.noupdate || data.nosql)
                            add = false;
                        if (data.noprop)
                        {
                            add = false;
                            addkey = false;
                        }

                    }
                    if (add)
                        sql1 += ((sql1 != "") ? "," : "") + property.Name + "=" + prefix + property.Name;
                    if (addkey)
                        sql2 += ((sql2 != "") ? " AND " : "") + property.Name + "=" + prefix + property.Name + "_key";
                }
                sql += sql1 + " WHERE " + sql2;
            }
            else if (tipo == SentenceType.Delete)
            {
                sql = "DELETE FROM " + tablename + " WHERE ";
                string sql2 = "";

                foreach (PropertyInfo property in properties)
                {
                    bool add = true;
                    bool addkey = false;
                    object[] attributes = property.GetCustomAttributes(typeof(Data), true);
                    if (attributes.Length > 0)
                    {
                        Data data = (Data)attributes[0];
                        if (data.originalkey)
                            add = false;
                        if (data.key )
                            addkey = true;
                        if (data.noprop)
                        {
                            add = false;
                            addkey = false;
                        }

                    }
                    if (addkey)
                        //sql2 += ((sql2 != "") ? " AND " : "") + property.Name + "=" + prefix + property.Name + "_key";
                        sql2 += ((sql2 != "") ? " AND " : "") + property.Name + "=" + prefix + property.Name;
                }
                sql += sql2;
            }
            else if (tipo == SentenceType.GetByPK)
            {
                sql = "SELECT ";
                string sql1 = "";
                string sql2 = "";                

                foreach (PropertyInfo property in properties)
                {

                    bool add = true;
                    bool addkey = false;
                    bool addf = false;
                    
                    object[] attributes = property.GetCustomAttributes(typeof(Data), true);
                    if (attributes.Length > 0)
                    {
                        Data data = (Data)attributes[0];
                        if (data.originalkey)                        
                            add = false;
                        if (data.key)
                            addkey = true;
                        if (data.nosql)
                        {
                            add = false;
                            AgregaReferencia(tablename,data.tablaref,property.Name, data.camporef,data.foreign,data.keyref, data.join ,dt);                              
                        }
                        if (data.noprop)
                        {
                            add = false;
                            addkey = false;
                        }

                    }
                    if (add)
                        sql1 += ((sql1 != "") ? "," : "") + tablename + "." + property.Name;                    
                    if (addkey)
                        sql2 += ((sql2 != "") ? " AND " : "") + tablename + "."+ property.Name + "=" + prefix + property.Name + "_key";
                }
                
                sql += sql1 + GetSqlCamposReferencia(dt)  + " FROM " + tablename + GetSqlJoinsReferencia(dt)  + " WHERE " + sql2;
            }
            else if (tipo == SentenceType.GetById)
            {
                sql = "SELECT ";
                string sql1 = "";
                string sql2 = "";

                foreach (PropertyInfo property in properties)
                {

                    bool add = true;
                    bool addid = false;

                    if (property.Name.IndexOf("_id") >= 0)
                    {
                        addid = true;
                    }
                    object[] attributes = property.GetCustomAttributes(typeof(Data), true);
                    if (attributes.Length > 0)
                    {
                        Data data = (Data)attributes[0];
                        if (data.originalkey)
                            add = false;
                        if (data.nosql)
                        {
                            add = false;
                            AgregaReferencia(tablename, data.tablaref, property.Name, data.camporef, data.foreign, data.keyref, data.join, dt);
                        }
                        if (data.noprop)
                        {
                            add = false;                        
                        }

                    }
                    if (add)
                        sql1 += ((sql1 != "") ? "," : "") + tablename + "." + property.Name;
                    if (addid)
                        sql2 += ((sql2 != "") ? " AND " : "") + tablename + "." + property.Name + "=" + prefix + property.Name;
                }

                sql += sql1 + GetSqlCamposReferencia(dt) + " FROM " + tablename + GetSqlJoinsReferencia(dt) + " WHERE " + sql2;
            }
            else if (tipo == SentenceType.GetAll || tipo == SentenceType.GetAllTop )
            {
                sql = "SELECT ";
                if (tipo == SentenceType.GetAllTop)
                    sql +=  " TOP %top% ";
                string sql1 = "";                
                foreach (PropertyInfo property in properties)
                {

                    bool add = true;
                    bool addkey = false;
                    object[] attributes = property.GetCustomAttributes(typeof(Data), true);
                    if (attributes.Length > 0)
                    {
                        Data data = (Data)attributes[0];
                        if (data.originalkey)
                            add = false;
                        if (data.nosql)
                        {
                            add = false;
                            AgregaReferencia(tablename, data.tablaref, property.Name, data.camporef, data.foreign, data.keyref, data.join, dt);                              
                        }
                        if (data.noprop)
                        {
                            add = false;
                        }
                    }
                    if (add)
                        sql1 += ((sql1 != "") ? "," : "") + tablename + "." + property.Name;
                }

                //sql += sql1 + " FROM " + tablename;
                sql += sql1 + GetSqlCamposReferencia(dt) + " FROM " + tablename + GetSqlJoinsReferencia(dt);
            }            
            else if (tipo == SentenceType.GetAllByPage)
            {
                sql = "SELECT ";
                string sql1 = "";
                string sql2 = "";
                foreach (PropertyInfo property in properties)
                {

                    bool add = true;
                    bool addf = true;
                    bool addkey = false;
                    object[] attributes = property.GetCustomAttributes(typeof(Data), true);
                    if (attributes.Length > 0)
                    {
                        Data data = (Data)attributes[0];
                        if (data.originalkey)
                            add = false;
                        if (data.nosql)
                        {
                            addf = false;
                            AgregaReferencia(tablename, data.tablaref, property.Name, data.camporef, data.foreign, data.keyref, data.join, dt);                              

                        }
                        if (data.noprop)
                        {
                            add = false;
                        }
                    }
                    if (add)
                    {
                        sql1 += ((sql1 != "") ? "," : "") + " t." + property.Name;
                        if (addf)
                        sql2 += ((sql2 != "") ? "," : "") + tablename + "." + property.Name;
                        //sql1 += ((sql1 != "") ? "," : "") + tablename + "." + property.Name;
                    }
                }

                sql += sql1 +  " FROM (SELECT ROW_NUMBER() OVER(ORDER BY %orderby%) RowNr, " + sql2 + GetSqlCamposReferencia(dt) + " FROM " + tablename+  GetSqlJoinsReferencia(dt) + " %whereclause%) t WHERE RowNr BETWEEN %desde% AND %hasta%";

            }
            else if (tipo == SentenceType.GetRecordCount)
            {
                sql = "SELECT COUNT(*) FROM "+tablename;                

            }
            else if (tipo == SentenceType.GetMax)
            {
                sql = "SELECT MAX(%campo%) FROM " + tablename;

            }

            else if (tipo == SentenceType.GetSum)
            {
                sql = "SELECT SUM(%campo%) FROM " + tablename;

            }
            else if (tipo == SentenceType.DeleteAll)
            {
                sql = "DELETE FROM " + tablename;                
            }          

            return sql;
        }
        

        #region  Tipos SQL

                

        public static SqlDbType GetDBType(Type theType)
        {
            SqlParameter param;
            System.ComponentModel.TypeConverter tc;
            param = new SqlParameter();
            tc = System.ComponentModel.TypeDescriptor.GetConverter(param.DbType);
            if (tc.CanConvertFrom(theType))
            {
                param.DbType = (DbType)tc.ConvertFrom(theType.Name);
            }
            else
            {
                // try to forcefully convert
                try
                {
                    param.DbType = (DbType)tc.ConvertFrom(theType.Name);
                }
                catch (Exception e)
                {
                    // ignore the exception
                }
            }
            return param.SqlDbType;
        }

        #endregion



        



        #region  Insert

       



        /*
         public static bool Insert(DataTable dt, object conexion, object transaccion)
         {
             string sql = CreaSentencia("INSERT", dt);
             SqlCommand dCmd = CreaCommand(sql, dt, conexion);
             return Execute(dCmd, transaccion);
         }

         public static bool Insert(List<DataTable> Listdt)
         {
             Conexion.Connect();
             List<SqlCommand> ListdCmd = new List<SqlCommand>();
             foreach (DataTable dt in Listdt)
             {
                 string sql = CreaSentencia("INSERT", dt);
                 SqlCommand dCmd = CreaCommand(sql, dt);
                 ListdCmd.Add(dCmd);
             }
             return Execute(ListdCmd);
         }

         public static bool InsertAutonumerico(List<DataTable> Listdt)
         {
             Conexion.Connect();
             List<SqlCommand> ListdCmd = new List<SqlCommand>();
             foreach (DataTable dt in Listdt)
             {
                 string sql = CreaSentencia("INSERT AUTONUMERICO", dt);
                 SqlCommand dCmd = CreaCommand(sql, dt);
                 ListdCmd.Add(dCmd);
             }
             return Execute(ListdCmd);
         }

         public static bool Insert(List<DataTable> Listdt, object conexion, object transaccion)
         {
             List<SqlCommand> ListdCmd = new List<SqlCommand>();
             foreach (DataTable dt in Listdt)
             {
                 string sql = CreaSentencia("INSERT", dt);
                 SqlCommand dCmd = CreaCommand(sql, dt, conexion);
                 ListdCmd.Add(dCmd);
             }
             return Execute(ListdCmd, transaccion);
         }

         public static bool InsertAutonumerico(List<DataTable> Listdt, object conexion, object transaccion)
         {
             List<SqlCommand> ListdCmd = new List<SqlCommand>();
             foreach (DataTable dt in Listdt)
             {
                 string sql = CreaSentencia("INSERT AUTONUMERICO", dt);
                 SqlCommand dCmd = CreaCommand(sql, dt, conexion);
                 ListdCmd.Add(dCmd);
             }
             return Execute(ListdCmd, transaccion);
         }

         */

        #endregion




        #region  Update

      

        /*

        public static bool Update(DataTable dt, object conexion, object transaccion)
        {
            string sql = CreaSentencia("UPDATE", dt);
            SqlCommand dCmd = CreaCommand(sql, dt, conexion);
            return Execute(dCmd, transaccion);
        }

        public static bool Update(List<DataTable> Listdt)
        {
            Conexion.Connect();
            List<SqlCommand> ListdCmd = new List<SqlCommand>();
            foreach (DataTable dt in Listdt)
            {
                string sql = CreaSentencia("UPDATE", dt);
                SqlCommand dCmd = CreaCommand(sql, dt);
                ListdCmd.Add(dCmd);
            }
            return Execute(ListdCmd);
        }

        public static bool Update(List<DataTable> Listdt, object conexion, object transaccion)
        {
            List<SqlCommand> ListdCmd = new List<SqlCommand>();
            foreach (DataTable dt in Listdt)
            {
                string sql = CreaSentencia("UPDATE", dt);
                SqlCommand dCmd = CreaCommand(sql, dt, conexion);
                ListdCmd.Add(dCmd);
            }
            return Execute(ListdCmd, transaccion);
        }
         */

        #endregion

        #region  Delete

       
        /*

        public static bool Delete(DataTable dt, object conexion, object transaccion)
        {
            string sql = CreaSentencia("DELETE", dt);
            SqlCommand dCmd = CreaCommand(sql, dt, conexion);
            return Execute(dCmd, transaccion);
        }

        public static bool Delete(List<DataTable> Listdt)
        {
            Conexion.Connect();
            List<SqlCommand> ListdCmd = new List<SqlCommand>();
            foreach (DataTable dt in Listdt)
            {
                string sql = CreaSentencia("DELETE", dt);
                SqlCommand dCmd = CreaCommand(sql, dt);
                ListdCmd.Add(dCmd);
            }
            return Execute(ListdCmd);
        }

        public static bool Delete(List<DataTable> Listdt, object conexion, object transaccion)
        {
            List<SqlCommand> ListdCmd = new List<SqlCommand>();
            foreach (DataTable dt in Listdt)
            {
                string sql = CreaSentencia("DELETE", dt);
                SqlCommand dCmd = CreaCommand(sql, dt, conexion);
                ListdCmd.Add(dCmd);
            }
            return Execute(ListdCmd, transaccion);
        }
        */
        #endregion



        
       

     



        //public static DataSet Select(string sql, object conexion, object transaccion)
        //{
        //    SqlDataAdapter dAd = new SqlDataAdapter(sql, (SqlConnection)conexion);
        //    return Fill(dAd, transaccion);
        //}

        //public static DataSet Select(string sql, List<Parametro> parametros)
        //{
        //    Conexion.Connect();
        //    SqlDataAdapter dAd = CreaDataAdapter(sql, parametros);
        //    return Fill(dAd);
        //}
        //public static DataSet Select(string sql, List<Parametro> parametros, object conexion, object transaccion)
        //{
        //    SqlDataAdapter dAd = CreaDataAdapter(sql, parametros, conexion);
        //    return Fill(dAd, transaccion);
        //}






        

        #region  Fill

        private static DataSet FillDataSet(SqlDataAdapter dAd)
        {
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
 
            try
            {
                dAd.Fill(dSet);
                return dSet;
            }
            catch (Exception ex)
            {
                throw ex;
                //return dSet;
            }
            finally
            {
                dSet.Dispose();
                dAd.Dispose();                
            }
        }

        private static DataSet FillDataSet(SqlDataAdapter dAd, object transaccion)
        {
            DataSet dSet = new DataSet();
            try
            {
                dAd.SelectCommand.Transaction = (SqlTransaction)transaccion;
                dAd.Fill(dSet);
                return dSet;
            }
            catch (Exception ex)
            {
                throw ex;
                //Log.Add(ex);
                //return dSet;
            }
            finally
            {
                dSet.Dispose();
                dAd.Dispose();
            }
        }

        private static DataTable FillDataTable(SqlDataAdapter dAd)
        {
            DataTable dt = new DataTable();
            try
            {
                dAd.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
                //Log.Add(ex);
                //return dt;
            }
            finally
            {
                dt.Dispose();
                dAd.Dispose();                
            }
        }

        private static DataTable FillDataTable(SqlDataAdapter dAd, object transaccion)
        {
            DataTable dt = new DataTable();
            try
            {
                dAd.SelectCommand.Transaction = (SqlTransaction)transaccion;
                dAd.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
                //Log.Add(ex);
                //return dt;
            }
            finally
            {
                dt.Dispose();
                dAd.Dispose();
            }
        }


        #endregion


        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

    }
}
