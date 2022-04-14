using BusinessLibrary.Models;
using DatabaseLibrary.Core;
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
                    instance != null,
                    statusResponse.Message,
                    instance
                );
            statusCode = statusResponse.StatusCode;
            return response;
        }
    }
}
