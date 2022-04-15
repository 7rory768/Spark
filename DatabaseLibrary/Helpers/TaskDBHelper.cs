﻿using BusinessLibrary.Models;
using DatabaseLibrary.Core;
using System.Data;
using System.Net;
using Task = BusinessLibrary.Models.Task;

namespace DatabaseLibrary.Helpers
{
    public class TaskDBHelper : DBHelper
    {
        private static Task fromRow(DataRow row, DbContext context)
        {
            Task task = new Task(
                            id: int.Parse(row["id"].ToString()),
                            projectId: int.Parse(row["projectId"].ToString()),
                            listId: int.Parse(row["listId"].ToString()),
                            name: row["name"].ToString(),
                            description: row["description"].ToString(),
                            dateCreated: DateTime.Parse(row["dateCreated"].ToString()).ToLocalTime(),
                            priority: int.Parse(row["priority"].ToString()),
                            deadline: string.IsNullOrEmpty(row["deadline"].ToString()) ? null : DateOnly.Parse(row["deadline"].ToString().Split(" ")[0]),
                            completed: bool.Parse(row["completed"].ToString()),
                            completionPoints: int.Parse(row["completionPoints"].ToString())
                            );

            task.assignedUsers = getAssignedToTask(task, context);
            return task;
        }
        private static List<User> getAssignedToTask(Task task, DbContext context)
        {
            return getAssignedToTask(task.id, context);
        }

        private static List<User> getAssignedToTask(int taskId, DbContext context)
        {
            List<User> assignedToTask = new List<User>();

            // Get from database
            DataTable table = context.ExecuteDataQueryProcedure
                (
                    procedure: "getAssignedToTask",
                    parameters: new Dictionary<string, object>()
                    {
                            { "_taskId", taskId },
                    },
                    message: out string message
                );
            if (table == null)
                throw new Exception(message);

            foreach (DataRow row in table.Rows)
                assignedToTask.Add(UserDBHelper.fromRow(row));

            return assignedToTask;
        }

        public static Task? Add(int projectId, int listId, string name, string description, DateOnly? deadline, int completionPoints, List<string> assignedUsers, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(true, name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name");
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
                        procedure: "createTask",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_projectId", projectId },
                            { "_listId", listId },
                            { "_name", name},
                            { "_description", description},
                            { "_deadline", deadline },
                            { "_completionPoints", completionPoints },
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                int taskId = int.Parse(table.Rows[0]["id"].ToString());

                foreach (string username in assignedUsers)
                    assignToTask(taskId, username, context, out statusResponse);

                statusResponse = new StatusResponse("Created task successfully");
                return fromRow(table.Rows[0], context);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        public static Task? Update(int taskId, string name, string description, DateOnly? deadline, int completionPoints, bool completed, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(true, name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name");
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
                            { "_id", taskId},
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
                return fromRow(table.Rows[0], context);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        public static Task? assignToTask(int taskId, string username, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(false, username))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid username");
                }

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "assignToTask",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_taskId", taskId },
                            { "_username", username},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                statusResponse = new StatusResponse(string.Format("Assigned {0} to task {1} successfully", username, taskId));
                return fromRow(table.Rows[0], context);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        public static Task? unassignFromTask(int taskId, string username, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(false, username))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid username");
                }

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "unassignFromTask",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_taskId", taskId },
                            { "_username", username},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                statusResponse = new StatusResponse(string.Format("Unassigned {0} from task {1} successfully", username, taskId));

                return fromRow(table.Rows[0], context);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        public static Task? moveTask(int taskId, int listId, int newPriority, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (newPriority < 0)
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a positive position");
                }

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "moveTask",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_listId", listId},
                            { "_taskId", taskId },
                            { "_newPriority", newPriority},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                statusResponse = new StatusResponse("Moved task successfully");
                return fromRow(table.Rows[0], context);
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        public static List<Task> GetAll(int listId, DbContext context, out StatusResponse statusResponse)
        {
            List<Task> objects = new List<Task>();

            try
            {
                // Get from database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "getTasks",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_listId", listId},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                // Return value
                statusResponse = new StatusResponse("Got tasks successfully");

                foreach (DataRow row in table.Rows)
                    objects.Add(fromRow(row, context));

                return objects;
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return objects;
            }
        }

        public static bool Delete(int taskId, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                // Remove from database
                int rowsAffected = context.ExecuteNonQueryProcedure
                    (
                        procedure: "deleteTask",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_id", taskId },
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
