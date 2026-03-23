

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class ArchivoBLL
    {
        #region Constructor

        public ArchivoBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Archivo obj)
        {
            return ArchivoDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Archivo obj)
        {
            return ArchivoDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Archivo obj)
        {
            return ArchivoDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Archivo obj)
        {
            return ArchivoDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Archivo obj)
        {
            return ArchivoDAL.Update(obj);
        }
        public static int Update(BLL bll, Archivo obj)
        {
            return ArchivoDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Archivo obj)
        {
            return ArchivoDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Archivo obj)
        {
            return ArchivoDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Archivo GetByPK(Archivo obj)
        {
            return ArchivoDAL.GetByPK(obj);
        }
        public static List<Archivo> GetAll(string WhereClause, string OrderBy)
        {
            return ArchivoDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Archivo> GetAll(WhereParams parametros, string OrderBy)
        {
            return ArchivoDAL.GetAll(parametros, OrderBy);
        }

         public static List<Archivo> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return ArchivoDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Archivo> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return ArchivoDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Archivo> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return ArchivoDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return ArchivoDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return ArchivoDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return ArchivoDAL.GetMax(campo);
        }
        #endregion
    }
}
