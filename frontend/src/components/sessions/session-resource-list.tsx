import { SessionResource } from '@/lib/types';
import SessionResourceCard from './session-resource-card';
import AddResourceButton from './add-resource-button';

interface SessionResourceListProps {
  resources: SessionResource[];
  sessionId: string;
}

export default function SessionResourceList({
  resources,
  sessionId,
}: SessionResourceListProps) {
  if (!resources || resources.length === 0) {
    return (
      <section className='space-y-6'>
        <div className='rounded-2xl bg-muted/50 p-6 shadow-inner'>
          <p className='text-xs font-semibold uppercase tracking-[0.3em] text-muted-foreground/70'>
            Supporting materials
          </p>
          <div className='mt-2 flex items-center justify-between gap-4'>
            <h2 className='text-2xl font-semibold text-foreground sm:text-3xl'>
              Resources
            </h2>
            <AddResourceButton sessionId={sessionId} />
          </div>
          <p className='mt-2 text-sm text-muted-foreground'>
            No resources yet.
          </p>
        </div>
      </section>
    );
  }

  return (
    <section className='space-y-6'>
      <div className='rounded-2xl bg-muted/50 p-6 shadow-inner'>
        <p className='text-xs font-semibold uppercase tracking-[0.3em] text-muted-foreground/70'>
          Supporting materials
        </p>
        <div className='mt-2 flex items-center justify-between gap-4'>
          <h2 className='text-2xl font-semibold text-foreground sm:text-3xl'>
            Available resources
          </h2>
          <AddResourceButton sessionId={sessionId} />
        </div>
        <p className='mt-2 text-sm text-muted-foreground'>
          {resources.length} {resources.length === 1 ? 'resource' : 'resources'}{' '}
          shared
        </p>
      </div>

      <div className='grid gap-6 lg:grid-cols-2'>
        {resources.map((resource) => (
          <SessionResourceCard
            key={resource.id}
            resource={resource}
            sessionId={sessionId}
          />
        ))}
      </div>
    </section>
  );
}
