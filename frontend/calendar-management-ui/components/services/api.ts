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
  UserProfileDto,
  UpdateProfileDto,
  ChangePasswordDto,
  UserSettingsDto,
  UpdateSettingsDto,
  MeetingParticipantDto,
  EmailNotificationLogDto,
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

  // ===== User =====

  async getUserProfile(): Promise<UserProfileDto> {
    const response = await this.client.get<UserProfileDto>("/api/users/profile");
    return response.data;
  }

  async updateProfile(dto: UpdateProfileDto): Promise<void> {
    await this.client.put("/api/users/profile", dto);
  }

  async changePassword(dto: ChangePasswordDto): Promise<void> {
    await this.client.put("/api/users/password", dto);
  }

  async getUserSettings(): Promise<UserSettingsDto> {
    const response = await this.client.get<UserSettingsDto>(
      "/api/users/settings"
    );
    return response.data;
  }

  async updateSettings(dto: UpdateSettingsDto): Promise<void> {
    await this.client.put("/api/users/settings", dto);
  }

  async deleteAccount(): Promise<void> {
    await this.client.delete("/api/users/account");
  }

  // ===== Health =====

  async healthCheck(): Promise<string> {
    const response = await this.client.get("/health");
    return response.data;
  }

  // ===== AI Assistant =====

  async sendAIMessage(
    message: string,
    conversationId?: string | null
  ): Promise<{ message: string; conversationId: string }> {
    const response = await this.client.post<{ message: string; conversationId: string }>(
      "/api/ai/chat",
      { message, conversationId }
    );
    return response.data;
  }

  async getAIHistory(conversationId: string): Promise<any[]> {
    const response = await this.client.get<any[]>(
      `/api/ai/history/${conversationId}`
    );
    return response.data;
  }

  async clearAIHistory(conversationId: string): Promise<void> {
    await this.client.delete(`/api/ai/history/${conversationId}`);
  }

  // ===== Authentication =====

  async forgotPassword(email: string): Promise<{ message: string }> {
    const response = await this.client.post<{ message: string }>(
      "/api/auth/forgot-password",
      { email }
    );
    return response.data;
  }

  async resetPassword(
    email: string,
    token: string,
    newPassword: string,
    confirmPassword: string
  ): Promise<{ message: string }> {
    const response = await this.client.post<{ message: string }>(
      "/api/auth/reset-password",
      { email, token, newPassword, confirmPassword }
    );
    return response.data;
  }

  // ===== Meeting Participants =====

  async getMeetingParticipants(meetingId: number): Promise<MeetingParticipantDto[]> {
    const response = await this.client.get<MeetingParticipantDto[]>(
      `/api/meetings/${meetingId}/participants`
    );
    return response.data;
  }

  async addMeetingParticipant(
    meetingId: number,
    dto: { email: string; participantName?: string; userId?: number }
  ): Promise<number> {
    const response = await this.client.post<number>(
      `/api/meetings/${meetingId}/participants`,
      dto
    );
    return response.data;
  }

  async removeMeetingParticipants(meetingId: number): Promise<void> {
    await this.client.delete(`/api/meetings/${meetingId}/participants`);
  }

  async getMeetingNotifications(meetingId: number): Promise<EmailNotificationLogDto[]> {
    const response = await this.client.get<EmailNotificationLogDto[]>(
      `/api/meetings/${meetingId}/notifications`
    );
    return response.data;
  }

  async getPendingNotifications(limit: number = 50): Promise<EmailNotificationLogDto[]> {
    const response = await this.client.get<EmailNotificationLogDto[]>(
      "/api/meetings/notifications/pending",
      { params: { limit } }
    );
    return response.data;
  }
}

export const apiService = new ApiService();
export default apiService;
