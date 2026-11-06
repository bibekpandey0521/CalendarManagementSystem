using System.Text;
using CalendarManagement.Api.Data;
using CalendarManagement.Api.Interfaces;
using CalendarManagement.Api.Models;
using Microsoft.Data.SqlClient;

namespace CalendarManagement.Api.Data;

/// <summary>
/// Applies the database deployment script when the schema is missing and seeds
/// sample events on a fresh installation. Runs once at application startup.
/// </summary>
public sealed class DbInitializer
{
    private const string DeployScriptPathRelative = "Database\\Scripts\\Deploy-Database.sql";

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IEventRepository _eventRepository;

    public DbInitializer(IDbConnectionFactory connectionFactory, IEventRepository eventRepository)
    {
        _connectionFactory = connectionFactory;
        _eventRepository = eventRepository;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var deployScriptPath = FindDeployScript();
        if (deployScriptPath is null)
        {
            throw new InvalidOperationException("The database deployment script was not found.");
        }

        if (!await HasCalendarEventsTableAsync(cancellationToken))
        {
            await ExecuteScriptAsync(deployScriptPath, cancellationToken);
        }

        if (!await _eventRepository.ExistsAsync(cancellationToken))
        {
            await SeedAsync(cancellationToken);
        }
    }

    private static string? FindDeployScript()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        for (var i = 0; i < 8 && directory is not null; i++)
        {
            var candidate = Path.Combine(directory.FullName, DeployScriptPathRelative);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private async Task<bool> HasCalendarEventsTableAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE object_id = OBJECT_ID(N'dbo.CalendarEvents')";
        command.CommandType = System.Data.CommandType.Text;
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private async Task ExecuteScriptAsync(string scriptPath, CancellationToken cancellationToken)
    {
        var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
        var batches = new List<string>();
        var currentBatch = new List<string>();

        foreach (var rawLine in script.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (rawLine.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                if (currentBatch.Count > 0)
                {
                    batches.Add(string.Join("\r\n", currentBatch));
                    currentBatch.Clear();
                }

                continue;
            }

            currentBatch.Add(rawLine);
        }

        if (currentBatch.Count > 0)
        {
            batches.Add(string.Join("\r\n", currentBatch));
        }

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        foreach (var batch in batches)
        {
            await using var command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = batch;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private async Task SeedAsync(CancellationToken cancellationToken = default)
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
                Category = EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventTypeId = (int)EventType.Meeting,
                EventCategoryId = (int)EventCategory.Work,
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
                Category = EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventTypeId = (int)EventType.Meeting,
                EventCategoryId = (int)EventCategory.Work,
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
                Category = EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventTypeId = (int)EventType.Event,
                EventCategoryId = (int)EventCategory.Work,
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
                Category = EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventTypeId = (int)EventType.Meeting,
                EventCategoryId = (int)EventCategory.Work,
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
                Category = EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventTypeId = (int)EventType.Meeting,
                EventCategoryId = (int)EventCategory.Work,
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
                Category = EventCategory.Personal,
                Status = EventStatus.Confirmed,
                EventTypeId = (int)EventType.Meeting,
                EventCategoryId = (int)EventCategory.Personal,
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
                Category = EventCategory.Work,
                Status = EventStatus.Tentative,
                EventTypeId = (int)EventType.Meeting,
                EventCategoryId = (int)EventCategory.Work,
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
                Category = EventCategory.Birthday,
                Status = EventStatus.Confirmed,
                EventTypeId = (int)EventType.Event,
                EventCategoryId = (int)EventCategory.Birthday,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = 1440,
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
                Category = EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventTypeId = (int)EventType.Meeting,
                EventCategoryId = (int)EventCategory.Work,
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
                Category = EventCategory.Personal,
                Status = EventStatus.Confirmed,
                EventTypeId = (int)EventType.Event,
                EventCategoryId = (int)EventCategory.Personal,
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
                Category = EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventTypeId = (int)EventType.Meeting,
                EventCategoryId = (int)EventCategory.Work,
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
                Category = EventCategory.Work,
                Status = EventStatus.Confirmed,
                EventTypeId = (int)EventType.Meeting,
                EventCategoryId = (int)EventCategory.Work,
                EventStatusId = (int)EventStatus.Confirmed,
                ReminderMinutesBefore = 30,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        foreach (var sample in samples)
        {
            await _eventRepository.AddAsync(sample, cancellationToken);
        }
    }
}
