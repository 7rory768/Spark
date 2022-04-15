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
            if (!ContainsRequiredKeys(data, "projectId", "listId", "name"))
                return GetMissingKeysResponse(data, out statusCode, includeDetailedErrors, "projectId", "listId", "name");

            // Extract paramters
#pragma warning disable CS8604 // Possible null reference argument.
            int projectId = data["projectId"].Value<int>();
            int listId = data["listId"].Value<int>();
            string name = data["name"].Value<string>();
            string description = data["description"].Value<string>();
            DateOnly? deadline = data.ContainsKey("deadline") ? DateOnly.FromDateTime(data["deadline"].Value<DateTime>().Date) : null;
            int completionPoints = data.ContainsKey("completionPoints") ? data["completionPoints"].Value<int>() : 0;

            List<string> assignedUsers = new List<string>();
            if (data.ContainsKey("assignedUsers"))
            {
                foreach (JObject obj in data["assignedUsers"])
                    assignedUsers.Add(obj["username"].Value<string>());
            }
#pragma warning restore CS8604 // Possible null reference argument.


            var instance = DatabaseLibrary.Helpers.TaskDBHelper.Add(projectId, listId, name, description, deadline, completionPoints, assignedUsers, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a task.");
        }
        public static ResponseMessage Update(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            if (!ContainsRequiredKeys(data, "id", "name", "description", "completionPoints", "completed"))
                return GetMissingKeysResponse(data, out statusCode, includeDetailedErrors, "id", "name", "description", "completionPoints", "completed");

            // Extract paramters
            int id = data["id"].Value<int>();
            string name = data["name"].Value<string>();
            string description = data["description"].Value<string>();
            data.TryGetValue("deadline", out JToken deadline);
            int completionPoints = data["completionPoints"].Value<int>();
            bool completed = data["completed"].Value<bool>();


            var instance = DatabaseLibrary.Helpers.TaskDBHelper.Update(id, name, description, deadline?.Value<DateOnly>("deadline"), completionPoints, completed, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a task.");
        }

        public static ResponseMessage AssignToTask(JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            if (!ContainsRequiredKeys(data, "id", "username"))
                return GetMissingKeysResponse(data, out statusCode, includeDetailedErrors, "id", "username");

            // Extract paramters
            int id = data["id"].Value<int>();
            string username = data["username"].Value<string>();

            var instance = DatabaseLibrary.Helpers.TaskDBHelper.assignToTask(id, username, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while assigning the user to the task.");
        }

        public static ResponseMessage UnassignFromTask(JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            if (!ContainsRequiredKeys(data, "id", "username"))
                return GetMissingKeysResponse(data, out statusCode, includeDetailedErrors, "id", "username");

            // Extract paramters
            int id = data["id"].Value<int>();
            string username = data["username"].Value<string>();

            var instance = DatabaseLibrary.Helpers.TaskDBHelper.unassignFromTask(id, username, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while unassigning the user from the task.");
        }

        public static ResponseMessage MoveTask(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            if (!ContainsRequiredKeys(data, "id", "listId", "newPriority"))
                return GetMissingKeysResponse(data, out statusCode, includeDetailedErrors, "id", "listId", "newPriority");

            // Extract paramters
            int id = data["id"].Value<int>();
            int listId = data["listId"].Value<int>();
            int newPriority = data["newPriority"].Value<int>();

            var instance = DatabaseLibrary.Helpers.TaskDBHelper.moveTask(id, listId, newPriority, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while moving the task.");
        }

        public static ResponseMessage GetTasks(User user, int listId,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.TaskDBHelper.GetAll(listId, context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the lists.");
        }


        public static ResponseMessage DeleteTask(User user, int taskId,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            bool success = DatabaseLibrary.Helpers.TaskDBHelper.Delete(taskId, context, out StatusResponse statusResponse);
            return getResponse(success, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while deleting the label.");
        }
    }
}
