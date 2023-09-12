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

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

var jwtSettings = configuration.GetSection("JwtSettings");
var secretKey = Environment.GetEnvironmentVariable("SECRET");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
    b => b.MigrationsAssembly("FeedbackPlatform")));

builder.Services.AddSignalR();

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

builder.Services.AddHttpClient();
builder.Services.AddElasticSearch(builder.Configuration);
builder.Services.AddScoped<IAuthenticationManager, Service.AuthenticationManager>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IServiceManager, Service.ServiceManager>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddIdentity<User, IdentityRole<Guid>>(o =>
{
    o.Password.RequireDigit = true;
    o.Password.RequireLowercase = true;
    o.Password.RequireUppercase = true;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequiredLength = 8;
    o.User.RequireUniqueEmail = true;
})
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

app.UseEndpoints(configure: endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<CommentHub>("/CommentHub");
    endpoints.MapHub<ArtworkHub>("/ArtworkHub");
    endpoints.MapHub<LikeHub>("/LikeHub");
});

app.ConfigureExceptionHandler();

app.Run();
