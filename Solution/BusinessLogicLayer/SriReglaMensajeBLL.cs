using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;

namespace BusinessLogicLayer
{
    public class SriReglaMensajeBLL
    {
        #region Insert

        public static int Insert(SriReglaMensaje obj)
        {
            return SriReglaMensajeDAL.Insert(obj);
        }

        public static int Insert(BLL bll, SriReglaMensaje obj)
        {
            return SriReglaMensajeDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(SriReglaMensaje obj)
        {
            return SriReglaMensajeDAL.InsertIdentity(obj);
        }

        public static int InsertIdentity(BLL bll, SriReglaMensaje obj)
        {
            return SriReglaMensajeDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(SriReglaMensaje obj)
        {
            return SriReglaMensajeDAL.Update(obj);
        }

        public static int Update(BLL bll, SriReglaMensaje obj)
        {
            return SriReglaMensajeDAL.Update(bll.transaction, obj);
        }

        #endregion

        #region Delete

        public static int Delete(SriReglaMensaje obj)
        {
            return SriReglaMensajeDAL.Delete(obj);
        }

        public static int Delete(BLL bll, SriReglaMensaje obj)
        {
            return SriReglaMensajeDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static SriReglaMensaje GetByPK(SriReglaMensaje obj)
        {
            return SriReglaMensajeDAL.GetByPK(obj);
        }

        public static List<SriReglaMensaje> GetAll(string WhereClause, string OrderBy)
        {
            return SriReglaMensajeDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<SriReglaMensaje> GetAll(WhereParams parametros, string OrderBy)
        {
            return SriReglaMensajeDAL.GetAll(parametros, OrderBy);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return SriReglaMensajeDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return SriReglaMensajeDAL.GetRecordCount(parametros, OrderBy);
        }

        #endregion
    }
}
