import { notFound } from 'next/navigation';
import SessionForm from '@/components/sessions/session-form';
import { getCommunity } from '@/lib/actions/community-actions';
import { Metadata } from 'next/dist/lib/metadata/types/metadata-interface';

export function generateMetadata(): Metadata {
  return {
    title: 'Gathering · New session',
    description: 'Create a new session for your community.',
  };
}

interface NewSessionPageProps {
  searchParams: Promise<{ communityId?: string }>;
}

export default async function NewSessionPage({
  searchParams,
}: NewSessionPageProps) {
  const { communityId } = await searchParams;

  if (!communityId) {
    notFound();
  }

  const community = await getCommunity(communityId);

  if (!community) {
    notFound();
  }

  return (
    <div className='container mx-auto py-10'>
      <div className='mx-auto max-w-3xl space-y-6'>
        <div>
          <h1 className='text-3xl font-bold'>
            Add Session to {community.name}
          </h1>
          <p className='mt-2 text-muted-foreground'>
            Create a new session for your community
          </p>
        </div>
        <SessionForm mode='create' communityData={community} />
      </div>
    </div>
  );
}
