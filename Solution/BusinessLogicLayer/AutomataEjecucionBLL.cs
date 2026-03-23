using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;

namespace BusinessLogicLayer
{
    public class AutomataEjecucionBLL
    {
        #region Insert

        public static int Insert(AutomataEjecucion obj)
        {
            return AutomataEjecucionDAL.Insert(obj);
        }

        public static int Insert(BLL bll, AutomataEjecucion obj)
        {
            return AutomataEjecucionDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(AutomataEjecucion obj)
        {
            return AutomataEjecucionDAL.InsertIdentity(obj);
        }

        public static int InsertIdentity(BLL bll, AutomataEjecucion obj)
        {
            return AutomataEjecucionDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(AutomataEjecucion obj)
        {
            return AutomataEjecucionDAL.Update(obj);
        }

        public static int Update(BLL bll, AutomataEjecucion obj)
        {
            return AutomataEjecucionDAL.Update(bll.transaction, obj);
        }

        #endregion

        #region Delete

        public static int Delete(AutomataEjecucion obj)
        {
            return AutomataEjecucionDAL.Delete(obj);
        }

        public static int Delete(BLL bll, AutomataEjecucion obj)
        {
            return AutomataEjecucionDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static AutomataEjecucion GetByPK(AutomataEjecucion obj)
        {
            return AutomataEjecucionDAL.GetByPK(obj);
        }

        public static List<AutomataEjecucion> GetAll(string WhereClause, string OrderBy)
        {
            return AutomataEjecucionDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<AutomataEjecucion> GetAll(WhereParams parametros, string OrderBy)
        {
            return AutomataEjecucionDAL.GetAll(parametros, OrderBy);
        }

        public static List<AutomataEjecucion> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return AutomataEjecucionDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return AutomataEjecucionDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return AutomataEjecucionDAL.GetRecordCount(parametros, OrderBy);
        }

        #endregion
    }
}
