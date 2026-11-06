"use client"

import { useState, useEffect } from "react"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import {
  Calendar as CalendarIcon,
  Clock,
  MapPin,
  Type,
  Users,
  Tag,
  X,
  Check,
} from "lucide-react"
import { apiService } from "@/components/services/api"
import type {
  CalendarEventDto,
  CreateEventDto,
  UpdateEventDto,
  EventType,
  EventCategory,
  EventStatus,
} from "@/types/calendar"

const EVENT_TYPES: { value: EventType; label: string }[] = [
  { value: 1, label: "Event" },
  { value: 2, label: "Meeting" },
]

interface CategoryOption {
  value: EventCategory;
  label: string;
  color: string;
}

interface StatusOption {
  value: EventStatus;
  label: string;
  color: string;
}

const EVENT_CATEGORIES: CategoryOption[] = [
  { value: 1, label: "Work", color: "bg-blue-100 text-blue-700" },
  { value: 2, label: "Personal", color: "bg-pink-100 text-pink-700" },
  { value: 3, label: "Birthday", color: "bg-amber-100 text-amber-700" },
  { value: 4, label: "Holiday", color: "bg-emerald-100 text-emerald-700" },
  { value: 5, label: "Other", color: "bg-gray-100 text-gray-700" },
]

const EVENT_STATUSES: StatusOption[] = [
  { value: 1, label: "Confirmed", color: "bg-emerald-100 text-emerald-700" },
  { value: 2, label: "Tentative", color: "bg-amber-100 text-amber-700" },
  { value: 3, label: "Cancelled", color: "bg-rose-100 text-rose-700" },
]

interface EventFormProps {
  open: boolean
  onClose: () => void
  onSuccess: () => void
  eventToEdit?: CalendarEventDto | null
}

export function EventFormDialog({
  open,
  onClose,
  onSuccess,
  eventToEdit,
}: EventFormProps) {
  const [isLoading, setIsLoading] = useState(false)
  const [formData, setFormData] = useState<CreateEventDto & UpdateEventDto>({
    title: eventToEdit?.title ?? "",
    description: eventToEdit?.description ?? "",
    startTime: eventToEdit?.startTime ?? "",
    endTime: eventToEdit?.endTime ?? "",
    isAllDay: eventToEdit?.isAllDay ?? false,
    location: eventToEdit?.location ?? "",
    type: (eventToEdit?.type as EventType) ?? 1,
    category: (eventToEdit?.category as EventCategory) ?? 1,
    status: (eventToEdit?.status as EventStatus) ?? 1,
    reminderMinutesBefore: eventToEdit?.reminderMinutesBefore ?? null,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    if (eventToEdit) {
      setFormData({
        title: eventToEdit.title,
        description: eventToEdit.description ?? "",
        startTime: eventToEdit.startTime,
        endTime: eventToEdit.endTime,
        isAllDay: eventToEdit.isAllDay,
        location: eventToEdit.location ?? "",
        type: eventToEdit.type as EventType,
        category: eventToEdit.category as EventCategory,
        status: eventToEdit.status as EventStatus,
        reminderMinutesBefore:
          eventToEdit.reminderMinutesBefore ?? null,
      })
    }
  }, [eventToEdit])

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {}
    if (!formData.title.trim()) {
      newErrors.title = "Title is required"
    } else if (formData.title.trim().length < 2) {
      newErrors.title = "Title must be at least 2 characters"
    }
    if (!formData.startTime) {
      newErrors.startTime = "Start time is required"
    }
    if (!formData.endTime) {
      newErrors.endTime = "End time is required"
    }
    if (formData.startTime && formData.endTime && formData.endTime <= formData.startTime) {
      newErrors.endTime = "End time must be after start time"
    }
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    setIsLoading(true)
    try {
      if (eventToEdit) {
        await apiService.updateEvent(eventToEdit.id, formData)
      } else {
        await apiService.createEvent(formData)
      }
      onSuccess()
      onClose()
    } catch (err: any) {
      setErrors({
        ...errors,
        submit: err.detail || "Failed to save event",
      })
    } finally {
      setIsLoading(false)
    }
  }

  const formatDateTimeForInput = (isoString?: string): string => {
    if (!isoString) return ""
    return new Date(isoString).toISOString().slice(0, 16)
  }

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogTrigger asChild>
        <Button variant="outline" size="sm" disabled={isLoading}>
          {eventToEdit ? "Edit Event" : "Create Event"}
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>
            {eventToEdit ? "Edit Event" : "Create New Event"}
          </DialogTitle>
          <DialogDescription>
            Fill in the event details below.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4 pt-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="title">Title</Label>
              <Input
                id="title"
                value={formData.title}
                onChange={(e) =>
                  setFormData({ ...formData, title: e.target.value })
                }
                placeholder="e.g. Team stand-up"
                className={errors.title ? "border-red-500" : ""}
              />
              {errors.title && (
                <p className="text-xs text-red-600">{errors.title}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label htmlFor="type">Event Type</Label>
              <select
                id="type"
                value={String(formData.type ?? "")}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    type: Number(e.target.value) as EventType,
                  })
                }
                className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              >
                {EVENT_TYPES.map((t) => (
                  <option key={t.value} value={String(t.value)}>
                    {t.label}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="description">Description</Label>
            <Input
              id="description"
              value={formData.description ?? ""}
              onChange={(e) =>
                setFormData({ ...formData, description: e.target.value })
              }
              placeholder="Add details about this event"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="startTime">Start Date & Time</Label>
              <Input
                id="startTime"
                type="datetime-local"
                value={formatDateTimeForInput(formData.startTime)}
                onChange={(e) =>
                  setFormData({ ...formData, startTime: e.target.value })
                }
                className={errors.startTime ? "border-red-500" : ""}
              />
              {errors.startTime && (
                <p className="text-xs text-red-600">{errors.startTime}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label htmlFor="endTime">End Date & Time</Label>
              <Input
                id="endTime"
                type="datetime-local"
                value={formatDateTimeForInput(formData.endTime)}
                onChange={(e) =>
                  setFormData({ ...formData, endTime: e.target.value })
                }
                className={errors.endTime ? "border-red-500" : ""}
              />
              {errors.endTime && (
                <p className="text-xs text-red-600">{errors.endTime}</p>
              )}
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="category">Category</Label>
              <select
                id="category"
                value={String(formData.category ?? "")}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    category: Number(e.target.value) as EventCategory,
                  })
                }
                className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              >
                {EVENT_CATEGORIES.map((c) => (
                  <option key={c.value} value={String(c.value)}>
                    {c.label}
                  </option>
                ))}
              </select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="status">Status</Label>
              <select
                id="status"
                value={String(formData.status ?? "")}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    status: Number(e.target.value) as EventStatus,
                  })
                }
                className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              >
                {EVENT_STATUSES.map((s) => (
                  <option key={s.value} value={String(s.value)}>
                    {s.label}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="location">Location</Label>
            <Input
              id="location"
              value={formData.location ?? ""}
              onChange={(e) =>
                setFormData({ ...formData, location: e.target.value })
              }
              placeholder="e.g. Conference Room A or Zoom link"
            />
          </div>

          <div className="flex items-center gap-2">
            <input
              id="isAllDay"
              type="checkbox"
              checked={formData.isAllDay}
              onChange={(e) =>
                setFormData({ ...formData, isAllDay: e.target.checked })
              }
              className="rounded border-input bg-background text-primary focus:ring-primary"
            />
            <Label htmlFor="isAllDay">All-day event</Label>
          </div>

          {errors.submit && (
            <div className="rounded-lg bg-red-50 p-3 text-sm text-red-700">
              {errors.submit}
            </div>
          )}

          <div className="flex flex-row-reverse gap-2 mt-6">
            <Button
              type="submit"
              disabled={isLoading}
              className="w-full"
            >
              {isLoading ? "Saving..." : "Save Event"}
            </Button>
            <Button
              type="button"
              variant="outline"
              onClick={onClose}
              disabled={isLoading}
            >
              Cancel
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  )
}
