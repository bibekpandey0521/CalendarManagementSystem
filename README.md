# Calendar Management System

A modern, responsive Calendar Management System with an ASP.NET Core Web API backend (ADO.NET + stored procedures), a Next.js/TypeScript frontend (Tailwind CSS, Shadcn UI, FullCalendar, Axios), and an SSDT SQL Server database project — fully integrated and tested end-to-end.

## Solution Architecture

```
CalendarManagementSystem/
│
├── backend/CalendarManagement.Api/          # ASP.NET Core Web API (.NET 10)
│   ├── Controllers/                          # EventsController, DashboardController
│   ├── Models/                               # Domain models (CalendarEvent, EventEnums)
│   ├── DTOs/                                 # CreateEventDto, UpdateEventDto, EventDto
│   ├── Interfaces/                           # IEventRepository, IEventService, IDashboardService
│   ├── Repositories/                         # EventRepository (ADO.NET + stored procedures)
│   ├── Services/                             # EventService, DashboardService (business logic)
│   ├── Data/                                 # DbConnectionFactory (ADO.NET connection)
│   ├── Middleware/                           # ExceptionHandlingMiddleware (RFC 7807)
│   ├── Program.cs                            # DI, CORS, Swagger, health checks
│   ├── appsettings.json                      # Connection string & CORS
│   └── Properties/launchSettings.json        # Port 5050, Development env
│
├── frontend/calendar-management-ui/          # Next.js 16 + TypeScript
│   ├── app/
│   │   ├── layout.tsx                        # Root layout
│   │   └── page.tsx                          # Dashboard + Calendar views
│   ├── components/
│   │   ├── services/                         # api.ts (Axios service layer)
│   │   ├── ui/                               # Shadcn UI components
│   │   └── features/
│   │       ├── sidebar/                      # Sidebar navigation
│   │       └── events/                       # EventFormDialog (create/edit)
│   ├── hooks/                                # useEvents, useDashboard, useUpcomingEvents, usePastEvents
│   ├── types/                                # calendar.ts (TypeScript types)
│   ├── utils/                                # format.ts (date/format utilities)
│   ├── next.config.ts                        # Next.js config (webpack mode)
│   ├── tailwind.config.js                    # Tailwind config
│   ├── postcss.config.js                     # PostCSS with Tailwind
│   └── package.json                          # Dependencies
│
└── Database/                                 # SSDT SQL Server Database Project
    ├── CalendarManagement.Database.sqlproj   # SQL Server Database Project
    ├── Tables/                               # 7 table definitions
    ├── StoredProcedures/                     # 8 stored procedures
    ├── Functions/                            # fn_CalendarEvent_IsOverlapping
    ├── Views/                                # vw_CalendarEvents_Details
    ├── Indexes/                              # 8 indexes on CalendarEvents
    ├── Scripts/                              # Deploy, pre/post-deployment
    └── Deployment/                           # Publish profile
```

## Features

### Calendar Features
- **Month, Week, Day, and List views** (FullCalendar)
- **Create, edit, view, and delete events** with full CRUD
- **Meeting scheduling** with title, description, date, time, location
- **Meeting reminders** (configurable minutes before event)
- **Upcoming and past meetings** filtering
- **Event search and filtering** by type, category, status, date range
- **Dashboard** with summary cards (total events, upcoming, today's events, meetings)

### Backend Features
- Repository + Service pattern with clean separation of concerns
- ADO.NET with stored procedures for all database operations
- Swagger/OpenAPI documentation at `/swagger`
- RFC 7807 exception handling middleware
- Health checks endpoint at `/health`
- CORS configured for Next.js frontend (`http://localhost:3000`)
- Database seeding on first run (12 sample events)

### Database Features
- Separate SSDT project (buildable/publishable independently)
- Tables, stored procedures, views, functions, indexes in separate files
- Proper primary keys, foreign keys, constraints
- DACPAC generation support
- Pre/post-deployment scripts with data seeding

---

## Prerequisites

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 10.0.301+ | Build/run the ASP.NET Core API |
| Node.js | v22+ | Run the Next.js frontend |
| npm | 10+ | Install frontend dependencies |
| SQL Server LocalDB | `(localdb)\MSSQLLocalDB` | Database engine |
| Visual Studio 2022 | 17.0+ | Open/publish SSDT database project (optional) |

### Verify LocalDB

```powershell
sqllocaldb info MSSQLLocalDB
# If stopped:
sqllocaldb start MSSQLLocalDB
```

---

## Phase 1: Database Deployment (SSDT)

The database project is located at `Database/CalendarManagement.Database.sqlproj`. It can be deployed two ways:

### Option A: Visual Studio (Recommended for SSDT/DACPAC)

1. **Open Visual Studio 2022** (with SQL Server Data Tools workload installed)
2. **Open the database project**: `File` → `Open` → `Project/Solution` → select `Database/CalendarManagement.Database.sqlproj`
3. **Configure the publish profile**: Open `Database/Deployment/CalendarManagement.Database.publish.xml` and set:
   - Target database connection: `Server=(localdb)\MSSQLLocalDB;Database=CalendarManagementDb;Trusted_Connection=True;`
4. **Publish**: Right-click the project → `Publish` → select profile → `Publish`
5. **Generate DACPAC**: Right-click project → `Publish` → `Generate script` or use `SqlPackage.exe`:
   ```cmd
   SqlPackage.exe /a:Extract /sfn:"Database\bin\Debug\CalendarManagement.Database.dacpac" /tfn:"CalendarManagementDatabase.dacpac"
   ```

### Option B: Command Line (sqlcmd)

If you don't have Visual Studio, you can deploy using the provided deployment script:

```powershell
# 1. Create the database (if it doesn't exist)
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -d master -Q "IF DB_ID(N'CalendarManagementDb') IS NULL CREATE DATABASE CalendarManagementDb;"

# 2. Run the full deployment script (creates tables, procs, functions, indexes, seeds data)
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -d CalendarManagementDb -I -b -i "Database/Scripts/Deploy-Database.sql"
```

### Option C: MSBuild (SSDT Build)

```cmd
msbuild Database\CalendarManagement.Database.sqlproj /p:Configuration=Debug
```

This builds the project and produces a DACPAC at `Database/bin/Debug/CalendarManagement.Database.dacpac`.

### Verify Database Deployment

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -d CalendarManagementDb -Q "
SELECT name FROM sys.tables ORDER BY name;
SELECT name FROM sys.procedures ORDER BY name;
SELECT COUNT(*) AS EventCount FROM dbo.CalendarEvents;
"
```

Expected: 4 tables, 8 stored procedures, 8+ events.

---

## Phase 2: Backend API

### Build & Run

```powershell
cd backend/CalendarManagement.Api

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run (development, port 5050)
dotnet run
```

The API will start on **http://localhost:5050**.

### Verify

```powershell
# Health check
Invoke-WebRequest -Uri "http://localhost:5050/health" -UseBasicParsing
# Expected: 200 Healthy

# Swagger UI
# Open browser: http://localhost:5050/swagger
```

### Configuration

`backend/CalendarManagement.Api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CalendarManagementDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://127.0.0.1:3000"
    ]
  }
}
```

### API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `GET /health` | GET | Health check |
| `GET /api/events?page=1&pageSize=20&type=2&scope=1` | GET | List events (paged, filterable) |
| `GET /api/events/upcoming?limit=5&meetingsOnly=false` | GET | Upcoming events |
| `GET /api/events/past?limit=5` | GET | Past events |
| `GET /api/events/{id}` | GET | Get single event |
| `POST /api/events` | POST | Create event |
| `PUT /api/events/{id}` | PUT | Update event (with overlap validation) |
| `DELETE /api/events/{id}` | DELETE | Delete event |
| `GET /api/dashboard/summary` | GET | Dashboard statistics |
| `GET /swagger/v1/swagger.json` | GET | OpenAPI spec |

### Example: Create an Event

```bash
curl -X POST http://localhost:5050/api/events ^
  -H "Content-Type: application/json" ^
  -d "{
    \"title\": \"Team Meeting\",
    \"description\": \"Weekly sync\",
    \"startTime\": \"2026-11-06T10:00:00Z\",
    \"endTime\": \"2026-11-06T11:00:00Z\",
    \"isAllDay\": false,
    \"location\": \"Conference Room A\",
    \"type\": 2,
    \"category\": 1,
    \"status\": 1,
    \"reminderMinutesBefore\": 15
  }"
```

---

## Phase 3: Frontend

### Install & Run

```powershell
cd frontend/calendar-management-ui

# Install dependencies
npm install

# Run development server (webpack mode - required for FullCalendar compatibility)
npx next dev --webpack
```

The frontend will start on **http://localhost:3000**.

### Note on `--webpack` Flag

Next.js 16 uses Turbopack by default, but `@fullcalendar/core` has subpath export issues with Turbopack. Use `--webpack` for development:

```powershell
npx next dev --webpack
```

For production builds:

```powershell
npm run build
npm start
```

### Frontend Features

- **Dashboard** with summary cards and upcoming events list
- **Calendar** with Month, Week, Day, and List views
- **Event form** dialog for create/edit with validation
- **Sidebar** navigation (Dashboard, Calendar, Events, Meetings, Reports, Settings)
- **Responsive design** (mobile sidebar overlay)
- **Loading and error states**
- **Axios API service layer** connected to backend at `http://localhost:5050`

---

## Phase 4: End-to-End Testing

With both services running:

1. **Backend**: http://localhost:5050 (Health: `GET /health` → 200 Healthy)
2. **Frontend**: http://localhost:3000 (Home: 200 OK)
3. **Swagger**: http://localhost:5050/swagger

### Test Flow

```powershell
# 1. Health check
Invoke-WebRequest -Uri "http://localhost:5050/health" -UseBasicParsing

# 2. List events
Invoke-RestMethod -Uri "http://localhost:5050/api/events?page=1&pageSize=5" -UseBasicParsing

# 3. Get dashboard summary
Invoke-RestMethod -Uri "http://localhost:5050/api/dashboard/summary" -UseBasicParsing

# 4. Create event via API
$body = @{ title="Test"; startTime="2026-11-07T10:00:00Z"; endTime="2026-11-07T11:00:00Z"; type=2; category=1; status=1 } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5050/api/events" -Method Post -ContentType "application/json" -Body $body
```

Then open http://localhost:3000 in your browser to see the calendar UI with the data.

---

## Database Project Structure (SSDT)

| Folder | Contents |
|--------|----------|
| `Tables/` | `CalendarEvents.sql`, `EventTypes.sql`, `EventCategories.sql`, `EventStatuses.sql` |
| `StoredProcedures/` | `usp_CalendarEvent_Create.sql`, `usp_CalendarEvent_Update.sql`, `usp_CalendarEvent_Delete.sql`, `usp_CalendarEvent_GetById.sql`, `usp_CalendarEvent_GetUpcoming.sql`, `usp_CalendarEvent_GetPast.sql`, `usp_CalendarEvent_GetStatistics.sql`, `usp_CalendarEvent_SelectAll.sql` |
| `Functions/` | `fn_CalendarEvent_IsOverlapping.sql` (overlap detection) |
| `Views/` | `vw_CalendarEvents_Details.sql` (joined event details) |
| `Indexes/` | `CI_CalendarEvents_Filtering.sql` (8 indexes on CalendarEvents) |
| `Scripts/` | `Deploy-Database.sql` (full deployment), `Pre-Deployment.sql`, `Post-Deployment.sql` (seed data), `IndexesWithQuotedIdentifier.sql` |
| `Deployment/` | `CalendarManagement.Database.publish.xml` (publish profile) |

### Stored Procedure Naming Convention

All CRUD procedures follow `usp_CalendarEvent_*`:
- `usp_CalendarEvent_Create` — INSERT with OUTPUT id
- `usp_CalendarEvent_Update` — UPDATE by Id
- `usp_CalendarEvent_Delete` — DELETE by Id
- `usp_CalendarEvent_GetById` — SELECT single event
- `usp_CalendarEvent_SelectAll` — Paged/filtered/sorted list
- `usp_CalendarEvent_GetUpcoming` — Future events
- `usp_CalendarEvent_GetPast` — Past events
- `usp_CalendarEvent_GetStatistics` — Dashboard counters

---

## Troubleshooting

### API won't start on port 5050
```powershell
# Kill existing dotnet processes
Get-Process -Name dotnet | Stop-Process -Force

# Check if port is in use
netstat -ano | Select-String 5050
```

### Frontend FullCalendar errors
Use webpack mode (not Turbopack):
```powershell
npx next dev --webpack
```

### Database connection errors
1. Verify LocalDB is running: `sqllocaldb info MSSQLLocalDB`
2. Check connection string in `appsettings.json`
3. Run deployment script: `Database/Scripts/Deploy-Database.sql`

### PUT/DELETE returning 404
Fixed: `ExecuteNonQueryAsync` returns -1 with `SET NOCOUNT ON`; code now falls back to `@@ROWCOUNT`.

### Re-add projects to solution
```powershell
cd CalendarManagementSystem
dotnet sln CalendarManagementSystem.slnx add backend/CalendarManagement.Api/CalendarManagement.Api.csproj
```

---

## Development Instructions (Implementation Order)

1. ✅ Create solution and folder architecture
2. ✅ Create SQL Server Database Project (SSDT)
3. ✅ Implement database tables and stored procedures
4. ✅ Build ASP.NET Core Web API
5. ✅ Build Next.js frontend
6. ✅ Integrate frontend with backend
7. ✅ Test all endpoints end-to-end
8. ⏳ Write README.md (this document)

---

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core Web API (.NET 10), C# |
| Data Access | ADO.NET + SQL Server stored procedures |
| Database | SQL Server LocalDB, SSDT Database Project |
| Frontend | Next.js 16, TypeScript, React 19 |
| Styling | Tailwind CSS v4 |
| UI Components | Shadcn UI pattern (Radix UI primitives) |
| Calendar | FullCalendar (dayGrid, timeGrid, list, interaction) |
| HTTP Client | Axios |
| API Docs | Swagger / OpenAPI (Swashbuckle 10.3.1) |
| Icons | Lucide React |

---

## License

MIT