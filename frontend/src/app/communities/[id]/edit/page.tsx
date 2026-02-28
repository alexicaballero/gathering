import { notFound } from 'next/navigation';

import CommunityForm from '@/components/communities/community-form';
import { getCommunity } from '@/lib/actions/community-actions';
import type { Metadata } from 'next';

export function generateMetadata(): Metadata {
  return {
    title: 'Gathering · Edit community',
    description: 'Edit an existing community of practice.',
  };
}

interface EditCommunityPageProps {
  params: Promise<{ id: string }>;
}

export default async function EditCommunityPage({
  params,
}: EditCommunityPageProps) {
  const { id } = await params;

  const community = await getCommunity(id);

  if (!community) {
    notFound();
  }

  return (
    <div className='mx-auto max-w-4xl px-4 py-8 sm:px-6 sm:py-12 lg:px-8'>
      <CommunityForm mode='edit' initialData={community} />
    </div>
  );
}
