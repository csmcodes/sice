

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class PerfilBLL
    {
        #region Constructor

        public PerfilBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Perfil obj)
        {
            return PerfilDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Perfil obj)
        {
            return PerfilDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Perfil obj)
        {
            return PerfilDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Perfil obj)
        {
            return PerfilDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Perfil obj)
        {
            return PerfilDAL.Update(obj);
        }
        public static int Update(BLL bll, Perfil obj)
        {
            return PerfilDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Perfil obj)
        {
            return PerfilDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Perfil obj)
        {
            return PerfilDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Perfil GetByPK(Perfil obj)
        {
            return PerfilDAL.GetByPK(obj);
        }
        public static List<Perfil> GetAll(string WhereClause, string OrderBy)
        {
            return PerfilDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Perfil> GetAll(WhereParams parametros, string OrderBy)
        {
            return PerfilDAL.GetAll(parametros, OrderBy);
        }

         public static List<Perfil> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return PerfilDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Perfil> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return PerfilDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Perfil> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return PerfilDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return PerfilDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return PerfilDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return PerfilDAL.GetMax(campo);
        }
        #endregion
    }
}
