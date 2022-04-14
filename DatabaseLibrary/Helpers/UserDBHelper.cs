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
    public class UserDBHelper : DBHelper
    {
        private static MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        public static User fromRow(DataRow row)
        {
            return cache.Set(row["username"].ToString(), new User(
                            username: row["username"].ToString(),
                            fName: row["fName"].ToString(),
                            lName: row["lName"]?.ToString(),
                            password: row["password"].ToString(),
                            email: row["email"].ToString(),
                            dateCreated: DateTime.Parse(row["dateCreated"].ToString()).ToLocalTime()
                            ));
        }

        public static User? GetUser(string username, DbContext context)
        {
            try
            {
                if (isNotAlphaNumeric(username.Trim()))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid user name.");
                }

                // Validate
                if (string.IsNullOrEmpty(username.Trim())) return null;

                // Check Cache first
                User user = cache.Get<User>(username);
                if (user != null) return user;

                // Get from database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "getUser",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_username", username }
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                if (table.Rows.Count == 0)
                {
                    cache.Remove(username);
                    return null;
                }
                else return fromRow(table.Rows[0]);
            }
            catch (Exception exception)
            {
                return null;
            }
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

                // Dont check cache since passwords are protected

                bool success = false;

                // Login in database
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
                    return fromRow(row);
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
        public static User Add(string username, string firstName, string? lastName, string password, string email, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                // Validate
                if (string.IsNullOrEmpty(firstName.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a first name.");
                if (string.IsNullOrEmpty(lastName.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a last name.");
                if (string.IsNullOrEmpty(email.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide an email address.");
                if (string.IsNullOrEmpty(username.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a username.");
                if (string.IsNullOrEmpty(password.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a password.");


                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "createUser",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_username", username },
                            { "_fName", firstName },
                            { "_lName", lastName },
                            { "_password", password },
                            { "_email", email },
                        },
                        message: out string message
                    );

                if (table == null)
                    throw new Exception(message);

                DataRow row = table.Rows[0];

                // Return value
                if (string.IsNullOrEmpty(row["username"].ToString()))
                {
                    statusResponse = new StatusResponse("User already exists");
                    return null;
                }
                else
                {
                    statusResponse = new StatusResponse("User created successfully");
                    return fromRow(row);
                }
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        // Updates a user's info
        public static User Update(string username, string firstName, string? lastName, string password, string email, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                // Validate
                if (string.IsNullOrEmpty(firstName.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a first name.");
                if (string.IsNullOrEmpty(lastName.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a last name.");
                if (string.IsNullOrEmpty(email.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide an email address.");
                if (string.IsNullOrEmpty(username.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a username.");
                if (string.IsNullOrEmpty(password.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a password.");


                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "updateUser",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_username", username },
                            { "_fName", firstName },
                            { "_lName", lastName },
                            { "_password", password },
                            { "_email", email },
                        },
                        message: out string message
                    );

                if (table == null)
                    throw new Exception(message);

                DataRow row = table.Rows[0];

                // Return value
                //if (string.IsNullOrEmpty(row["username"].ToString()))
                //{
                //    statusResponse = new StatusResponse("User information unsuccessfully changed");
                //   return null;
                //}
                //else
                //{
                    statusResponse = new StatusResponse("User information successfully changed");
                    return fromRow(row);
                //}
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
                    fromRow(row);

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
