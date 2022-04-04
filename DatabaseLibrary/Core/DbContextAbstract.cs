using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DatabaseLibrary.Core
{
    /// <summary>
    /// Database context abstract.
    /// Template used by all provider implementations.
    /// </summary>
    public abstract class DbContext
    {

        #region Constructors

        /// <summary>
        /// Uses the connection string to connect to the database.
        /// </summary>
        public DbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #endregion

        #region Variables

        /// <summary>
        /// Connection string used to connect to the database.
        /// </summary>
        public string ConnectionString { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Performs a setup required before running any further commands or queries.
        /// Usually gets called once on startup.
        /// </summary>
        /// <param name="message">A detailed error message stating the reason when it fails.</param>
        /// <returns>States if the initialization has been completed without any issue.</returns>
        public abstract bool Initialize(out string message);

        /// <summary>
        /// Executes the provided command.
        /// </summary>
        /// <returns>States the number of rows affected by the command.</returns>
        public abstract int ExecuteNonQueryCommand(string commandText, Dictionary<string, object> parameters, out string message);

        /// <summary>
        /// Executes the provided command and retrieves some data.
        /// </summary>
        /// <param name="message">A detailed error message stating the reason when it fails.</param>
        /// <returns>Datatabe with all the rows that are retrieved.</returns>
        public abstract DataTable ExecuteDataQueryCommand(string commandText, Dictionary<string, object> parameters, out string message);

        /// <summary>
        /// Executes the provided stored procedure.
        /// </summary>
        /// <param name="message">A detailed error message stating the reason when it fails.</param>
        /// <returns>States the number of rows affected by the command.</returns>
        public abstract int ExecuteNonQueryProcedure(string procedure, Dictionary<string, object> parameters, out string message);

        /// <summary>
        /// Executes the provided stored procedure and retrieves some data.
        /// </summary>
        /// <param name="message">A detailed error message stating the reason when it fails.</param>
        /// <returns>Datatabe with all the rows that are retrieved.</returns>
        public abstract DataTable ExecuteDataQueryProcedure(string procedure, Dictionary<string, object> parameters, out string message);

        #endregion

    }
}
