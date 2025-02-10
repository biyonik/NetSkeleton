using System.Reflection;
using API.Filters;
using API.Middleware;
using Application.Cache.Extensions;
using Application.Cache.Settings;
using Application.Cache.Strategies;
using Application.Common.Extensions;
using Asp.Versioning;
using Infrastructure.DependencyInjection;
using Infrastructure.Email.Extensions;
using Infrastructure.FileStorage.Extensions;
using Infrastructure.FileStorage.Settings;
using Infrastructure.Identity.Extensions;
using Infrastructure.Logging.Extensions;
using Microsoft.OpenApi.Models;
using Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddCQRSServices(Assembly.GetAssembly(typeof(CQRSExtensions))!);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddAuthorizationServices();
builder.Services.AddAuthorizationInfrastructure();
builder.Services.AddApplicationServices(Assembly.GetAssembly(typeof(ServiceExtensions))!);
builder.Services.AddApplicationLayer();
builder.Services.AddCacheServices(options =>
{
    options.Strategy = CacheStrategy.Memory;
    options.MemoryCache = new MemoryCacheSettings
    {
        SizeLimit = 1024
    };
});

builder.Services.AddEmailServices(options =>
{
    options.DefaultFromName = "My App";
    options.DefaultFromEmail = "app@netskeleton.com";
});

builder.Services.AddStorageServices(options =>
{
    options.Strategy = StorageStrategy.Local;
    if (options.Local != null)
    {
        options.Local.BasePath = Path.Combine(builder.Environment.ContentRootPath, "Storage");
        options.Local.BaseUrl = "https://localhost:5001/storage";
    }
});

builder.Services.AddCustomLogging(builder.Configuration);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    // JWT authentication için
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Permission filter'ı ekle
    c.OperationFilter<PermissionOperationFilter>();
    
    // XML comments için
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<DynamicAuthorizationMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
