import React, { useEffect } from 'react';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import { useAuthStore } from '@/stores/auth';
import { useUIStore } from '@/stores/ui';
import { Sidebar } from './Sidebar';
import { Header } from './Header';
import { MobileSidebar } from './MobileSidebar';

export const Layout: React.FC = () => {
  const { isAuthenticated, user } = useAuthStore();
  const { sidebarState, isMobile, setIsMobile } = useUIStore();
  const location = useLocation();
  const navigate = useNavigate();

  // Check authentication
  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login');
    }
  }, [isAuthenticated, navigate]);

  // Handle mobile detection
  useEffect(() => {
    const handleResize = () => {
      const mobile = window.innerWidth < 768;
      setIsMobile(mobile);
      
      if (mobile && sidebarState !== 'mobile') {
        // Auto-close sidebar on mobile
        // This will be handled by the UI store
      }
    };

    handleResize();
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, [setIsMobile, sidebarState]);

  // Don't render layout for login page
  if (location.pathname === '/login') {
    return <Outlet />;
  }

  if (!isAuthenticated || !user) {
    return null;
  }

  return (
    <div className="min-h-screen bg-background">
      {/* Mobile Sidebar Overlay */}
      {isMobile && sidebarState === 'mobile' && (
        <div className="fixed inset-0 z-40 lg:hidden">
          <div className="fixed inset-0 bg-black/50" />
          <MobileSidebar />
        </div>
      )}

      {/* Desktop Sidebar */}
      {!isMobile && (
        <div
          className={cn(
            'fixed left-0 top-0 z-30 h-full transition-transform duration-300 ease-in-out',
            sidebarState === 'collapsed' ? 'w-16' : 'w-64'
          )}
        >
          <Sidebar />
        </div>
      )}

      {/* Main Content */}
      <div
        className={cn(
          'transition-all duration-300 ease-in-out',
          !isMobile && sidebarState === 'collapsed' ? 'ml-16' : 'ml-0',
          !isMobile && sidebarState === 'expanded' ? 'ml-64' : 'ml-0'
        )}
      >
        {/* Header */}
        <Header />

        {/* Page Content */}
        <main className="p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
};

// Helper function for conditional classes
function cn(...classes: (string | undefined | null | false)[]): string {
  return classes.filter(Boolean).join(' ');
}