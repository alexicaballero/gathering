// create session list component
import { Session } from '@/lib/types';
import SessionCard from './session-card';

interface SessionListProps {
  sessions: Session[];
}

export default async function SessionList({ sessions }: SessionListProps) {
  return (
    <section className='mt-6 space-y-6'>
      <div className='flex flex-col gap-3 rounded-3xl bg-muted/50 p-6 text-muted-foreground shadow-inner sm:flex-row sm:items-end sm:justify-between sm:gap-4'>
        <div>
          <p className='text-xs font-semibold uppercase tracking-[0.3em] text-muted-foreground/70'>
            Practice sessions
          </p>
          <h2 className='mt-2 text-3xl font-semibold text-foreground sm:text-4xl'>
            Available sessions
          </h2>
        </div>
      </div>

      {sessions.length === 0 ? (
        <p className='text-muted-foreground'>
          No active sessions available for this community at this time.
        </p>
      ) : (
        <div className='grid gap-6 sm:grid-cols-2 lg:grid-cols-3'>
          {sessions.map((session) => (
            <SessionCard key={session.id} session={session} />
          ))}
        </div>
      )}
    </section>
  );
}
