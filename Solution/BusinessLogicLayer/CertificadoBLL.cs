

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;
namespace BusinessLogicLayer
{
    public class CertificadoBLL
    {
        #region Constructor

        public CertificadoBLL()
        {

        }

        #endregion

        #region Insert

        public static int Insert(Certificado obj)
        {
            return CertificadoDAL.Insert(obj);
        }
        public static int Insert(BLL bll, Certificado obj)
        {
            return CertificadoDAL.Insert(bll.transaction, obj);
        }

        public static int InsertIdentity(Certificado obj)
        {
            return CertificadoDAL.InsertIdentity(obj);
        }
        public static int InsertIdentity(BLL bll, Certificado obj)
        {
            return CertificadoDAL.InsertIdentity(bll.transaction, obj);
        }

        #endregion

        #region Update

        public static int Update(Certificado obj)
        {
            return CertificadoDAL.Update(obj);
        }
        public static int Update(BLL bll, Certificado obj)
        {
            return CertificadoDAL.Update(bll.transaction, obj);
        }
        #endregion

        #region Delete

        public static int Delete(Certificado obj)
        {
            return CertificadoDAL.Delete(obj);
        }
       

        public static int Delete(BLL bll, Certificado obj)
        {
            return CertificadoDAL.Delete(bll.transaction, obj);
        }

        #endregion

        #region Select

        public static Certificado GetByPK(Certificado obj)
        {
            return CertificadoDAL.GetByPK(obj);
        }
        public static List<Certificado> GetAll(string WhereClause, string OrderBy)
        {
            return CertificadoDAL.GetAll(WhereClause, OrderBy);
        }

        public static List<Certificado> GetAll(WhereParams parametros, string OrderBy)
        {
            return CertificadoDAL.GetAll(parametros, OrderBy);
        }

         public static List<Certificado> GetAllTop(WhereParams parametros, string OrderBy, int Top)
        {
            return CertificadoDAL.GetAllTop(parametros, OrderBy, Top);
        }

        public static List<Certificado> GetAllByPage(string WhereClause, string OrderBy, int desde, int hasta)
        {
            return CertificadoDAL.GetAllbyPage(WhereClause, OrderBy, desde, hasta);
        }

        public static List<Certificado> GetAllByPage(WhereParams parametros, string OrderBy, int desde, int hasta)
        {
            return CertificadoDAL.GetAllbyPage(parametros, OrderBy, desde, hasta);
        }

        public static int GetRecordCount(string WhereClause, string OrderBy)
        {
            return CertificadoDAL.GetRecordCount(WhereClause, OrderBy);
        }

        public static int GetRecordCount(WhereParams parametros, string OrderBy)
        {
            return CertificadoDAL.GetRecordCount(parametros, OrderBy);
        }

        public static int GetMax(string campo)
        {
            return CertificadoDAL.GetMax(campo);
        }
        #endregion
    }
}
