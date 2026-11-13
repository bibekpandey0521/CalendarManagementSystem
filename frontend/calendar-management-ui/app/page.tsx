/*
 * Calendar Management System - Home Page (Dashboard)
 * Next.js 16 App Router + TypeScript + Tailwind CSS + Axios
 */

"use client"

import { useState, useEffect } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
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
  X,
  Menu,
  LayoutDashboard,
  List,
  ArrowRight,
} from "lucide-react"
import { useEvents, useDashboard, useUpcomingEvents } from "@/hooks/useEvents"
import type { CalendarEventDto } from "@/types/calendar"
import { cn } from "@/components/ui/cn"
import { formatDate, formatTime } from "@/utils/format"

export default function Home() {
  const router = useRouter()
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [selectedEvent, setSelectedEvent] = useState<CalendarEventDto | null>(null)
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

  const handleEventClick = (id: number) => {
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
    <div className="min-h-screen bg-muted/40 flex flex-col">
      {/* Top Navigation */}
      <header className="sticky top-0 z-30 bg-card border-b border-border">
        <div className="flex items-center justify-between px-4 py-3 bg-card">
          <div className="flex items-center gap-2">
            <button
              onClick={() => setSidebarOpen(!sidebarOpen)}
              className="rounded-lg p-2 text-muted-foreground hover:bg-accent hover:text-accent-foreground md:hidden cursor-pointer"
              aria-label="Toggle sidebar"
            >
              <Menu className="h-4 w-4" />
            </button>
            <div className="flex items-center gap-2">
              <div className="w-9 h-9 rounded-lg bg-primary text-primary-foreground flex items-center justify-center font-bold">
                C
              </div>
              <h1 className="font-semibold text-lg tracking-tight">Calendar Management</h1>
            </div>
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

      <div className="flex flex-1 min-h-0">
        {/* Desktop Sidebar (inline in flex layout, hidden on mobile) */}
        <Sidebar
          isOpen={sidebarOpen}
          onClose={() => setSidebarOpen(false)}
          onCreateEvent={handleCreate}
        />

        {/* Mobile Sidebar Overlay */}
        {sidebarOpen && (
          <div
            className="fixed inset-0 z-40 bg-black/80 backdrop-blur-sm md:hidden"
            onClick={() => setSidebarOpen(false)}
          />
        )}

        {/* Main Content */}
        <main className="flex-1 min-w-0 overflow-y-auto p-4 md:p-6">
          <DashboardView
            totalEvents={totalEvents}
            upcomingCount={upcomingCount}
            todaysCount={todaysCount}
            meetingsCount={meetingsCount}
            upcomingEvents={upcomingEvents}
            loading={loading}
            error={error}
            refetch={refetch}
            onEventClick={handleEventClick}
            onViewAll={() => router.push("/events")}
            router={router}
          />
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
  onEventClick,
  onViewAll,
  router,
}: {
  totalEvents: number
  upcomingCount: number
  todaysCount: number
  meetingsCount: number
  upcomingEvents: CalendarEventDto[]
  loading: boolean
  error: any
  refetch: () => void
  onEventClick: (id: number) => void
  onViewAll: () => void
  router: any
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
        <p className="text-sm text-red-600 mt-1">{error.detail || "Unknown error"}</p>
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

      {/* Summary Cards - All Clickable */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <Link
          href="/events"
          onClick={() => router.push("/events")}
          className="block"
        >
          <StatCard
            title="Total Events"
            value={totalEvents}
            icon={CalendarIcon}
            color="bg-blue-500"
          />
        </Link>
        <Link
          href="/events?scope=upcoming"
          onClick={() => router.push("/events?scope=upcoming")}
          className="block"
        >
          <StatCard
            title="Upcoming"
            value={upcomingCount}
            icon={Clock}
            color="bg-amber-500"
          />
        </Link>
        <Link
          href="/calendar"
          onClick={() => router.push("/calendar")}
          className="block"
        >
          <StatCard
            title="Today"
            value={todaysCount}
            icon={CalendarIcon}
            color="bg-emerald-500"
          />
        </Link>
        <Link
          href="/meetings"
          onClick={() => router.push("/meetings")}
          className="block"
        >
          <StatCard
            title="Meetings"
            value={meetingsCount}
            icon={Users}
            color="bg-purple-500"
          />
        </Link>
      </div>

      {/* Upcoming Meetings */}
      <div className="bg-white rounded-xl border border-border shadow-sm">
        <div className="px-6 py-4 border-b border-border">
          <div className="flex items-center justify-between">
            <h3 className="font-semibold text-foreground">Upcoming Events</h3>
            <Link
              href="/events"
              onClick={onViewAll}
              className="text-sm text-blue-600 hover:underline flex items-center gap-1"
            >
              View all <ArrowRight className="h-3 w-3" />
            </Link>
          </div>
        </div>
        <div className="divide-y divide-border">
          {upcomingEvents.length === 0 ? (
            <div className="p-6 text-center text-muted-foreground">
              No upcoming events.
            </div>
          ) : (
            upcomingEvents.map((ev) => (
              <div
                key={ev.id}
                onClick={() => {
                  onEventClick(ev.id)
                }}
                className="px-6 py-4 flex items-center justify-between cursor-pointer hover:bg-accent transition-colors"
              >
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
    <Card className="cursor-pointer hover:shadow-md transition-shadow">
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
