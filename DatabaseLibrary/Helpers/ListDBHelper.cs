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
                            id: int.Parse(row["id"].ToString()),
                            projectId: int.Parse(row["projectId"].ToString()),
                            name: row["name"].ToString(),
                            dateCreated: DateTime.Parse(row["dateCreated"].ToString()).ToLocalTime(),
                            position: int.Parse(row["position"].ToString())
                            );
        }

        public static TaskList? Add(int projectId, string name, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(true, name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name");
                }

                name = MySQLEscape(name);

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "createList",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_name", name }
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

        public static TaskList? Update(TaskList taskList, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(true, taskList.name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name");
                }

                taskList.name = MySQLEscape(taskList.name);

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "updateList",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_id", taskList.id },
                            { "_name", taskList.name }
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                statusResponse = new StatusResponse("Updated list successfully");
                return fromRow(table.Rows[0]);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        public static TaskList moveList(int projectId, int listId, int newPosition, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (newPosition < 0)
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a positive position");
                }

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "moveList",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_listId", listId},
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

        public static bool Delete(int listId, int projectId, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                // Remove from database
                int rowsAffected = context.ExecuteNonQueryProcedure
                    (
                        procedure: "deleteList",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_listId", listId},
                            { "_projectId", projectId},
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
