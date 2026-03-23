using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using SqlDataBasePG;
using System.Data;

namespace DataAccessLayer
{
    public class SriReglaMensajeDAL
    {
        #region Insert

        public static int Insert(SriReglaMensaje obj)
        {
            return SqlDataBasePG.DB.InsertSQL(obj.GetProperties(), "sri_regla_mensaje", obj);
        }

        public static int Insert(DAL dal, SriReglaMensaje obj)
        {
            return SqlDataBasePG.DB.InsertSQL(dal.transaccionpg, obj.GetProperties(), "sri_regla_mensaje", obj);
        }

        public static int InsertIdentity(SriReglaMensaje obj)
        {
            return SqlDataBasePG.DB.InsertIdentitySQL(obj.GetProperties(), "sri_regla_mensaje", obj);
        }

        public static int InsertIdentity(DAL dal, SriReglaMensaje obj)
        {
            return SqlDataBasePG.DB.InsertIdentitySQL(dal.transaccionpg, obj.GetProperties(), "sri_regla_mensaje", obj);
        }

        #endregion

        #region Update

        public static int Update(SriReglaMensaje obj)
        {
            return SqlDataBasePG.DB.UpdateSQL(obj.GetProperties(), "sri_regla_mensaje", obj);
        }

        public static int Update(DAL dal, SriReglaMensaje obj)
        {
            return SqlDataBasePG.DB.UpdateSQL(dal.transaccionpg, obj.GetProperties(), "sri_regla_mensaje", obj);
        }

        #endregion

        #region Delete

        public static int Delete(SriReglaMensaje obj)
        {
            return SqlDataBasePG.DB.DeleteSQL(obj.GetProperties(), "sri_regla_mensaje", obj);
        }

        public static int Delete(DAL dal, SriReglaMensaje obj)
        {
            return SqlDataBasePG.DB.DeleteSQL(dal.transaccionpg, obj.GetProperties(), "sri_regla_mensaje", obj);
        }

        #endregion

        #region Get By Primary Key

        public static SriReglaMensaje GetByPK(SriReglaMensaje obj)
        {
            SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
            try
            {
                transaction.BeginTransaction();
                IDataReader reader = SqlDataBasePG.DB.GetByPKSql(transaction, obj.GetProperties(), "sri_regla_mensaje", obj);
                do
                {
                    if (!reader.Read()) break;
                    obj = new SriReglaMensaje(reader);
                } while (true);
                reader.Close();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return obj;
        }

        #endregion

        #region Get All

        public static List<SriReglaMensaje> GetAll(string WhereClause, string OrderBy)
        {
            List<SriReglaMensaje> list = new List<SriReglaMensaje>();
            SriReglaMensaje obj = new SriReglaMensaje();
            SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
            try
            {
                transaction.BeginTransaction();
                IDataReader reader = SqlDataBasePG.DB.GetAll(transaction, WhereClause, OrderBy, obj.GetProperties(), "sri_regla_mensaje");
                do
                {
                    if (!reader.Read()) break;
                    list.Add(new SriReglaMensaje(reader));
                } while (true);
                reader.Close();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return list;
        }

        public static List<SriReglaMensaje> GetAll(WhereParams parametros, string OrderBy)
        {
            List<SriReglaMensaje> list = new List<SriReglaMensaje>();
            SriReglaMensaje obj = new SriReglaMensaje();
            SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
            try
            {
                transaction.BeginTransaction();
                IDataReader reader = SqlDataBasePG.DB.GetAll(transaction, parametros, OrderBy, obj.GetProperties(), "sri_regla_mensaje");
                do
                {
                    if (!reader.Read()) break;
                    list.Add(new SriReglaMensaje(reader));
                } while (true);
                reader.Close();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            return list;
        }

        #endregion

        #region Get Record Count

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return SqlDataBasePG.DB.GetRecordCount(WhereClause, OrderBy, "sri_regla_mensaje");
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return SqlDataBasePG.DB.GetRecordCount(parametros, OrderBy, "sri_regla_mensaje");
        }

        #endregion
    }
}
