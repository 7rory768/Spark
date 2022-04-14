using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Spark.ControllerHelpers
{
    public class ProjectHelper : ControllerHelper
    {
        public static ResponseMessage Add(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int teamId = data["teamId"].Value<int>();
            string name = data["name"].Value<string>();
            int budget = data["budget"].Value<int>();

            // Add instance to database
            var instance = DatabaseLibrary.Helpers.ProjectDBHelper.Add(teamId, name, budget, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a new project.");
        }

        /// <summary>
        /// Gets list of participating projects.
        /// </summary>
        /// <param name="includeDetailedErrors">States whether the internal server error message should be detailed or not.</param>
        public static ResponseMessage GetParticipatingProjects(User user,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.ProjectDBHelper.GetAll(user.username, context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the projects.");
        }
    }
}
