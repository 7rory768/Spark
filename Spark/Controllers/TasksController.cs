using System.Net;
using BusinessLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Spark.ContextHelpers;
using Spark.ControllerHelpers;
using Task = BusinessLibrary.Models.Task;

namespace Spark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : SparkControllerBase
    {
        public TasksController(IWebHostEnvironment hostingEnvironment, AppSettingsHelper appSettings, DatabaseContextHelper database) : base(hostingEnvironment, appSettings, database)
        {
            // Initalize values in SparkControllerBase
        }

        // Gets all tasks for a task list
        [HttpGet]
        [Route("{listId}")]
        public ResponseMessage Get(int listId)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TaskHelper.GetTasks(getUser(), listId,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Moves a task
        [HttpPost]
        [Route("update")]
        public ResponseMessage Update([FromBody] Task task)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TaskHelper.Update(getUser(), task,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Assigns a user to a task
        [HttpPost]
        [Route("assign")]
        public ResponseMessage AssignToTask([FromBody] JObject data)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TaskHelper.AssignToTask(data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Unassigns a user from a task
        [HttpPost]
        [Route("unassign")]
        public ResponseMessage UnassignFromTask([FromBody] JObject data)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TaskHelper.UnassignFromTask(data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Moves a task
        [HttpPost]
        [Route("move")]
        public ResponseMessage Move([FromBody] JObject data)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TaskHelper.MoveTask(getUser(), data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Creates a new task
        [HttpPost]
        [Route("create")]
        public ResponseMessage Create([FromBody] JObject data)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TaskHelper.Add(getUser(), data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Deletes a task
        [HttpDelete]
        [Route("{id}")]
        public ResponseMessage Delete(int id)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TaskHelper.DeleteTask(getUser(), id,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

    }
}
