using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatabaseLibrary.Helpers
{
    public class LabelDBHelper : DBHelper
    {


        private static Label fromRow(DataRow row)
        {
            return new Label(
                            id: int.Parse(row["id"].ToString()),
                            projectId: int.Parse(row["projectId"].ToString()),
                            name: row["name"].ToString(),
                            color: row["color"].ToString()
                            );
        }

        public static Label? Add(int projectId, string name, string color, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(true, name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid username.");
                }
                else if (!isValidColor(color))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid color in HEX format (Ex. #ffffff).");
                }

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "createLabel",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_name", name },
                            { "_color", color},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                statusResponse = new StatusResponse("Created label successfully");
                return fromRow(table.Rows[0]);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        /// <summary>
        /// Get all labels for a project
        /// </summary>
        public static List<Label> GetAll(int projectId, DbContext context, out StatusResponse statusResponse)
        {
            List<Label> labels = new List<Label>();

            try
            {
                // Get from database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "getLabels",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                // Return value
                statusResponse = new StatusResponse("Got project labels successfully");

                foreach (DataRow row in table.Rows)
                    labels.Add(fromRow(row));

                return labels;
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return labels;
            }
        }

        public static bool Delete(int id, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                // Remove from database
                int rowsAffected = context.ExecuteNonQueryProcedure
                    (
                        procedure: "deleteLabel",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_id", id },
                        },
                        message: out string message
                    );
                if (rowsAffected < 1)
                    throw new Exception(message);

                statusResponse = new StatusResponse("Deleted label successfully");
                return true;
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
            }

            return false;
        }

    }
}
