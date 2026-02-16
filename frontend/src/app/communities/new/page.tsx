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
    <div className='container mx-auto py-10'>
      <div className='mx-auto max-w-3xl space-y-6'>
        <CommunityForm mode='create' />
      </div>
    </div>
  );
}
