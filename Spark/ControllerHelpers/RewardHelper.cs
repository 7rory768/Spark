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

        // get total for user
        public static ResponseMessage GetUserRewards(User user, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            int total = DatabaseLibrary.Helpers.RewardDBHelper.getTotal(user.username, context, out StatusResponse statusResponse);
            return getResponse(total, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the rewards for user.");
        }

        // get total for user on team
        public static ResponseMessage GetUserRewardsInTeam(User user, int teamId, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            int total = DatabaseLibrary.Helpers.RewardDBHelper.getTotalInTeam(user.username, teamId, context, out StatusResponse statusResponse);
            return getResponse(total, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the rewards for user within team.");
        }
    }
}
