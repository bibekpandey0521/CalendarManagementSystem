/**
 * Calendar Management System - TypeScript Types
 * Shared types used across the frontend application
 */

export type EventType = 1 | 2; // 1 = Event, 2 = Meeting
export type EventStatus = 1 | 2 | 3; // 1 = Confirmed, 2 = Tentative, 3 = Cancelled
export type EventCategory = 1 | 2 | 3 | 4 | 5; // Work, Personal, Birthday, Holiday, Other
export type EventScope = 0 | 1 | 2; // 0 = All, 1 = Upcoming, 2 = Past

export interface CalendarEventDto {
  id: number;
  title: string;
  description?: string | null;
  startTime: string; // ISO 8601 UTC
  endTime: string; // ISO 8601 UTC
  isAllDay: boolean;
  location?: string | null;
  type: EventType;
  category: EventCategory;
  status: EventStatus;
  eventTypeName?: string;
  eventCategoryName?: string;
  eventStatusName?: string;
  reminderMinutesBefore?: number | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateEventDto {
  title: string;
  description?: string | null;
  startTime: string;
  endTime: string;
  isAllDay: boolean;
  location?: string | null;
  type: EventType;
  category: EventCategory;
  status: EventStatus;
  reminderMinutesBefore?: number | null;
}

export interface UpdateEventDto {
  title: string;
  description?: string | null;
  startTime: string;
  endTime: string;
  isAllDay: boolean;
  location?: string | null;
  type: EventType;
  category: EventCategory;
  status: EventStatus;
  reminderMinutesBefore?: number | null;
}

export interface EventQueryParameters {
  page?: number;
  pageSize?: number;
  sortBy?: "startTime" | "endTime" | "title" | "createdAt";
  sortDirection?: "asc" | "desc";
  type?: EventType;
  category?: EventCategory;
  status?: EventStatus;
  from?: string;
  to?: string;
  search?: string;
  hasReminder?: boolean;
  scope?: EventScope;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface DashboardSummary {
  totalEvents: number;
  upcomingEvents: number;
  todaysEvents: number;
  completedEvents: number;
  remindersSet: number;
  upcomingMeetings: number;
  completedMeetings: number;
  nextMeeting: CalendarEventDto | null;
}

export interface CalendarViewEvent {
  id: string | number;
  title: string;
  start: string;
  end: string;
  allDay?: boolean;
  location?: string | null;
  description?: string | null;
  category?: string;
  status?: string;
  color?: string;
}

export type ViewType = "dayGridMonth" | "timeGridWeek" | "timeGridDay" | "listDay";

export interface ApiError {
  type: string;
  title: string;
  status: number;
  detail: string;
  instance: string;
  traceId: string;
  errors?: Record<string, string[]>;
}

// Meeting Participant Types

export interface MeetingParticipantDto {
  id: number;
  meetingId: number;
  userId?: number | null;
  participantName?: string | null;
  email: string;
  createdAt: string;
}

export interface CreateMeetingParticipantDto {
  email: string;
  participantName?: string;
  userId?: number;
}

// Email Notification Types

export interface EmailNotificationLogDto {
  id: number;
  meetingId: number;
  participantId?: number | null;
  recipientEmail: string;
  notificationType: string;
  status: string;
  attemptCount: number;
  sentAt?: string | null;
  errorMessage?: string | null;
  createdAt: string;
}

// User Types

export interface UserProfileDto {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  isEmailVerified: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface UpdateProfileDto {
  firstName: string;
  lastName: string;
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface UserSettingsDto {
  id: number;
  userId: number;
  theme: string;
  notificationsEnabled: boolean;
  emailRemindersEnabled: boolean;
}

export interface UpdateSettingsDto {
  theme: string;
  notificationsEnabled: boolean;
  emailRemindersEnabled: boolean;
}

