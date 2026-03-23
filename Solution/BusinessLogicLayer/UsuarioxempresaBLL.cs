

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class UsuarioxempresaBLL
    {
        #region Constructor

        public UsuarioxempresaBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Usuarioxempresa obj)
        {
            return UsuarioxempresaDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Usuarioxempresa obj)
        {
            return UsuarioxempresaDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Usuarioxempresa obj)
        {
            return UsuarioxempresaDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Usuarioxempresa obj)
        {
            return UsuarioxempresaDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Usuarioxempresa obj)
        {
            return UsuarioxempresaDAL.Update(obj);
        }
        public static int Update(BLL bll, Usuarioxempresa obj)
        {
            return UsuarioxempresaDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Usuarioxempresa obj)
        {
            return UsuarioxempresaDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Usuarioxempresa obj)
        {
            return UsuarioxempresaDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Usuarioxempresa GetByPK(Usuarioxempresa obj)
        {
            return UsuarioxempresaDAL.GetByPK(obj);
        }
        public static List<Usuarioxempresa> GetAll(string WhereClause, string OrderBy)
        {
            return UsuarioxempresaDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Usuarioxempresa> GetAll(WhereParams parametros, string OrderBy)
        {
            return UsuarioxempresaDAL.GetAll(parametros, OrderBy);
        }

         public static List<Usuarioxempresa> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return UsuarioxempresaDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Usuarioxempresa> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return UsuarioxempresaDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Usuarioxempresa> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return UsuarioxempresaDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return UsuarioxempresaDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return UsuarioxempresaDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return UsuarioxempresaDAL.GetMax(campo);
        }
        #endregion
    }
}
