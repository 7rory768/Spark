using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BusinessLibrary.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Spark.ContextHelpers;
using Spark.ControllerHelpers;
using Newtonsoft.Json.Linq;

namespace Spark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListsController : SparkControllerBase
    {
        public ListsController(IWebHostEnvironment hostingEnvironment, AppSettingsHelper appSettings, DatabaseContextHelper database) : base(hostingEnvironment, appSettings, database)
        {
            // Initalize values in SparkControllerBase
        }

        // Gets all lists for a project
        [HttpGet]
        [Route("{projectId}")]
        public ResponseMessage Get(int projectId)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = ListHelper.GetLists(getUser(), projectId,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Gets all lists for a project
        [HttpPost]
        [Route("update")]
        public ResponseMessage Update([FromBody] TaskList taskList)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = ListHelper.Update(getUser(), taskList,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Moves a list
        [HttpPost]
        [Route("move")]
        public ResponseMessage Move([FromBody] JObject data)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = ListHelper.MoveList(getUser(), data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Creates a new list
        [HttpPost]
        [Route("create")]
        public ResponseMessage Create([FromBody] JObject data)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = ListHelper.Add(getUser(), data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Deletes a list
        [HttpDelete]
        [Route("{id}")]
        public ResponseMessage Delete(int id)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = ListHelper.DeleteList(getUser(), id,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

    }
}
