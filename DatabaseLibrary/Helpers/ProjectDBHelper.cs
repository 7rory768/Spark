using DatabaseLibrary.Core;
using BusinessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Text;

namespace DatabaseLibrary.Helpers
{
    public class ProjectDBHelper
    {
        private static Project fromRow(DataRow row)
        {
            return new Project(
                            id: int.Parse(row["id"].ToString()),
                            teamId: int.Parse(row["teamId"].ToString()),
                            name: row["name"].ToString(),
                            budget: int.Parse(row["budget"].ToString()),
                            dateCreated: DateTime.Parse(row["dateCreated"].ToString()).ToLocalTime(),
                            mgrUsername: row["mgrUsername"]?.ToString()
                            );
        }

        public static Project? Add(int teamId, string name, int budget, string? mgrUsername, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                // Validate
                if (string.IsNullOrEmpty(name.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a name.");

                bool success = false;

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "createProject",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_teamId", teamId },
                            { "_name", name },
                            { "_budget", budget},
                            { "_mgrUsername", mgrUsername},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                DataRow row = table.Rows[0];

                // Return value
                if (string.IsNullOrEmpty(row["id"].ToString()))
                {
                    statusResponse = new StatusResponse("Failed to create project");
                    return null;
                }
                else
                {
                    statusResponse = new StatusResponse("Created project successfully");

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
        /// Get all projects the current user can see
        /// </summary>
        public static IEnumerable<Project> GetAll(string username, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                // Validate


                // Generate a new instance
                User instance = new User
                    (
                        username, firstName, lastName, password, email, DateTime.Now, userType
                    );

                // Add to database
                int rowsAffected = context.ExecuteNonQueryCommand
                    (
                        commandText: "INSERT INTO users (username, fName, lName, password, email, userType) values (@username, @fName, @lName, @password, @email, @userType)",
                        parameters: new Dictionary<string, object>()
                        {
                            { "@username", instance.username },
                            { "@fName", instance.fName },
                            { "@lName", instance.lName },
                            {"@password", instance.password },
                            {"@email", instance.email },
                            {"@userType", instance.userType }
                        },
                        message: out string message
                    );
                if (rowsAffected == -1)
                    throw new Exception(message);

                // Return value
                statusResponse = new StatusResponse("User added successfully");
                return instance;
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
