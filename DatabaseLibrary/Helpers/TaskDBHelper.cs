using BusinessLibrary.Models;
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
            task.checklists = getChecklists(task, context);
            return task;
        }

        private static Checklist fromRowChecklist(DataRow row, DbContext context, bool loadItems = true)
        {
            Checklist checklist = new Checklist(
                            id: int.Parse(row["id"].ToString()),
                            taskId: int.Parse(row["taskId"].ToString()),
                            title: row["title"].ToString()
                            );

            if (loadItems) checklist.items = getChecklistItems(checklist, context);
            return checklist;
        }

        private static ChecklistItem fromRowChecklistItem(DataRow row)
        {
            ChecklistItem item = new ChecklistItem(
                            id: int.Parse(row["id"].ToString()),
                            checklistId: int.Parse(row["checklistId"].ToString()),
                            description: row["description"].ToString(),
                            completed: bool.Parse(row["completed"].ToString())
                            );

            return item;
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

        private static List<Checklist> getChecklists(Task task, DbContext context)
        {
            List<Checklist> checklists = new List<Checklist>();

            // Get from database
            DataTable table = context.ExecuteDataQueryProcedure
                (
                    procedure: "getChecklists",
                    parameters: new Dictionary<string, object>()
                    {
                            { "_taskId", task.id },
                    },
                    message: out string message
                );
            if (table == null)
                throw new Exception(message);

            foreach (DataRow row in table.Rows)
                checklists.Add(fromRowChecklist(row, context));

            return checklists;
        }

        private static List<ChecklistItem> getChecklistItems(Checklist checklist, DbContext context)
        {
            List<ChecklistItem> items = new List<ChecklistItem>();

            // Get from database
            DataTable table = context.ExecuteDataQueryProcedure
                (
                    procedure: "getChecklistItems",
                    parameters: new Dictionary<string, object>()
                    {
                            { "_checklistId", checklist.id },
                    },
                    message: out string message
                );
            if (table == null)
                throw new Exception(message);

            foreach (DataRow row in table.Rows)
                items.Add(fromRowChecklistItem(row));

            return items;
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

        public static Task? Update(User user, Task task, DbContext context, out StatusResponse statusResponse)
        {
            try
            {
                if (isNotAlphaNumeric(true, task.name))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Please provide a valid name");
                }
                else if (task.description.Contains('`'))
                {
                    throw new StatusException(HttpStatusCode.BadRequest, "Found a prohibited character in the description");
                }

                // SQL Injection Projection
                task.description = MySQLEscape(task.description);

                // Add to database
                DataTable table = context.ExecuteDataQueryProcedure
                    (
                        procedure: "updateTask",
                        parameters: new Dictionary<string, object>()
                        {
                            { "_id", task.id},
                            { "_name", task.name},
                            { "_description", task.description},
                            { "_deadline", task.deadline },
                            { "_completionPoints", task.completionPoints },
                            { "_completed", task.completed},
                        },
                        message: out string message
                    );
                if (table == null)
                    throw new Exception(message);

                Task newTask = fromRow(table.Rows[0], context);

                // COMPARE CHANGES

                for (int index = newTask.checklists.Count - 1; index >= 0; index--)
                {
                    Checklist oldChecklist = newTask.checklists[index];
                    bool existsInUpdate = false;

                    foreach (Checklist updateChecklist in task.checklists)
                    {
                        if (updateChecklist.id == oldChecklist.id)
                        {
                            existsInUpdate = true;

                            // UPDATE CHECKLIST
                            if (updateChecklist.title != oldChecklist.title)
                            {
                                TaskDBHelper.updateChecklist(updateChecklist, context);
                            }

                            updateChecklistItems(updateChecklist, oldChecklist, context);

                            break;
                        }
                    }

                    if (!existsInUpdate)
                    {
                        // DELETE CHECKLIST
                        if (deleteChecklist(oldChecklist, context))
                        {
                            newTask.checklists.RemoveAt(index);
                        }
                    }
                }

                foreach (Checklist updateChecklist in task.checklists)
                {
                    bool newChecklist = true;

                    foreach (Checklist oldChecklist in newTask.checklists)
                    {
                        if (updateChecklist.id == oldChecklist.id)
                        {
                            newChecklist = false;
                            break;
                        }
                    }

                    // CREATE NEW CHECKLIST
                    if (newChecklist)
                    {
                        Checklist checklist = createChecklist(updateChecklist, context);
                        if (checklist != null) newTask.checklists.Add(checklist);
                    }
                }

                statusResponse = new StatusResponse("Updated task successfully");

                return newTask;
            }
            catch (Exception exception)
            {
                statusResponse = new StatusResponse(exception);
                return null;
            }
        }

        private static Checklist updateChecklistItems(Checklist updatedChecklist, Checklist oldChecklist, DbContext context)
        {
            // COMPARE CHANGES

            for (int index = oldChecklist.items.Count - 1; index >= 0; index--)
            {
                ChecklistItem oldItem = oldChecklist.items[index];
                bool existsInUpdate = false;

                foreach (ChecklistItem updateItem in updatedChecklist.items)
                {
                    if (updateItem.id == oldItem.id)
                    {
                        existsInUpdate = true;

                        // UPDATE CHECKLIST ITEM
                        if (updateItem.description != oldItem.description || updateItem.completed != oldItem.completed)
                        {
                            TaskDBHelper.updateChecklistItem(updateItem, context);
                        }

                        break;
                    }
                }

                if (!existsInUpdate)
                {
                    // DELETE CHECKLIST  ITEM
                    if (deleteChecklistItem(oldItem, context))
                    {
                        oldChecklist.items.RemoveAt(index);
                    }
                }
            }

            foreach (ChecklistItem updateItem in updatedChecklist.items)
            {
                bool newItem = true;

                foreach (ChecklistItem oldItem in oldChecklist.items)
                {
                    if (updateItem.id == oldItem.id)
                    {
                        newItem = false;
                        break;
                    }
                }

                // CREATE NEW CHECKLIST ITEM
                if (newItem)
                {
                    ChecklistItem item = createChecklistItem(updateItem, context);
                    if (item != null) oldChecklist.items.Add(item);
                }
            }

            return oldChecklist;
        }

        private static Checklist createChecklist(Checklist updateChecklist, DbContext context)
        {
            DataTable table = context.ExecuteDataQueryProcedure
        (
            procedure: "createChecklist",
            parameters: new Dictionary<string, object>()
            {
                            { "_taskId", updateChecklist.taskId},
                            { "_title", updateChecklist.title}
            },
            message: out string message
        );
            if (table == null)
                throw new Exception(message);

            Checklist checklist = fromRowChecklist(table.Rows[0], context, false);

            if (updateChecklist.items != null)
            {
                foreach (ChecklistItem item in updateChecklist.items)
                {
                    checklist.items.Add(createChecklistItem(item, context));
                }
            }

            return checklist;
        }

        private static ChecklistItem? createChecklistItem(ChecklistItem item, DbContext context)
        {
            DataTable table = context.ExecuteDataQueryProcedure
                        (
                            procedure: "createChecklistItem",
                            parameters: new Dictionary<string, object>()
                            {
                            { "_checklistId", item.checklistId},
                            { "_description", item.description},
                            { "_completed", item.completed},
                            },
                            message: out string message
                        );
            if (table == null)
                throw new Exception(message);

            return fromRowChecklistItem(table.Rows[0]);
        }

        private static void updateChecklist(Checklist updateChecklist, DbContext context)
        {
            DataTable table = context.ExecuteDataQueryProcedure
        (
            procedure: "updateChecklist",
            parameters: new Dictionary<string, object>()
            {
                            { "_id", updateChecklist.id},
                            { "_title", updateChecklist.title}
            },
            message: out string message
        );
            if (table == null)
                throw new Exception(message);
        }

        private static void updateChecklistItem(ChecklistItem updatedItem, DbContext context)
        {
            DataTable table = context.ExecuteDataQueryProcedure
        (
            procedure: "updateChecklistItem",
            parameters: new Dictionary<string, object>()
            {
                            { "_id", updatedItem.id},
                            { "_description", updatedItem.description},
                            { "_completed", updatedItem.completed}
            },
            message: out string message
        );
            if (table == null)
                throw new Exception(message);
        }

        private static bool deleteChecklist(Checklist checklist, DbContext context)
        {
            DataTable table = context.ExecuteDataQueryProcedure
        (
            procedure: "deleteChecklist",
            parameters: new Dictionary<string, object>()
            {
                    { "_checklistId", checklist.id},
            },
            message: out string message
        );
            if (table == null)
                throw new Exception(message);

            return true;
        }

        private static bool deleteChecklistItem(ChecklistItem item, DbContext context)
        {
            DataTable table = context.ExecuteDataQueryProcedure
        (
            procedure: "deleteChecklistItem",
            parameters: new Dictionary<string, object>()
            {
                    { "_itemId", item.id},
            },
            message: out string message
        );
            if (table == null)
                throw new Exception(message);

            return true;
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
