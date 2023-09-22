using Entities;
using Entities.Models;
using FeedbackPlatform.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FeedbackPlatform.Hubs;
using Service;
using Service.IService;
using System.Text;
using Contracts;
using Repository;
using FeedbackPlatform.Extensions.Validator;
using FeedbackPlatform.Extensions.ModelsManipulationLogics;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Text.Json;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

var jwtSettings = configuration.GetSection("JwtSettings");
var secretKey = Environment.GetEnvironmentVariable("SECRET");

Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticSearch:Uri"]))
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"feedbackfusion-logs-{builder.Environment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow.AddHours(3):yyyy-MM}"
        })
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
    b => b.MigrationsAssembly("FeedbackPlatform")));

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    builder =>
    {
        builder.AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials();
    }));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddElasticSearch(builder.Configuration);
builder.Services.AddScoped<IAuthenticationManager, Service.AuthenticationManager>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IServiceManager, Service.ServiceManager>();
builder.Services.AddTransient<IPasswordValidator<IdentityUser>, EnglishLettersOnlyPasswordValidator<IdentityUser>>();
builder.Services.AddTransient<TagExtension>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddIdentity<User, IdentityRole<Guid>>(o =>
{
    o.User.RequireUniqueEmail = true;
    o.Password.RequireDigit = true;
    o.Password.RequireLowercase = true;
    o.Password.RequireUppercase = true;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequiredLength = 8;
})
.AddPasswordValidator<EnglishLettersOnlyPasswordValidator<User>>()
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<ApplicationContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
        ValidAudience = jwtSettings.GetSection("validAudience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(configure: endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<CommentHub>("/CommentHub");
    endpoints.MapHub<ArtworkHub>("/ArtworkHub");
    endpoints.MapHub<LikeHub>("/LikeHub");
    endpoints.MapHub<UserHub>("/UserHub");
});

app.ConfigureExceptionHandler();

app.Run();
