using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Spark.ControllerHelpers
{
    public class ListHelper : ControllerHelper
    {
        public static ResponseMessage Add(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int projectId = data["projectId"].Value<int>();
            string name = data["name"].Value<string>();
            int position = data["position"].Value<int>();

            var instance = DatabaseLibrary.Helpers.ListDBHelper.Add(projectId, name, position, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a list.");
        }
        public static ResponseMessage MoveList(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int projectId = data["projectId"].Value<int>();
            string name = data["name"].Value<string>();
            int newPosition = data["newPosition"].Value<int>();

            var instance = DatabaseLibrary.Helpers.ListDBHelper.moveList(projectId, name, newPosition, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while moving the list.");
        }

        public static ResponseMessage GetLists(User user, int projectId,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.ListDBHelper.GetAll(projectId, context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the lists.");
        }


        public static ResponseMessage DeleteLabel(User user, int projectId, string name,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            bool success = DatabaseLibrary.Helpers.ListDBHelper.Delete(projectId, name, context, out StatusResponse statusResponse);
            return getResponse(success, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while deleting the label.");
        }
    }
}
