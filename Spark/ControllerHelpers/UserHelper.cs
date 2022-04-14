using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Spark.ControllerHelpers
{
    public class UserHelper : ControllerHelper
    {
        public static ResponseMessage Add(JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract parameters
            string username = data["username"].Value<string>();
            string firstName = data.GetValue("firstName").Value<string>();
            string lastName = (data.ContainsKey("lastName")) ? data.GetValue("lastName").Value<string>() : null;
            string password = data["password"].Value<string>();
            string email = data.GetValue("email").Value<string>();

            // Add instance to database
            var instance = DatabaseLibrary.Helpers.UserDBHelper.Add(username, firstName, lastName, password, email, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while adding a new user.");
        }

        public static ResponseMessage GetCollection(DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            var instances = DatabaseLibrary.Helpers.UserDBHelper.GetCollection(context, out StatusResponse statusResponse);
            return getResponse(instances, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while getting the users.");
        }

        public static ResponseMessage Login(JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            string username = data["username"].Value<string>();
            string password = data["password"].Value<string>();

            User user = DatabaseLibrary.Helpers.UserDBHelper.Login(username, password, context, out StatusResponse statusResponse);
            return getResponse(user, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while logging in the user.");
        }

        public static ResponseMessage Update(User user, JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            string firstName = data.GetValue("firstName").Value<string>();
            string lastName = (data.ContainsKey("lastName")) ? data.GetValue("lastName").Value<string>() : null;
            string password = data["password"].Value<string>();
            string email = data.GetValue("email").Value<string>();

            var instance = DatabaseLibrary.Helpers.UserDBHelper.Update(user.username, firstName, lastName, password, email, context, out StatusResponse statusResponse);
            return getResponse(instance, out statusCode, statusResponse, includeDetailedErrors, "Something went wrong while updating a the user's information.");
        }

    }
}
