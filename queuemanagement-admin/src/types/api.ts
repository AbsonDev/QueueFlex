// Base types
export interface BaseEntity {
  id: string;
  createdAt: string;
  updatedAt: string;
}

export interface Tenant extends BaseEntity {
  name: string;
  subdomain: string;
  isActive: boolean;
  settings: TenantSettings;
}

export interface TenantSettings {
  theme: 'light' | 'dark' | 'auto';
  language: string;
  timezone: string;
  businessHours: BusinessHours;
  notifications: NotificationSettings;
}

export interface BusinessHours {
  monday: DaySchedule;
  tuesday: DaySchedule;
  wednesday: DaySchedule;
  thursday: DaySchedule;
  friday: DaySchedule;
  saturday: DaySchedule;
  sunday: DaySchedule;
}

export interface DaySchedule {
  isOpen: boolean;
  openTime: string;
  closeTime: string;
  breakStart?: string;
  breakEnd?: string;
}

export interface NotificationSettings {
  email: boolean;
  push: boolean;
  sms: boolean;
  webhook: boolean;
}

// User types
export interface User extends BaseEntity {
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  avatar?: string;
  isActive: boolean;
  lastLoginAt?: string;
  role: UserRole;
  permissions: Permission[];
  unitId?: string;
  unit?: Unit;
  settings: UserSettings;
}

export type UserRole = 'admin' | 'manager' | 'attendant' | 'viewer';

export interface Permission {
  resource: string;
  actions: string[];
}

export interface UserSettings {
  theme: 'light' | 'dark' | 'auto';
  language: string;
  notifications: UserNotificationSettings;
}

export interface UserNotificationSettings {
  queueUpdates: boolean;
  ticketAssignments: boolean;
  sessionReminders: boolean;
  performanceReports: boolean;
}

// Unit types
export interface Unit extends BaseEntity {
  name: string;
  code: string;
  address: string;
  phone: string;
  email: string;
  isActive: boolean;
  capacity: number;
  businessHours: BusinessHours;
  services: Service[];
  resources: Resource[];
}

export interface Resource extends BaseEntity {
  name: string;
  type: 'room' | 'counter' | 'equipment';
  isAvailable: boolean;
  unitId: string;
}

// Service types
export interface Service extends BaseEntity {
  name: string;
  description?: string;
  code: string;
  color: string;
  estimatedDuration: number; // in minutes
  priority: number;
  isActive: boolean;
  unitId: string;
  unit?: Unit;
}

// Queue types
export interface Queue extends BaseEntity {
  name: string;
  code: string;
  description?: string;
  status: QueueStatus;
  isActive: boolean;
  capacity: number;
  currentCount: number;
  averageWaitTime: number; // in minutes
  unitId: string;
  unit?: Unit;
  serviceId: string;
  service?: Service;
  settings: QueueSettings;
}

export type QueueStatus = 'open' | 'closed' | 'paused' | 'maintenance';

export interface QueueSettings {
  allowTransfer: boolean;
  maxWaitTime: number; // in minutes
  autoClose: boolean;
  closeTime: string;
  notifications: QueueNotificationSettings;
}

export interface QueueNotificationSettings {
  onTicketCreated: boolean;
  onTicketCalled: boolean;
  onQueueFull: boolean;
  onWaitTimeExceeded: boolean;
}

// Ticket types
export interface Ticket extends BaseEntity {
  number: string;
  status: TicketStatus;
  priority: number;
  estimatedWaitTime: number; // in minutes
  actualWaitTime?: number; // in minutes
  queueId: string;
  queue?: Queue;
  serviceId: string;
  service?: Service;
  unitId: string;
  unit?: Unit;
  assignedToId?: string;
  assignedTo?: User;
  calledAt?: string;
  servedAt?: string;
  completedAt?: string;
  cancelledAt?: string;
  cancellationReason?: string;
  notes?: string;
  customerInfo?: CustomerInfo;
}

export type TicketStatus = 'waiting' | 'called' | 'serving' | 'completed' | 'cancelled' | 'transferred';

export interface CustomerInfo {
  name?: string;
  phone?: string;
  email?: string;
  document?: string;
  notes?: string;
}

// Session types
export interface Session extends BaseEntity {
  status: SessionStatus;
  startTime: string;
  endTime?: string;
  pauseStartTime?: string;
  pauseEndTime?: string;
  totalPauseTime: number; // in minutes
  totalActiveTime: number; // in minutes
  userId: string;
  user?: User;
  unitId: string;
  unit?: Unit;
  ticketsServed: number;
  averageServiceTime: number; // in minutes
  rating?: number;
  feedback?: string;
}

export type SessionStatus = 'active' | 'paused' | 'ended';

// Dashboard types
export interface DashboardMetrics {
  totalQueues: number;
  activeQueues: number;
  totalTickets: number;
  waitingTickets: number;
  activeSessions: number;
  averageWaitTime: number;
  averageServiceTime: number;
  totalUsers: number;
  onlineUsers: number;
  todayStats: TodayStats;
  queueOverview: QueueOverview[];
  performanceMetrics: PerformanceMetrics;
}

export interface TodayStats {
  ticketsCreated: number;
  ticketsServed: number;
  ticketsCancelled: number;
  averageWaitTime: number;
  averageServiceTime: number;
  customerSatisfaction: number;
}

export interface QueueOverview {
  queueId: string;
  queueName: string;
  status: QueueStatus;
  currentCount: number;
  capacity: number;
  averageWaitTime: number;
  estimatedWaitTime: number;
}

export interface PerformanceMetrics {
  totalSessions: number;
  averageSessionDuration: number;
  totalTicketsServed: number;
  averageTicketsPerSession: number;
  topPerformers: TopPerformer[];
}

export interface TopPerformer {
  userId: string;
  userName: string;
  ticketsServed: number;
  averageServiceTime: number;
  rating: number;
}

// API Response types
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// Filter types
export interface TicketFilters {
  status?: TicketStatus[];
  queueId?: string;
  serviceId?: string;
  unitId?: string;
  assignedToId?: string;
  dateFrom?: string;
  dateTo?: string;
  search?: string;
}

export interface QueueFilters {
  status?: QueueStatus[];
  unitId?: string;
  serviceId?: string;
  isActive?: boolean;
  search?: string;
}

export interface UserFilters {
  role?: UserRole[];
  unitId?: string;
  isActive?: boolean;
  search?: string;
}

// Form types
export interface LoginForm {
  email: string;
  password: string;
  tenant?: string;
}

export interface CreateQueueForm {
  name: string;
  code: string;
  description?: string;
  capacity: number;
  unitId: string;
  serviceId: string;
  settings: Partial<QueueSettings>;
}

export interface CreateTicketForm {
  queueId: string;
  priority?: number;
  customerInfo?: CustomerInfo;
  notes?: string;
}

export interface CreateUserForm {
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  unitId?: string;
  permissions: Permission[];
}

// Real-time types
export interface SignalREvent {
  type: string;
  data: any;
  timestamp: string;
}

export interface QueueUpdateEvent {
  queueId: string;
  status: QueueStatus;
  currentCount: number;
  averageWaitTime: number;
}

export interface TicketEvent {
  ticketId: string;
  status: TicketStatus;
  queueId: string;
  assignedToId?: string;
}

export interface DashboardUpdateEvent {
  metrics: Partial<DashboardMetrics>;
}