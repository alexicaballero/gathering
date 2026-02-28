import {
  getSessionById,
  getSessionResources,
} from '@/lib/actions/session-actions';
import type { Metadata } from 'next';
import SessionDetailHeader from '@/components/sessions/session-detail-header';
import SessionResourceList from '@/components/sessions/session-resource-list';
import { NotFoundMessage } from '@/components/not-found-message';

export function generateMetadata(): Metadata {
  return {
    title: `Gathering - session details`,
    description: `Session details page for Gathering.`,
  };
}

interface SessionPageProps {
  params: Promise<{ id: string }>;
}

export default async function SessionPage({ params }: SessionPageProps) {
  const { id } = await params;

  const [session, resources] = await Promise.all([
    getSessionById(id),
    getSessionResources(id),
  ]);

  if (!session) {
    return (
      <NotFoundMessage
        title='Session not found'
        description='The selected session does not exist or has been deleted.'
      />
    );
  }

  return (
    <div className='mx-auto max-w-6xl space-y-8 px-4 py-8 sm:px-6 lg:space-y-12 lg:px-8'>
      <SessionDetailHeader session={session} />
      <SessionResourceList resources={resources} sessionId={id} />
    </div>
  );
}
