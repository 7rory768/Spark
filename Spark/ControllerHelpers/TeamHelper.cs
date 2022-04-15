using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Spark.ControllerHelpers
{
    public class TeamHelper : ControllerHelper
    {
        /// <summary>
        /// Gets list of participating teams.
        /// </summary>
        /// <param name="includeDetailedErrors">States whether the internal server error message should be detailed or not.</param>
        public static ResponseMessage GetParticipatingTeams(User user,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.TeamDBHelper.GetAll(user.username, context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the teams.");
        }

        // Get list of members in a team
        public static ResponseMessage GetMembers(int id, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.UserDBHelper.GetAllMembers(id, context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the teams.");
        }

        // Add/Creates a new team
        public static ResponseMessage Add(JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            string name = data["name"].Value<string>();
            string mgrUsername = data["mgrUsername"].Value<string>(); 

            // Add instance to database
            var instance = DatabaseLibrary.Helpers.TeamDBHelper.Add(name, mgrUsername, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a new team.");
        }

        //Delete team
        public static ResponseMessage DeleteTeam(int id, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            bool success = DatabaseLibrary.Helpers.TeamDBHelper.Delete(id, context, out StatusResponse statusResponse);
            return getResponse(success, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while deleting the label.");
        }
    }
}
