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
      <CardFooter className='px-6 pb-6 pt-3'>
        <Button asChild variant='default'>
          <Link
            href={`/sessions/${session.id}`}
            className='text-sm font-medium text-primary-foreground'
          >
            View details
          </Link>
        </Button>
      </CardFooter>
    </Card>
  );
}
