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
  const scheduleDate = session.scheduledAt ? new Date(session.scheduledAt) : null;
  const isValidDate = scheduleDate && !isNaN(scheduleDate.getTime());
  const formattedDate = isValidDate
    ? new Intl.DateTimeFormat('en-US', {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      }).format(scheduleDate)
    : 'Date not set';

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
              className='h-72 w-full bg-cover bg-center sm:h-96 lg:h-112'
              style={{
                backgroundImage: `linear-gradient(180deg, rgba(15,23,42,0.3), rgba(15,23,42,0.7)), url(${session.image})`,
              }}
            />
          ) : (
            <div className='flex h-72 w-full items-center justify-center bg-linear-to-br from-primary/20 to-primary/10 sm:h-96 lg:h-112'>
              <span className='text-9xl font-bold text-primary/20'>
                {session.title.charAt(0).toUpperCase()}
              </span>
            </div>
          )}
        </div>

        <div className='space-y-8 p-8 sm:p-10 lg:p-12'>
          <div className='space-y-4'>
            <h1 className='text-4xl font-bold tracking-tight text-foreground sm:text-5xl lg:text-6xl'>
              {session.title}
            </h1>
            <p className='text-lg leading-relaxed text-muted-foreground sm:text-xl'>
              {session.description || 'No description available'}
            </p>
          </div>

          <div className='flex flex-wrap gap-6 border-t border-border pt-6 text-sm'>
            <div className='flex items-center gap-3'>
              <Calendar className='h-5 w-5 text-primary' />
              <span className='text-muted-foreground'>{formattedDate}</span>
            </div>
            <div className='flex items-center gap-3'>
              <span className='font-semibold text-foreground'>Speaker:</span>
              <span className='text-muted-foreground'>{session.speaker}</span>
            </div>
          </div>

          {/* Action Buttons */}
          <div className='flex flex-wrap gap-3 pt-6'>
            <Button asChild variant='outline' size='lg' className='gap-2'>
              <Link href={`/sessions/${session.id}/edit`}>
                <Edit2 className='h-4 w-4' />
                Edit Session
              </Link>
            </Button>
            <SessionDeleteButton
              sessionId={session.id}
              communityId={session.communityId}
              size='default'
            />
          </div>
        </div>
      </div>
    </div>
  );
}
