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
    public class ProjectDBHelper : DBHelper
    {
        private static Project fromRow(DataRow row)
        {
            return new Project(
                            id: int.Parse(row["id"].ToString()),
                            teamId: int.Parse(row["teamId"].ToString()),
                            name: row["name"].ToString(),
                            budget: int.Parse(row["budget"].ToString()),
                            dateCreated: DateTime.Parse(row["dateCreated"].ToString()).ToLocalTime()
                            );
        }

        public static Project? Add(string username, int teamId, string name, int budget, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(name?.Trim()))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid project name.");
                }

                bool success = false;

                // TODO: Make sure user is manager of the team first
                // Rachel Added
                // Get manager data from database
                DataTable tableManager = context.ExecuteDataQueryProcedure
                    (
                        procedure: "getTeamManager",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_teamId", teamId },
                            { "_user", username },
                        },
                        message: out string outMessage
                    );
                if (tableManager == null) // if user is not manager of the team
                {
                    statusResponse = new StatusResponse("User does not have permissions to create a project!");
                    return null;
                }

                // If manager
                else
                {
                    // Add to database if Manager of team
                    DataTable table = context.ExecuteDataQueryProcedure
                        (
                            procedure: "createProject",
                            parameters: new Dictionary<string, object>()
                            {
                            { "_teamId", teamId },
                            { "_name", name },
                            { "_budget", budget},
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
                if (isNotAlphaNumeric(username.Trim()))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid username.");
                }

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

        public static Project? Get(string username, int projectId, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(username.Trim()))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid username.");
                }

                // Get from database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "getProject",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_username", username },
                            { "_projectId", projectId},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                // Return value
                statusResponse = new StatusResponse("Got project successfully");

                return fromRow(table.Rows[0]);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

    }
}
