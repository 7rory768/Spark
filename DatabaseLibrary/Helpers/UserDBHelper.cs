using DatabaseLibrary.Core;
using BusinessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace DatabaseLibrary.Helpers
{
    public class UserDBHelper
    {
        private static MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        private static User fromRow(DataRow row)
        {
            return new User(
                            username: row["username"].ToString(),
                            fName: row["fName"].ToString(),
                            lName: row["lName"]?.ToString(),
                            password: row["password"].ToString(),
                            email: row["email"].ToString(),
                            dateCreated: DateTime.Parse(row["dateCreated"].ToString()).ToLocalTime(),
                            userType: row["userType"].ToString()
                            );
        }

        public static User? Login(string username, string password, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                // Validate
                if (string.IsNullOrEmpty(username.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a username.");
                if (string.IsNullOrEmpty(password.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a password.");

                // Check Cache first
                User user = cache.Get<User>(username);

                if (user != null)
                {
                    if (user.password != password)
                    {
                        statusResponse = new StatusResponse("Incorrect password");
                        return null;
                    }

                    statusResponse = new StatusResponse("User logged in successfully");
                    return user;
                }

                bool success = false;

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "loginUser",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_username", username },
                            { "_password", password }
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                DataRow row = table.Rows[0];

                // Return value
                if (string.IsNullOrEmpty(row["success"].ToString()))
                {
                    statusResponse = new StatusResponse("Unknown user");
                    return null;
                }
                else if (!bool.Parse(row["success"].ToString()))
                {
                    statusResponse = new StatusResponse("Incorrect password");
                    return null;
                }
                else
                {
                    statusResponse = new StatusResponse("User logged in successfully");
                    return cache.Set(username, fromRow(row));
                }
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        /// <summary>
        /// Adds a new instance into the database.
        /// </summary>
        public static User Add(string username, string firstName, string? lastName, string password, string? email, string userType, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                // Validate
                if (string.IsNullOrEmpty(username.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a username.");
                if (string.IsNullOrEmpty(firstName.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a first name.");
                if (string.IsNullOrEmpty(password.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a password.");
                if (!UserType.isValid(userType))
                    throw new StatusException(HttpStatusCode.BadRequest, userType + " is not a valid user type.");

                // Generate a new instance
                User instance = new User
                    (
                        username, firstName, lastName, password, email, DateTime.Now, userType
                    );

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "createUser",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_username", instance.username },
                            { "_fName", instance.fName },
                            { "_lName", instance.lName },
                            {"_password", instance.password },
                            {"_email", instance.email },
                            {"_userType", instance.userType }
                        },
                        message: out string message
                    );

                if (table == null)
                    throw new Exception(message);

                // Return value
                statusResponse = new StatusResponse("User added successfully");
                return cache.Set(username, fromRow(table.Rows[0]));
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list of instances.
        /// </summary>
        public static List<User> GetCollection(
            DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                // Get from database
                DataTable table = context.ExecuteDataQueryCommand
                    (
                        commandText: "SELECT * FROM users",
                        parameters: new Dictionary<string, object>()
                        {

                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                // Parse data
                List<User> instances = new List<User>();
                foreach (DataRow row in table.Rows)
                    instances.Add(fromRow(row));

                // Return value
                statusResponse = new StatusResponse("Users list has been retrieved successfully.");
                return instances;
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

    }
}
