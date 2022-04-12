using Newtonsoft.Json.Serialization;
using MySqlConnector;
using Spark;
using Spark.ContextHelpers;
using DatabaseLibrary.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


// Setup app settings helper
IServiceCollection services = builder.Services.AddScoped<AppSettingsHelper>();

// Setup database context helper
builder.Services.AddScoped<DatabaseContextHelper>();
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddControllers();

// Json Seralizer

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new() { Title = "TodoApi", Version = "v1" });
//});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    TableUpdateManager.createTables(scope.ServiceProvider.GetService<DatabaseContextHelper>().DbContext);
    TableUpdateManager.createProcedures(scope.ServiceProvider.GetService<DatabaseContextHelper>().DbContext);
    TableUpdateManager.updateTables(scope.ServiceProvider.GetService<DatabaseContextHelper>().DbContext);
}

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
