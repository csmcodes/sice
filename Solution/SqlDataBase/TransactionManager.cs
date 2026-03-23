using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BusinessObjects;
using System.Data;

namespace SqlDataBase
{
    public class TransactionManager
    {
        public SqlTransaction transaction { get; set; }
        public SqlConnection connection { get; set; }

        private string connectionString = System.Configuration.ConfigurationSettings.AppSettings["connection"].ToString();
        private bool _transactionOpen = false;


        public TransactionManager()
        {
            connection  = new SqlConnection(connectionString);
        }

        public TransactionManager(SqlConnection connection)
        {
            this.connection = connection;
            
        }

        /// <summary>
        ///	Gets a value that indicates if a transaction is currently open and operating. 
        /// </summary>
        /// <value>Return true if a transaction session is currently open and operating; otherwise false.</value>
        public bool IsOpen
        {
            get { return _transactionOpen; }
        }


        /// <summary>
        ///		Begins a transaction.
        /// </summary>
        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (IsOpen)
                throw new InvalidOperationException("Transaction already open.");

            //Open connection
            try
            {
                this.connection.Open();
                this.transaction = this.connection.BeginTransaction(isolationLevel);
                this._transactionOpen = true;
            }
            // in the event of an error, close the connection and destroy the transobject.
            catch (DataException ex)
            {                
                if (this.connection != null) this.connection.Close();
                if (this.transaction != null) this.transaction.Dispose();
                throw ex;
                //this.connection.Close();
                //this.transaction.Dispose();
                //throw new DataException("Error while creating connection for transaction.", e);
            }			
        }


        /// <summary>
        ///		Commit the transaction to the datasource.
        /// </summary>
        public void Commit()
        {
            if (!this.IsOpen)
            {
                throw new InvalidOperationException("Transaction needs to begin first.");
            }

            this.transaction.Commit();
            //assuming the commit was sucessful.
            this.connection.Close();
            this.transaction.Dispose();
            this._transactionOpen = false;
        }

        /// <summary>
        ///	Rollback the transaction.
        /// </summary>
        public void Rollback()
        {
            if (!this.IsOpen)
            {
                throw new InvalidOperationException("Transaction needs to begin first.");
            }

            this.transaction.Rollback();
            this.connection.Close();
            this.transaction.Dispose();
            this._transactionOpen = false;
        }
    }
}
