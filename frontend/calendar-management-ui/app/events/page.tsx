/*
 * Calendar Management System - Events Page
 * Next.js 16 App Router + TypeScript + Tailwind CSS + Axios
 */

"use client"

import { useState } from "react"
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
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import {
  Plus,
  X,
  Menu,
  LayoutDashboard as LayoutDashboardSvg,
  Calendar as CalendarIcon,
  Clock as ClockIcon,
  List as ListIcon,
  Search,
  Filter,
  ArrowLeft,
  ChevronRight as ChevronRightIcon,
  ChevronLeft as ChevronLeftIcon,
  Calendar as CalendarSvg,
  Trash2,
  Edit3,
  Eye,
} from "lucide-react"
import { useEvents } from "@/hooks/useEvents"
import type { CalendarEventDto, EventType, EventCategory, EventStatus } from "@/types/calendar"
import { cn } from "@/components/ui/cn"
import { formatDate, formatTime, getCategoryColor, getStatusBadge, getEventTypeLabel } from "@/utils/format"

const EVENT_TYPES = [
  { value: 1, label: "Event" },
  { value: 2, label: "Meeting" },
]

const EVENT_CATEGORIES = [
  { value: 1, label: "Work", color: "bg-blue-100 text-blue-700" },
  { value: 2, label: "Personal", color: "bg-pink-100 text-pink-700" },
  { value: 3, label: "Birthday", color: "bg-amber-100 text-amber-700" },
  { value: 4, label: "Holiday", color: "bg-emerald-100 text-emerald-700" },
  { value: 5, label: "Other", color: "bg-gray-100 text-gray-700" },
]

const EVENT_STATUSES = [
  { value: 1, label: "Confirmed", color: "bg-emerald-100 text-emerald-700" },
  { value: 2, label: "Tentative", color: "bg-amber-100 text-amber-700" },
  { value: 3, label: "Cancelled", color: "bg-rose-100 text-rose-700" },
]

export default function EventsPage() {
  const router = useRouter()
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [selectedEvent, setSelectedEvent] = useState<CalendarEventDto | null>(null)
  const [view, setView] = useState<"list" | "table">("list")
  const [search, setSearch] = useState("")
  const [typeFilter, setTypeFilter] = useState<number | null>(null)
  const [categoryFilter, setCategoryFilter] = useState<number | null>(null)
  const [statusFilter, setStatusFilter] = useState<number | null>(null)
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(20)

  const {
    events,
    loading,
    error,
    refetch,
    totalCount,
    createEvent,
    deleteEvent,
  } = useEvents({
    page,
    pageSize,
    search: search || undefined,
    type: (typeFilter ?? undefined) as EventType | undefined,
    category: (categoryFilter ?? undefined) as EventCategory | undefined,
    status: (statusFilter ?? undefined) as EventStatus | undefined,
  })

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

  const filteredEvents = events.filter((e) => {
    if (search && !e.title.toLowerCase().includes(search.toLowerCase())) return false
    if (typeFilter && e.type !== typeFilter) return false
    if (categoryFilter && e.category !== categoryFilter) return false
    if (statusFilter && e.status !== statusFilter) return false
    return true
  })

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
                E
              </div>
              <h1 className="font-semibold text-lg tracking-tight">Events</h1>
            </div>
          </div>

          <div className="flex items-center gap-2 hidden md:flex">
            <div className="relative">
              <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
              <input
                type="text"
                placeholder="Search events…"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-9 h-9 rounded-lg border border-input bg-background px-3 py-2 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
              />
            </div>
            <div className="h-6 w-px bg-border"></div>
            <Button
              onClick={() => {
                setTypeFilter(null)
                setCategoryFilter(null)
                setStatusFilter(null)
              }}
              size="sm"
              variant="outline"
            >
              <Filter className="h-4 w-4 mr-2" />
              Clear Filters
            </Button>
          </div>

          <div className="flex items-center gap-2">
            <Button onClick={handleCreate} size="sm">
              <Plus className="h-4 w-4 mr-2" />
              New Event
            </Button>
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
          <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
            <div>
              <h2 className="text-2xl font-bold text-foreground">Events</h2>
              <p className="text-muted-foreground">
                {filteredEvents.length} of {totalCount || events.length} events
              </p>
            </div>
            <div className="flex items-center gap-2">
              <Button
                onClick={() => setView("list")}
                size="sm"
                className={cn("border", view === "list" ? "bg-blue-100 text-blue-700" : "")}
              >
                <ListIcon className="h-4 w-4" />
              </Button>
              <Button
                onClick={() => setView("table")}
                size="sm"
                className={cn("border", view === "table" ? "bg-blue-100 text-blue-700" : "")}
              >
                <CalendarSvg className="h-4 w-4" />
              </Button>
            </div>
          </div>

          {loading ? (
            <div className="flex flex-col items-center justify-center h-96">
              <div className="w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
              <p className="mt-3 text-sm text-muted-foreground">Loading events…</p>
            </div>
          ) : error ? (
            <div className="rounded-lg bg-red-50 p-4 border border-red-100">
              <h3 className="text-sm font-semibold text-red-800">Error loading events</h3>
              <p className="text-sm text-red-600 mt-1">{error.detail || "Unknown error"}</p>
              <button onClick={refetch} className="mt-3 text-sm text-blue-600 hover:underline">
                Retry
              </button>
            </div>
          ) : filteredEvents.length === 0 ? (
            <div className="rounded-lg border border-border bg-background p-8 text-center">
              <p className="text-muted-foreground">No events found.</p>
            </div>
          ) : (
            view === "table" ? (
              <EventsTable
                events={filteredEvents}
                onEventClick={handleEventClick}
                onDelete={handleDelete}
              />
            ) : (
              <EventsList
                events={filteredEvents}
                onEventClick={handleEventClick}
                onDelete={handleDelete}
              />
            )
          )}

          {/* Pagination */}
          {totalCount && totalCount > pageSize && (
            <div className="mt-6 flex items-center justify-between">
              <button
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={page === 1}
                className={cn(
                  "px-3 py-1.5 rounded-lg border border-input bg-background text-sm",
                  page === 1 && "opacity-50 cursor-not-allowed"
                )}
              >
                <ChevronLeftIcon className="h-4 w-4" />
              </button>
              <span className="px-3 py-1.5 text-sm text-muted-foreground">
                Page {page} of {Math.ceil(totalCount / pageSize)}
              </span>
              <button
                onClick={() => setPage((p) => p + 1)}
                disabled={(page * pageSize) >= (totalCount || 0)}
                className={cn(
                  "px-3 py-1.5 rounded-lg border border-input bg-background text-sm",
                  (page * pageSize) >= (totalCount || 0) && "opacity-50 cursor-not-allowed"
                )}
              >
                <ChevronRightIcon className="h-4 w-4" />
              </button>
            </div>
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

function EventsList({
  events,
  onEventClick,
  onDelete,
}: {
  events: CalendarEventDto[]
  onEventClick: (id: number) => void
  onDelete: (id: number) => void
}) {
  return (
    <div className="divide-y divide-border">
      {events.map((ev) => (
        <div
          key={ev.id}
          onClick={() => onEventClick(ev.id)}
          className="px-6 py-4 flex items-center justify-between cursor-pointer hover:bg-accent transition-colors"
        >
          <div className="flex-1">
            <div className="flex items-center gap-2">
              <h4 className="font-medium text-foreground">{ev.title}</h4>
              <Badge variant="secondary" className="text-xs">
                {getEventTypeLabel(ev.type)}
              </Badge>
            </div>
            <p className="text-sm text-muted-foreground mt-1">
              {formatDate(ev.startTime)} {formatTime(ev.startTime)}
              {ev.location && " · " + ev.location}
            </p>
          </div>
          <Badge
            variant="secondary"
            className="text-xs"
            style={{
              backgroundColor: getStatusBadge(ev.status).color,
              color: "white",
            }}
          >
            {getStatusBadge(ev.status).label}
          </Badge>
        </div>
      ))}
    </div>
  )
}

function EventsTable({
  events,
  onEventClick,
  onDelete,
}: {
  events: CalendarEventDto[]
  onEventClick: (id: number) => void
  onDelete: (id: number) => void
}) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b border-border">
            <th className="text-left px-6 py-3 text-muted-foreground font-medium">Title</th>
            <th className="text-left px-6 py-3 text-muted-foreground font-medium">Type</th>
            <th className="text-left px-6 py-3 text-muted-foreground font-medium">Category</th>
            <th className="text-left px-6 py-3 text-muted-foreground font-medium">Status</th>
            <th className="text-left px-6 py-3 text-muted-foreground font-medium">Date</th>
            <th className="text-right px-6 py-3 text-muted-foreground font-medium">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-border">
          {events.map((ev) => (
            <tr
              key={ev.id}
              onClick={() => onEventClick(ev.id)}
              className="cursor-pointer hover:bg-accent transition-colors"
            >
              <td className="px-6 py-3">
                <div className="font-medium text-foreground">{ev.title}</div>
                {ev.description && (
                  <p className="text-xs text-muted-foreground truncate max-w-[200px]">{ev.description}</p>
                )}
              </td>
              <td className="px-6 py-3">
                <span className="text-xs text-muted-foreground">{getEventTypeLabel(ev.type)}</span>
              </td>
              <td className="px-6 py-3">
                <span
                  className="text-xs font-medium"
                  style={{ color: getCategoryColor(ev.category) }}
                >
                  {ev.eventCategoryName || ev.category}
                </span>
              </td>
              <td className="px-6 py-3">
                <Badge variant="secondary" className="text-xs">
                  {getStatusBadge(ev.status).label}
                </Badge>
              </td>
              <td className="px-6 py-3">
                <div className="text-sm text-muted-foreground">
                  {formatDate(ev.startTime)}
                </div>
                <div className="text-xs text-muted-foreground">{formatTime(ev.startTime)}</div>
              </td>
              <td className="px-6 py-3 text-right">
                <button
                  onClick={(e) => {
                    e.stopPropagation()
                    onDelete(ev.id)
                  }}
                  className="text-red-600 hover:text-red-800 p-1.5"
                  aria-label="Delete event"
                >
                  <Trash2 className="h-4 w-4" />
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
