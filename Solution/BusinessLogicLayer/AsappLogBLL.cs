using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessLayer;

namespace BusinessLogicLayer
{
    public class AsappLogBLL
    {
        #region Constructor

        public AsappLogBLL()
        {
        }

        #endregion

        #region Insert

        public static int Insert(AsappLog obj)
        {
            return AsappLogDAL.Insert(obj);
        }

        public static int Insert(BLL bll, AsappLog obj)
        {
            return AsappLogDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(AsappLog obj)
        {
            return AsappLogDAL.InsertIdentity(obj);
        }

        public static int InsertIdentity(BLL bll, AsappLog obj)
        {
            return AsappLogDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static List<AsappLog> GetAll(string WhereClause, string OrderBy)
        {
            return AsappLogDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<AsappLog> GetAll(WhereParams parametros, string OrderBy)
        {
            return AsappLogDAL.GetAll(parametros, OrderBy);
        }

        public static List<AsappLog> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return AsappLogDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<AsappLog> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return AsappLogDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return AsappLogDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return AsappLogDAL.GetRecordCount(parametros, OrderBy);
        }

        #endregion
    }
}
