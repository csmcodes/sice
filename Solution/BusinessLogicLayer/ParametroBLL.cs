

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class ParametroBLL
    {
        #region Constructor

        public ParametroBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Parametro obj)
        {
            return ParametroDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Parametro obj)
        {
            return ParametroDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Parametro obj)
        {
            return ParametroDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Parametro obj)
        {
            return ParametroDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Parametro obj)
        {
            return ParametroDAL.Update(obj);
        }
        public static int Update(BLL bll, Parametro obj)
        {
            return ParametroDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Parametro obj)
        {
            return ParametroDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Parametro obj)
        {
            return ParametroDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Parametro GetByPK(Parametro obj)
        {
            return ParametroDAL.GetByPK(obj);
        }
        public static List<Parametro> GetAll(string WhereClause, string OrderBy)
        {
            return ParametroDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Parametro> GetAll(WhereParams parametros, string OrderBy)
        {
            return ParametroDAL.GetAll(parametros, OrderBy);
        }

         public static List<Parametro> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return ParametroDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Parametro> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return ParametroDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Parametro> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return ParametroDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return ParametroDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return ParametroDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return ParametroDAL.GetMax(campo);
        }
        #endregion
    }
}
