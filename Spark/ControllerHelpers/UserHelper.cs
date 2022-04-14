﻿using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Spark.ControllerHelpers
{
    public class UserHelper
    {
        /// <summary>
        /// Creates a user.
        /// </summary>
        /// <param name="includeDetailedErrors">States whether the internal server error message should be detailed or not.</param>
        public static ResponseMessage Add(JObject data,
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Extract paramters
            string username = data["username"].Value<string>();
            string firstName = data.GetValue("firstName").Value<string>();
            string lastName = (data.ContainsKey("lastName")) ? data.GetValue("lastName").Value<string>() : null;
            string password = data["password"].Value<string>();
            string email = data.GetValue("email").Value<string>();
            string userType = data.GetValue("userType").Value<string>();

            // Add instance to database
            var instance = DatabaseLibrary.Helpers.UserDBHelper.Add(username, firstName, lastName, password, email, userType,
                context, out StatusResponse statusResponse);

            // Get rid of detailed internal server error message (when requested)
            if (statusResponse.StatusCode == HttpStatusCode.InternalServerError && !includeDetailedErrors)
                statusResponse.Message = "Something went wrong while adding a new user.";

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

        /// <summary>
        /// Gets list of students.
        /// </summary>
        /// <param name="includeDetailedErrors">States whether the internal server error message should be detailed or not.</param>
        public static ResponseMessage GetCollection(
            DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            // Get instances from database
            var instances = DatabaseLibrary.Helpers.UserDBHelper.GetCollection(
                context, out StatusResponse statusResponse);

            // Get rid of detailed error message (when requested)
            if (statusResponse.StatusCode == HttpStatusCode.InternalServerError && !includeDetailedErrors)
                statusResponse.Message = "Something went wrong while retrieving the users";

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

        public static ResponseMessage Login(JObject data, DbContext context, out HttpStatusCode statusCode, bool includeDetailedErrors = false)
        {
            string username = data["username"].Value<string>();
            string password = data["password"].Value<string>();


            User user = DatabaseLibrary.Helpers.UserDBHelper.Login(username, password, context, out StatusResponse statusResponse);

            // Get rid of detailed error message (when requested)
            if (statusResponse.StatusCode == HttpStatusCode.InternalServerError && !includeDetailedErrors)
                statusResponse.Message = "Something went wrong while retrieving the users";

            // Return response
            var response = new ResponseMessage
                (
                    user != null,
                    statusResponse.Message,
                    user
                );
            statusCode = statusResponse.StatusCode;
            return response;
        }

    }
}
