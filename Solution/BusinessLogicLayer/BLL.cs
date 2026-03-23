using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObjects;
using DataAccessLayer;
using System.Data;

namespace BusinessLogicLayer
{
    public class BLL
    {
        public DAL transaction { get; set; }

        public BLL()
        {
            transaction = new DAL(); 
        }

               
        public void CreateTransaction()
        {
            transaction.CreateTransaction();              
        }

        public void BeginTransaction()
        {
            transaction.BeginTransaction(); 
        }

        public void Commit()
        {
            transaction.Commit(); 
        }

        public void Rollback()
        {
            transaction.Rollback();
        }

    }
}
