'use client';

import { useEffect, useState } from 'react';
import { Loader2, Plus, Trash2, Edit2 } from 'lucide-react';
import Link from 'next/link';

import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Session } from '@/lib/types';
import {
  getSessionsByCommunity,
  deleteSession,
} from '@/lib/actions/session-actions';

interface CommunitySessionsPanelProps {
  communityId: string;
  communityName: string;
}

export default function CommunitySessionsPanel({
  communityId,
  communityName,
}: CommunitySessionsPanelProps) {
  const [sessions, setSessions] = useState<Session[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    const loadSessions = async () => {
      try {
        setIsLoading(true);
        setError(null);
        const data = await getSessionsByCommunity(communityId);
        setSessions(data);
      } catch (err) {
        setError('Failed to load sessions');
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    };

    loadSessions();
  }, [communityId]);

  const handleDeleteSession = async (sessionId: string) => {
    if (confirm('Are you sure you want to delete this session?')) {
      try {
        setDeletingId(sessionId);
        await deleteSession(sessionId); // This now automatically revalidates the cache
        setSessions(sessions.filter((s) => s.id !== sessionId));
      } catch (err) {
        setError('Failed to delete session');
        console.error(err);
      } finally {
        setDeletingId(null);
      }
    }
  };

  const getStateColor = (state: string | number) => {
    // Convert numeric enum values to string
    let stateStr: string;
    if (typeof state === 'number') {
      switch (state) {
        case 0:
          stateStr = 'scheduled';
          break;
        case 1:
          stateStr = 'completed';
          break;
        case 2:
          stateStr = 'canceled';
          break;
        default:
          stateStr = 'scheduled';
      }
    } else {
      stateStr = state.toLowerCase();
    }

    switch (stateStr) {
      case 'scheduled':
        return 'bg-blue-100 text-blue-800';
      case 'completed':
        return 'bg-green-100 text-green-800';
      case 'canceled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getStateText = (state: string | number) => {
    if (typeof state === 'number') {
      switch (state) {
        case 0:
          return 'Scheduled';
        case 1:
          return 'Completed';
        case 2:
          return 'Canceled';
        default:
          return 'Scheduled';
      }
    }
    // If it's already a string, capitalize first letter
    return state.charAt(0).toUpperCase() + state.slice(1).toLowerCase();
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  return (
    <div className='mt-8 space-y-4'>
      <div className='flex items-center justify-between'>
        <div>
          <h3 className='text-lg font-semibold'>Community Sessions</h3>
          <p className='text-sm text-muted-foreground'>
            Manage sessions for {communityName}
          </p>
        </div>
        <Link href={`/sessions/new?communityId=${communityId}`}>
          <Button size='sm' className='gap-2'>
            <Plus className='h-4 w-4' />
            Add Session
          </Button>
        </Link>
      </div>

      {error && (
        <Card className='border-red-200 bg-red-50 p-3 text-sm text-red-800'>
          {error}
        </Card>
      )}

      {isLoading ? (
        <Card className='flex items-center justify-center p-8'>
          <div className='flex items-center gap-2 text-muted-foreground'>
            <Loader2 className='h-4 w-4 animate-spin' />
            <span>Loading sessions...</span>
          </div>
        </Card>
      ) : sessions.length === 0 ? (
        <Card className='p-8 text-center'>
          <p className='text-muted-foreground'>
            No sessions yet. Create one to get started.
          </p>
        </Card>
      ) : (
        <div className='space-y-3'>
          {sessions.map((session) => (
            <Card key={session.id} className='p-4'>
              <div className='flex items-start justify-between gap-4'>
                <div className='flex-1'>
                  <div className='flex items-center gap-2'>
                    <h4 className='font-semibold'>{session.title}</h4>
                    <span
                      className={`inline-flex rounded-full px-2 py-1 text-xs font-medium ${getStateColor(
                        session.state,
                      )}`}
                    >
                      {getStateText(session.state)}
                    </span>
                  </div>
                  <p className='mt-1 text-sm text-muted-foreground'>
                    {session.description}
                  </p>
                  <div className='mt-2 flex flex-col gap-1 text-xs text-muted-foreground'>
                    <p>
                      <strong>Speaker:</strong> {session.speaker}
                    </p>
                    <p>
                      <strong>Schedule:</strong> {formatDate(session.schedule)}
                    </p>
                  </div>
                </div>
                <div className='flex gap-2'>
                  <Link href={`/sessions/${session.id}/edit`}>
                    <Button variant='outline' size='sm' className='gap-1'>
                      <Edit2 className='h-3 w-3' />
                      Edit
                    </Button>
                  </Link>
                  <Button
                    variant='outline'
                    size='sm'
                    className='gap-1 text-red-600 hover:text-red-700'
                    onClick={() => handleDeleteSession(session.id)}
                    disabled={deletingId === session.id}
                  >
                    {deletingId === session.id ? (
                      <Loader2 className='h-3 w-3 animate-spin' />
                    ) : (
                      <Trash2 className='h-3 w-3' />
                    )}
                    Delete
                  </Button>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
