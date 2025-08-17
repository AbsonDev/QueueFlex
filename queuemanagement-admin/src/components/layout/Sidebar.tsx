import React from 'react';
import { NavLink } from 'react-router-dom';
import { useUIStore } from '@/stores/ui';
import { Button } from '@/components/ui/Button';
import { Badge } from '@/components/ui/Badge';
import { cn } from '@/lib/utils';
import {
  LayoutDashboard,
  Users,
  Clock,
  Ticket,
  Building2,
  Settings,
  BarChart3,
  ChevronLeft,
  ChevronRight,
  Bell,
} from 'lucide-react';

interface NavItem {
  title: string;
  href: string;
  icon: React.ComponentType<{ className?: string }>;
  badge?: string | number;
  children?: Omit<NavItem, 'children'>[];
}

const navigation: NavItem[] = [
  {
    title: 'Dashboard',
    href: '/dashboard',
    icon: LayoutDashboard,
  },
  {
    title: 'Queues',
    href: '/queues',
    icon: Clock,
    badge: '12',
  },
  {
    title: 'Tickets',
    href: '/tickets',
    icon: Ticket,
    badge: '45',
  },
  {
    title: 'Sessions',
    href: '/sessions',
    icon: Users,
    badge: '8',
  },
  {
    title: 'Users',
    href: '/users',
    icon: Users,
    badge: '24',
  },
  {
    title: 'Units',
    href: '/units',
    icon: Building2,
    badge: '6',
  },
  {
    title: 'Analytics',
    href: '/analytics',
    icon: BarChart3,
  },
  {
    title: 'Settings',
    href: '/settings',
    icon: Settings,
  },
];

export const Sidebar: React.FC = () => {
  const { sidebarState, toggleSidebar } = useUIStore();
  const isCollapsed = sidebarState === 'collapsed';

  return (
    <div className="flex h-full w-full flex-col bg-card border-r">
      {/* Header */}
      <div className="flex h-16 items-center justify-between px-4 border-b">
        {!isCollapsed && (
          <div className="flex items-center space-x-2">
            <div className="h-8 w-8 bg-primary rounded-lg flex items-center justify-center">
              <span className="text-sm font-bold text-white">QM</span>
            </div>
            <span className="font-semibold">Queue Management</span>
          </div>
        )}
        
        <Button
          variant="ghost"
          size="icon"
          onClick={toggleSidebar}
          className="h-8 w-8"
        >
          {isCollapsed ? <ChevronRight className="h-4 w-4" /> : <ChevronLeft className="h-4 w-4" />}
        </Button>
      </div>

      {/* Navigation */}
      <nav className="flex-1 space-y-1 p-2">
        {navigation.map((item) => (
          <NavLink
            key={item.href}
            to={item.href}
            className={({ isActive }) =>
              cn(
                'flex items-center space-x-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors hover:bg-accent hover:text-accent-foreground',
                isActive ? 'bg-accent text-accent-foreground' : 'text-muted-foreground',
                isCollapsed && 'justify-center'
              )
            }
          >
            <item.icon className="h-5 w-5 flex-shrink-0" />
            {!isCollapsed && (
              <>
                <span className="flex-1">{item.title}</span>
                {item.badge && (
                  <Badge variant="secondary" className="ml-auto">
                    {item.badge}
                  </Badge>
                )}
              </>
            )}
          </NavLink>
        ))}
      </nav>

      {/* Footer */}
      {!isCollapsed && (
        <div className="border-t p-4">
          <div className="rounded-lg bg-muted p-3">
            <div className="flex items-center space-x-2">
              <Bell className="h-4 w-4 text-muted-foreground" />
              <div className="flex-1">
                <p className="text-xs font-medium">New features available</p>
                <p className="text-xs text-muted-foreground">
                  Check out the latest updates
                </p>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};