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
    public class ProjectsController : SparkControllerBase
    {
        public ProjectsController(IWebHostEnvironment hostingEnvironment, AppSettingsHelper appSettings, DatabaseContextHelper database) : base(hostingEnvironment, appSettings, database)
        {
            // Initalize values in SparkControllerBase
        }

        // Gets all projects the user is participating in
        [HttpGet]
        [Route("viewProjects")]
        public ResponseMessage GetProjects()
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = ProjectHelper.GetParticipatingProjects(getUser(),
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Creates a new project
        [HttpPost]
        [Route("create")]
        public ResponseMessage Create([FromBody] JObject data)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = ProjectHelper.Add(getUser(), data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Gets a project
        [HttpGet]
        [Route("{id}")]
        public ResponseMessage Get(int id)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = ProjectHelper.Get(getUser(), id,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Gets all projects led by a team
        [HttpGet]
        [Route("team/{id}")]
        public ResponseMessage GetTeamProjects(int id)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = ProjectHelper.GetTeamProjects(id,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Modifies a projects information
        [HttpPost]
        [Route("update")]
        public ResponseMessage UpdateProject([FromBody] JObject data)
        {
            var response = ProjectHelper.Update(getUser(), data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Modifies a projects information
        [HttpPost]
        [Route("delete")]
        public ResponseMessage DeleteProject([FromBody] JObject data)
        {
            var response = ProjectHelper.Delete(getUser(), data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }
    }
}
