/*
 * Calendar Management System - Calendar Page
 * Next.js 16 App Router + FullCalendar v7 + Axios
 */

"use client"

import React, { useCallback, useEffect, useRef, useState } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import dynamic from "next/dynamic"
import {
  Sidebar,
} from "@/app/components/features/sidebar/Sidebar"
import { EventFormDialog } from "@/app/components/features/events/EventFormDialog"
const CalendarView = dynamic(
  () =>
    import("@/app/components/features/calendar/CalendarView").then(
      (mod) => mod.CalendarView
    ),
  { ssr: false }
) as React.ComponentType<{
  events: any[]
  viewType: string
  onEventClick: (info: any) => void
  onDateClick: (info: any) => void
  loading?: boolean
  error?: any
  calendarApiRef?: React.MutableRefObject<any>
}>
import {
  Button,
} from "@/components/ui/button"
import {
  Plus,
  ChevronLeft,
  ChevronRight,
  Clock as ClockIcon,
  Sun as SunIcon,
  List as ListIcon,
  Calendar as CalendarIcon,
  Menu as MenuIcon,
} from "lucide-react"
import { useEvents } from "@/hooks/useEvents"
import type { CalendarEventDto } from "@/types/calendar"

export default function CalendarPage() {
  const router = useRouter()
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [selectedEvent, setSelectedEvent] = useState<CalendarEventDto | null>(null)
  const [viewType, setViewType] = useState<string>("dayGridMonth")

  const calendarApiRef = useRef<any>(null)

  const {
    events,
    loading,
    error,
    refetch,
    createEvent,
    deleteEvent,
  } = useEvents({ page: 1, pageSize: 500 })

  const handleEventClick = useCallback((info: any) => {
    const id = parseInt(info.event.id, 10)
    const ev = events.find((e) => e.id === id)
    setSelectedEvent(ev ?? null)
    setDialogOpen(true)
  }, [events])

  const handleDateClick = useCallback((info: any) => {
    setSelectedEvent(null)
    setDialogOpen(true)
  }, [])

  const handleViewChange = useCallback((view: string) => {
    setViewType(view)
  }, [])

  const handlePrev = useCallback(() => {
    calendarApiRef.current?.prev()
  }, [])

  const handleNext = useCallback(() => {
    calendarApiRef.current?.next()
  }, [])

  const handleToday = useCallback(() => {
    calendarApiRef.current?.today()
  }, [])

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

  const viewConfigs = [
    { value: "dayGridMonth", label: "Month", icon: CalendarIcon },
    { value: "timeGridWeek", label: "Week", icon: ClockIcon },
    { value: "timeGridDay", label: "Day", icon: SunIcon },
    { value: "listDay", label: "List", icon: ListIcon },
  ]

  // Get current month and year for display
  const [currentMonthYear, setCurrentMonthYear] = useState("")

  useEffect(() => {
    setCurrentMonthYear(
      new Date().toLocaleDateString(undefined, {
        month: "long",
        year: "numeric",
      })
    )
  }, [])

  return (
    <div className="min-h-screen bg-muted/40 flex flex-col">
      {/* Top Navigation - App Header */}
      <header className="sticky top-0 z-30 bg-card border-b border-border">
        <div className="flex items-center justify-between px-4 py-3 bg-card">
          <div className="flex items-center gap-2">
            <button
              onClick={() => setSidebarOpen(!sidebarOpen)}
              className="rounded-lg p-2 text-muted-foreground hover:bg-accent hover:text-accent-foreground md:hidden cursor-pointer"
              aria-label="Toggle sidebar"
            >
              <MenuIcon className="h-4 w-4" />
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
          </div>
        </div>
      </header>

      <div className="flex flex-1 min-h-0">
        {/* Desktop Sidebar - inline below header */}
        <Sidebar
          isOpen={sidebarOpen}
          onClose={() => setSidebarOpen(false)}
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
          <div className="flex flex-col gap-4">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
              <div>
                <h2 className="text-2xl font-bold text-foreground">{currentMonthYear}</h2>
                <p className="text-muted-foreground">Manage your events and meetings</p>
              </div>
            </div>

            {/* Calendar Toolbar inside main content */}
            <div className="flex items-center gap-1 px-4 py-2 bg-gray-50 rounded-lg border border-border">
              <Button
                variant="outline"
                size="sm"
                onClick={handlePrev}
                className="h-9 px-3"
              >
                <ChevronLeft className="h-4 w-4" />
              </Button>
              <Button
                variant="outline"
                size="sm"
                onClick={handleToday}
                className="h-9 px-3"
              >
                <ClockIcon className="h-4 w-4" />
                Today
              </Button>
              <Button
                variant="outline"
                size="sm"
                onClick={handleNext}
                className="h-9 px-3"
              >
                <ChevronRight className="h-4 w-4" />
              </Button>
              <div className="flex items-center gap-1 px-2 py-1 bg-white rounded-lg border border-border">
                {viewConfigs.map((vc) => (
                  <Button
                    key={vc.value}
                    variant={viewType === vc.value ? "default" : "outline"}
                    size="sm"
                    onClick={() => handleViewChange(vc.value)}
                    className="h-8 px-2"
                  >
                    <vc.icon className="h-3.5 w-3.5 mr-1.5" />
                    {vc.label}
                  </Button>
                ))}
              </div>
            </div>

            <CalendarView
              events={events}
              viewType={viewType}
              onEventClick={handleEventClick}
              onDateClick={handleDateClick}
              loading={loading}
              error={error}
              calendarApiRef={calendarApiRef}
            />
          </div>
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
