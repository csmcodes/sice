using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using SqlDataBasePG;
using System.Data;

namespace DataAccessLayer
{
    public class AutomataLogDAL
    {
        #region Insert

        public static int Insert(AutomataLog obj)
        {
            return SqlDataBasePG.DB.InsertSQL(obj.GetProperties(), "automata_log", obj);
        }

        public static int Insert(DAL dal, AutomataLog obj)
        {
            return SqlDataBasePG.DB.InsertSQL(dal.transaccionpg, obj.GetProperties(), "automata_log", obj);
        }

        public static int InsertIdentity(AutomataLog obj)
        {
            return SqlDataBasePG.DB.InsertIdentitySQL(obj.GetProperties(), "automata_log", obj);
        }

        public static int InsertIdentity(DAL dal, AutomataLog obj)
        {
            return SqlDataBasePG.DB.InsertIdentitySQL(dal.transaccionpg, obj.GetProperties(), "automata_log", obj);
        }

        #endregion

        #region Update

        public static int Update(AutomataLog obj)
        {
            return SqlDataBasePG.DB.UpdateSQL(obj.GetProperties(), "automata_log", obj);
        }

        public static int Update(DAL dal, AutomataLog obj)
        {
            return SqlDataBasePG.DB.UpdateSQL(dal.transaccionpg, obj.GetProperties(), "automata_log", obj);
        }

        #endregion

        #region Delete

        public static int Delete(AutomataLog obj)
        {
            return SqlDataBasePG.DB.DeleteSQL(obj.GetProperties(), "automata_log", obj);
        }

        public static int Delete(DAL dal, AutomataLog obj)
        {
            return SqlDataBasePG.DB.DeleteSQL(dal.transaccionpg, obj.GetProperties(), "automata_log", obj);
        }

        #endregion

        #region Get By Primary Key

        public static AutomataLog GetByPK(AutomataLog obj)
        {
            SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
            try
            {
                transaction.BeginTransaction();
                IDataReader reader = SqlDataBasePG.DB.GetByPKSql(transaction, obj.GetProperties(), "automata_log", obj);
                do
                {
                    if (!reader.Read()) break;
                    obj = new AutomataLog(reader);
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

        public static List<AutomataLog> GetAll(string WhereClause, string OrderBy)
        {
            List<AutomataLog> list = new List<AutomataLog>();
            AutomataLog obj = new AutomataLog();
            SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
            try
            {
                transaction.BeginTransaction();
                IDataReader reader = SqlDataBasePG.DB.GetAll(transaction, WhereClause, OrderBy, obj.GetProperties(), "automata_log");
                do
                {
                    if (!reader.Read()) break;
                    list.Add(new AutomataLog(reader));
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

        public static List<AutomataLog> GetAll(WhereParams parametros, string OrderBy)
        {
            List<AutomataLog> list = new List<AutomataLog>();
            AutomataLog obj = new AutomataLog();
            SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
            try
            {
                transaction.BeginTransaction();
                IDataReader reader = SqlDataBasePG.DB.GetAll(transaction, parametros, OrderBy, obj.GetProperties(), "automata_log");
                do
                {
                    if (!reader.Read()) break;
                    list.Add(new AutomataLog(reader));
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

        public static List<AutomataLog> GetAllbyPage(WhereParams parametros, string OrderBy, int Desde, int Hasta)
        {
            List<AutomataLog> list = new List<AutomataLog>();
            AutomataLog obj = new AutomataLog();
            SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
            try
            {
                transaction.BeginTransaction();
                IDataReader reader = SqlDataBasePG.DB.GetAllByPage(transaction, parametros, OrderBy, Desde, Hasta, obj.GetProperties(), "automata_log");
                do
                {
                    if (!reader.Read()) break;
                    list.Add(new AutomataLog(reader));
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
            return SqlDataBasePG.DB.GetRecordCount(WhereClause, OrderBy, "automata_log");
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return SqlDataBasePG.DB.GetRecordCount(parametros, OrderBy, "automata_log");
        }

        #endregion

        #region Delete All

        public static int DeleteAll(WhereParams parametros)
        {
            return SqlDataBasePG.DB.DeleteAll(parametros, "automata_log");
        }

        #endregion
    }
}
