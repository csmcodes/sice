

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class ComprobanteBLL
    {
        #region Constructor

        public ComprobanteBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Comprobante obj)
        {
            return ComprobanteDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Comprobante obj)
        {
            return ComprobanteDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Comprobante obj)
        {
            return ComprobanteDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Comprobante obj)
        {
            return ComprobanteDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Comprobante obj)
        {
            return ComprobanteDAL.Update(obj);
        }
        public static int Update(BLL bll, Comprobante obj)
        {
            return ComprobanteDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Comprobante obj)
        {
            return ComprobanteDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Comprobante obj)
        {
            return ComprobanteDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Comprobante GetByPK(Comprobante obj)
        {
            return ComprobanteDAL.GetByPK(obj);
        }
        public static List<Comprobante> GetAll(string WhereClause, string OrderBy)
        {
            return ComprobanteDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Comprobante> GetAll(WhereParams parametros, string OrderBy)
        {
            return ComprobanteDAL.GetAll(parametros, OrderBy);
        }

         public static List<Comprobante> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return ComprobanteDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Comprobante> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return ComprobanteDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Comprobante> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return ComprobanteDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return ComprobanteDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return ComprobanteDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return ComprobanteDAL.GetMax(campo);
        }
        #endregion
    }
}
