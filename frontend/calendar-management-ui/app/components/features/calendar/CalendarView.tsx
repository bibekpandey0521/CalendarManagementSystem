/*
 * CalendarView - FullCalendar component for the Calendar Management System
 * Next.js 16 App Router + TypeScript + Tailwind CSS + FullCalendar v7 + Axios
 */

"use client"

import FullCalendar from "@fullcalendar/react"
import dayGridPlugin from "@fullcalendar/react/daygrid"
import timeGridPlugin from "@fullcalendar/react/timegrid"
import listPlugin from "@fullcalendar/react/list"
import interactionPlugin from "@fullcalendar/react/interaction"
import type { CalendarEventDto } from "@/types/calendar"
import { getCategoryColor } from "@/utils/format"
import { useCallback, useEffect, useRef } from "react"

const calendarPlugins = [dayGridPlugin, timeGridPlugin, listPlugin, interactionPlugin]

interface CalendarViewProps {
  events: CalendarEventDto[]
  viewType: string
  onEventClick: (arg: any) => void
  onDateClick: (arg: any) => void
  loading?: boolean
  error?: any
  calendarApiRef?: React.MutableRefObject<any>
}

export function CalendarView({
  events,
  viewType,
  onEventClick,
  onDateClick,
  loading = false,
  error,
  calendarApiRef,
}: CalendarViewProps) {
  const fullCalendarRef = useRef<any>(null)

  // Callback ref: capture the FullCalendar component instance and its API
  const setCalendarRef = useCallback((el: any) => {
    fullCalendarRef.current = el
    if (el && calendarApiRef) {
      calendarApiRef.current = el.getApi()
    }
  }, [calendarApiRef])

  // Sync calendar view when viewType changes externally using the API
  useEffect(() => {
    if (calendarApiRef?.current) {
      calendarApiRef.current.changeView(viewType)
    }
  }, [viewType])

  if (loading) {
    return (
      <div className="flex flex-col items-center justify-center h-96">
        <div className="w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
        <p className="mt-3 text-sm text-muted-foreground">Loading calendar…</p>
      </div>
    )
  }

  if (error) {
    return (
      <div className="rounded-lg bg-red-50 p-4 border border-red-100">
        <h3 className="text-sm font-semibold text-red-800">Error loading calendar</h3>
        <p className="text-sm text-red-600 mt-1">{error.detail || error.message || "Unknown error"}</p>
      </div>
    )
  }

  return (
    <div className="bg-white rounded-xl border border-border shadow-sm">
      <div className="h-[calc(100vh-240px)] min-h-[700px]">
        <FullCalendar
          ref={setCalendarRef}
          plugins={calendarPlugins}
          initialView={viewType}
          events={events.map((e) => ({
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
          }))}
          eventClick={onEventClick}
          dateClick={onDateClick}
          editable={false}
          selectable={false}
          height="auto"
          nowIndicator={true}
          views={{
            dayGridMonth: { titleFormat: { year: "numeric", month: "long" } },
            timeGridWeek: { titleFormat: { year: "numeric", month: "numeric", day: "numeric" } },
            timeGridDay: { titleFormat: { year: "numeric", month: "numeric", day: "numeric" } },
            listDay: { titleFormat: { year: "numeric", month: "numeric", day: "numeric" } },
          }}
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
