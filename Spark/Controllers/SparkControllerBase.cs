using BusinessLibrary.Models;
using DatabaseLibrary.Helpers;
using Microsoft.AspNetCore.Mvc;
using Spark.ContextHelpers;

namespace Spark.Controllers
{
    public class SparkControllerBase : ControllerBase
    {

        #region Initialization

        /// <summary>
        /// Reference to the hosting environment instance added in the Startup.cs.
        /// </summary>
        protected readonly IWebHostEnvironment HostingEnvironment;

        /// <summary>
        /// Reference to the app settings helper instance added in the Startup.cs.
        /// </summary>
        protected readonly AppSettingsHelper AppSettings;

        /// <summary>
        /// Reference to the database context helper instance added in the Startup.cs.
        /// </summary>
        protected readonly DatabaseContextHelper Database;

        /// <summary>
        /// Constructor called by the service provider.
        /// Using injection to get the arguments.
        /// </summary>
        public SparkControllerBase(IWebHostEnvironment hostingEnvironment, AppSettingsHelper appSettings,
            DatabaseContextHelper database)
        {
            HostingEnvironment = hostingEnvironment;
            AppSettings = appSettings;
            Database = database;
        }

        protected bool isAuthenticated()
        {
            return Request.Headers.ContainsKey("Authorization");
        }
        protected string getUsername()
        {
            return Request.Headers["Authorization"];
        }

        protected User? getUser()
        {
            return UserDBHelper.GetUser(getUsername(), Database.DbContext);
        }
    }
}
