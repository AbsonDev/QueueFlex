import { create } from 'zustand';
import { persist } from 'zustand/middleware';

type Theme = 'light' | 'dark' | 'auto';
type SidebarState = 'expanded' | 'collapsed' | 'mobile';

interface UIState {
  theme: Theme;
  sidebarState: SidebarState;
  isMobile: boolean;
  notifications: Notification[];
  modals: ModalState;
}

interface Notification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  timestamp: Date;
  read: boolean;
}

interface ModalState {
  isOpen: boolean;
  type: string | null;
  data: any;
}

interface UIActions {
  setTheme: (theme: Theme) => void;
  setSidebarState: (state: SidebarState) => void;
  setIsMobile: (isMobile: boolean) => void;
  addNotification: (notification: Omit<Notification, 'id' | 'timestamp' | 'read'>) => void;
  removeNotification: (id: string) => void;
  markNotificationAsRead: (id: string) => void;
  clearNotifications: () => void;
  openModal: (type: string, data?: any) => void;
  closeModal: () => void;
  toggleSidebar: () => void;
}

type UIStore = UIState & UIActions;

export const useUIStore = create<UIStore>()(
  persist(
    (set, get) => ({
      // Initial state
      theme: 'auto',
      sidebarState: 'expanded',
      isMobile: false,
      notifications: [],
      modals: {
        isOpen: false,
        type: null,
        data: null,
      },

      // Actions
      setTheme: (theme: Theme) => set({ theme }),
      
      setSidebarState: (state: SidebarState) => set({ sidebarState: state }),
      
      setIsMobile: (isMobile: boolean) => set({ isMobile }),
      
      addNotification: (notification) => {
        const newNotification: Notification = {
          ...notification,
          id: Date.now().toString(),
          timestamp: new Date(),
          read: false,
        };
        
        set((state) => ({
          notifications: [newNotification, ...state.notifications].slice(0, 10), // Keep only last 10
        }));
      },
      
      removeNotification: (id: string) => {
        set((state) => ({
          notifications: state.notifications.filter((n) => n.id !== id),
        }));
      },
      
      markNotificationAsRead: (id: string) => {
        set((state) => ({
          notifications: state.notifications.map((n) =>
            n.id === id ? { ...n, read: true } : n
          ),
        }));
      },
      
      clearNotifications: () => set({ notifications: [] }),
      
      openModal: (type: string, data?: any) => {
        set({
          modals: {
            isOpen: true,
            type,
            data,
          },
        });
      },
      
      closeModal: () => {
        set({
          modals: {
            isOpen: false,
            type: null,
            data: null,
          },
        });
      },
      
      toggleSidebar: () => {
        const { sidebarState } = get();
        if (sidebarState === 'expanded') {
          set({ sidebarState: 'collapsed' });
        } else if (sidebarState === 'collapsed') {
          set({ sidebarState: 'expanded' });
        }
      },
    }),
    {
      name: 'ui-storage',
      partialize: (state) => ({
        theme: state.theme,
        sidebarState: state.sidebarState,
      }),
    }
  )
);

// Helper functions
export const getSystemTheme = (): 'light' | 'dark' => {
  if (typeof window === 'undefined') return 'light';
  return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
};

export const getEffectiveTheme = (theme: Theme): 'light' | 'dark' => {
  if (theme === 'auto') {
    return getSystemTheme();
  }
  return theme;
};