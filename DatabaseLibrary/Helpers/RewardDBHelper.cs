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

        //// Get a user's total points for a team
        //public static int GetTotal(string username, int id, DbContext context, out StatusResponse statusResponse)
        //{
        //    int totalPoints;

        //    try
        //    {
        //        // Get from database
        //        DataTable table = context.ExecuteDataQueryProcedure
        //            (
        //                procedure: "getUserTotalPoints",
        //                parameters: new Dictionary<string, object>()
        //                {
        //                    { "_username", username },
        //                    { "_teamId", id },
        //                },
        //                message: out string message
        //            );
        //        if (table == null)
        //            throw new Exception(message);

        //        // Return value
        //        statusResponse = new StatusResponse("Got users points successfully");

        //        totalPoints = table.Rows[0].Field<int>(0);
        //        if (totalPoints != null)
        //        {
        //            return totalPoints;
        //        }

        //    }
        //    catch (Exception exception)
        //    {
        //        statusResponse = new StatusResponse(exception);
        //        return 0;
        //    }
        //}

    }
}
