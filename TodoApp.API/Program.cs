using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TodoApp.API.Infrastructure.ExceptionHandling;
using TodoApp.BLL.Interfaces;
using TodoApp.BLL.Options;
using TodoApp.BLL.Pipelines;
using TodoApp.BLL.Pipelines.Steps;
using TodoApp.BLL.Services;
using TodoApp.BLL.Validators.Tasks;
using TodoApp.DAL;
using TodoApp.DAL.Repositories;
using TodoApp.Domain.Contracts.Interfaces.Repositories;
using TodoApp.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddDbContext<TodoAppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(5);
        options.Lockout.AllowedForNewUsers = true;

        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<TodoAppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services
    .AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.Key) &&
            options.Key.Length >= 32,
        "JWT key must contain at least 32 characters.")
    .ValidateOnStart();

var jwtOptions =
    builder.Configuration
        .GetSection(JwtOptions.SectionName)
        .Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),

            ValidateLifetime = true,

            ClockSkew = TimeSpan.FromSeconds(30),

            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<ITodoTaskService, TodoTaskService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateTodoTaskRequestValidator>();

builder.Services.AddScoped<ITodoTaskQueryPipelineStep, TodoTaskFilteringPipelineStep>();
builder.Services.AddScoped<ITodoTaskQueryPipelineStep, TodoTaskSortingPipelineStep>();
builder.Services.AddScoped<ITodoTaskQueryPipelineStep, TodoTaskPaginationPipelineStep>();

builder.Services.AddScoped<TodoTaskQueryPipeline>();

builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateTodoTaskRequestValidator>();
builder.Services.AddScoped<IValidationService, ValidationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TodoApp API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "Enter JWT token in the following format: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Angular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();