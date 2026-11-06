/*
 * Calendar Management System - Home Page
 * Next.js 16 App Router + TypeScript + Tailwind CSS + FullCalendar + Axios
 */

"use client"

import { useState, useEffect } from "react"
import { Calendar } from "@fullcalendar/react"
import dayGridPlugin from "@fullcalendar/daygrid"
import timeGridPlugin from "@fullcalendar/timegrid"
import listPlugin from "@fullcalendar/list"
import interactionPlugin from "@fullcalendar/interaction"
import {
  Sidebar,
} from "@/app/components/features/sidebar/Sidebar"
import { EventFormDialog } from "@/app/components/features/events/EventFormDialog"
import {
  Button,
} from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import {
  Plus,
  Calendar as CalendarIcon,
  Clock,
  Users,
  BarChart3,
  Mail,
  ChevronLeft,
  X,
  Menu,
  LayoutDashboard,
  List,
} from "lucide-react"
import { useEvents, useDashboard, useUpcomingEvents } from "@/hooks/useEvents"
import type { CalendarEventDto } from "@/types/calendar"
import { cn } from "@/components/ui/cn"
import { formatDate, formatTime, getCategoryColor, isToday, isUpcoming } from "@/utils/format"

const API_BASE = process.env.NEXT_PUBLIC_API_BASE || "http://localhost:5050"

const calendarPlugins = [dayGridPlugin, timeGridPlugin, listPlugin, interactionPlugin]

const VIEW_OPTIONS: { value: any; label: string; icon: any }[] = [
  { value: "dayGridMonth", label: "Month", icon: CalendarIcon },
  { value: "timeGridWeek", label: "Week", icon: Clock },
  { value: "timeGridDay", label: "Day", icon: Mail },
  { value: "listDay", label: "List", icon: List as any },
]

export default function Home() {
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [selectedEvent, setSelectedEvent] = useState<CalendarEventDto | null>(null)
  const [activeView, setActiveView] = useState<"dashboard" | "calendar">("dashboard")
  const [viewType, setViewType] = useState<any>("dayGridMonth")
  const [currentDate, setCurrentDate] = useState<string>("")

  const {
    events,
    loading,
    error,
    refetch,
    createEvent,
    updateEvent,
    deleteEvent,
  } = useEvents({ page: 1, pageSize: 500 })

  const { summary } = useDashboard()
  const { events: upcomingEvents } = useUpcomingEvents(5, false)

  const calendarEvents = events.map((e) => ({
    id: e.id.toString(),
    title: e.title,
    start: e.startTime,
    end: e.endTime,
    allDay: e.isAllDay,
    backgroundColor: getCategoryColor(e.category),
    borderColor: getCategoryColor(e.category),
    extendedProps: {
      category: e.eventCategoryName,
      status: e.eventStatusName,
      location: e.location,
      description: e.description,
    },
  }))

  const handleEventClick = (info: any) => {
    const id = parseInt(info.event.id, 10)
    const ev = events.find((e) => e.id === id)
    setSelectedEvent(ev ?? null)
    setDialogOpen(true)
  }

  const handleCreate = () => {
    setSelectedEvent(null)
    setDialogOpen(true)
  }

  const handleDelete = async (id: number) => {
    if (!window.confirm("Are you sure you want to delete this event?")) return
    await deleteEvent(id)
    setDialogOpen(false)
    setSelectedEvent(null)
  }

  const handleSave = () => {
    setDialogOpen(false)
    setSelectedEvent(null)
  }

  useEffect(() => {
    setCurrentDate(
      new Date().toLocaleDateString(undefined, {
        weekday: "long",
        month: "long",
        day: "numeric",
      })
    )
  }, [])

  const totalEvents = summary?.totalEvents ?? events.length
  const upcomingCount = summary?.upcomingEvents ?? 0
  const todaysCount = summary?.todaysEvents ?? 0
  const meetingsCount = summary?.upcomingMeetings ?? 0

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Top Navigation */}
      <header className="sticky top-0 z-30 bg-white border-b border-border">
        <div className="flex items-center justify-between px-4 py-3 bg-white">
          <div className="flex items-center gap-2">
            <button
              onClick={() => setSidebarOpen(!sidebarOpen)}
              className="rounded-lg p-2 text-muted-foreground hover:bg-accent hover:text-accent-foreground md:hidden"
              aria-label="Toggle sidebar"
            >
              <Menu className="h-5 w-5" />
            </button>
            <div className="flex items-center gap-2">
              <div className="w-9 h-9 rounded-lg bg-blue-600 flex items-center justify-center text-white font-bold">
                C
              </div>
              <h1 className="font-semibold text-lg tracking-tight">Calendar Management</h1>
            </div>
          </div>

          <div className="flex items-center gap-2 hidden md:flex">
            {VIEW_OPTIONS.map((v) => (
              <button
                key={v.label}
                onClick={() => setViewType(v.value)}
                className={cn(
                  "flex items-center gap-2 rounded-lg px-3 py-1.5 text-sm font-medium transition-colors border",
                  activeView === "calendar" && viewType === v.value
                    ? "bg-blue-100 text-blue-700 border-blue-200"
                    : "text-muted-foreground hover:bg-accent border-transparent"
                )}
              >
                <v.icon className="h-4 w-4" />
                <span>{v.label}</span>
              </button>
            ))}
          </div>

          <div className="flex items-center gap-2">
            <Button onClick={handleCreate} size="sm">
              <Plus className="h-4 w-4 mr-2" />
              New Event
            </Button>
            <div className="h-6 w-px bg-border"></div>
            <span className="text-xs text-muted-foreground hidden sm:block">
              {currentDate}
            </span>
          </div>
        </div>
      </header>

      <div className="flex flex-1 overflow-hidden">
        {/* Mobile Sidebar Overlay */}
        {sidebarOpen && <div className="fixed inset-0 z-40 bg-black/80 backdrop-blur-sm md:hidden" onClick={() => setSidebarOpen(false)} />}

        {/* Sidebar */}
        <aside
          className={cn(
            "fixed left-0 top-0 h-full z-40 border-r border-border transition-all duration-300",
            sidebarOpen ? "w-64" : "w-0 overflow-hidden",
            sidebarOpen ? "translate-x-0" : "-translate-x-full",
            "md:relative md:static md:translate-x-0 md:w-64 md:border-0",
            "bg-background"
          )}
        >
          <div className="flex h-14 items-center justify-between px-4 border-b border-border">
            <div className="flex items-center gap-2">
              <div className="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center text-white font-bold">
                C
              </div>
              <span className="font-semibold text-lg tracking-tight">Calendar</span>
            </div>
            <button
              onClick={() => setSidebarOpen(false)}
              className="rounded-md p-1.5 text-muted-foreground hover:bg-accent hover:text-accent-foreground md:hidden"
              aria-label="Close sidebar"
            >
              <X className="h-4 w-4" />
            </button>
          </div>

          <nav className="flex-1 px-2 py-4 space-y-1 overflow-y-auto">
            <button
              onClick={() => {
                setActiveView("dashboard")
                setSidebarOpen(false)
              }}
              className={cn(
                "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors w-full text-left",
                activeView === "dashboard"
                  ? "bg-blue-100 text-blue-700"
                  : "text-foreground hover:bg-accent"
              )}
            >
              <LayoutDashboard className="h-5 w-5" />
              <span>Dashboard</span>
            </button>
            <button
              onClick={() => {
                setActiveView("calendar")
                setSidebarOpen(false)
              }}
              className={cn(
                "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors w-full text-left",
                activeView === "calendar"
                  ? "bg-blue-100 text-blue-700"
                  : "text-foreground hover:bg-accent"
              )}
            >
              <CalendarIcon className="h-5 w-5" />
              <span>Calendar</span>
            </button>
            <button
              className="flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors w-full text-left text-foreground hover:bg-accent"
            >
              <List className="h-5 w-5" />
              <span>Events</span>
            </button>
            <button
              className="flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors w-full text-left text-foreground hover:bg-accent"
            >
              <Clock className="h-5 w-5" />
              <span>Meetings <Badge variant="secondary" className="ml-auto">3</Badge></span>
            </button>
            <button
              className="flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors w-full text-left text-foreground hover:bg-accent"
            >
              <BarChart3 className="h-5 w-5" />
              <span>Reports</span>
            </button>
          </nav>

          <div className="p-4 border-t border-border">
            <Button onClick={handleCreate} className="w-full">
              <Plus className="h-4 w-4 mr-2" />
              New Event
            </Button>
          </div>
        </aside>

        {/* Main Content */}
        <main className="flex-1 overflow-y-auto p-4 md:p-6">
          {activeView === "dashboard" ? (
            <DashboardView
              totalEvents={totalEvents}
              upcomingCount={upcomingCount}
              todaysCount={todaysCount}
              meetingsCount={meetingsCount}
              upcomingEvents={upcomingEvents}
              loading={loading}
              error={error}
              refetch={refetch}
            />
          ) : (
            <CalendarView
              events={calendarEvents}
              viewType={viewType}
              onEventClick={handleEventClick}
              loading={loading}
              error={error}
            />
          )}
        </main>
      </div>

      {/* Event Form Dialog */}
      <EventFormDialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        onSuccess={() => {
          refetch()
          setDialogOpen(false)
          setSelectedEvent(null)
        }}
        eventToEdit={selectedEvent}
      />
    </div>
  )
}

function DashboardView({
  totalEvents,
  upcomingCount,
  todaysCount,
  meetingsCount,
  upcomingEvents,
  loading,
  error,
  refetch,
}: {
  totalEvents: number
  upcomingCount: number
  todaysCount: number
  meetingsCount: number
  upcomingEvents: CalendarEventDto[]
  loading: boolean
  error: any
  refetch: () => void
}) {
  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center h-96">
        <div className="w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
        <p className="mt-3 text-sm text-muted-foreground">Loading dashboard…</p>
      </div>
    )
  }

  if (error) {
    return (
      <div className="rounded-lg bg-red-50 p-4 border border-red-100">
        <h3 className="text-sm font-semibold text-red-800">Error loading dashboard</h3>
        <p className="text-sm text-red-600 mt-1">{error.detail || error.message || "Unknown error"}</p>
        <button onClick={refetch} className="mt-3 text-sm text-blue-600 hover:underline">
          Retry
        </button>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-foreground">Dashboard</h2>
          <p className="text-muted-foreground">Overview of your calendar activities</p>
        </div>
        <Button onClick={refetch}>
          <Clock className="h-4 w-4 mr-2" />
          Refresh
        </Button>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard
          title="Total Events"
          value={totalEvents}
          icon={CalendarIcon}
          color="bg-blue-500"
        />
        <StatCard
          title="Upcoming"
          value={upcomingCount}
          icon={Clock}
          color="bg-amber-500"
        />
        <StatCard
          title="Today"
          value={todaysCount}
          icon={CalendarIcon}
          color="bg-emerald-500"
        />
        <StatCard
          title="Meetings"
          value={meetingsCount}
          icon={Users}
          color="bg-purple-500"
        />
      </div>

      {/* Upcoming Meetings */}
      <div className="bg-white rounded-xl border border-border shadow-sm">
        <div className="px-6 py-4 border-b border-border">
          <div className="flex items-center justify-between">
            <h3 className="font-semibold text-foreground">Upcoming Events</h3>
            <a href="#" className="text-sm text-blue-600 hover:underline">
              View all
            </a>
          </div>
        </div>
        <div className="divide-y divide-border">
          {upcomingEvents.length === 0 ? (
            <div className="p-6 text-center text-muted-foreground">
              No upcoming events.
            </div>
          ) : (
            upcomingEvents.map((ev) => (
              <div key={ev.id} className="px-6 py-4 flex items-center justify-between">
                <div>
                  <h4 className="font-medium text-foreground">{ev.title}</h4>
                  <p className="text-sm text-muted-foreground">
                    {formatDate(ev.startTime)} {formatTime(ev.startTime)}
                  </p>
                </div>
                <Badge variant="secondary" className="text-xs">
                  {ev.eventCategoryName || ev.category}
                </Badge>
              </div>
            ))
          )}
        </div>
      </div>
    </div>
  )
}

function StatCard({
  title,
  value,
  icon: Icon,
  color,
}: {
  title: string
  value: number
  icon: any
  color: string
}) {
  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium">{title}</CardTitle>
        <div className={cn("w-8 h-8 rounded-lg flex items-center justify-center", color)}>
          <Icon className="h-5 w-5 text-white" />
        </div>
      </CardHeader>
      <CardContent>
        <div className="text-2xl font-bold text-foreground">{value}</div>
      </CardContent>
    </Card>
  )
}

function CalendarView({
  events,
  viewType,
  onEventClick,
  loading,
  error,
}: {
  events: any[]
  viewType: any
  onEventClick: (info: any) => void
  loading: boolean
  error: any
}) {
  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center h-96">
        <div className="w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
        <p className="mt-3 text-sm text-muted-foreground">Loading calendar…</p>
      </div>
    )
  }

  return (
    <div className="bg-white rounded-xl border border-border shadow-sm">
      <div className="h-[700px]">
        <Calendar
          plugins={[
            dayGridPlugin as any,
            timeGridPlugin as any,
            listPlugin as any,
            interactionPlugin as any,
          ]}
          initialView={viewType}
          events={events}
          eventClick={onEventClick}
          editable={false}
          selectable={false}
          height="auto"
          initialEvents={events.map((e) => ({
            id: e.id,
            title: e.title,
            start: e.start,
            end: e.end,
            allDay: e.allDay,
            backgroundColor: e.backgroundColor,
            borderColor: e.borderColor,
            extendedProps: e.extendedProps,
          }))}
        />
      </div>
      {events.length === 0 && (
        <div className="p-8 text-center text-muted-foreground">
          No events found.
        </div>
      )}
    </div>
  )
}
