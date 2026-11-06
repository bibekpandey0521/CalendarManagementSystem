"use client"

import { useState } from "react"
import Link from "next/link"
import {
  Calendar,
  Home,
  Users,
  BarChart3,
  Clock,
  Mail,
  Settings,
  LayoutDashboard,
  List,
  Plus,
  ChevronLeft,
  ChevronRight,
  X,
  Menu,
} from "lucide-react"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { cn } from "@/components/ui/cn"

interface SidebarProps {
  isOpen: boolean
  onClose: () => void
  activeSection: string
  onSectionChange: (section: string) => void
}

const navigation = [
  {
    name: "Dashboard",
    href: "/",
    icon: LayoutDashboard,
    badge: undefined,
  },
  {
    name: "Calendar",
    href: "/calendar",
    icon: Calendar,
    badge: undefined,
  },
  {
    name: "Events",
    href: "/events",
    icon: List,
    badge: undefined,
  },
  {
    name: "Meetings",
    href: "/meetings",
    icon: Clock,
    badge: 3,
  },
  {
    name: "Reports",
    href: "/reports",
    icon: BarChart3,
    badge: undefined,
  },
  {
    name: "Settings",
    href: "/settings",
    icon: Settings,
    badge: undefined,
  },
]

export function Sidebar({
  isOpen,
  onClose,
  activeSection,
  onSectionChange,
}: SidebarProps) {
  const [collapsed, setCollapsed] = useState(false)

  return (
    <>
      {/* Mobile overlay */}
      {isOpen && !collapsed && (
        <div
          className="fixed inset-0 z-40 bg-black/80 backdrop-blur-sm"
          onClick={onClose}
        />
      )}

      {/* Desktop sidebar (always visible) */}
      <aside
        className={cn(
          "fixed left-0 top-0 h-full z-40 border-r border-border transition-all duration-300",
          collapsed ? "w-16" : "w-64",
          "bg-background"
        )}
      >
        <div className="flex h-14 items-center justify-between px-4 border-b border-border">
          {!collapsed && (
            <div className="flex items-center gap-2">
              <div className="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center text-white font-bold">
                C
              </div>
              <span className="font-semibold text-lg tracking-tight">
                Calendar
              </span>
            </div>
          )}
          <button
            onClick={() => setCollapsed(!collapsed)}
            className="rounded-md p-1.5 text-muted-foreground hover:bg-accent hover:text-accent-foreground"
            aria-label="Toggle sidebar"
          >
            {collapsed ? (
              <ChevronRight className="h-4 w-4" />
            ) : (
              <ChevronLeft className="h-4 w-4" />
            )}
          </button>
        </div>

        <nav className="flex-1 px-2 py-4 space-y-1 overflow-y-auto">
          {navigation.map((item) => {
            const isActive = activeSection === item.name.toLowerCase()
            return (
              <Link
                key={item.name}
                href={item.href}
                onClick={() => {
                  onSectionChange(item.name.toLowerCase())
                  if (!collapsed) onClose()
                }}
                className={cn(
                  "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors",
                  isActive
                    ? "bg-blue-100 text-blue-700"
                    : "text-foreground hover:bg-accent",
                  collapsed && "justify-center px-2"
                )}
              >
                <item.icon className={cn("h-5 w-5", collapsed && "mx-auto")} />
                {!collapsed && <span>{item.name}</span>}
                {item.badge && (
                  <Badge variant="secondary" className="ml-auto">
                    {item.badge}
                  </Badge>
                )}
              </Link>
            )
          })}
        </nav>

        <div className="p-4 border-t border-border">
          <Button
            onClick={() => {
              // Create new event action
            }}
            className="w-full"
          >
            <Plus className="h-4 w-4 mr-2" />
            New Event
          </Button>
        </div>
      </aside>

      {/* Mobile sidebar */}
      {isOpen && !collapsed && (
        <aside
          className="fixed left-0 top-0 h-full z-50 w-72 bg-background border-r border-border transform transition-transform duration-300"
        >
          <div className="flex h-14 items-center justify-between px-4 border-b border-border">
            <h2 className="font-semibold text-lg">Navigation</h2>
            <button
              onClick={onClose}
              className="rounded-md p-1.5 text-muted-foreground hover:bg-accent hover:text-accent-foreground"
              aria-label="Close sidebar"
            >
              <X className="h-4 w-4" />
            </button>
          </div>
          <nav className="flex-1 px-2 py-4 space-y-1 overflow-y-auto">
            {navigation.map((item) => {
              const isActive = activeSection === item.name.toLowerCase()
              return (
                <Link
                  key={item.name}
                  href={item.href}
                  onClick={() => {
                    onSectionChange(item.name.toLowerCase())
                    onClose()
                  }}
                  className={cn(
                    "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors",
                    isActive
                      ? "bg-blue-100 text-blue-700"
                      : "text-foreground hover:bg-accent"
                  )}
                >
                  <item.icon className="h-5 w-5" />
                  <span>{item.name}</span>
                  {item.badge && (
                    <Badge variant="secondary" className="ml-auto">
                      {item.badge}
                    </Badge>
                  )}
                </Link>
              )
            })}
          </nav>
        </aside>
      )}
    </>
  )
}

export default Sidebar
