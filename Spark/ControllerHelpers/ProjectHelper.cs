using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Spark.ControllerHelpers
{
    public class ProjectHelper
    {
        public static ResponseMessage Add(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int teamId = data["teamId"].Value<int>();
            string name = data["name"].Value<string>();
            int budget = data["budget"].Value<int>();

            // TODO: SqlInjection protection

            // Add instance to database
            var dbInstance = DatabaseLibrary.Helpers.ProjectDBHelper.Add(teamId, name, budget, context, out StatusResponse statusResponse);

            // Get rid of detailed internal server error message (when requested)
            if (statusResponse.StatusCode == HttpStatusCode.InternalServerError && !includeDetailedErrors)
                statusResponse.Message = "Something went wrong while adding a new user.";

            // Return response
            var response = new ResponseMessage
                (
                    dbInstance != null,
                    statusResponse.Message,
                    dbInstance
                );
            statusCode = statusResponse.StatusCode;
            return response;
        }

        /// <summary>
        /// Gets list of participating projects.
        /// </summary>
        /// <param name="includeDetailedErrors">States whether the internal server error message should be detailed or not.</param>
        public static ResponseMessage GetParticipatingProjects(User user,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Get instances from database
            var instances = DatabaseLibrary.Helpers.ProjectDBHelper.GetAll(user.username,
                context, out StatusResponse statusResponse);

            // Get rid of detailed error message (when requested)
            if (statusResponse.StatusCode == HttpStatusCode.InternalServerError && !includeDetailedErrors)
                statusResponse.Message = "Something went wrong while retrieving the users";

            // Return response
            var response = new ResponseMessage
                (
                    instances != null,
                    statusResponse.Message,
                    instances
                );
            statusCode = statusResponse.StatusCode;
            return response;
        }
    }
}
