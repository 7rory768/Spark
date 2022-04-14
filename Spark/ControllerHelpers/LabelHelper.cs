using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Spark.ControllerHelpers
{
    public class LabelHelper : ControllerHelper
    {
        public static ResponseMessage Add(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            int projectId = data["projectId"].Value<int>();
            string name = data["name"].Value<string>();
            string color = data["color"].Value<string>();

            var instance = DatabaseLibrary.Helpers.LabelDBHelper.Add(projectId, name, color, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a label.");
        }

        public static ResponseMessage GetLabels(User user, int projectId,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.LabelDBHelper.GetAll(projectId, context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the labels.");
        }


        public static ResponseMessage DeleteLabel(User user, int projectId, string name,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            bool success = DatabaseLibrary.Helpers.LabelDBHelper.Delete(projectId, name, context, out StatusResponse statusResponse);
            return getResponse(success, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while deleting the label.");
        }
    }
}
