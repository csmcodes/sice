

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class CorreoadminBLL
    {
        #region Constructor

        public CorreoadminBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Correoadmin obj)
        {
            return CorreoadminDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Correoadmin obj)
        {
            return CorreoadminDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Correoadmin obj)
        {
            return CorreoadminDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Correoadmin obj)
        {
            return CorreoadminDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Correoadmin obj)
        {
            return CorreoadminDAL.Update(obj);
        }
        public static int Update(BLL bll, Correoadmin obj)
        {
            return CorreoadminDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Correoadmin obj)
        {
            return CorreoadminDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Correoadmin obj)
        {
            return CorreoadminDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Correoadmin GetByPK(Correoadmin obj)
        {
            return CorreoadminDAL.GetByPK(obj);
        }
        public static List<Correoadmin> GetAll(string WhereClause, string OrderBy)
        {
            return CorreoadminDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Correoadmin> GetAll(WhereParams parametros, string OrderBy)
        {
            return CorreoadminDAL.GetAll(parametros, OrderBy);
        }

         public static List<Correoadmin> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return CorreoadminDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Correoadmin> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return CorreoadminDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Correoadmin> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return CorreoadminDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return CorreoadminDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return CorreoadminDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return CorreoadminDAL.GetMax(campo);
        }
        #endregion
    }
}
