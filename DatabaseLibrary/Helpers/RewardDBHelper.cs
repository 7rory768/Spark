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
    public class RewardDBHelper : DBHelper
    {
        private static Reward fromRow(DataRow row)
        {
            return new Reward(
                            //string username, int numPoints, int teamId, int projectId, DateTime dateGiven

                            //id: int.Parse(row["id"].ToString()),
                            username: row["username"].ToString(),
                            numPoints: int.Parse(row["numPoints"].ToString()),
                            teamId: int.Parse(row["teamId"].ToString()),
                            projectId: int.Parse(row["projectId"].ToString()),
                            dateGiven: DateTime.Parse(row["dateGiven"].ToString()).ToLocalTime()
                            );
        }

        public static Reward? Add(string username, int numPoints, int teamId, int projectId, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
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
                    statusResponse = new StatusResponse("User does not have permissions to reward points!");
                    return null;
                }

                // If manager
                else
                {
                    // Add to database if Manager of team
                    DataTable table = context.ExecuteDataQueryProcedure
                        (
                            procedure: "createReward",
                            parameters: new Dictionary<string, object>()
                            {
                            { "_username", username },
                            { "_numPoints", numPoints },
                            { "_teamId", teamId},
                            { "_projectId", projectId},
                            },
                            message: out string message
                        );
                    if (table == null)
                        throw new Exception(message);

                    DataRow row = table.Rows[0];

                    // Return value
                    if (string.IsNullOrEmpty(row["username"].ToString()))
                    {
                        statusResponse = new StatusResponse("Failed to add reward");
                        return null;
                    }
                    else
                    {
                        statusResponse = new StatusResponse("Saved reward successfully");

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

        public static int getTotal(string username, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "getTotalPointsForUser",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_username", username },
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                DataRow row = table.Rows[0];

                statusResponse = new StatusResponse("Got points for user!");
                return int.Parse(row["totalPoints"].ToString());
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return -1;
            }
        }

        public static int getTotalInTeam(string username, int teamId, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "getTotalPointsForUserInTeam",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_username", username },
                            { "_teamId", teamId },
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                DataRow row = table.Rows[0];

                statusResponse = new StatusResponse("Got points for user in team!");
                if (string.IsNullOrEmpty(row["totalPoints"].ToString())) {
                    return 0;
                }
                else return int.Parse(row["totalPoints"].ToString());
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return -1;
            }
        }

    }
}
