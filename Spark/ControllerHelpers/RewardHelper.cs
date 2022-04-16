using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Spark.ControllerHelpers
{
    public class RewardHelper : ControllerHelper
    {
        // add a reward 
        public static ResponseMessage AddReward(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int numPoints = data["numPoints"].Value<int>();
            int teamId = data["teamId"].Value<int>();
            int projectId = data["projectId"].Value<int>();

            // Add instance to database
            var instance = DatabaseLibrary.Helpers.RewardDBHelper.Add(user.username, numPoints, teamId, projectId, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a new Reward.");
        }

        //// 
        //public static ResponseMessage GetUserTeamRewards(User user, int id, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        //{
        //    var instances = DatabaseLibrary.Helpers.RewardDBHelper.GetTotal(user.username, id, context, out StatusResponse statusResponse);
        //    return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the projects.");
        //}
    }
}
