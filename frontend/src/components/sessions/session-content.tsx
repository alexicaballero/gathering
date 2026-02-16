import { SessionResource, SessionResourceType } from '@/lib/types';
import { Card, CardContent } from '@/components/ui/card';

interface SessionContentProps {
  resources: SessionResource[];
}

export default function SessionContent({ resources }: SessionContentProps) {
  // Filter resources to get only Notes type
  const notesResources = resources.filter(
    (r) => r.type === SessionResourceType.Notes && r.notes,
  );

  if (notesResources.length === 0) {
    return null;
  }

  return (
    <section className='space-y-6'>
      <div className='rounded-2xl bg-muted/50 p-6 shadow-inner'>
        <p className='text-xs font-semibold uppercase tracking-[0.3em] text-muted-foreground/70'>
          Content
        </p>
        <h2 className='mt-2 text-2xl font-semibold text-foreground sm:text-3xl'>
          Session Notes
        </h2>
      </div>

      {notesResources.map((resource) => (
        <Card key={resource.id}>
          <CardContent className='prose prose-slate max-w-none p-6 dark:prose-invert sm:p-8'>
            {resource.title && (
              <h3 className='mb-4 text-xl font-semibold'>{resource.title}</h3>
            )}
            <div className='whitespace-pre-wrap text-base leading-7 text-foreground/90'>
              {resource.notes}
            </div>
          </CardContent>
        </Card>
      ))}
    </section>
  );
}
