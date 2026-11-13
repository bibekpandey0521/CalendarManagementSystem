/*
 * Calendar Management System - Meetings Page
 * Next.js 16 App Router + TypeScript + Tailwind CSS + Axios
 */

"use client"

import { useState, useMemo } from "react"
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
  Users as UsersIcon,
} from "lucide-react"
import { useEvents } from "@/hooks/useEvents"
import type { CalendarEventDto } from "@/types/calendar"
import { cn } from "@/components/ui/cn"
import { formatDate, formatTime, getEventTypeLabel, getStatusBadge, getCategoryColor } from "@/utils/format"

export default function MeetingsPage() {
  const router = useRouter()
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [selectedEvent, setSelectedEvent] = useState<CalendarEventDto | null>(null)
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
    type: 2, // Meeting type only
  })

  const meetings = useMemo(() => events.filter((e) => e.type === 2), [events])

  const isClient = typeof window !== "undefined"
  const now = isClient ? new Date() : new Date("2000-01-01")

  const upcomingMeetings = useMemo(() => meetings.filter((e) => new Date(e.startTime) > now), [meetings, now])
  const pastMeetings = useMemo(() => meetings.filter((e) => new Date(e.endTime) <= now), [meetings, now])

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
    if (!window.confirm("Are you sure you want to delete this meeting?")) return
    await deleteEvent(id)
    setDialogOpen(false)
    setSelectedEvent(null)
  }

  const handleSave = () => {
    setDialogOpen(false)
    setSelectedEvent(null)
  }

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
                M
              </div>
              <h1 className="font-semibold text-lg tracking-tight">Meetings</h1>
            </div>
          </div>

          <div className="flex items-center gap-2">
            <Button onClick={handleCreate} size="sm">
              <Plus className="h-4 w-4 mr-2" />
              New Meeting
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
          {/* Summary Cards */}
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
            <MeetingStatCard
              title="Total Meetings"
              value={meetings.length}
              icon={<ClockIcon className="h-5 w-5 text-white" />}
              color="bg-blue-500"
            />
            <MeetingStatCard
              title="Upcoming"
              value={upcomingMeetings.length}
              icon={<CalendarIcon className="h-5 w-5 text-white" />}
              color="bg-amber-500"
            />
            <MeetingStatCard
              title="Completed"
              value={pastMeetings.length}
              icon={<CheckIcon className="h-5 w-5 text-white" />}
              color="bg-emerald-500"
            />
            <MeetingStatCard
              title="Attendees"
              value={meetings.length * 3}
              icon={<UsersIcon className="h-5 w-5 text-white" />}
              color="bg-purple-500"
            />
          </div>

          {loading ? (
            <div className="flex flex-col items-center justify-center h-96">
              <div className="w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
              <p className="mt-3 text-sm text-muted-foreground">Loading meetings…</p>
            </div>
          ) : error ? (
            <div className="rounded-lg bg-red-50 p-4 border border-red-100">
              <h3 className="text-sm font-semibold text-red-800">Error loading meetings</h3>
              <p className="text-sm text-red-600 mt-1">{error.detail || "Unknown error"}</p>
              <button onClick={refetch} className="mt-3 text-sm text-blue-600 hover:underline">
                Retry
              </button>
            </div>
          ) : meetings.length === 0 ? (
            <div className="rounded-lg border border-border bg-background p-8 text-center">
              <p className="text-muted-foreground">No meetings found.</p>
            </div>
          ) : (
            <div className="rounded-lg border border-border bg-card shadow-sm overflow-hidden">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-border bg-muted/50">
                    <th className="text-left px-6 py-3 text-muted-foreground font-medium">Title</th>
                    <th className="text-left px-6 py-3 text-muted-foreground font-medium">Date</th>
                    <th className="text-left px-6 py-3 text-muted-foreground font-medium">Status</th>
                    <th className="text-left px-6 py-3 text-muted-foreground font-medium">Reminder</th>
                    <th className="text-right px-6 py-3 text-muted-foreground font-medium">Actions</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border">
                  {meetings.map((ev) => (
                    <tr
                      key={ev.id}
                      onClick={() => handleEventClick(ev.id)}
                      className="cursor-pointer hover:bg-accent transition-colors"
                    >
                      <td className="px-6 py-3">
                        <div className="font-medium text-foreground">{ev.title}</div>
                        {ev.description && (
                          <p className="text-xs text-muted-foreground">{ev.description}</p>
                        )}
                      </td>
                      <td className="px-6 py-3">
                        <div className="text-sm text-muted-foreground">
                          {formatDate(ev.startTime)}
                        </div>
                        <div className="text-xs text-muted-foreground">{formatTime(ev.startTime)}</div>
                      </td>
                      <td className="px-6 py-3">
                        <Badge variant="secondary" className="text-xs">
                          {getStatusBadge(ev.status).label}
                        </Badge>
                      </td>
                      <td className="px-6 py-3">
                        {ev.reminderMinutesBefore ? (
                          <span className="text-xs text-muted-foreground">
                            {ev.reminderMinutesBefore} min before
                          </span>
                        ) : (
                          <span className="text-xs text-muted-foreground">None</span>
                        )}
                      </td>
                      <td className="px-6 py-3 text-right">
                        <button
                          onClick={(e) => {
                            e.stopPropagation()
                            handleDelete(ev.id)
                          }}
                          className="text-red-600 hover:text-red-800 p-1.5"
                          aria-label="Delete meeting"
                        >
                          <TrashIcon className="h-4 w-4" />
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
              {totalCount && totalCount > pageSize && (
                <div className="mt-4 flex items-center justify-between">
                  <button
                    onClick={() => setPage((p) => Math.max(1, p - 1))}
                    disabled={page === 1}
                    className="px-3 py-1.5 rounded-lg border border-input bg-background text-sm"
                  >
                    <ArrowLeftIcon className="h-4 w-4" />
                  </button>
                  <span className="px-3 py-1.5 text-sm text-muted-foreground">
                    Page {page} of {Math.ceil(totalCount / pageSize)}
                  </span>
                  <button
                    onClick={() => setPage((p) => p + 1)}
                    disabled={(page * pageSize) >= (totalCount || 0)}
                    className="px-3 py-1.5 rounded-lg border border-input bg-background text-sm"
                  >
                    <ArrowRightIcon className="h-4 w-4" />
                  </button>
                </div>
              )}
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

function BarChartSvg({ className }: { className?: string }) {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className}>
      <line x1="18" y1="20" x2="18" y2="10" />
      <line x1="12" y1="20" x2="12" y2="4" />
      <line x1="6" y1="20" x2="6" y2="14" />
    </svg>
  )
}

function CheckIcon({ className }: { className?: string }) {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className}>
      <polyline points="20 6 9 17 4 12" />
    </svg>
  )
}

function TrashIcon({ className }: { className?: string }) {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className}>
      <polyline points="3 6 5 6 21 6" />
      <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
    </svg>
  )
}

function ArrowLeftIcon({ className }: { className?: string }) {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className}>
      <line x1="19" y1="12" x2="5" y2="12" />
      <polyline points="12 19 5 12 12 5" />
    </svg>
  )
}

function ArrowRightIcon({ className }: { className?: string }) {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className}>
      <line x1="5" y1="12" x2="19" y2="12" />
      <polyline points="12 5 19 12 12 19" />
    </svg>
  )
}

function MeetingStatCard({
  title,
  value,
  icon,
  color,
}: {
  title: string
  value: number
  icon: React.ReactNode
  color: string
}) {
  return (
    <Card className="cursor-pointer hover:shadow-md transition-shadow">
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium">{title}</CardTitle>
        <div className="w-8 h-8 rounded-lg flex items-center justify-center" style={{ backgroundColor: color }}>
          {icon}
        </div>
      </CardHeader>
      <CardContent>
        <div className="text-2xl font-bold text-foreground">{value}</div>
      </CardContent>
    </Card>
  )
}
