import Link from 'next/link';

import { Button } from '@/components/ui/button';
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
  const scheduleDate = new Date(session.schedule);
  const formattedDate = new Intl.DateTimeFormat('en-US', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  }).format(scheduleDate);

  return (
    <Card className='overflow-hidden'>
      <div className='border-b border-border'>
        {session.image ? (
          <div
            className='h-40 w-full bg-cover bg-center'
            style={{
              backgroundImage: `linear-gradient(180deg, rgba(15,23,42,0.2), rgba(15,23,42,0.65)), url(${session.image})`,
            }}
          />
        ) : (
          <div className='flex h-40 w-full items-center justify-center bg-muted'>
            <span className='text-4xl font-bold text-muted-foreground'>
              {initial}
            </span>
          </div>
        )}
      </div>
      <CardHeader className='gap-1 px-6 pt-6'>
        <CardTitle>{title}</CardTitle>
        <CardDescription>
          {session.description ?? 'No detailed description available.'}
        </CardDescription>
      </CardHeader>
      <CardContent className='px-6'>
        <p className='text-sm font-medium text-foreground'>
          Speaker: {session.speaker}
        </p>
      </CardContent>
      <CardContent className='px-6 pt-0 pb-2'>
        <p className='text-xs uppercase tracking-[0.3em] text-muted-foreground/70'>
          Date: {formattedDate}
        </p>
      </CardContent>
      <CardFooter className='px-6 pb-6 pt-3 flex flex-col gap-3'>
        <Button asChild variant='default' className='w-full'>
          <Link
            href={`/sessions/${session.id}`}
            className='text-sm font-medium text-primary-foreground'
          >
            View details
          </Link>
        </Button>

        <div className='flex gap-2 w-full'>
          <Button asChild variant='outline' size='sm' className='flex-1 gap-1'>
            <Link href={`/sessions/${session.id}/edit`}>
              <Edit2 className='h-3 w-3' />
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
