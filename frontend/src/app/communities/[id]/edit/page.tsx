import { notFound } from 'next/navigation';

import CommunityForm from '@/components/communities/community-form';
import { getCommunity } from '@/lib/actions/community-actions';
import { Metadata } from 'next/dist/lib/metadata/types/metadata-interface';

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
    <div className='container mx-auto py-10'>
      <div className='mx-auto max-w-3xl space-y-6'>
        <CommunityForm mode='edit' initialData={community} />
      </div>
    </div>
  );
}
