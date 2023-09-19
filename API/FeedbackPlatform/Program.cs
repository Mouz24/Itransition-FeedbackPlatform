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

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

var jwtSettings = configuration.GetSection("JwtSettings");
var secretKey = Environment.GetEnvironmentVariable("SECRET");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
    b => b.MigrationsAssembly("FeedbackPlatform")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    builder.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

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

app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<CommentHub>("/CommentHub");
app.MapHub<ArtworkHub>("/ArtworkHub");
app.MapHub<LikeHub>("/LikeHub");
app.MapHub<UserHub>("/UserHub");

app.UseEndpoints(configure: endpoints =>
{
    endpoints.MapControllers();
});

app.ConfigureExceptionHandler();

app.Run();
