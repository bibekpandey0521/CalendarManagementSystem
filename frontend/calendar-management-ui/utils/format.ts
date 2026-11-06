/**
 * Calendar Management System - Utility Functions
 * Date formatting, color mapping, and helper utilities
 */

/**
 * Format an ISO date string to a readable date string
 */
export function formatDate(isoString: string | null | undefined): string {
  if (!isoString) return "-";
  try {
    return new Date(isoString).toLocaleDateString(undefined, {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  } catch {
    return "-";
  }
}

/**
 * Format an ISO date string to a readable time string
 */
export function formatTime(isoString: string | null | undefined): string {
  if (!isoString) return "-";
  try {
    return new Date(isoString).toLocaleTimeString(undefined, {
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch {
    return "-";
  }
}

/**
 * Format an ISO date string to a full datetime string
 */
export function formatDateTime(isoString: string | null | undefined): string {
  if (!isoString) return "-";
  try {
    return new Date(isoString).toLocaleString(undefined, {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch {
    return "-";
  }
}

/**
 * Get a display color for an event category
 */
export function getCategoryColor(category: number): string {
  const colors: Record<number, string> = {
    1: "#3b82f6", // Work - blue
    2: "#ec4899", // Personal - pink
    3: "#f59e0b", // Birthday - amber
    4: "#10b981", // Holiday - emerald
    5: "#6b7280", // Other - gray
  };
  return colors[category] ?? "#6b7280";
}

/**
 * Get a display badge variant for an event status
 */
export function getStatusBadge(status: number): {
  label: string;
  color: string;
} {
  const statuses: Record<number, { label: string; color: string }> = {
    1: { label: "Confirmed", color: "#10b981" }, // green
    2: { label: "Tentative", color: "#f59e0b" }, // amber
    3: { label: "Cancelled", color: "#ef4444" }, // red
  };
  return (
    statuses[status] ?? { label: "Unknown", color: "#6b7280" }
  );
}

/**
 * Get a display label for an event type
 */
export function getEventTypeLabel(type: number): string {
  const types: Record<number, string> = {
    1: "Event",
    2: "Meeting",
  };
  return types[type] ?? "Event";
}

/**
 * Check if an event is happening today
 */
export function isToday(isoString: string | null | undefined): boolean {
  if (!isoString) return false;
  try {
    const eventDate = new Date(isoString);
    const today = new Date();
    return (
      eventDate.getDate() === today.getDate() &&
      eventDate.getMonth() === today.getMonth() &&
      eventDate.getFullYear() === today.getFullYear()
    );
  } catch {
    return false;
  }
}

/**
 * Check if an event is upcoming (in the future)
 */
export function isUpcoming(isoString: string | null | undefined): boolean {
  if (!isoString) return false;
  try {
    return new Date(isoString) > new Date();
  } catch {
    return false;
  }
}

/**
 * Convert minutes to a readable duration string
 */
export function formatDuration(minutes: number | null | undefined): string {
  if (!minutes && minutes !== 0) return "-";
  if (minutes < 60) return `${minutes} min`;
  const hours = Math.floor(minutes / 60);
  const mins = minutes % 60;
  return mins > 0 ? `${hours}h ${mins}m` : `${hours}h`;
}

/**
 * Truncate a string with ellipsis
 */
export function truncate(text: string, maxLength: number): string {
  if (!text) return "";
  if (text.length <= maxLength) return text;
  return `${text.slice(0, maxLength)}…`;
}

/**
 * Generate a random color for calendar events
 */
export function getRandomEventColor(): string {
  const colors = [
    "#3b82f6", // blue
    "#ec4899", // pink
    "#f59e0b", // amber
    "#10b981", // emerald
    "#8b5cf6", // violet
    "#ef4444", // red
    "#06b6d4", // cyan
    "#84cc16", // lime
  ];
  return colors[Math.floor(Math.random() * colors.length)];
}

/**
 * Debounce function for search inputs
 */
export function debounce<T extends (...args: any[]) => any>(
  func: T,
  wait: number
): (...args: Parameters<T>) => void {
  let timeout: NodeJS.Timeout | null = null;
  return (...args: Parameters<T>) => {
    if (timeout) clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
}
