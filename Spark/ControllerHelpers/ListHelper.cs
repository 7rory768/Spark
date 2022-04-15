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

            var instance = DatabaseLibrary.Helpers.ListDBHelper.Add(projectId, name, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a list.");
        }
        public static ResponseMessage Update(User user, TaskList taskList, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instance = DatabaseLibrary.Helpers.ListDBHelper.Update(taskList, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a list.");
        }

        public static ResponseMessage MoveList(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int listId = data["id"].Value<int>();
            int projectId = data["projectId"].Value<int>();
            int newPosition = data["newPosition"].Value<int>();

            var instance = DatabaseLibrary.Helpers.ListDBHelper.moveList(projectId, listId, newPosition, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while moving the list.");
        }

        public static ResponseMessage GetLists(User user, int projectId,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.ListDBHelper.GetAll(projectId, context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the lists.");
        }


        public static ResponseMessage DeleteList(User user, int listId,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.ListDBHelper.Delete(listId, context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while deleting the label.");
        }
    }
}
