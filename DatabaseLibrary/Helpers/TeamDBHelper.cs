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
    public class TeamDBHelper : DBHelper
    {
        private static Team fromRow(DataRow row)
        {
            return new Team(
                            id: int.Parse(row["id"].ToString()),
                            name: row["name"].ToString(),
                            mgrUsername: row["mgrUsername"].ToString()
                            );
        }


        /// <summary>
        /// Get all team the current user can see
        /// </summary>
        public static List<Team> GetAll(string username, DbContext context, out StatusResponse statusResponse)
        {
            List<Team> teams = new List<Team>();

            try
            {
                if (isNotAlphaNumeric(username.Trim()))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid username.");
                }

                // Get from database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "getParticipatingTeams",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_username", username },
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                // Return value
                statusResponse = new StatusResponse("Got participating teams successfully");

                foreach (DataRow row in table.Rows)
                    teams.Add(fromRow(row));

                return teams;
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return teams;
            }
        }

        // Adding team to the database
        public static Team? Add(string username, int id, string name, string mgrUsername, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(name?.Trim()))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid team name.");
                }

                bool success = false;

                // Add to database if Manager of team
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "createTeam",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_id", id },
                            { "_name", name },
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
                    statusResponse = new StatusResponse("Failed to create team");
                    return null;
                }
                else
                {
                    statusResponse = new StatusResponse("Created team successfully");

                    return fromRow(row);
                }
            }
            
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

    }
}
