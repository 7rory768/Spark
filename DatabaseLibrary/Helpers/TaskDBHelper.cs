using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using System.Data;
using System.Net;
using Task = BusinessLibrary.Models.Task;

namespace DatabaseLibrary.Helpers
{
    public class TaskDBHelper : DBHelper
    {
        private static Task fromRow(DataRow row)
        {
            return new Task(
                            projectId: int.Parse(row["projectId"].ToString()),
                            listName: row["listName"].ToString(),
                            name: row["name"].ToString(),
                            description: row["description"].ToString(),
                            dateCreated: DateTime.Parse(row["dateCreated"].ToString()).ToLocalTime(),
                            priority: int.Parse(row["priority"].ToString()),
                            deadline: DateOnly.Parse(row["deadline"].ToString()),
                            completed: bool.Parse(row["completed"].ToString()),
                            completionPoints: int.Parse(row["completionPoints"].ToString())
                            );
        }

        public static Task? Add(int projectId, string listName, string name, string description, int priority, DateOnly? deadline, int completionPoints, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name");
                }
                else if (isNotAlphaNumeric(listName))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid list name");
                }
                else if (priority < 1)
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a positive non-zero priority");
                }
                else if (description.Contains('`'))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Found a prohibited character in the description");
                }

                // SQL Injection Projection
                description = MySQLEscape(description);

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "createList",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_listName", listName },
                            { "_name", name},
                            { "_description", description},
                            { "_priority", priority},
                            { "_deadline", deadline },
                            { "_completionPoints", completionPoints },
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

        public static Task? Update(int projectId, string listName, string name, string description, DateOnly? deadline, int completionPoints, bool completed, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name");
                }
                else if (isNotAlphaNumeric(listName))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid list name");
                }
                else if (description.Contains('`'))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Found a prohibited character in the description");
                }

                // SQL Injection Projection
                description = MySQLEscape(description);

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "updateTask",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_listName", listName },
                            { "_name", name},
                            { "_description", description},
                            { "_deadline", deadline },
                            { "_completionPoints", completionPoints },
                            { "_completed", completed},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                statusResponse = new StatusResponse("Updated task successfully");
                return fromRow(table.Rows[0]);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        public static Task? moveTask(int projectId, string listName, string name, int newPriority, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name");
                }
                else if (isNotAlphaNumeric(listName))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid list name");
                }
                else if (newPriority < 1)
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a positive non-zero position");
                }

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "moveTask",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_listName", listName },
                            { "_name", name },
                            { "_newPriority", newPriority},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                statusResponse = new StatusResponse("Moved task successfully");
                return fromRow(table.Rows[0]);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        public static List<Task> GetAll(int projectId, string listName, DbContext context, out StatusResponse statusResponse)
        {
            List<Task> objects = new List<Task>();

            try
            {
                if (isNotAlphaNumeric(listName))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid list name");
                }

                // Get from database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "getTasks",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_listName", listName},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                // Return value
                statusResponse = new StatusResponse("Got tasks successfully");

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

        public static bool Delete(int projectId, string listName, string name, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name.");
                }
                else if (isNotAlphaNumeric(name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid list name.");
                }

                // Add to database
                int rowsAffected = context.ExecuteNonQueryProcedure
                    (
                        procedure: "deleteTask",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_listName", listName },
                            { "_name", name },
                        },
                        message: out string message
                    );
                if (rowsAffected < 1)
                    throw new Exception(message);

                statusResponse = new StatusResponse("Deleted task successfully");
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
