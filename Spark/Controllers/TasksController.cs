﻿using System.Net;
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

        // Gets all tasks for a project list
        [HttpGet]
        [Route("{projectId}/{listName}")]
        public ResponseMessage Get(int projectId, string listName)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TaskHelper.GetTasks(getUser(), projectId, listName,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Moves a task
        [HttpPost]
        [Route("update")]
        public ResponseMessage Update([FromBody] JObject data)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TaskHelper.Update(getUser(), data,
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
        [Route("{projectId}/{listName}/{name}")]
        public ResponseMessage Delete(int projectId, string listName, string name)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TaskHelper.DeleteTask(getUser(), projectId, listName, name,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

    }
}