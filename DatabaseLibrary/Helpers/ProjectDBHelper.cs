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
        public static List<Project> GetAll(string username, DbContext context, out StatusResponse statusResponse)
        {
            List<Project> projects = new List<Project>();

            try
            {

                // Get from database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "getParticipatingProjects",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_username", username },
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                // Return value
                statusResponse = new StatusResponse("Got participating projects successfully");

                foreach (DataRow row in table.Rows)
                    projects.Add(fromRow(row));

                return projects;
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return projects;
            }
        }

    }
}
