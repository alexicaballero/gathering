import Link from 'next/link';
import SessionList from '@/components/sessions/session-list';
import { Button } from '@/components/ui/button';
import CommunityDeleteButton from '@/components/communities/community-delete-button';
import { getCommunity } from '@/lib/actions/community-actions';
import { getSessionsByCommunity } from '@/lib/actions/session-actions';
import { Metadata } from 'next/dist/lib/metadata/types/metadata-interface';
import { NotFoundMessage } from '@/components/not-found-message';
import { Edit2, Plus } from 'lucide-react';

export function generateMetadata(): Metadata {
  return {
    title: 'Gathering - community details',
    description: 'Community details page for Gathering.',
  };
}

interface CommunityPageProps {
  params: Promise<{ id: string }>;
}

export default async function CommunityPage({ params }: CommunityPageProps) {
  const { id } = await params;

  const communityPromise = getCommunity(id);
  const sessionsPromise = getSessionsByCommunity(id);

  const community = await communityPromise;

  if (!community) {
    await sessionsPromise.catch(() => undefined);
    return (
      <NotFoundMessage
        title='Comunidad no encontrada'
        description='La comunidad que estás buscando no existe.'
      />
    );
  }

  const sessions = await sessionsPromise;

  return (
    <div className='container mx-auto py-10'>
      <h1 className='text-3xl font-bold mb-4'>{community.name}</h1>
      <p className='text-lg text-muted-foreground'>{community.description}</p>
      <div className='mt-6 flex flex-wrap gap-3'>
        <Button asChild variant='outline' size='sm'>
          <Link href={`/communities/${community.id}/edit`}>
            <Edit2 className='h-4 w-4' /> Edit
          </Link>
        </Button>
        <Button asChild size='sm'>
          <Link href={`/sessions/new?communityId=${community.id}`}>
            <Plus className='h-4 w-4' /> Add Session
          </Link>
        </Button>
        <CommunityDeleteButton
          communityId={community.id}
          communityName={community.name}
          size='sm'
        />
      </div>
      <SessionList sessions={sessions} />
    </div>
  );
}
