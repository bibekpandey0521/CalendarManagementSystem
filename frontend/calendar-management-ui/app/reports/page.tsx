/*
 * Calendar Management System - Reports Page
 * Next.js 16 App Router + TypeScript + Tailwind CSS + Axios
 */

"use client"

import { useState } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import {
  Sidebar,
} from "@/app/components/features/sidebar/Sidebar"
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
import {
  ChevronLeft,
  X,
  Menu,
  LayoutDashboard as LayoutDashboardSvg,
  Calendar as CalendarIcon,
  Clock as ClockIcon,
  List as ListIcon,
  Users as UsersIcon,
  BarChart3,
} from "lucide-react"
import { useEvents, useDashboard } from "@/hooks/useEvents"
import type { CalendarEventDto } from "@/types/calendar"
import { cn } from "@/components/ui/cn"
import { formatDate, formatTime, getCategoryColor, getEventTypeLabel, getStatusBadge } from "@/utils/format"
import { Badge } from "@/components/ui/badge"

export default function ReportsPage() {
  const router = useRouter()
  const [sidebarOpen, setSidebarOpen] = useState(false)

  const {
    events,
    loading,
    error,
    refetch,
  } = useEvents({ page: 1, pageSize: 500 })

  const { summary } = useDashboard()

  const totalEvents = summary?.totalEvents ?? events.length
  const upcomingCount = summary?.upcomingEvents ?? 0
  const todaysCount = summary?.todaysEvents ?? 0
  const meetingsCount = summary?.upcomingMeetings ?? 0
  const completedMeetings = summary?.completedMeetings ?? 0

  const eventsByCategory = events.reduce((acc: Record<string, number>, e) => {
    const cat = e.eventCategoryName || `Category ${e.category}`
    acc[cat] = (acc[cat] || 0) + 1
    return acc
  }, {} as Record<string, number>)

  const eventsByStatus = events.reduce((acc: Record<string, number>, e) => {
    const status = getStatusBadge(e.status).label
    acc[status] = (acc[status] || 0) + 1
    return acc
  }, {} as Record<string, number>)

  const eventsByType = events.reduce((acc: Record<string, number>, e) => {
    const type = getEventTypeLabel(e.type)
    acc[type] = (acc[type] || 0) + 1
    return acc
  }, {} as Record<string, number>)

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
                R
              </div>
              <h1 className="font-semibold text-lg tracking-tight">Reports</h1>
            </div>
          </div>

          <div className="flex items-center gap-2">
            <Button onClick={refetch} size="sm" variant="outline" className="cursor-pointer">
              <ArrowRightSvg className="h-4 w-4 mr-2" />
              Refresh Data
            </Button>
          </div>
        </div>
      </header>

      <div className="flex flex-1 min-h-0">
        {/* Desktop Sidebar */}
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
          <div className="mx-auto max-w-[1200px] px-6 py-8">
            {loading ? (
              <div className="flex flex-col items-center justify-center h-96">
                <div className="w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
                <p className="mt-3 text-sm text-muted-foreground">Loading reports…</p>
              </div>
            ) : (
              <div className="space-y-6">
                <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                  <div>
                    <h2 className="text-2xl font-semibold tracking-tight text-foreground">Reports</h2>
                    <p className="text-sm text-muted-foreground mt-1">Analytics and insights for your calendar activities</p>
                  </div>
                  <Button onClick={refetch} className="cursor-pointer">
                    <ArrowRightSvg className="h-4 w-4 mr-2" />
                    Refresh
                  </Button>
                </div>

                {/* Summary Cards */}
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                  <ReportStatCard
                    title="Total Events"
                    value={totalEvents}
                    icon={<CalendarIcon className="h-5 w-5 text-white" />}
                    color="bg-blue-600"
                  />
                  <ReportStatCard
                    title="Upcoming"
                    value={upcomingCount}
                    icon={<ClockIcon className="h-5 w-5 text-white" />}
                    color="bg-amber-500"
                  />
                  <ReportStatCard
                    title="Today"
                    value={todaysCount}
                    icon={<CalendarIcon className="h-5 w-5 text-white" />}
                    color="bg-emerald-500"
                  />
                  <ReportStatCard
                    title="Meetings"
                    value={meetingsCount}
                    icon={<UsersIcon className="h-5 w-5 text-white" />}
                    color="bg-purple-500"
                  />
                </div>

                {/* Charts Section */}
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                  <div className="lg:col-span-2 bg-card border rounded-xl shadow-sm p-6">
                    <CardHeader>
                      <CardTitle className="text-base font-semibold">Events by Category</CardTitle>
                      <CardDescription>Breakdown of events across categories</CardDescription>
                    </CardHeader>
                    <CardContent>
                      <div className="space-y-4">
                        {Object.entries(eventsByCategory).map(([cat, count]) => (
                          <div key={cat} className="flex items-center justify-between py-2">
                            <div className="flex items-center gap-3">
                              <div
                                className="w-3 h-3 rounded-full"
                                style={{ backgroundColor: getCategoryColor(parseInt(cat.replace("Category ", "")) || 1) }}
                              ></div>
                              <span className="text-sm text-foreground">{cat}</span>
                            </div>
                            <div className="text-sm text-muted-foreground">{count}</div>
                          </div>
                        ))}
                        {Object.keys(eventsByCategory).length === 0 && (
                          <div className="text-center text-muted-foreground py-4">
                            No category data available.
                          </div>
                        )}
                      </div>
                    </CardContent>
                  </div>

                  <div className="bg-card border rounded-xl shadow-sm p-6">
                    <CardHeader>
                      <CardTitle className="text-base font-semibold">Events by Status</CardTitle>
                      <CardDescription>Breakdown of events across statuses</CardDescription>
                    </CardHeader>
                    <CardContent>
                      <div className="space-y-4">
                        {Object.entries(eventsByStatus).map(([status, count]) => (
                          <div key={status} className="flex items-center justify-between py-2">
                            <div className="flex items-center gap-3">
                              <Badge
                                variant="secondary"
                                className="text-xs px-2 py-1"
                                style={{
                                  backgroundColor: status === "Confirmed" ? "#10b981" : status === "Tentative" ? "#f59e0b" : "#ef4444",
                                  color: "white",
                                }}
                              >
                                {status}
                              </Badge>
                            </div>
                            <div className="text-sm text-muted-foreground">{count}</div>
                          </div>
                        ))}
                        {Object.keys(eventsByStatus).length === 0 && (
                          <div className="text-center text-muted-foreground py-4">
                            No status data available.
                          </div>
                        )}
                      </div>
                    </CardContent>
                  </div>

                  <div className="bg-card border rounded-xl shadow-sm p-6">
                    <CardHeader>
                      <CardTitle className="text-base font-semibold">Events by Type</CardTitle>
                      <CardDescription>Breakdown of events vs meetings</CardDescription>
                    </CardHeader>
                    <CardContent>
                      <div className="space-y-4">
                        {Object.entries(eventsByType).map(([type, count]) => (
                          <div key={type} className="flex items-center justify-between py-2">
                            <div className="flex items-center gap-3">
                              <div
                                className="w-3 h-3 rounded-full"
                                style={{ backgroundColor: type === "Meeting" ? "#3b82f6" : "#ec4899" }}
                              ></div>
                              <span className="text-sm text-foreground">{type}</span>
                            </div>
                            <div className="text-sm text-muted-foreground">{count}</div>
                          </div>
                        ))}
                        {Object.keys(eventsByType).length === 0 && (
                          <div className="text-center text-muted-foreground py-4">
                            No type data available.
                          </div>
                        )}
                      </div>
                    </CardContent>
                  </div>
                </div>

                {/* Recent Activity */}
                <div className="bg-card border rounded-xl shadow-sm">
                  <CardHeader>
                    <CardTitle className="text-base font-semibold">Recent Activity</CardTitle>
                    <CardDescription>Latest events and meetings</CardDescription>
                  </CardHeader>
                  <CardContent>
                    <div className="divide-y divide-border">
                      {events.slice(0, 10).map((ev) => (
                        <div key={ev.id} className="px-6 py-4 flex items-center justify-between">
                          <div className="flex-1">
                            <div className="font-medium text-foreground">{ev.title}</div>
                            <p className="text-sm text-muted-foreground">
                              {formatDate(ev.startTime)} {formatTime(ev.startTime)}
                            </p>
                          </div>
                          <Badge variant="secondary" className="text-xs">
                            {getStatusBadge(ev.status).label}
                          </Badge>
                        </div>
                      ))}
                      {events.length === 0 && (
                        <div className="text-center text-muted-foreground py-8">
                          No recent activity.
                        </div>
                      )}
                    </div>
                  </CardContent>
                </div>
              </div>
            )}
          </main>
        </div>
      </div>
    </div>
  )
}

function ArrowRightSvg({ className }: { className?: string }) {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className}>
      <line x1="5" y1="12" x2="19" y2="12" />
      <polyline points="12 5 19 12 12 19" />
    </svg>
  )
}

function ReportStatCard({
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
