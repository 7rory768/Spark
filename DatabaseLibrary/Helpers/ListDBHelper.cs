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
    public class ListDBHelper : DBHelper
    {


        private static TaskList fromRow(DataRow row)
        {
            return new TaskList(
                            projectId: int.Parse(row["projectId"].ToString()),
                            name: row["name"].ToString(),
                            dateCreated: DateTime.Parse(row["dateCreated"].ToString()).ToLocalTime(),
                            position: int.Parse(row["color"].ToString())
                            );
        }

        public static TaskList? Add(int projectId, string name, int position, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name");
                }
                else if (position < 1)
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a positive non-zero position");
                }

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "createList",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_name", name },
                            { "_position", position},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                statusResponse = new StatusResponse("Created list successfully");
                return fromRow(table.Rows[0]);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        public static TaskList moveList(int projectId, string name, int newPosition, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name");
                }
                else if (newPosition < 1)
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a positive non-zero position");
                }

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "moveList",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_name", name },
                            { "_newPosition", newPosition},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                statusResponse = new StatusResponse("Moved list successfully");
                return fromRow(table.Rows[0]);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        public static List<TaskList> GetAll(int projectId, DbContext context, out StatusResponse statusResponse)
        {
            List<TaskList> objects = new List<TaskList>();

            try
            {
                // Get from database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "getLists",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                // Return value
                statusResponse = new StatusResponse("Got project lists successfully");

                foreach (DataRow row in table.Rows)
                    objects.Add(fromRow(row));

                return objects;
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return objects;
            }
        }

        public static bool Delete(int projectId, string name, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name.");
                }

                // Add to database
                int rowsAffected = context.ExecuteNonQueryProcedure
                    (
                        procedure: "deleteLabel",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_name", name },
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
