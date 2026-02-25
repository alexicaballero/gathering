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
    <div className='mx-auto max-w-4xl px-4 py-8 sm:px-6 sm:py-12 lg:px-8'>
      <SessionForm mode='create' communityData={community} />
    </div>
  );
}
