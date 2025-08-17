import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { dashboardService } from '@/services/dashboard.service';
import { useSignalR } from '@/hooks/useSignalR';
import { MetricCard } from '@/components/ui/MetricCard';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/Card';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { 
  Users, 
  Clock, 
  Ticket, 
  Building2, 
  Plus,
  MoreHorizontal
} from 'lucide-react';
import { formatDuration, formatNumber, getStatusColor } from '@/lib/utils';

export const Dashboard: React.FC = () => {
  // Fetch dashboard data
  const { data: metrics, isLoading, error } = useQuery({
    queryKey: ['dashboard'],
    queryFn: () => dashboardService.getOverview(),
    refetchInterval: 30000, // Refetch every 30 seconds
  });

  // SignalR connection for real-time updates
  useSignalR({
    hubUrl: '/hubs/dashboard',
    onDashboardUpdate: (data) => {
      // This will automatically update the React Query cache
      console.log('Dashboard updated via SignalR:', data);
    },
  });

  if (isLoading) {
    return <DashboardSkeleton />;
  }

  if (error) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <p className="text-lg font-medium text-destructive mb-2">
            Failed to load dashboard
          </p>
          <p className="text-muted-foreground">
            Please try refreshing the page
          </p>
        </div>
      </div>
    );
  }

  if (!metrics) {
    return null;
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Dashboard</h1>
          <p className="text-muted-foreground">
            Overview of your queue management system
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button>
            <Plus className="h-4 w-4 mr-2" />
            New Queue
          </Button>
          <Button variant="outline">
            <MoreHorizontal className="h-4 w-4" />
          </Button>
        </div>
      </div>

      {/* Metrics Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <MetricCard
          title="Active Queues"
          value={metrics.activeQueues}
          icon={Clock}
          trend={{ value: 12, isPositive: true }}
          description="Currently open queues"
        />
        <MetricCard
          title="Waiting Tickets"
          value={metrics.waitingTickets}
          icon={Ticket}
          trend={{ value: 8, isPositive: false }}
          description="Tickets in queue"
        />
        <MetricCard
          title="Active Sessions"
          value={metrics.activeSessions}
          icon={Users}
          trend={{ value: 15, isPositive: true }}
          description="Ongoing service sessions"
        />
        <MetricCard
          title="Total Users"
          value={metrics.totalUsers}
          icon={Building2}
          trend={{ value: 5, isPositive: true }}
          description="Registered users"
        />
      </div>

      {/* Performance Metrics */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Performance Overview</CardTitle>
            <CardDescription>
              Key performance indicators for today
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex items-center justify-between">
              <span className="text-sm font-medium">Average Wait Time</span>
              <div className="flex items-center space-x-2">
                <span className="text-2xl font-bold">
                  {formatDuration(metrics.averageWaitTime)}
                </span>
                <Badge variant="secondary">
                  {metrics.averageWaitTime < 15 ? 'Good' : 'High'}
                </Badge>
              </div>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-sm font-medium">Average Service Time</span>
              <div className="flex items-center space-x-2">
                <span className="text-2xl font-bold">
                  {formatDuration(metrics.averageServiceTime)}
                </span>
                <Badge variant="secondary">
                  {metrics.averageServiceTime < 20 ? 'Fast' : 'Normal'}
                </Badge>
              </div>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-sm font-medium">Customer Satisfaction</span>
              <div className="flex items-center space-x-2">
                <span className="text-2xl font-bold">
                  {metrics.todayStats.customerSatisfaction}%
                </span>
                <Badge variant="success">
                  {metrics.todayStats.customerSatisfaction > 80 ? 'Excellent' : 'Good'}
                </Badge>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Today's Statistics</CardTitle>
            <CardDescription>
              Summary of today's activities
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="text-center p-4 bg-green-50 rounded-lg">
                <p className="text-2xl font-bold text-green-600">
                  {formatNumber(metrics.todayStats.ticketsServed)}
                </p>
                <p className="text-sm text-green-600">Tickets Served</p>
              </div>
              <div className="text-center p-4 bg-blue-50 rounded-lg">
                <p className="text-2xl font-bold text-blue-600">
                  {formatNumber(metrics.todayStats.ticketsCreated)}
                </p>
                <p className="text-sm text-blue-600">Tickets Created</p>
              </div>
              <div className="text-center p-4 bg-yellow-50 rounded-lg">
                <p className="text-2xl font-bold text-yellow-600">
                  {formatNumber(metrics.todayStats.ticketsCancelled)}
                </p>
                <p className="text-sm text-yellow-600">Cancelled</p>
              </div>
              <div className="text-center p-4 bg-purple-50 rounded-lg">
                <p className="text-2xl font-bold text-purple-600">
                  {formatNumber(metrics.performanceMetrics.totalSessions)}
                </p>
                <p className="text-sm text-purple-600">Sessions</p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Queue Overview */}
      <Card>
        <CardHeader>
          <CardTitle>Queue Overview</CardTitle>
          <CardDescription>
            Real-time status of all queues
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {metrics.queueOverview.map((queue) => (
              <div
                key={queue.queueId}
                className="p-4 border rounded-lg hover:shadow-md transition-shadow"
              >
                <div className="flex items-center justify-between mb-2">
                  <h4 className="font-medium">{queue.queueName}</h4>
                  <Badge className={getStatusColor(queue.status)}>
                    {queue.status}
                  </Badge>
                </div>
                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Current:</span>
                    <span className="font-medium">{queue.currentCount}/{queue.capacity}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Wait Time:</span>
                    <span className="font-medium">{formatDuration(queue.averageWaitTime)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Estimated:</span>
                    <span className="font-medium">{formatDuration(queue.estimatedWaitTime)}</span>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Top Performers */}
      <Card>
        <CardHeader>
          <CardTitle>Top Performers</CardTitle>
          <CardDescription>
            Best performing attendants this week
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {metrics.performanceMetrics.topPerformers.map((performer, index) => (
              <div
                key={performer.userId}
                className="flex items-center justify-between p-4 border rounded-lg"
              >
                <div className="flex items-center space-x-4">
                  <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                    <span className="text-sm font-medium text-primary">
                      {index + 1}
                    </span>
                  </div>
                  <div>
                    <p className="font-medium">{performer.userName}</p>
                    <p className="text-sm text-muted-foreground">
                      {formatNumber(performer.ticketsServed)} tickets served
                    </p>
                  </div>
                </div>
                <div className="text-right">
                  <div className="flex items-center space-x-2">
                    <span className="text-sm font-medium">
                      {formatDuration(performer.averageServiceTime)}
                    </span>
                    <Badge variant="success">
                      {performer.rating}/5
                    </Badge>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

// Loading skeleton
const DashboardSkeleton: React.FC = () => {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="space-y-2">
          <div className="h-8 w-48 bg-muted rounded animate-pulse" />
          <div className="h-4 w-64 bg-muted rounded animate-pulse" />
        </div>
        <div className="space-x-2">
          <div className="h-10 w-32 bg-muted rounded animate-pulse" />
          <div className="h-10 w-10 bg-muted rounded animate-pulse" />
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {Array.from({ length: 4 }).map((_, i) => (
          <div key={i} className="h-32 bg-muted rounded-lg animate-pulse" />
        ))}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {Array.from({ length: 2 }).map((_, i) => (
          <div key={i} className="h-64 bg-muted rounded-lg animate-pulse" />
        ))}
      </div>
    </div>
  );
};