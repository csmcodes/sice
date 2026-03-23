

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class SysdiagramsBLL
    {
        #region Constructor

        public SysdiagramsBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Sysdiagrams obj)
        {
            return SysdiagramsDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Sysdiagrams obj)
        {
            return SysdiagramsDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Sysdiagrams obj)
        {
            return SysdiagramsDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Sysdiagrams obj)
        {
            return SysdiagramsDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Sysdiagrams obj)
        {
            return SysdiagramsDAL.Update(obj);
        }
        public static int Update(BLL bll, Sysdiagrams obj)
        {
            return SysdiagramsDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Sysdiagrams obj)
        {
            return SysdiagramsDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Sysdiagrams obj)
        {
            return SysdiagramsDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Sysdiagrams GetByPK(Sysdiagrams obj)
        {
            return SysdiagramsDAL.GetByPK(obj);
        }
        public static List<Sysdiagrams> GetAll(string WhereClause, string OrderBy)
        {
            return SysdiagramsDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Sysdiagrams> GetAll(WhereParams parametros, string OrderBy)
        {
            return SysdiagramsDAL.GetAll(parametros, OrderBy);
        }

         public static List<Sysdiagrams> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return SysdiagramsDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Sysdiagrams> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return SysdiagramsDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Sysdiagrams> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return SysdiagramsDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return SysdiagramsDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return SysdiagramsDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return SysdiagramsDAL.GetMax(campo);
        }
        #endregion
    }
}
