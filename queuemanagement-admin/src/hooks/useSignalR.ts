import { useEffect, useRef, useCallback, useState } from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useAuthStore } from '@/stores/auth';
import { useQueryClient } from '@tanstack/react-query';

interface SignalRConfig {
  hubUrl: string;
  onQueueUpdate?: (data: any) => void;
  onTicketUpdate?: (data: any) => void;
  onDashboardUpdate?: (data: any) => void;
  onUserStatusUpdate?: (data: any) => void;
  onSessionUpdate?: (data: any) => void;
  autoReconnect?: boolean;
  logLevel?: LogLevel;
}

interface UseSignalRReturn {
  connection: HubConnection | null;
  isConnected: boolean;
  isConnecting: boolean;
  error: string | null;
  sendMessage: (methodName: string, ...args: any[]) => Promise<void>;
  invoke: <T>(methodName: string, ...args: any[]) => Promise<T>;
}

export const useSignalR = (config: SignalRConfig): UseSignalRReturn => {
  const { token } = useAuthStore();
  const queryClient = useQueryClient();
  const connectionRef = useRef<HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const [isConnecting, setIsConnecting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Create connection
  const createConnection = useCallback(() => {
    if (!token) return null;

    const connection = new HubConnectionBuilder()
      .withUrl(config.hubUrl, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          // Exponential backoff: 1s, 2s, 4s, 8s, 16s, 30s, 30s...
          if (retryContext.previousRetryCount === 0) {
            return 1000;
          }
          if (retryContext.previousRetryCount === 1) {
            return 2000;
          }
          if (retryContext.previousRetryCount === 2) {
            return 4000;
          }
          if (retryContext.previousRetryCount === 3) {
            return 8000;
          }
          if (retryContext.previousRetryCount === 4) {
            return 16000;
          }
          return 30000;
        },
      })
      .configureLogging(config.logLevel || LogLevel.Information)
      .build();

    return connection;
  }, [token, config.hubUrl, config.logLevel]);

  // Setup event handlers
  const setupEventHandlers = useCallback((connection: HubConnection) => {
    // Queue updates
    connection.on('QueueStatusUpdate', (data) => {
      if (config.onQueueUpdate) {
        config.onQueueUpdate(data);
      }
      
      // Update React Query cache
      queryClient.setQueryData(['queue', data.queueId], (oldData: any) => ({
        ...oldData,
        ...data,
      }));
      
      // Invalidate related queries
      queryClient.invalidateQueries({ queryKey: ['queues'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    });

    // Ticket updates
    connection.on('TicketCreated', (data) => {
      if (config.onTicketUpdate) {
        config.onTicketUpdate(data);
      }
      
      queryClient.invalidateQueries({ queryKey: ['tickets'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    });

    connection.on('TicketStatusUpdate', (data) => {
      if (config.onTicketUpdate) {
        config.onTicketUpdate(data);
      }
      
      queryClient.setQueryData(['ticket', data.ticketId], (oldData: any) => ({
        ...oldData,
        ...data,
      }));
      
      queryClient.invalidateQueries({ queryKey: ['tickets'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    });

    connection.on('TicketCalled', (data) => {
      if (config.onTicketUpdate) {
        config.onTicketUpdate(data);
      }
      
      queryClient.invalidateQueries({ queryKey: ['tickets'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    });

    // Dashboard updates
    connection.on('DashboardUpdate', (data) => {
      if (config.onDashboardUpdate) {
        config.onDashboardUpdate(data);
      }
      
      queryClient.setQueryData(['dashboard'], (oldData: any) => ({
        ...oldData,
        ...data,
      }));
    });

    // User status updates
    connection.on('UserStatusUpdate', (data) => {
      if (config.onUserStatusUpdate) {
        config.onUserStatusUpdate(data);
      }
      
      queryClient.invalidateQueries({ queryKey: ['users'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    });

    // Session updates
    connection.on('SessionUpdate', (data) => {
      if (config.onSessionUpdate) {
        config.onSessionUpdate(data);
      }
      
      queryClient.invalidateQueries({ queryKey: ['sessions'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    });

    // Connection events
    connection.onclose((error) => {
      setIsConnected(false);
      if (error) {
        setError(`Connection closed: ${error.message}`);
      }
    });

    connection.onreconnecting((error) => {
      setIsConnected(false);
      setIsConnecting(true);
      setError(`Reconnecting: ${error?.message || 'Unknown error'}`);
    });

    connection.onreconnected((connectionId) => {
      setIsConnected(true);
      setIsConnecting(false);
      setError(null);
      console.log('SignalR reconnected with connection ID:', connectionId);
    });
  }, [config, queryClient]);

  // Start connection
  const startConnection = useCallback(async () => {
    if (!token) return;

    const connection = createConnection();
    if (!connection) return;

    try {
      setIsConnecting(true);
      setError(null);
      
      await connection.start();
      
      connectionRef.current = connection;
      setIsConnected(true);
      setIsConnecting(false);
      
      setupEventHandlers(connection);
      
      console.log('SignalR connected successfully');
    } catch (err) {
      setIsConnecting(false);
      setError(err instanceof Error ? err.message : 'Failed to connect to SignalR');
      console.error('SignalR connection failed:', err);
    }
  }, [token, createConnection, setupEventHandlers]);

  // Stop connection
  const stopConnection = useCallback(async () => {
    if (connectionRef.current) {
      try {
        await connectionRef.current.stop();
        connectionRef.current = null;
        setIsConnected(false);
        setIsConnecting(false);
        setError(null);
      } catch (err) {
        console.error('Error stopping SignalR connection:', err);
      }
    }
  }, []);

  // Send message
  const sendMessage = useCallback(async (methodName: string, ...args: any[]) => {
    if (connectionRef.current && isConnected) {
      try {
        await connectionRef.current.send(methodName, ...args);
      } catch (err) {
        console.error(`Error sending message ${methodName}:`, err);
        throw err;
      }
    } else {
      throw new Error('SignalR connection not available');
    }
  }, [isConnected]);

  // Invoke method
  const invoke = useCallback(async <T>(methodName: string, ...args: any[]): Promise<T> => {
    if (connectionRef.current && isConnected) {
      try {
        return await connectionRef.current.invoke<T>(methodName, ...args);
      } catch (err) {
        console.error(`Error invoking method ${methodName}:`, err);
        throw err;
      }
    } else {
      throw new Error('SignalR connection not available');
    }
  }, [isConnected]);

  // Effect to manage connection lifecycle
  useEffect(() => {
    if (token) {
      startConnection();
    }

    return () => {
      stopConnection();
    };
  }, [token, startConnection, stopConnection]);

  // Effect to handle token changes
  useEffect(() => {
    if (token && !isConnected && !isConnecting) {
      startConnection();
    }
  }, [token, isConnected, isConnecting, startConnection]);

  return {
    connection: connectionRef.current,
    isConnected,
    isConnecting,
    error,
    sendMessage,
    invoke,
  };
};