using System.Text.Json;
using System.Text.Json.Serialization;
using CalendarManagement.Api.Data;
using CalendarManagement.Api.Exceptions;
using CalendarManagement.Api.Interfaces;
using CalendarManagement.Api.Middleware;
using CalendarManagement.Api.Models;
using CalendarManagement.Api.Repositories;
using CalendarManagement.Api.Services;
using CalendarManagement.Api.Services.Authentication;
using CalendarManagement.Api.Services.Background;
using CalendarManagement.Api.Services.Email;
using CalendarManagement.Api.Modules.AI.Interfaces;
using CalendarManagement.Api.Modules.AI.Services;
using CalendarManagement.Api.Modules.AI.Tools;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.OpenApiInfo
        {
            Title = "Calendar Management API",
            Version = "v1",
            Description = "REST API for the Calendar Management System. Provides endpoints for authentication, dashboard summaries, calendar events, meetings, participants, notifications, users, settings and the AI assistant.",
            Contact = new Microsoft.OpenApi.OpenApiContact
            {
                Name = "Calendar Management System",
                Url = new Uri("https://github.com/bibekpandey0521/CalendarManagementSystem")
            },
            License = new Microsoft.OpenApi.OpenApiLicense
            {
                Name = "MIT"
            }
        });

    // Include XML documentation comments generated from the project.
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    // Keep operations grouped by controller and ordered consistently.
    options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Default" });
    options.OrderActionsBy(api => $"{api.ActionDescriptor.RouteValues["controller"]}_{api.HttpMethod}_{api.RelativePath}");
});

builder.Services.AddHealthChecks();
builder.Services.AddHttpClient("OpenAI");
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IMeetingParticipantRepository, MeetingParticipantRepository>();
builder.Services.AddScoped<IEmailNotificationLogRepository, EmailNotificationLogRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();

// AI Assistant Module
builder.Services.AddScoped<IAIProvider, OpenAIProvider>();
builder.Services.AddScoped<IAITool, GetUpcomingEventsTool>();
builder.Services.AddScoped<IAITool, GetPastEventsTool>();
builder.Services.AddScoped<IAITool, GetDashboardSummaryTool>();
builder.Services.AddScoped<IAITool, GetEventByIdTool>();
builder.Services.AddScoped<IAITool, CreateEventTool>();
builder.Services.AddScoped<IAITool, DeleteEventTool>();
builder.Services.AddScoped<IAIAssistantService, AIAssistantService>();
builder.Services.AddScoped<MeetingParticipantRepository>();
builder.Services.AddScoped<EmailNotificationLogRepository>();
builder.Services.AddHostedService<EmailNotificationWorker>();
builder.Services.AddScoped<CalendarManagement.Api.Middleware.ExceptionHandlingMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Calendar Management API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Calendar Management API - Swagger UI";
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.DisplayRequestDuration();
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Seed the database if empty (development only)
if (app.Environment.IsDevelopment())
{
    await SeedIfDatabaseEmptyAsync(app.Services);
}

app.Run();

static async Task SeedIfDatabaseEmptyAsync(IServiceProvider services)
{
    await using var scope = services.CreateAsyncScope();
    var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
    var exists = await eventRepository.ExistsAsync();

    if (!exists)
    {
        var now = DateTime.UtcNow;

        static DateTime LocalAt(int dayOffset, int hour, int minute = 0)
        {
            var local = DateTime.Today.AddDays(dayOffset).AddHours(hour).AddMinutes(minute);
            return DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime();
        }

        var samples = new List<CalendarEvent>
        {
            new()
            {
                Title = "Team stand-up",
                Description = "Daily sync with the engineering team. Share progress and blockers.",
                StartTime = LocalAt(0, 9),
                EndTime = LocalAt(0, 9, 30),
                Location = "Meet - Room A",
                Type = EventType.Meeting,
                EventTypeId = (int)EventType.Meeting,
                Category = EventCategory.Work,
                EventCategoryId = (int)EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = 10,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Title = "Design review - Calendar 2.0",
                Description = "Walk through the new month and week views with the product team.",
                StartTime = LocalAt(0, 11),
                EndTime = LocalAt(0, 12, 30),
                Location = "Conference Room B",
                Type = EventType.Meeting,
                EventTypeId = (int)EventType.Meeting,
                Category = EventCategory.Work,
                EventCategoryId = (int)EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = 30,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Title = "Focus time",
                Description = "Deep work block - notifications off.",
                StartTime = LocalAt(0, 14),
                EndTime = LocalAt(0, 16),
                Location = null,
                Type = EventType.Event,
                EventTypeId = (int)EventType.Event,
                Category = EventCategory.Work,
                EventCategoryId = (int)EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = null,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Title = "Client kickoff call",
                Description = "Kick off the Q4 engagement and align on success criteria.",
                StartTime = LocalAt(1, 10),
                EndTime = LocalAt(1, 11),
                Location = "Zoom",
                Type = EventType.Meeting,
                EventTypeId = (int)EventType.Meeting,
                Category = EventCategory.Work,
                EventCategoryId = (int)EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = 60,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Title = "Sprint planning",
                Description = "Plan the next two-week sprint and estimate the backlog.",
                StartTime = LocalAt(2, 13),
                EndTime = LocalAt(2, 15),
                Location = "Conference Room A",
                Type = EventType.Meeting,
                EventTypeId = (int)EventType.Meeting,
                Category = EventCategory.Work,
                EventCategoryId = (int)EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = 15,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Title = "One-on-one with Priya",
                Description = "Monthly career and growth check-in.",
                StartTime = LocalAt(3, 15),
                EndTime = LocalAt(3, 15, 30),
                Location = "Cafe on the second floor",
                Type = EventType.Meeting,
                EventTypeId = (int)EventType.Meeting,
                Category = EventCategory.Personal,
                EventCategoryId = (int)EventCategory.Personal,
                Status = EventStatus.Confirmed,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = 5,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Title = "Product roadmap workshop",
                Description = "Cross-functional workshop to prioritise the H1 roadmap.",
                StartTime = LocalAt(5, 10),
                EndTime = LocalAt(5, 12),
                Location = "Innovation Lab",
                Type = EventType.Meeting,
                EventTypeId = (int)EventType.Meeting,
                Category = EventCategory.Work,
                EventCategoryId = (int)EventCategory.Work,
                Status = EventStatus.Tentative,
                EventStatusId = (int)EventStatus.Tentative,
                ReminderMinutesBefore = 60,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Title = "Aarav's birthday",
                Description = "Bring the cake!",
                StartTime = LocalAt(7, 18),
                EndTime = LocalAt(7, 21),
                Location = "Family home",
                Type = EventType.Event,
                EventTypeId = (int)EventType.Event,
                Category = EventCategory.Birthday,
                EventCategoryId = (int)EventCategory.Birthday,
                Status = EventStatus.Confirmed,
                EventStatusId = (int)EventStatus.Confirmed,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Title = "Quarterly all-hands",
                Description = "Company update, Q3 results and Q4 priorities.",
                StartTime = LocalAt(10, 16),
                EndTime = LocalAt(10, 17),
                Location = "Auditorium",
                Type = EventType.Meeting,
                EventTypeId = (int)EventType.Meeting,
                Category = EventCategory.Work,
                EventCategoryId = (int)EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = 120,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Title = "Gym session",
                Description = "Leg day.",
                StartTime = LocalAt(-1, 7),
                EndTime = LocalAt(-1, 8),
                Location = "Neighbourhood gym",
                Type = EventType.Event,
                EventTypeId = (int)EventType.Event,
                Category = EventCategory.Personal,
                EventCategoryId = (int)EventCategory.Personal,
                Status = EventStatus.Confirmed,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = null,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Title = "Sprint retrospective",
                Description = "What went well, what did not, and what we will change.",
                StartTime = LocalAt(-2, 15),
                EndTime = LocalAt(-2, 16),
                Location = "Meet - Room C",
                Type = EventType.Meeting,
                EventTypeId = (int)EventType.Meeting,
                Category = EventCategory.Work,
                EventCategoryId = (int)EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = 10,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Title = "Budget review",
                Description = "Review Q3 spend against plan.",
                StartTime = LocalAt(-5, 11),
                EndTime = LocalAt(-5, 12),
                Location = "Finance room",
                Type = EventType.Meeting,
                EventTypeId = (int)EventType.Meeting,
                Category = EventCategory.Work,
                EventCategoryId = (int)EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = 30,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        foreach (var sample in samples)
        {
            await eventRepository.AddAsync(sample);
        }
    }
}