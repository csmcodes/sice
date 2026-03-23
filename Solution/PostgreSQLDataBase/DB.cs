using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using System.Reflection;
using BusinessObjects;
using System.Data;
using System.Web;

namespace SqlDataBasePG
{
    public class DB
    {

        protected static string prefix = "@";


        #region Insert

        public static int Insert(TransactionManager transaction, PropertyInfo[] properties, string cmdText, object context)
        {
            NpgsqlCommand cmd = CreateCommand(cmdText, CommandType.Text, transaction);
            cmd = AddParameters(cmd, properties, SentenceType.Insert, context);
            return ExecuteNonQuery(cmd);
        }

        public static int Insert(PropertyInfo[] properties, string cmdText, object context)
        {
            int res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = Insert(transaction, properties, cmdText, context);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;

        }

        public static int InsertSQL(TransactionManager transaction, PropertyInfo[] properties, string tablename, object context)
        {
            string sql = Sentences.GetSentence(SentenceType.Insert, properties, tablename);
            return Insert(transaction, properties, sql, context);
        }

        public static int InsertSQL(PropertyInfo[] properties, string tablename, object context)
        {
            string sql = Sentences.GetSentence(SentenceType.Insert, properties, tablename);
            return Insert(properties, sql, context);
        }

        #endregion

        #region Insert Identity

        public static int InsertIdentity(TransactionManager transaction, PropertyInfo[] properties, string cmdText, object context)
        {
            NpgsqlCommand cmd = CreateCommand(cmdText, CommandType.Text, transaction);
            cmd = AddParameters(cmd, properties, SentenceType.InsertIdentity, context);
            return int.Parse(ExecuteEscalar(cmd).ToString());
        }

        public static int InsertIdentity(PropertyInfo[] properties, string cmdText, object context)
        {
            int res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = InsertIdentity(transaction, properties, cmdText, context);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;

        }

        public static int InsertIdentitySQL(TransactionManager transaction, PropertyInfo[] properties, string tablename, object context)
        {
            string sql = Sentences.GetSentence(SentenceType.InsertIdentity, properties, tablename);
            return InsertIdentity(transaction, properties, sql, context);
        }

        public static int InsertIdentitySQL(PropertyInfo[] properties, string tablename, object context)
        {
            string sql = Sentences.GetSentence(SentenceType.InsertIdentity, properties, tablename);
            return InsertIdentity(properties, sql, context);
        }

        #endregion

        #region Update

        public static int Update(TransactionManager transaction, PropertyInfo[] properties, string cmdText, object context)
        {
            NpgsqlCommand cmd = CreateCommand(cmdText, CommandType.Text, transaction);
            cmd = AddParameters(cmd, properties, SentenceType.Update, context);
            return ExecuteNonQuery(cmd);
        }

        public static int Update(PropertyInfo[] properties, string cmdText, object context)
        {
            int res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = Update(transaction, properties, cmdText, context);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;

        }


        public static int UpdateSQL(TransactionManager transaction, PropertyInfo[] properties, string tablename, object context)
        {
            string sql = Sentences.GetSentence(SentenceType.Update, properties, tablename);
            return Update(transaction, properties, sql, context);
        }

        public static int UpdateSQL(PropertyInfo[] properties, string tablename, object context)
        {
            string sql = Sentences.GetSentence(SentenceType.Update, properties, tablename);
            return Update(properties, sql, context);
        }

        #endregion

        #region Delete
        public static int Delete(TransactionManager transaction, PropertyInfo[] properties, string cmdText, object context)
        {
            NpgsqlCommand cmd = CreateCommand(cmdText, CommandType.Text, transaction);
            cmd = AddParameters(cmd, properties, SentenceType.Delete, context);
            return ExecuteNonQuery(cmd);
        }

        public static int Delete(PropertyInfo[] properties, string cmdText, object context)
        {
            int res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = Delete(transaction, properties, cmdText, context);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;

        }



        public static int DeleteSQL(TransactionManager transaction, PropertyInfo[] properties, string tablename, object context)
        {
            string sql = Sentences.GetSentence(SentenceType.Delete, properties, tablename);
            return Delete(transaction, properties, sql, context);
        }

        public static int DeleteSQL(PropertyInfo[] properties, string tablename, object context)
        {
            string sql = Sentences.GetSentence(SentenceType.Delete, properties, tablename);
            return Delete(properties, sql, context);
        }

        #endregion

        #region Delete All

        public static int DeleteAll(string WhereClause, string tablename)
        {
            int res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = DeleteAll(transaction, WhereClause, tablename);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;
        }

        public static int DeleteAll(TransactionManager transaction, string WhereClause, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.DeleteAll, null, tablename);
            sql += (WhereClause != "") ? " WHERE " + WhereClause : "";
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            return ExecuteNonQuery(cmd);
        }




        public static int DeleteAll(WhereParams parametros, string tablename)
        {
            int res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = DeleteAll(transaction, parametros, tablename);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;
        }



        public static int DeleteAll(TransactionManager transaction, WhereParams parametros, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.DeleteAll, null, tablename);
            string sqlwhere = SetWhereClause(parametros);
            sql += (sqlwhere != "") ? " WHERE " + sqlwhere : "";
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return ExecuteNonQuery(cmd);
        }

        #endregion



        #region Get By Primary Key

        public static IDataReader GetByPKSql(TransactionManager transaction, PropertyInfo[] properties, string tablename, object context)
        {
            string sql = Sentences.GetSentence(SentenceType.GetByPK, properties, tablename);
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, properties, SentenceType.GetByPK, context);
            return ExecuteReader(cmd);
        }


        #endregion

        #region Get By ID

        public static IDataReader GetById(TransactionManager transaction, PropertyInfo[] properties, string tablename, object context)
        {
            string sql = Sentences.GetSentence(SentenceType.GetById, properties, tablename);
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, properties, SentenceType.GetById, context);
            return ExecuteReader(cmd);
        }

        #endregion

        #region Get All

        //DESCONTINUADO
        public static IDataReader GetAll(TransactionManager transaction, string WhereClause, string OrderBy, int Top, string cmdText)
        {
            NpgsqlCommand cmd = CreateCommand(cmdText, CommandType.StoredProcedure, transaction);
            cmd.Parameters.Add(AddParameter(prefix + "WhereClause", GetDBType(typeof(System.String)), WhereClause));
            cmd.Parameters.Add(AddParameter(prefix + "OrderBy", GetDBType(typeof(System.String)), OrderBy));
            cmd.Parameters.Add(AddParameter(prefix + "Top", GetDBType(typeof(System.Int32)), Top));
            return ExecuteReader(cmd);
        }




        public static IDataReader GetAll(TransactionManager transaction, string WhereClause, string OrderBy, PropertyInfo[] properties, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetAll, properties, tablename);
            sql += (WhereClause != "") ? " WHERE " + WhereClause : "";
            sql += (OrderBy != "") ? " ORDER BY " + OrderBy : "";
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            return ExecuteReader(cmd);
        }




        public static IDataReader GetAll(TransactionManager transaction, WhereParams parametros, string OrderBy, PropertyInfo[] properties, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetAll, properties, tablename);
            string sqlwhere = SetWhereClause(parametros);
            sql += (sqlwhere != "") ? " WHERE " + sqlwhere : "";
            sql += (OrderBy != "") ? " ORDER BY " + OrderBy : "";
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return ExecuteReader(cmd);
        }

        public static IDataReader GetAll(TransactionManager transaction, WhereParams parametros, string OrderBy, string sql)
        {
            string sqlwhere = SetWhereClause(parametros);
            sql += (sqlwhere != "") ? " WHERE " + sqlwhere : "";
            sql += (OrderBy != "") ? " ORDER BY " + OrderBy : "";
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return ExecuteReader(cmd);
        }

        public static IDataReader GetAllGroup(TransactionManager transaction, WhereParams parametros, string OrderBy, string sql)
        {
            string sqlwhere = SetWhereClause(parametros);
             sqlwhere = (sqlwhere != "") ? " WHERE " + sqlwhere : "";
            
            sql = sql.Replace("%whereclause%", sqlwhere);
            sql += (OrderBy != "") ? " ORDER BY " + OrderBy : "";
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return ExecuteReader(cmd);
        }




        public static IDataReader GetAllView(TransactionManager transaction, WhereParams parametros, string sql)
        {

            for (int i = 0; i < parametros.valores.Length; i++)
            {
                sql = sql.Replace("{" + i.ToString() + "}", prefix + "par" + i.ToString());
            }
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return ExecuteReader(cmd);
        }

        public static IDataReader GetAllHaving(TransactionManager transaction, WhereParams parametros, string OrderBy, string sql)
        {

            string sqlwhere = SetWhereClause(parametros);
            if (sqlwhere != "")
                sqlwhere = " HAVING " + sqlwhere;
            if (OrderBy != "")
                OrderBy = " ORDER BY " + OrderBy;
            sql = sql.Replace("%orderby%", OrderBy).Replace("%whereclause%", sqlwhere);

            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return ExecuteReader(cmd);
        }



        #endregion

        #region Get All Top

        public static IDataReader GetAllTop(TransactionManager transaction, string WhereClause, string OrderBy, int top, PropertyInfo[] properties, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetAllTop, properties, tablename);
            sql += (WhereClause != "") ? " WHERE " + WhereClause : "";
            sql += (OrderBy != "") ? " ORDER BY " + OrderBy : "";
            sql = sql.Replace("%top%", top.ToString());
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            return ExecuteReader(cmd);
        }




        public static IDataReader GetAllTop(TransactionManager transaction, WhereParams parametros, string OrderBy, int top, PropertyInfo[] properties, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetAllTop, properties, tablename);
            string sqlwhere = SetWhereClause(parametros);
            sql += (sqlwhere != "") ? " WHERE " + sqlwhere : "";
            sql += (OrderBy != "") ? " ORDER BY " + OrderBy : "";
            sql += " LIMIT " + top.ToString();             
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return ExecuteReader(cmd);
        }



        public static IDataReader GetAllTop(TransactionManager transaction, WhereParams parametros, string OrderBy, int top, string sql)
        {
           // string sql = Sentences.GetSentence(SentenceType.GetAllTop, properties, tablename);
            string sqlwhere = SetWhereClause(parametros);
            sql += (sqlwhere != "") ? " WHERE " + sqlwhere : "";
            sql += (OrderBy != "") ? " ORDER BY " + OrderBy : "";
            sql += " LIMIT " + top.ToString();
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return ExecuteReader(cmd);
        }
        #endregion

        #region Get All by Page

        public static IDataReader GetAllByPage(TransactionManager transaction, string WhereClause, string OrderBy, int Desde, int hasta, PropertyInfo[] properties, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetAllByPage, properties, tablename);
            if (WhereClause != "")
                WhereClause = " WHERE " + WhereClause;

            sql = sql.Replace("%orderby%", OrderBy).Replace("%whereclause%", WhereClause).Replace("%desde%", Desde.ToString()).Replace("%hasta%", hasta.ToString());

            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            return ExecuteReader(cmd);
        }



        public static IDataReader GetAllByPage(TransactionManager transaction, WhereParams parametros, string OrderBy, int Desde, int hasta, PropertyInfo[] properties, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetAllByPage, properties, tablename);
            string sqlwhere = SetWhereClause(parametros);
            if (sqlwhere != "")
                sqlwhere = " WHERE " + sqlwhere;
            sql = sql.Replace("%orderby%", OrderBy).Replace("%whereclause%", sqlwhere).Replace("%desde%", Desde.ToString()).Replace("%hasta%", hasta.ToString());

            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return ExecuteReader(cmd);
        }
        public static IDataReader GetAllByPage(TransactionManager transaction, WhereParams parametros, string OrderBy, int Desde, int hasta, string sql)
        {

            string sqlwhere = SetWhereClause(parametros);
            if (sqlwhere != "")
                sqlwhere = " WHERE " + sqlwhere;
            sql = sql.Replace("%orderby%", OrderBy).Replace("%whereclause%", sqlwhere).Replace("%desde%", Desde.ToString()).Replace("%hasta%", hasta.ToString());

            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return ExecuteReader(cmd);
        }

        public static IDataReader GetAllByPageHaving(TransactionManager transaction, WhereParams parametros, string OrderBy, int Desde, int hasta, string sql)
        {

            string sqlwhere = SetWhereClause(parametros);
            if (sqlwhere != "")
                sqlwhere = " HAVING " + sqlwhere;
            sql = sql.Replace("%orderby%", OrderBy).Replace("%whereclause%", sqlwhere).Replace("%desde%", Desde.ToString()).Replace("%hasta%", hasta.ToString());

            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return ExecuteReader(cmd);
        }
        #endregion

        #region Get Record Count

        public static int GetRecordCount(string WhereClause, string OrderBy, string tablename)
        {
            int res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = GetRecordCount(transaction, WhereClause, OrderBy, tablename);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;
        }

        public static int GetRecordCount(TransactionManager transaction, string WhereClause, string OrderBy, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetRecordCount, null, tablename);
            sql += (WhereClause != "") ? " WHERE " + WhereClause : "";
            sql += (OrderBy != "") ? " ORDER BY " + OrderBy : "";
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            return int.Parse(ExecuteEscalar(cmd).ToString());
        }




        public static int GetRecordCount(WhereParams parametros, string OrderBy, string tablename)
        {
            int res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = GetRecordCount(transaction, parametros, OrderBy, tablename);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;
        }



        public static int GetRecordCount(TransactionManager transaction, WhereParams parametros, string OrderBy, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetRecordCount, null, tablename);
            string sqlwhere = SetWhereClause(parametros);
            sql += (sqlwhere != "") ? " WHERE " + sqlwhere : "";
            sql += (OrderBy != "") ? " ORDER BY " + OrderBy : "";
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return int.Parse(ExecuteEscalar(cmd).ToString());
        }

        #endregion

        #region Get Max

        public static int GetMax(string campo, string tablename)
        {
            int res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = GetMax(transaction, campo, tablename);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;

        }

        public static int GetMax(TransactionManager transaction, string campo, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetMax, null, tablename);
            sql = sql.Replace("%campo%", campo);
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            object res = ExecuteEscalar(cmd);
            if (res.ToString() != "")
                return (int)res;
            else
                return 0;
        }

        public static int GetMax(string campo, string WhereClause, string tablename)
        {
            int res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = GetMax(transaction, campo, WhereClause, tablename);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;

        }

        public static int GetMax(TransactionManager transaction, string campo, string WhereClause, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetMax, null, tablename);
            sql = sql.Replace("%campo%", campo);
            sql += (WhereClause != "") ? " WHERE " + WhereClause : "";
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            object res = ExecuteEscalar(cmd);
            if (res.ToString() != "")
                return (int)res;
            else
                return 0;
        }

        public static int GetMax(string campo, WhereParams parametros, string tablename)
        {
            int res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = GetMax(transaction, campo, parametros, tablename);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;

        }

        public static int GetMax(TransactionManager transaction, string campo, WhereParams parametros, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetMax, null, tablename);
            sql = sql.Replace("%campo%", campo);
            string sqlwhere = SetWhereClause(parametros);
            sql += (sqlwhere != "") ? " WHERE " + sqlwhere : "";


            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            object res = ExecuteEscalar(cmd);
            if (res.ToString() != "")
                return (int)res;
            else
                return 0;
        }



        #endregion

        #region Get Sum

        public static decimal GetSum(string campo, string WhereClause, string OrderBy, string tablename)
        {
            decimal res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = GetSum(transaction,campo, WhereClause, OrderBy, tablename);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;
        }

        public static decimal GetSum(TransactionManager transaction,string campo, string WhereClause, string OrderBy, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetSum, null, tablename);
            sql = sql.Replace("%campo%", campo);
            sql += (WhereClause != "") ? " WHERE " + WhereClause : "";
            sql += (OrderBy != "") ? " ORDER BY " + OrderBy : "";
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            return decimal.Parse(ExecuteEscalar(cmd).ToString());
        }




        public static decimal GetSum(string campo, WhereParams parametros, string OrderBy, string tablename)
        {
            decimal res = 0;
            TransactionManager transaction = new TransactionManager();
            try
            {
                transaction.BeginTransaction();
                res = GetSum(transaction,campo, parametros, OrderBy, tablename);
                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return res;
        }



        public static decimal GetSum(TransactionManager transaction,string campo, WhereParams parametros, string OrderBy, string tablename)
        {
            string sql = Sentences.GetSentence(SentenceType.GetSum, null, tablename);
            sql = sql.Replace("%campo%", campo);
            string sqlwhere = SetWhereClause(parametros);
            sql += (sqlwhere != "") ? " WHERE " + sqlwhere : "";
            sql += (OrderBy != "") ? " ORDER BY " + OrderBy : "";
            NpgsqlCommand cmd = CreateCommand(sql, CommandType.Text, transaction);
            cmd = AddParameters(cmd, parametros);
            return decimal.Parse(ExecuteEscalar(cmd).ToString());
        }

        #endregion

        #region Set Where Clause



        private static string SetWhereClause(WhereParams parametros)
        {
            string sqlwhere = parametros.where;

            for (int i = 0; i < parametros.valores.Length; i++)
            {
                sqlwhere = sqlwhere.Replace("{" + i.ToString() + "}", prefix + "par" + i.ToString());
            }
            if (sqlwhere == null)
                sqlwhere = "";
            return sqlwhere;
        }


        #endregion

        #region Create Command

        private static NpgsqlCommand CreateCommand(string cmdText, CommandType cmdType, TransactionManager transaction)
        {
            cmdText = cmdText.Replace(" like ", " ilike ");
            cmdText = cmdText.Replace("+", "||");  
            NpgsqlCommand cmd = new NpgsqlCommand(cmdText, transaction.connection, transaction.transaction);
            cmd.CommandType = cmdType;
            return cmd;
        }

        #endregion

        #region Add Parameters

        public static NpgsqlParameter AddParameter(string name, NpgsqlDbType tipo, object dato)
        {
            NpgsqlParameter  parametro = new NpgsqlParameter(name, tipo);
            if (dato != null)
                parametro.Value = dato;
            else
                parametro.Value = DBNull.Value;
            return parametro;
        }
        private static PropertyInfo GetProperty(PropertyInfo[] properties, string name)
        {
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == name)
                    return property;
            }
            return null;
        }

        private static NpgsqlCommand AddParameters(NpgsqlCommand cmd, PropertyInfo[] properties, SentenceType tipo, object context)
        {

            foreach (PropertyInfo property in properties)
            {
                bool originalkey = false;
                bool add = true;
                bool addkey = false;
                bool audit = false;
                object[] attributes = property.GetCustomAttributes(typeof(Data), true);
                if (attributes.Length > 0)
                {
                    Data data = (Data)attributes[0];
                    originalkey = data.originalkey;

                    if (tipo == SentenceType.Insert || tipo == SentenceType.InsertIdentity)
                    {
                        if (data.auto || data.originalkey)
                            add = false;
                    }
                    if (tipo == SentenceType.GetByPK)
                    {
                        if (!data.originalkey)
                            add = false;
                    }
                    if (tipo == SentenceType.GetById)
                    {
                        if (property.Name.IndexOf("_id") < 0)
                            add = false;
                    }
                    if (tipo == SentenceType.Delete)
                    {
                        if (!data.key)
                            add = false;
                    }
                    if (data.noprop)
                        add = false;

                    //if (data.auto && tipo == SentenceType.Insert)
                    //    add = false;
                    //if (!data.key && tipo == SentenceType.Delete)
                    //    add = false;
                }
                else
                {
                    if (tipo == SentenceType.Delete || tipo == SentenceType.GetByPK)
                        add = false;
                    if (tipo == SentenceType.GetById)
                    {
                        if (property.Name.IndexOf("_id") < 0)
                            add = false;

                    }
                }

                /*if (property.Name.ToUpper() == "CREA_USR" || property.Name.ToUpper() == "CREA_FECHA" || property.Name.ToUpper() == "MOD_USR" || property.Name.ToUpper() == "MOD_FECHA")
                {
                    if (tipo == SentenceType.Insert || tipo == SentenceType.InsertIdentity || tipo == SentenceType.Update)
                    {
                        add = false;
                        audit = true;
                    }
                }*/


                if (add)
                {
                    Type t = property.PropertyType;

                    // We need to check whether the property is NULLABLE
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                        t = property.PropertyType.GetGenericArguments()[0];
                    }
                    object valor = property.GetValue(context, null);
                    if (originalkey && valor == null)
                    {
                        PropertyInfo p = GetProperty(properties, property.Name.Replace("_key", ""));
                        valor = p.GetValue(context, null);
                    }

                    cmd.Parameters.Add(AddParameter(prefix + property.Name, GetDBType(t), valor));
                }
                //if (audit)
                //{
                //    Type t = property.PropertyType;
                //    // We need to check whether the property is NULLABLE
                //    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                //    {
                //        // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                //        t = property.PropertyType.GetGenericArguments()[0];
                //    }
                //    cmd.Parameters.Add(AddParameter(prefix + property.Name, GetDBType(t), property.GetValue(context, null)));
                //    /*Usuario usr = (Usuario)HttpContext.Current.Session["usuario"];
                //    if (property.Name.ToUpper() == "CREA_USR" || property.Name.ToUpper() == "MOD_USR")
                //        cmd.Parameters.Add(AddParameter(prefix + property.Name, GetDBType(t), usr.usr_id));
                //    if (property.Name.ToUpper() == "CREA_FECHA" || property.Name.ToUpper() == "MOD_FECHA")
                //        cmd.Parameters.Add(AddParameter(prefix + property.Name, GetDBType(t), DateTime.Now));*/

                //}

            }
            return cmd;
        }



        private static NpgsqlCommand AddParameters(NpgsqlCommand cmd, WhereParams parametros)
        {
            int contador = 0;
            foreach (object item in parametros.valores)
            {
                cmd.Parameters.Add(AddParameter(prefix + "par" + contador, GetDBType(item.GetType()), item));
                contador++;
            }
            return cmd;
        }




        #endregion

        #region Get Data Base Type
        public static NpgsqlDbType GetDBType(Type theType)
        {
            NpgsqlParameter param = new NpgsqlParameter(); 
            System.ComponentModel.TypeConverter tc;
            
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
            return param.NpgsqlDbType;
        }

        #endregion

        #region Execute

        private static int ExecuteNonQuery(NpgsqlCommand dCmd)
        {


            /*
            int res = 0;
            try
            {
                res = dCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Add(ex);
                throw ex;
            }
            finally
            {
                dCmd.Dispose();                
            }
            return res;*/
            int res = dCmd.ExecuteNonQuery();
            dCmd.Dispose();
            return res;
        }






        private static object ExecuteEscalar(NpgsqlCommand dCmd)
        {
            /*object res = null;
            try
            {
                res = dCmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Log.Add(ex);
            }
            finally
            {
                dCmd.Dispose();                
            }*/
            object res = dCmd.ExecuteScalar();
            dCmd.Dispose();
            return res;
        }


        private static IDataReader ExecuteReader(NpgsqlCommand dCmd)
        {

            object res = dCmd.ExecuteReader();
            dCmd.Dispose();
            return (IDataReader)res;
            /*try
            {
                return dCmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Log.Add(ex);
                return null;
            }
            finally
            {
                dCmd.Dispose();                
            }*/
        }

        #endregion





    }
}
