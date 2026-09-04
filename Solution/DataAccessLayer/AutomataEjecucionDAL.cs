using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using SqlDataBasePG;
using System.Data;

namespace DataAccessLayer
{
    public class AutomataEjecucionDAL
    {
        #region Insert

        public static int Insert(AutomataEjecucion obj)
        {
            return SqlDataBasePG.DB.InsertSQL(obj.GetProperties(), "automata_ejecucion", obj);
        }

        public static int Insert(DAL dal, AutomataEjecucion obj)
        {
            return SqlDataBasePG.DB.InsertSQL(dal.transaccionpg, obj.GetProperties(), "automata_ejecucion", obj);
        }

        public static int InsertIdentity(AutomataEjecucion obj)
        {
            return SqlDataBasePG.DB.InsertIdentitySQL(obj.GetProperties(), "automata_ejecucion", obj);
        }

        public static int InsertIdentity(DAL dal, AutomataEjecucion obj)
        {
            return SqlDataBasePG.DB.InsertIdentitySQL(dal.transaccionpg, obj.GetProperties(), "automata_ejecucion", obj);
        }

        #endregion

        #region Update

        public static int Update(AutomataEjecucion obj)
        {
            return SqlDataBasePG.DB.UpdateSQL(obj.GetProperties(), "automata_ejecucion", obj);
        }

        public static int Update(DAL dal, AutomataEjecucion obj)
        {
            return SqlDataBasePG.DB.UpdateSQL(dal.transaccionpg, obj.GetProperties(), "automata_ejecucion", obj);
        }

        #endregion

        #region Delete

        public static int Delete(AutomataEjecucion obj)
        {
            return SqlDataBasePG.DB.DeleteSQL(obj.GetProperties(), "automata_ejecucion", obj);
        }

        public static int Delete(DAL dal, AutomataEjecucion obj)
        {
            return SqlDataBasePG.DB.DeleteSQL(dal.transaccionpg, obj.GetProperties(), "automata_ejecucion", obj);
        }

        #endregion

        #region Get By Primary Key

        public static AutomataEjecucion GetByPK(AutomataEjecucion obj)
        {
            SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
            try
            {
                transaction.BeginTransaction();
                IDataReader reader = SqlDataBasePG.DB.GetByPKSql(transaction, obj.GetProperties(), "automata_ejecucion", obj);
                do
                {
                    if (!reader.Read()) break;
                    obj = new AutomataEjecucion(reader);
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

        public static List<AutomataEjecucion> GetAll(string WhereClause, string OrderBy)
        {
            List<AutomataEjecucion> list = new List<AutomataEjecucion>();
            AutomataEjecucion obj = new AutomataEjecucion();
            SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
            try
            {
                transaction.BeginTransaction();
                IDataReader reader = SqlDataBasePG.DB.GetAll(transaction, WhereClause, OrderBy, obj.GetProperties(), "automata_ejecucion");
                do
                {
                    if (!reader.Read()) break;
                    list.Add(new AutomataEjecucion(reader));
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

        public static List<AutomataEjecucion> GetAll(WhereParams parametros, string OrderBy)
        {
            List<AutomataEjecucion> list = new List<AutomataEjecucion>();
            AutomataEjecucion obj = new AutomataEjecucion();
            SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
            try
            {
                transaction.BeginTransaction();
                IDataReader reader = SqlDataBasePG.DB.GetAll(transaction, parametros, OrderBy, obj.GetProperties(), "automata_ejecucion");
                do
                {
                    if (!reader.Read()) break;
                    list.Add(new AutomataEjecucion(reader));
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

        public static List<AutomataEjecucion> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            List<AutomataEjecucion> list = new List<AutomataEjecucion>();
            AutomataEjecucion obj = new AutomataEjecucion();
            SqlDataBasePG.TransactionManager transaction = new SqlDataBasePG.TransactionManager();
            try
            {
                transaction.BeginTransaction();
                IDataReader reader = SqlDataBasePG.DB.GetAllTop(transaction, parametros, OrderBy, Top, obj.GetProperties(), "automata_ejecucion");
                do
                {
                    if (!reader.Read()) break;
                    list.Add(new AutomataEjecucion(reader));
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
            return SqlDataBasePG.DB.GetRecordCount(WhereClause, OrderBy, "automata_ejecucion");
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return SqlDataBasePG.DB.GetRecordCount(parametros, OrderBy, "automata_ejecucion");
        }

        #endregion
    }
}
