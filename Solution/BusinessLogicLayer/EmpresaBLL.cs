

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class EmpresaBLL
    {
        #region Constructor

        public EmpresaBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Empresa obj)
        {
            return EmpresaDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Empresa obj)
        {
            return EmpresaDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Empresa obj)
        {
            return EmpresaDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Empresa obj)
        {
            return EmpresaDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Empresa obj)
        {
            return EmpresaDAL.Update(obj);
        }
        public static int Update(BLL bll, Empresa obj)
        {
            return EmpresaDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Empresa obj)
        {
            return EmpresaDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Empresa obj)
        {
            return EmpresaDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Empresa GetByPK(Empresa obj)
        {
            return EmpresaDAL.GetByPK(obj);
        }
        public static List<Empresa> GetAll(string WhereClause, string OrderBy)
        {
            return EmpresaDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Empresa> GetAll(WhereParams parametros, string OrderBy)
        {
            return EmpresaDAL.GetAll(parametros, OrderBy);
        }

         public static List<Empresa> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return EmpresaDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Empresa> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return EmpresaDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Empresa> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return EmpresaDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return EmpresaDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return EmpresaDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return EmpresaDAL.GetMax(campo);
        }
        #endregion
    }
}
