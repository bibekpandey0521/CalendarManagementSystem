/**
 * Calendar Management System - API Service Layer
 * Axios-based service for communicating with the ASP.NET Core Web API
 */

import axios, { AxiosInstance, AxiosResponse } from "axios";
import type {
  CalendarEventDto,
  CreateEventDto,
  UpdateEventDto,
  EventQueryParameters,
  PagedResult,
  DashboardSummary,
  ApiError,
} from "../../types/calendar";

const API_BASE =
  process.env.NEXT_PUBLIC_API_BASE || "http://localhost:5050";

class ApiService {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE,
      headers: {
        "Content-Type": "application/json",
      },
      timeout: 30000,
    });

    // Response error interceptor for consistent error handling
    this.client.interceptors.response.use(
      (response: AxiosResponse) => response,
      (error) => {
        if (error.response?.data) {
          const apiError = error.response.data as ApiError;
          return Promise.reject(apiError);
        }
        return Promise.reject({
          type: "client-error",
          title: "Network Error",
          status: 0,
          detail: error.message || "An unexpected error occurred",
          instance: "",
          traceId: "",
        } as ApiError);
      }
    );
  }

  // ===== Events =====

  async getEvents(
    params: EventQueryParameters = {}
  ): Promise<PagedResult<CalendarEventDto>> {
    const response = await this.client.get<PagedResult<CalendarEventDto>>(
      "/api/events",
      { params }
    );
    return response.data;
  }

  async getEventById(id: number): Promise<CalendarEventDto> {
    const response = await this.client.get<CalendarEventDto>(
      `/api/events/${id}`
    );
    return response.data;
  }

  async createEvent(dto: CreateEventDto): Promise<CalendarEventDto> {
    const response = await this.client.post<CalendarEventDto>(
      "/api/events",
      dto
    );
    return response.data;
  }

  async updateEvent(
    id: number,
    dto: UpdateEventDto
  ): Promise<CalendarEventDto> {
    const response = await this.client.put<CalendarEventDto>(
      `/api/events/${id}`,
      dto
    );
    return response.data;
  }

  async deleteEvent(id: number): Promise<void> {
    await this.client.delete(`/api/events/${id}`);
  }

  async getUpcomingEvents(
    limit: number = 5,
    meetingsOnly: boolean = false
  ): Promise<CalendarEventDto[]> {
    const response = await this.client.get<CalendarEventDto[]>(
      "/api/events/upcoming",
      { params: { limit, meetingsOnly } }
    );
    return response.data;
  }

  async getPastEvents(
    limit: number = 5,
    meetingsOnly: boolean = false
  ): Promise<CalendarEventDto[]> {
    const response = await this.client.get<CalendarEventDto[]>(
      "/api/events/past",
      { params: { limit, meetingsOnly } }
    );
    return response.data;
  }

  // ===== Dashboard =====

  async getDashboardSummary(): Promise<DashboardSummary> {
    const response = await this.client.get<DashboardSummary>(
      "/api/dashboard/summary"
    );
    return response.data;
  }

  // ===== Health =====

  async healthCheck(): Promise<string> {
    const response = await this.client.get("/health");
    return response.data;
  }
}

export const apiService = new ApiService();
export default apiService;
