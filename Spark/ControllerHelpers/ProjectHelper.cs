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
            var instance = DatabaseLibrary.Helpers.ProjectDBHelper.Add(user.username, teamId, name, budget, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a new project.");
        }

        /// <summary>
        /// Gets list of participating projects.
        /// </summary>
        /// <param name="includeDetailedErrors">States whether the internal server error message should be detailed or not.</param>
        public static ResponseMessage Get(User user, int projectId,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.ProjectDBHelper.Get(user.username, projectId, context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the projects.");
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

        public static ResponseMessage GetTeamProjects(int id, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.ProjectDBHelper.GetTeamProjects(id, context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the projects.");
        }

        //Update the information for a project
        public static ResponseMessage Update(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int projectId = data["projectId"].Value<int>();
            int teamId = data["teamId"].Value<int>();
            string name = data["name"].Value<string>();
            int budget = data["budget"].Value<int>();

            var instance = DatabaseLibrary.Helpers.ProjectDBHelper.Update(user.username, projectId,  teamId, name, budget, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while updating a the user's information.");
        }

        //Delete a project
        public static ResponseMessage Delete(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int projectId = data["projectId"].Value<int>();
            int teamId = data["teamId"].Value<int>();

            bool success = DatabaseLibrary.Helpers.ProjectDBHelper.Delete(user.username, projectId, teamId, context, out StatusResponse statusResponse);
            return getResponse(success, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while updating a the user's information.");
        }
        
    }
}
