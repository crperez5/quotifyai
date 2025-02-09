import { useState, useEffect, useCallback } from 'react'
import * as signalR from '@microsoft/signalr';

interface UseSignalRProps {
  url: string;
  onMessage?: (message: any) => void;
  reconnectAttempts?: number;
  reconnectInterval?: number;
}

export const useWebSocket = ({
  url,
  onMessage,
  reconnectAttempts = 3,
  reconnectInterval = 5000,
}: UseSignalRProps) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [attemptCount, setAttemptCount] = useState(0);

  const connect = useCallback(async () => {
    try {
      const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(url)
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: () => reconnectInterval,
        })
        .configureLogging(signalR.LogLevel.Error)
        .build();

      hubConnection.onclose(() => {
        setIsConnected(false);
        setError('Disconnected from server.');
      });

      hubConnection.on('ReceiveToken', (message: any) => {
        if (onMessage) {
          console.log(message);
          onMessage(message);
        }
      });

      hubConnection.onreconnecting(() => {
        setIsConnected(false);
        setError('Reconnecting...');
      });

      hubConnection.onreconnected(() => {
        setIsConnected(true);
        setError(null);
      });

      await hubConnection.start();

      console.info('SignalR connected');
      setIsConnected(true);
      setError(null);
      setConnection(hubConnection);

    } catch (err) {
      console.error('SignalR connection failed:', err);
      setError('Failed to connect to server.');
      if (attemptCount < reconnectAttempts) {
        setTimeout(() => {
          setAttemptCount((prev) => prev + 1);
          connect();
        }, reconnectInterval);
      }
    }
  }, [url, onMessage, attemptCount, reconnectAttempts, reconnectInterval]);

  useEffect(() => {
    connect();

    return () => {
      if (connection) {
        connection.stop().catch(err => console.error('Error disconnecting:', err));
      }
    };
  }, []);

  return {
    isConnected,
    error,
    joinConversation: (groupId: string) => { connection?.invoke('JoinConversation', groupId) },
    leaveConversation: (groupId: string) => { connection?.invoke('LeaveConversation', groupId) }
  };
};
