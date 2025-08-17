import { api } from '@/lib/api-client';
import { DashboardMetrics, ApiResponse } from '@/types/api';

export const dashboardService = {
  // Get dashboard overview
  getOverview: async (): Promise<DashboardMetrics> => {
    const response = await api.get<ApiResponse<DashboardMetrics>>('/dashboard/overview');
    return response.data;
  },

  // Get queue overview
  getQueueOverview: async (): Promise<DashboardMetrics['queueOverview']> => {
    const response = await api.get<ApiResponse<DashboardMetrics['queueOverview']>>('/dashboard/queues');
    return response.data;
  },

  // Get performance metrics
  getPerformanceMetrics: async (): Promise<DashboardMetrics['performanceMetrics']> => {
    const response = await api.get<ApiResponse<DashboardMetrics['performanceMetrics']>>('/dashboard/performance');
    return response.data;
  },

  // Get today's statistics
  getTodayStats: async (): Promise<DashboardMetrics['todayStats']> => {
    const response = await api.get<ApiResponse<DashboardMetrics['todayStats']>>('/dashboard/today');
    return response.data;
  },

  // Get metrics for a specific date range
  getMetricsByDateRange: async (startDate: string, endDate: string): Promise<Partial<DashboardMetrics>> => {
    const response = await api.get<ApiResponse<Partial<DashboardMetrics>>>('/dashboard/metrics', {
      params: { startDate, endDate },
    });
    return response.data;
  },

  // Get real-time queue status
  getRealTimeQueueStatus: async (): Promise<DashboardMetrics['queueOverview']> => {
    const response = await api.get<ApiResponse<DashboardMetrics['queueOverview']>>('/dashboard/queues/realtime');
    return response.data;
  },

  // Get active sessions count
  getActiveSessionsCount: async (): Promise<number> => {
    const response = await api.get<ApiResponse<number>>('/dashboard/sessions/active-count');
    return response.data;
  },

  // Get waiting tickets count
  getWaitingTicketsCount: async (): Promise<number> => {
    const response = await api.get<ApiResponse<number>>('/dashboard/tickets/waiting-count');
    return response.data;
  },

  // Get average wait time
  getAverageWaitTime: async (): Promise<number> => {
    const response = await api.get<ApiResponse<number>>('/dashboard/tickets/average-wait-time');
    return response.data;
  },

  // Get average service time
  getAverageServiceTime: async (): Promise<number> => {
    const response = await api.get<ApiResponse<number>>('/dashboard/sessions/average-service-time');
    return response.data;
  },
};