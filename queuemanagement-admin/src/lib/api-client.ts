import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse, AxiosError, InternalAxiosRequestConfig } from 'axios';
import { useAuthStore } from '@/stores/auth';

// Create axios instance
const apiClient: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api/v1',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = useAuthStore.getState().token;
    if (token) {
      config.headers.set('Authorization', `Bearer ${token}`);
    }

    // Add tenant header if available
    const tenant = useAuthStore.getState().tenant;
    if (tenant?.subdomain) {
      config.headers.set('X-Tenant', tenant.subdomain);
    }

    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle auth errors and token refresh
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    return response;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as any;

    // Handle 401 Unauthorized errors
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        // Try to refresh the token
        await useAuthStore.getState().refreshAuth();
        
        // Retry the original request with new token
        const newToken = useAuthStore.getState().token;
        if (newToken) {
          originalRequest.headers.set('Authorization', `Bearer ${newToken}`);
          return apiClient(originalRequest);
        }
      } catch (refreshError) {
        // If refresh fails, logout the user
        useAuthStore.getState().logout();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    // Handle other errors
    if (error.response?.data) {
      const errorData = error.response.data as any;
      const errorMessage = errorData.message || errorData.error || 'An error occurred';
      
      // You can add toast notifications here if needed
      console.error('API Error:', errorMessage);
    }

    return Promise.reject(error);
  }
);

// API response wrapper
export const apiResponse = <T>(response: AxiosResponse<T>) => {
  return response.data;
};

// Error handler
export const handleApiError = (error: any): string => {
  if (axios.isAxiosError(error)) {
    if (error.response?.data?.message) {
      return error.response.data.message;
    }
    if (error.response?.status === 404) {
      return 'Resource not found';
    }
    if (error.response?.status === 500) {
      return 'Internal server error';
    }
    if (error.code === 'ECONNABORTED') {
      return 'Request timeout';
    }
    return error.message || 'Network error';
  }
  return error.message || 'An unexpected error occurred';
};

// Export the configured client
export default apiClient;

// Export commonly used HTTP methods with proper typing
export const api = {
  get: <T>(url: string, config?: AxiosRequestConfig) =>
    apiClient.get<T>(url, config).then(apiResponse),
  
  post: <T>(url: string, data?: any, config?: AxiosRequestConfig) =>
    apiClient.post<T>(url, data, config).then(apiResponse),
  
  put: <T>(url: string, data?: any, config?: AxiosRequestConfig) =>
    apiClient.put<T>(url, data, config).then(apiResponse),
  
  patch: <T>(url: string, data?: any, config?: AxiosRequestConfig) =>
    apiClient.patch<T>(url, data, config).then(apiResponse),
  
  delete: <T>(url: string, config?: AxiosRequestConfig) =>
    apiClient.delete<T>(url, config).then(apiResponse),
};