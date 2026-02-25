import { notFound } from 'next/navigation';

import SessionForm from '@/components/sessions/session-form';
import { getSessionById } from '@/lib/actions/session-actions';
import { Metadata } from 'next/dist/lib/metadata/types/metadata-interface';

export function generateMetadata(): Metadata {
  return {
    title: 'Gathering · Edit session',
    description: 'Edit an existing session.',
  };
}

interface EditSessionPageProps {
  params: Promise<{ id: string }>;
}

export default async function EditSessionPage({
  params,
}: EditSessionPageProps) {
  const { id } = await params;

  const session = await getSessionById(id);

  if (!session) {
    notFound();
  }

  return (
    <div className='container mx-auto py-10'>
      <div className='mx-auto max-w-3xl space-y-6'>
        <div>
          <h1 className='text-3xl font-bold'>Edit Session</h1>
          <p className='mt-2 text-muted-foreground'>Update session details</p>
        </div>
        <SessionForm mode='edit' initialData={session} />
      </div>
    </div>
  );
}
