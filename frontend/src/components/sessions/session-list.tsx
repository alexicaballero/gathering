// create session list component
import { Session } from '@/lib/types';
import SessionCard from './session-card';

interface SessionListProps {
  sessions: Session[];
}

export default async function SessionList({ sessions }: SessionListProps) {
  return (
    <section className='space-y-8 py-12'>
      <div className='flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between'>
        <div>
          <p className='text-xs font-semibold uppercase tracking-[0.3em] text-muted-foreground/70'>
            Practice sessions
          </p>
          <h2 className='mt-3 text-3xl font-bold tracking-tight text-foreground sm:text-4xl'>
            Available sessions
          </h2>
        </div>
      </div>

      {sessions.length === 0 ? (
        <div className='rounded-lg border border-dashed border-border bg-muted/30 p-12 text-center'>
          <p className='text-lg text-muted-foreground'>
            No active sessions available for this community yet.
          </p>
        </div>
      ) : (
        <div className='grid gap-8 sm:grid-cols-2 lg:grid-cols-3'>
          {sessions.map((session) => (
            <SessionCard key={session.id} session={session} />
          ))}
        </div>
      )}
    </section>
  );
}
