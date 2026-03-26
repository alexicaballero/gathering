import Link from 'next/link';

import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import { Session } from '@/lib/types';
import { Edit2 } from 'lucide-react';
import SessionDeleteButton from './session-delete-button';

interface SessionCardProps {
  session: Session;
}

export default function SessionCard({ session }: SessionCardProps) {
  const title = session.title;
  const initial = title.charAt(0).toUpperCase();
  const scheduleDate = session.scheduledAt
    ? new Date(session.scheduledAt)
    : null;
  const isValidDate = scheduleDate && !isNaN(scheduleDate.getTime());
  const formattedDate = isValidDate
    ? new Intl.DateTimeFormat('en-US', {
        day: 'numeric',
        month: 'short',
        year: 'numeric',
      }).format(scheduleDate)
    : 'Date not set';

  // Check if session is upcoming
  const isUpcoming = isValidDate && scheduleDate > new Date();

  return (
    <Card className='group relative overflow-hidden transition-all hover:shadow-xl hover:scale-105 flex flex-col'>
      {/* Badge */}
      <Badge
        variant={isUpcoming ? 'info' : 'success'}
        className='absolute top-4 left-4 z-10'
      >
        {isUpcoming ? 'UPCOMING' : 'COMPLETED'}
      </Badge>

      <div className='border-b border-border'>
        {session.image ? (
          <div
            className='h-48 w-full bg-cover bg-center'
            style={{
              backgroundImage: `linear-gradient(180deg, rgba(15,23,42,0.2), rgba(15,23,42,0.65)), url(${session.image})`,
            }}
          />
        ) : (
          <div className='flex h-48 w-full items-center justify-center bg-linear-to-br from-primary/20 to-primary/10'>
            <span className='text-5xl font-bold text-primary/20'>
              {initial}
            </span>
          </div>
        )}
      </div>
      <CardHeader className='gap-2 px-6 pt-6'>
        <CardTitle className='line-clamp-2'>{title}</CardTitle>
        <CardDescription className='line-clamp-2'>
          {session.description ?? 'No detailed description available.'}
        </CardDescription>
      </CardHeader>
      <CardContent className='px-6'>
        <div className='space-y-2'>
          <p className='text-sm font-medium text-foreground'>
            Speaker: {session.speaker}
          </p>
          <p className='text-xs uppercase tracking-[0.3em] text-muted-foreground/80'>
            {formattedDate}
          </p>
        </div>
      </CardContent>
      <CardFooter className='flex-1 flex flex-col gap-3 px-6 pb-6 pt-4'>
        <Button asChild variant='default' className='w-full'>
          <Link
            href={`/sessions/${session.id}`}
            className='text-sm font-medium text-primary-foreground'
          >
            View details
          </Link>
        </Button>

        <div className='flex gap-2 w-full'>
          <Button asChild variant='outline' className='flex-1 gap-2'>
            <Link href={`/sessions/${session.id}/edit`}>
              <Edit2 className='h-4 w-4' />
              Edit
            </Link>
          </Button>
          <SessionDeleteButton
            sessionId={session.id}
            communityId={session.communityId}
            size='sm'
          />
        </div>
      </CardFooter>
    </Card>
  );
}
