using BusinessLibrary.Models;
using DatabaseLibrary.Helpers;
using Microsoft.AspNetCore.Mvc;
using Spark.ContextHelpers;
using System.Net;

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

        protected readonly ResponseMessage notAuthorizedMessage = new ResponseMessage
            (
                false,
                "You are not authorized to do this",
                null
            );

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

        #endregion

        protected bool isAuthenticated()
        {
            return Request.Headers.ContainsKey("Authorization");
        }

        protected ResponseMessage getNotAuthenticatedResponse()
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return notAuthorizedMessage;
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
