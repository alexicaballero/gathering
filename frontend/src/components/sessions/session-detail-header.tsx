import { Session } from '@/lib/types';
import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { ArrowLeft, Calendar, Edit2 } from 'lucide-react';
import SessionDeleteButton from './session-delete-button';

interface SessionDetailHeaderProps {
  session: Session;
}

export default function SessionDetailHeader({
  session,
}: SessionDetailHeaderProps) {
  const title = session.title;
  const initial = title.charAt(0).toUpperCase();
  const scheduleDate = new Date(session.schedule);
  const formattedDate = new Intl.DateTimeFormat('en-US', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(scheduleDate);

  return (
    <div className='space-y-6'>
      <Button asChild variant='ghost' size='sm' className='gap-2'>
        <Link href={`/communities/${session.communityId}`}>
          <ArrowLeft className='h-4 w-4' />
          Back to community
        </Link>
      </Button>

      <div className='overflow-hidden rounded-3xl border border-border bg-card shadow-lg'>
        <div className='border-b border-border'>
          {session.image ? (
            <div
              className='h-64 w-full bg-cover bg-center sm:h-80 lg:h-96'
              style={{
                backgroundImage: `linear-gradient(180deg, rgba(15,23,42,0.3), rgba(15,23,42,0.7)), url(${session.image})`,
              }}
            />
          ) : (
            <div className='flex h-64 w-full items-center justify-center bg-linear-to-br from-muted to-muted/50 sm:h-80 lg:h-96'>
              <span className='text-8xl font-bold text-muted-foreground/30'>
                {initial}
              </span>
            </div>
          )}
        </div>

        <div className='space-y-6 p-6 sm:p-8 lg:p-10'>
          <div className='space-y-4'>
            <h1 className='text-3xl font-bold tracking-tight text-foreground sm:text-4xl lg:text-5xl'>
              {title}
            </h1>
            <p className='text-lg text-muted-foreground sm:text-xl'>
              {session.description || 'No description available'}
            </p>
          </div>

          <div className='flex flex-wrap gap-6 text-sm text-muted-foreground'>
            <div className='flex items-center gap-2'>
              <Calendar className='h-4 w-4' />
              <span>{formattedDate}</span>
            </div>
            <div className='flex items-center gap-2'>
              <span className='font-semibold text-foreground'>Speaker:</span>
              <span>{session.speaker}</span>
            </div>
          </div>

          {/* Action Buttons */}
          <div className='flex flex-wrap gap-3 pt-4'>
            <Button asChild variant='outline' size='sm' className='gap-2'>
              <Link href={`/sessions/${session.id}/edit`}>
                <Edit2 className='h-4 w-4' />
                Edit Session
              </Link>
            </Button>
            <SessionDeleteButton
              sessionId={session.id}
              communityId={session.communityId}
              size='sm'
            />
          </div>
        </div>
      </div>
    </div>
  );
}
