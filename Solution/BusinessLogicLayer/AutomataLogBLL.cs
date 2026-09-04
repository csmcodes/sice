using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;

namespace BusinessLogicLayer
{
    public class AutomataLogBLL
    {
        #region Insert

        public static int Insert(AutomataLog obj)
        {
            return AutomataLogDAL.Insert(obj);
        }

        public static int Insert(BLL bll, AutomataLog obj)
        {
            return AutomataLogDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(AutomataLog obj)
        {
            return AutomataLogDAL.InsertIdentity(obj);
        }

        public static int InsertIdentity(BLL bll, AutomataLog obj)
        {
            return AutomataLogDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(AutomataLog obj)
        {
            return AutomataLogDAL.Update(obj);
        }

        public static int Update(BLL bll, AutomataLog obj)
        {
            return AutomataLogDAL.Update(bll.transaction, obj);
        }

        #endregion

        #region Delete

        public static int Delete(AutomataLog obj)
        {
            return AutomataLogDAL.Delete(obj);
        }

        public static int Delete(BLL bll, AutomataLog obj)
        {
            return AutomataLogDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static AutomataLog GetByPK(AutomataLog obj)
        {
            return AutomataLogDAL.GetByPK(obj);
        }

        public static List<AutomataLog> GetAll(string WhereClause, string OrderBy)
        {
            return AutomataLogDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<AutomataLog> GetAll(WhereParams parametros, string OrderBy)
        {
            return AutomataLogDAL.GetAll(parametros, OrderBy);
        }

        public static List<AutomataLog> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return AutomataLogDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return AutomataLogDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return AutomataLogDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int DeleteAll(WhereParams parametros)
        {
            return AutomataLogDAL.DeleteAll(parametros);
        }

        #endregion
    }
}
