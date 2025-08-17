import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { User, Tenant, LoginForm } from '@/types/api';

interface AuthState {
  user: User | null;
  tenant: Tenant | null;
  token: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

interface AuthActions {
  login: (credentials: LoginForm) => Promise<void>;
  logout: () => void;
  refreshAuth: () => Promise<void>;
  setUser: (user: User) => void;
  setTenant: (tenant: Tenant) => void;
  setToken: (token: string) => void;
  setRefreshToken: (refreshToken: string) => void;
  setError: (error: string | null) => void;
  setLoading: (loading: boolean) => void;
  clearError: () => void;
}

type AuthStore = AuthState & AuthActions;

export const useAuthStore = create<AuthStore>()(
  persist(
    (set, get) => ({
      // Initial state
      user: null,
      tenant: null,
      token: null,
      refreshToken: null,
      isAuthenticated: false,
      isLoading: false,
      error: null,

      // Actions
      login: async (credentials: LoginForm) => {
        set({ isLoading: true, error: null });
        try {
          // This will be implemented with the API client
          // For now, we'll simulate a successful login
          const mockUser: User = {
            id: '1',
            email: credentials.email,
            firstName: 'Admin',
            lastName: 'User',
            fullName: 'Admin User',
            isActive: true,
            role: 'admin',
            permissions: [
              { resource: '*', actions: ['*'] }
            ],
            settings: {
              theme: 'auto',
              language: 'en',
              notifications: {
                queueUpdates: true,
                ticketAssignments: true,
                sessionReminders: true,
                performanceReports: true,
              }
            },
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString(),
          };

          const mockTenant: Tenant = {
            id: '1',
            name: 'Demo Company',
            subdomain: credentials.tenant || 'demo',
            isActive: true,
            settings: {
              theme: 'auto',
              language: 'en',
              timezone: 'UTC',
              businessHours: {
                monday: { isOpen: true, openTime: '09:00', closeTime: '18:00' },
                tuesday: { isOpen: true, openTime: '09:00', closeTime: '18:00' },
                wednesday: { isOpen: true, openTime: '09:00', closeTime: '18:00' },
                thursday: { isOpen: true, openTime: '09:00', closeTime: '18:00' },
                friday: { isOpen: true, openTime: '09:00', closeTime: '18:00' },
                saturday: { isOpen: false, openTime: '09:00', closeTime: '18:00' },
                sunday: { isOpen: false, openTime: '09:00', closeTime: '18:00' },
              },
              notifications: {
                email: true,
                push: true,
                sms: false,
                webhook: false,
              }
            },
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString(),
          };

          const mockToken = 'mock-jwt-token-' + Date.now();
          const mockRefreshToken = 'mock-refresh-token-' + Date.now();

          set({
            user: mockUser,
            tenant: mockTenant,
            token: mockToken,
            refreshToken: mockRefreshToken,
            isAuthenticated: true,
            isLoading: false,
            error: null,
          });
        } catch (error) {
          set({
            error: error instanceof Error ? error.message : 'Login failed',
            isLoading: false,
          });
          throw error;
        }
      },

      logout: () => {
        set({
          user: null,
          tenant: null,
          token: null,
          refreshToken: null,
          isAuthenticated: false,
          error: null,
        });
      },

      refreshAuth: async () => {
        const { refreshToken } = get();
        if (!refreshToken) {
          get().logout();
          return;
        }

        set({ isLoading: true });
        try {
          // This will be implemented with the API client
          // For now, we'll simulate a successful refresh
          const mockToken = 'mock-jwt-token-' + Date.now();
          const mockRefreshToken = 'mock-refresh-token-' + Date.now();

          set({
            token: mockToken,
            refreshToken: mockRefreshToken,
            isLoading: false,
          });
        } catch (error) {
          set({
            error: error instanceof Error ? error.message : 'Token refresh failed',
            isLoading: false,
          });
          get().logout();
        }
      },

      setUser: (user: User) => set({ user }),
      setTenant: (tenant: Tenant) => set({ tenant }),
      setToken: (token: string) => set({ token }),
      setRefreshToken: (refreshToken: string) => set({ refreshToken }),
      setError: (error: string | null) => set({ error }),
      setLoading: (loading: boolean) => set({ isLoading: loading }),
      clearError: () => set({ error: null }),
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({
        user: state.user,
        tenant: state.tenant,
        token: state.token,
        refreshToken: state.refreshToken,
        isAuthenticated: state.isAuthenticated,
      }),
    }
  )
);