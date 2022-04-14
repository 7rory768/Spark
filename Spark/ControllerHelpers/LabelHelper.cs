using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Spark.ControllerHelpers
{
    public class LabelHelper
    {
        public static ResponseMessage Add(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int projectId = data["projectId"].Value<int>();
            string name = data["name"].Value<string>();
            string color = data["color"].Value<string>();

            // Add instance to database
            var dbInstance = DatabaseLibrary.Helpers.LabelDBHelper.Add(projectId, name, color, context, out StatusResponse statusResponse);

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

        public static ResponseMessage GetLabels(User user, int projectId,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Get instances from database
            var instances = DatabaseLibrary.Helpers.LabelDBHelper.GetAll(projectId, context, out StatusResponse statusResponse);

            // Get rid of detailed error message (when requested)
            if (statusResponse.StatusCode == HttpStatusCode.InternalServerError && !includeDetailedErrors)
                statusResponse.Message = "Something went wrong while retrieving the labels";

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


        public static ResponseMessage DeleteLabel(User user, int projectId, string name,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Get instances from database
            bool success = DatabaseLibrary.Helpers.LabelDBHelper.Delete(projectId, name, context, out StatusResponse statusResponse);

            // Get rid of detailed error message (when requested)
            if (statusResponse.StatusCode == HttpStatusCode.InternalServerError && !includeDetailedErrors)
                statusResponse.Message = "Something went wrong while retrieving the labels";

            // Return response
            var response = new ResponseMessage
                (
                    success,
                    statusResponse.Message,
                    null
                );
            statusCode = statusResponse.StatusCode;
            return response;
        }
    }
}
