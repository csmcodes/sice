

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class FormatoBLL
    {
        #region Constructor

        public FormatoBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Formato obj)
        {
            return FormatoDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Formato obj)
        {
            return FormatoDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Formato obj)
        {
            return FormatoDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Formato obj)
        {
            return FormatoDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Formato obj)
        {
            return FormatoDAL.Update(obj);
        }
        public static int Update(BLL bll, Formato obj)
        {
            return FormatoDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Formato obj)
        {
            return FormatoDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Formato obj)
        {
            return FormatoDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Formato GetByPK(Formato obj)
        {
            return FormatoDAL.GetByPK(obj);
        }
        public static List<Formato> GetAll(string WhereClause, string OrderBy)
        {
            return FormatoDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Formato> GetAll(WhereParams parametros, string OrderBy)
        {
            return FormatoDAL.GetAll(parametros, OrderBy);
        }

         public static List<Formato> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return FormatoDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Formato> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return FormatoDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Formato> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return FormatoDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return FormatoDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return FormatoDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return FormatoDAL.GetMax(campo);
        }
        #endregion
    }
}
