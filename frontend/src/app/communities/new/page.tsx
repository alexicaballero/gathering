import CommunityForm from '@/components/communities/community-form';
import { Metadata } from 'next/dist/lib/metadata/types/metadata-interface';

export function generateMetadata(): Metadata {
  return {
    title: 'Gathering · New community',
    description: 'Create a new community of practice.',
  };
}

export default function NewCommunityPage() {
  return (
    <div className='mx-auto max-w-4xl px-4 py-8 sm:px-6 sm:py-12 lg:px-8'>
      <CommunityForm mode='create' />
    </div>
  );
}
