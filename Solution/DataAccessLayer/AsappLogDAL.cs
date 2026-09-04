using System;
using System.Collections.Generic;
using System.Data;
using BusinessObjects;
using SqlDataBase;
using SqlDataBasePG;

namespace DataAccessLayer
{
    public class AsappLogDAL
    {
        #region Insert

        public static int Insert(AsappLog obj)
        {
            if (DAL.GetProvider() == Provider.SqlServer)
                return SqlDataBase.DB.InsertSQL(obj.GetProperties(), "asapp_log", obj);
            else if (DAL.GetProvider() == Provider.PostgreSQL)
                return SqlDataBasePG.DB.InsertSQL(obj.GetProperties(), "asapp_log", obj);
            else
                return 0;
        }

        public static int Insert(DAL dal, AsappLog obj)
        {
            if (DAL.GetProvider() == Provider.SqlServer)
                return SqlDataBase.DB.InsertSQL(dal.transaccion, obj.GetProperties(), "asapp_log", obj);
            else if (DAL.GetProvider() == Provider.PostgreSQL)
                return SqlDataBasePG.DB.InsertSQL(dal.transaccionpg, obj.GetProperties(), "asapp_log", obj);
            else
                return 0;
        }

        public static int InsertIdentity(AsappLog obj)
        {
            if (DAL.GetProvider() == Provider.SqlServer)
                return SqlDataBase.DB.InsertIdentitySQL(obj.GetProperties(), "asapp_log", obj);
            else if (DAL.GetProvider() == Provider.PostgreSQL)
                return SqlDataBasePG.DB.InsertIdentitySQL(obj.GetProperties(), "asapp_log", obj);
            else
                return 0;
        }

        public static int InsertIdentity(DAL dal, AsappLog obj)
        {
            if (DAL.GetProvider() == Provider.SqlServer)
                return SqlDataBase.DB.InsertIdentitySQL(dal.transaccion, obj.GetProperties(), "asapp_log", obj);
            else if (DAL.GetProvider() == Provider.PostgreSQL)
                return SqlDataBasePG.DB.InsertIdentitySQL(dal.transaccionpg, obj.GetProperties(), "asapp_log", obj);
            else
                return 0;
        }

        #endregion

        #region Get All

        public static List<AsappLog> GetAll(string WhereClause, string OrderBy)
        {
            List<AsappLog> list = new List<AsappLog>();
            AsappLog obj = new AsappLog();
            if (DAL.GetProvider() == Provider.SqlServer)
            {
                SqlDataBase.TransactionManager transaction = new SqlDataBase.TransactionManager();
                try
                {
                    transaction.BeginTransaction();
                    IDataReader reader = SqlDataBase.DB.GetAll(transaction, WhereClause, OrderBy, obj.GetProperties(), "asapp_log");
                    do
                    {
                        if (!reader.Read()) break;
                        list.Add(new AsappLog(reader));
                    } while (true);
                    reader.Close();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            else if (DAL.GetProvider() == Provider.PostgreSQL)
            {
                SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
                try
                {
                    transaction.BeginTransaction();
                    IDataReader reader = SqlDataBasePG.DB.GetAll(transaction, WhereClause, OrderBy, obj.GetProperties(), "asapp_log");
                    do
                    {
                        if (!reader.Read()) break;
                        list.Add(new AsappLog(reader));
                    } while (true);
                    reader.Close();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return list;
        }

        public static List<AsappLog> GetAll(WhereParams parametros, string OrderBy)
        {
            List<AsappLog> list = new List<AsappLog>();
            AsappLog obj = new AsappLog();
            if (DAL.GetProvider() == Provider.SqlServer)
            {
                SqlDataBase.TransactionManager transaction = new SqlDataBase.TransactionManager();
                try
                {
                    transaction.BeginTransaction();
                    IDataReader reader = SqlDataBase.DB.GetAll(transaction, parametros, OrderBy, obj.GetProperties(), "asapp_log");
                    do
                    {
                        if (!reader.Read()) break;
                        list.Add(new AsappLog(reader));
                    } while (true);
                    reader.Close();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            else if (DAL.GetProvider() == Provider.PostgreSQL)
            {
                SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
                try
                {
                    transaction.BeginTransaction();
                    IDataReader reader = SqlDataBasePG.DB.GetAll(transaction, parametros, OrderBy, obj.GetProperties(), "asapp_log");
                    do
                    {
                        if (!reader.Read()) break;
                        list.Add(new AsappLog(reader));
                    } while (true);
                    reader.Close();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return list;
        }

        #endregion

        #region Get All By Page

        public static List<AsappLog> GetAllbyPage(string WhereClause, string OrderBy, int Desde, int Hasta)
        {
            List<AsappLog> list = new List<AsappLog>();
            AsappLog obj = new AsappLog();
            if (DAL.GetProvider() == Provider.SqlServer)
            {
                SqlDataBase.TransactionManager transaction = new SqlDataBase.TransactionManager();
                try
                {
                    transaction.BeginTransaction();
                    IDataReader reader = SqlDataBase.DB.GetAllByPage(transaction, WhereClause, OrderBy, Desde, Hasta, obj.GetProperties(), "asapp_log");
                    do
                    {
                        if (!reader.Read()) break;
                        list.Add(new AsappLog(reader));
                    } while (true);
                    reader.Close();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            else if (DAL.GetProvider() == Provider.PostgreSQL)
            {
                SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
                try
                {
                    transaction.BeginTransaction();
                    IDataReader reader = SqlDataBasePG.DB.GetAllByPage(transaction, WhereClause, OrderBy, Desde, Hasta, obj.GetProperties(), "asapp_log");
                    do
                    {
                        if (!reader.Read()) break;
                        list.Add(new AsappLog(reader));
                    } while (true);
                    reader.Close();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return list;
        }

        public static List<AsappLog> GetAllbyPage(WhereParams parametros, string OrderBy, int Desde, int Hasta)
        {
            List<AsappLog> list = new List<AsappLog>();
            AsappLog obj = new AsappLog();
            if (DAL.GetProvider() == Provider.SqlServer)
            {
                SqlDataBase.TransactionManager transaction = new SqlDataBase.TransactionManager();
                try
                {
                    transaction.BeginTransaction();
                    IDataReader reader = SqlDataBase.DB.GetAllByPage(transaction, parametros, OrderBy, Desde, Hasta, obj.GetProperties(), "asapp_log");
                    do
                    {
                        if (!reader.Read()) break;
                        list.Add(new AsappLog(reader));
                    } while (true);
                    reader.Close();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            else if (DAL.GetProvider() == Provider.PostgreSQL)
            {
                SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
                try
                {
                    transaction.BeginTransaction();
                    IDataReader reader = SqlDataBasePG.DB.GetAllByPage(transaction, parametros, OrderBy, Desde, Hasta, obj.GetProperties(), "asapp_log");
                    do
                    {
                        if (!reader.Read()) break;
                        list.Add(new AsappLog(reader));
                    } while (true);
                    reader.Close();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return list;
        }

        #endregion

        #region Get Record Count

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            if (DAL.GetProvider() == Provider.SqlServer)
                return SqlDataBase.DB.GetRecordCount(WhereClause, OrderBy, "asapp_log");
            else if (DAL.GetProvider() == Provider.PostgreSQL)
                return SqlDataBasePG.DB.GetRecordCount(WhereClause, OrderBy, "asapp_log");
            else
                return 0;
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            if (DAL.GetProvider() == Provider.SqlServer)
                return SqlDataBase.DB.GetRecordCount(parametros, OrderBy, "asapp_log");
            else if (DAL.GetProvider() == Provider.PostgreSQL)
                return SqlDataBasePG.DB.GetRecordCount(parametros, OrderBy, "asapp_log");
            else
                return 0;
        }

        #endregion
    }
}
