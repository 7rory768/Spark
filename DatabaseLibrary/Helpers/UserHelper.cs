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
    public class UserHelper
    {
        /// <summary>
        /// Adds a new instance into the database.
        /// </summary>
        public static User Add(string username, string firstName, string? lastName, string password, string? email, UserType userType = UserType.EMPLOYEE, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                // Validate
                if (string.IsNullOrEmpty(firstName?.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a first name.");
                if (string.IsNullOrEmpty(lastName?.Trim()))
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a last name.");

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
                statusResponse = new StatusResponse("Student added successfully");
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
                    instances.Add(new User
                            (
                                username: row["username"].ToString(),
                                fName: row["fName"].ToString(),
                                lName: row["lName"].ToString(),
                                password: row["password"].ToString(),
                                email: row["email"].ToString(),
                                dateCreated: DateTime.Parse(row["dateCreated"].ToString()),
                                userType: UserTypeExtensions.parse(row["userType"].ToString())
                            )
                        );

                // Return value
                statusResponse = new StatusResponse("Students list has been retrieved successfully.");
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
