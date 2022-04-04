using DatabaseLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webservice.ContextHelpers
{
    /// <summary>
    /// This class ensures the database is up and ready.
    /// </summary>
    public class DatabaseContextHelper
    {

        #region Initialization

        /// <summary>
        /// Reference to the app settings helper instance added in the Startup.cs.
        /// </summary>
        private readonly AppSettingsHelper AppSettings;

        /// <summary>
        /// Constructor called by the service provider.
        /// Using injection to get the parameters.
        /// </summary>
        public DatabaseContextHelper(AppSettingsHelper appSettings)
        {
            AppSettings = appSettings;
        }

        #endregion

        #region Variables

        /// <summary>
        /// Retrieves a database context.
        /// </summary>
        public DbContext DbContext
        {
            get
            {
                return new DatabaseLibrary.Providers.MySqlDbContext(AppSettings.DATABASE_CONNECTION_STRING);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the databases and applies migrations.
        /// </summary>
        public bool Initialize(out string message)
        {
            return DbContext.Initialize(out message);
        }

        #endregion

    }
}
