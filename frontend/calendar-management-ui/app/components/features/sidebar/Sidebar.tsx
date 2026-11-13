"use client"

import { useState } from "react"
import { usePathname, useRouter } from "next/navigation"
import {
  LayoutDashboard,
  Calendar,
  List,
  Clock,
  BarChart3,
  Bot,
  Settings,
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
  onCreateEvent?: () => void
}

const navigation = [
  { name: "Dashboard", href: "/", icon: LayoutDashboard, badge: undefined },
  { name: "Calendar", href: "/calendar", icon: Calendar, badge: undefined },
  { name: "Events", href: "/events", icon: List, badge: undefined },
  { name: "Meetings", href: "/meetings", icon: Clock, badge: 3 },
  { name: "Reports", href: "/reports", icon: BarChart3, badge: undefined },
  { name: "AI Assistant", href: "/ai-assistant", icon: Bot, badge: undefined },
  { name: "Settings", href: "/settings", icon: Settings, badge: undefined },
]

export function Sidebar({
  isOpen,
  onClose,
  onCreateEvent,
}: SidebarProps) {
  const router = useRouter()
  const pathname = usePathname()

  const handleNavClick = (href: string) => {
    router.push(href)
    onClose()
  }

  return (
    <>
      {/* Mobile overlay */}
      {isOpen && (
        <div className="fixed inset-0 z-40 bg-black/80 backdrop-blur-sm md:hidden" onClick={onClose} />
      )}

      {/* Desktop sidebar - inline in flex layout (not fixed) */}
      <aside className="hidden md:flex flex-col h-full border-r border-border bg-sidebar w-64">
        <div className="flex h-14 items-center justify-between px-4 border-b border-border">
          <div className="flex items-center gap-2">
            <div className="w-9 h-9 rounded-lg bg-blue-50 text-blue-600 dark:bg-blue-950 flex items-center justify-center font-bold">
              C
            </div>
            <span className="font-semibold text-lg tracking-tight text-foreground">
              Calendar
            </span>
          </div>
        </div>
        <nav className="flex-1 px-3 py-4 space-y-0.5 overflow-y-auto">
          {navigation.map((item) => {
            const isActive = pathname === item.href
            return (
              <button
                key={item.name}
                onClick={() => handleNavClick(item.href)}
                className={cn(
                  "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors w-full text-left",
                  isActive
                    ? "bg-accent text-accent-foreground font-medium"
                    : "text-muted-foreground hover:bg-muted hover:text-foreground",
                )}
              >
                <item.icon className="h-5 w-5" />
                <span>{item.name}</span>
                {item.badge && (
                  <Badge variant="secondary" className="ml-auto">
                    {item.badge}
                  </Badge>
                )}
              </button>
            )
          })}
        </nav>
      </aside>

      {/* Mobile sidebar */}
      {isOpen && (
        <aside className="fixed left-0 top-0 h-full z-50 w-72 bg-sidebar border-r border-border md:hidden transform transition-transform duration-300 translate-x-0">
          <div className="flex h-14 items-center justify-between px-4 border-b border-border">
            <h2 className="font-semibold text-lg text-foreground">Navigation</h2>
            <button
              onClick={onClose}
              className="rounded-md p-1.5 text-muted-foreground hover:bg-accent hover:text-accent-foreground"
              aria-label="Close sidebar"
            >
              <X className="h-4 w-4" />
            </button>
          </div>
          <nav className="flex-1 px-3 py-4 space-y-0.5 overflow-y-auto">
            {navigation.map((item) => {
              const isActive = pathname === item.href
              return (
                <button
                  key={item.name}
                  onClick={() => { handleNavClick(item.href); onClose() }}
                  className={cn(
                    "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors w-full text-left",
                    isActive ? "bg-accent text-accent-foreground font-medium" : "text-muted-foreground hover:bg-muted hover:text-foreground",
                  )}
                >
                  <item.icon className="h-5 w-5" />
                  <span>{item.name}</span>
                  {item.badge && <Badge variant="secondary" className="ml-auto">{item.badge}</Badge>}
                </button>
              )
            })}
          </nav>
        </aside>
      )}
    </>
  )
}

export default Sidebar
