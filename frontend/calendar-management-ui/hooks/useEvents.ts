/**
 * Calendar Management System - React Hooks
 * Custom hooks for event and dashboard data management
 */

import { useState, useCallback, useEffect } from "react";
import type {
  CalendarEventDto,
  CreateEventDto,
  UpdateEventDto,
  EventQueryParameters,
  PagedResult,
  DashboardSummary,
  ApiError,
} from "../types/calendar";
import { apiService } from "@/components/services/api";

/**
 * Hook for managing a list of calendar events with pagination, filtering, and search
 */
export function useEvents(initialParams: EventQueryParameters = {}) {
  const [events, setEvents] = useState<CalendarEventDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(initialParams.page ?? 1);
  const [pageSize, setPageSize] = useState(initialParams.pageSize ?? 20);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<ApiError | null>(null);

  const fetchEvents = useCallback(
    async (params?: EventQueryParameters) => {
      setLoading(true);
      setError(null);
      try {
        const requestParams: EventQueryParameters = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          ...params,
        };
        const result: PagedResult<CalendarEventDto> =
          await apiService.getEvents(requestParams);
        setEvents(result.items);
        setTotalCount(result.totalCount);
        setPage(result.page);
        setPageSize(result.pageSize);
      } catch (err: any) {
        setError(err as ApiError);
      } finally {
        setLoading(false);
      }
    },
    [
      page,
      pageSize,
      initialParams.search,
      initialParams.type,
      initialParams.category,
      initialParams.status,
      initialParams.scope,
      initialParams.from,
      initialParams.to,
      initialParams.hasReminder,
    ]
  );

  useEffect(() => {
    fetchEvents();
  }, [
    fetchEvents,
    initialParams.search,
    initialParams.type,
    initialParams.category,
    initialParams.status,
    initialParams.scope,
    initialParams.from,
    initialParams.to,
    initialParams.hasReminder,
  ]);

  const refetch = useCallback(() => {
    fetchEvents();
  }, [fetchEvents]);

  const createEvent = async (
    dto: CreateEventDto
  ): Promise<CalendarEventDto | null> => {
    setLoading(true);
    setError(null);
    try {
      const created = await apiService.createEvent(dto);
      setEvents((prev) => [created, ...prev]);
      setTotalCount((prev) => prev + 1);
      return created;
    } catch (err: any) {
      setError(err as ApiError);
      return null;
    } finally {
      setLoading(false);
    }
  };

  const updateEvent = async (
    id: number,
    dto: UpdateEventDto
  ): Promise<CalendarEventDto | null> => {
    setLoading(true);
    setError(null);
    try {
      const updated = await apiService.updateEvent(id, dto);
      setEvents((prev) =>
        prev.map((e) => (e.id === id ? updated : e))
      );
      return updated;
    } catch (err: any) {
      setError(err as ApiError);
      return null;
    } finally {
      setLoading(false);
    }
  };

  const deleteEvent = async (id: number): Promise<boolean> => {
    setLoading(true);
    setError(null);
    try {
      await apiService.deleteEvent(id);
      setEvents((prev) => prev.filter((e) => e.id !== id));
      setTotalCount((prev) => Math.max(0, prev - 1));
      return true;
    } catch (err: any) {
      setError(err as ApiError);
      return false;
    } finally {
      setLoading(false);
    }
  };

  return {
    events,
    totalCount,
    page,
    pageSize,
    loading,
    error,
    refetch,
    setPage,
    setPageSize,
    fetchEvents,
    createEvent,
    updateEvent,
    deleteEvent,
  };
}

/**
 * Hook for fetching a single event by ID
 */
export function useEvent(id: number | null) {
  const [event, setEvent] = useState<CalendarEventDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<ApiError | null>(null);

  const fetchEvent = useCallback(async () => {
    if (!id) return;
    setLoading(true);
    setError(null);
    try {
      const evt = await apiService.getEventById(id);
      setEvent(evt);
    } catch (err: any) {
      setError(err as ApiError);
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    fetchEvent();
  }, [fetchEvent]);

  return { event, loading, error, refetch: fetchEvent };
}

/**
 * Hook for dashboard summary data
 */
export function useDashboard() {
  const [summary, setSummary] = useState<DashboardSummary | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<ApiError | null>(null);

  const fetchSummary = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await apiService.getDashboardSummary();
      setSummary(data);
    } catch (err: any) {
      setError(err as ApiError);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchSummary();
  }, [fetchSummary]);

  return { summary, loading, error, refetch: fetchSummary };
}

/**
 * Hook for upcoming events
 */
export function useUpcomingEvents(limit: number = 5, meetingsOnly: boolean = false) {
  const [events, setEvents] = useState<CalendarEventDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<ApiError | null>(null);

  const fetchEvents = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await apiService.getUpcomingEvents(limit, meetingsOnly);
      setEvents(data);
    } catch (err: any) {
      setError(err as ApiError);
    } finally {
      setLoading(false);
    }
  }, [limit, meetingsOnly]);

  useEffect(() => {
    fetchEvents();
  }, [fetchEvents]);

  return { events, loading, error, refetch: fetchEvents };
}

/**
 * Hook for past events
 */
export function usePastEvents(limit: number = 5, meetingsOnly: boolean = false) {
  const [events, setEvents] = useState<CalendarEventDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<ApiError | null>(null);

  const fetchEvents = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await apiService.getPastEvents(limit, meetingsOnly);
      setEvents(data);
    } catch (err: any) {
      setError(err as ApiError);
    } finally {
      setLoading(false);
    }
  }, [limit, meetingsOnly]);

  useEffect(() => {
    fetchEvents();
  }, [fetchEvents]);

  return { events, loading, error, refetch: fetchEvents };
}
