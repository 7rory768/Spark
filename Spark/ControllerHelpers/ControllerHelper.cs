using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Spark.ControllerHelpers
{
    public class ControllerHelper
    {
        protected static ResponseMessage getResponse(object instance, out HttpStatusCode statusCode, StatusResponse statusResponse, bool includeDetailedErrors, string errorMessage = "Something went wrong.")
        {
            // Get rid of detailed internal server error message (when requested)
            if (statusResponse.StatusCode == HttpStatusCode.InternalServerError && !includeDetailedErrors)
                statusResponse.Message = errorMessage;

            // Return response
            var response = new ResponseMessage
                (
                    state: instance != null ? (instance.GetType() == typeof(bool) ? (bool)instance : true) : false,
                    message: statusResponse.Message,
                    data: instance
                );
            statusCode = statusResponse.StatusCode;
            return response;
        }

        public static bool ContainsRequiredKeys(JObject data, params string[] keys)
        {
            foreach (string key in keys)
                if (!data.ContainsKey(key))
                    return false;

            return true;
        }

        public static string GetMissingKeysMessage(JObject data, params string[] keys)
        {
            List<string> missingKeys = new List<string>();

            string message = "Your body is missing the following required keys: ";

            foreach (string key in keys)
                if (!data.ContainsKey(key))
                    missingKeys.Add(key);

            message = message + string.Join(", ", missingKeys);

            return message;
        }

        public static ResponseMessage GetMissingKeysResponse(JObject data, out HttpStatusCode statusCode, bool includeDetailedErrors, params string[] keys)
        {
            return getResponse(false, out statusCode, new StatusResponse("Missing required keys."), includeDetailedErrors, GetMissingKeysMessage(data, keys));
        }
    }
}
