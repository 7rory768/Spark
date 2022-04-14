using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Spark.ControllerHelpers
{
    public class TaskHelper : ControllerHelper
    {
        public static ResponseMessage Add(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int projectId = data["projectId"].Value<int>();
            string listName = data["listName"].Value<string>();
            string name = data["name"].Value<string>();
            string description = data["description"].Value<string>();
            int priority = data["priority"].Value<int>();
            data.TryGetValue("deadline", out JToken deadline);
            int completionPoints = data["completionPoints"].Value<int>();


            var instance = DatabaseLibrary.Helpers.TaskDBHelper.Add(projectId, listName, name, description, priority, deadline?.Value<DateOnly>("deadline"), completionPoints, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a task.");
        }
        public static ResponseMessage Update(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int projectId = data["projectId"].Value<int>();
            string listName = data["listName"].Value<string>();
            string name = data["name"].Value<string>();
            string description = data["description"].Value<string>();
            data.TryGetValue("deadline", out JToken deadline);
            int completionPoints = data["completionPoints"].Value<int>();
            bool completed = data["completed"].Value<bool>();


            var instance = DatabaseLibrary.Helpers.TaskDBHelper.Update(projectId, listName, name, description, deadline?.Value<DateOnly>("deadline"), completionPoints, completed, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a task.");
        }
        public static ResponseMessage MoveTask(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int projectId = data["projectId"].Value<int>();
            string listName = data["listName"].Value<string>();
            string name = data["name"].Value<string>();
            int newPriority = data["newPriority"].Value<int>();

            var instance = DatabaseLibrary.Helpers.TaskDBHelper.moveTask(projectId, listName, name, newPriority, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while moving the task.");
        }

        public static ResponseMessage GetTasks(User user, int projectId, string listName,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.TaskDBHelper.GetAll(projectId, listName, context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the lists.");
        }


        public static ResponseMessage DeleteTask(User user, int projectId, string listName, string name,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            bool success = DatabaseLibrary.Helpers.TaskDBHelper.Delete(projectId, listName, name, context, out StatusResponse statusResponse);
            return getResponse(success, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while deleting the label.");
        }
    }
}
