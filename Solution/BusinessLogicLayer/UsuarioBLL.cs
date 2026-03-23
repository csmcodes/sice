

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class UsuarioBLL
    {
        #region Constructor

        public UsuarioBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Usuario obj)
        {
            return UsuarioDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Usuario obj)
        {
            return UsuarioDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Usuario obj)
        {
            return UsuarioDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Usuario obj)
        {
            return UsuarioDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Usuario obj)
        {
            return UsuarioDAL.Update(obj);
        }
        public static int Update(BLL bll, Usuario obj)
        {
            return UsuarioDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Usuario obj)
        {
            return UsuarioDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Usuario obj)
        {
            return UsuarioDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Usuario GetByPK(Usuario obj)
        {
            return UsuarioDAL.GetByPK(obj);
        }
        public static List<Usuario> GetAll(string WhereClause, string OrderBy)
        {
            return UsuarioDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Usuario> GetAll(WhereParams parametros, string OrderBy)
        {
            return UsuarioDAL.GetAll(parametros, OrderBy);
        }

         public static List<Usuario> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return UsuarioDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Usuario> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return UsuarioDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Usuario> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return UsuarioDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return UsuarioDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return UsuarioDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return UsuarioDAL.GetMax(campo);
        }
        #endregion
    }
}
