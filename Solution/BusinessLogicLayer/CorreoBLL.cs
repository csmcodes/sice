

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class CorreoBLL
    {
        #region Constructor

        public CorreoBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Correo obj)
        {
            return CorreoDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Correo obj)
        {
            return CorreoDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Correo obj)
        {
            return CorreoDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Correo obj)
        {
            return CorreoDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Correo obj)
        {
            return CorreoDAL.Update(obj);
        }
        public static int Update(BLL bll, Correo obj)
        {
            return CorreoDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Correo obj)
        {
            return CorreoDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Correo obj)
        {
            return CorreoDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Correo GetByPK(Correo obj)
        {
            return CorreoDAL.GetByPK(obj);
        }
        public static List<Correo> GetAll(string WhereClause, string OrderBy)
        {
            return CorreoDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Correo> GetAll(WhereParams parametros, string OrderBy)
        {
            return CorreoDAL.GetAll(parametros, OrderBy);
        }

         public static List<Correo> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return CorreoDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Correo> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return CorreoDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Correo> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return CorreoDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return CorreoDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return CorreoDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return CorreoDAL.GetMax(campo);
        }
        #endregion
    }
}
