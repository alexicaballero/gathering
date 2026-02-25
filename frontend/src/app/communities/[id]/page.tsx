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
    <div className='w-full'>
      {/* Community Header Banner */}
      <div className='relative overflow-hidden'>
        {community.image ? (
          <div
            className='h-64 w-full bg-cover bg-center sm:h-80 lg:h-96'
            style={{
              backgroundImage: `linear-gradient(180deg, rgba(15,23,42,0.3), rgba(15,23,42,0.7)), url(${community.image})`,
            }}
          />
        ) : (
          <div className='flex h-64 w-full items-center justify-center bg-gradient-to-br from-primary/20 to-primary/10 sm:h-80 lg:h-96'>
            <span className='text-8xl font-bold text-primary/20'>
              {community.name.charAt(0).toUpperCase()}
            </span>
          </div>
        )}
      </div>

      {/* Community Content */}
      <div className='mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8'>
        <div className='mb-8 space-y-4'>
          <div>
            <h1 className='text-4xl font-bold tracking-tight text-foreground sm:text-5xl'>
              {community.name}
            </h1>
            <p className='mt-4 text-lg text-muted-foreground'>
              {community.description}
            </p>
          </div>

          {/* Action Buttons */}
          <div className='flex flex-wrap gap-3 pt-4'>
            <Button asChild variant='outline' size='sm' className='gap-2'>
              <Link href={`/communities/${community.id}/edit`}>
                <Edit2 className='h-4 w-4' />
                Edit
              </Link>
            </Button>
            <Button asChild size='sm' className='gap-2'>
              <Link href={`/sessions/new?communityId=${community.id}`}>
                <Plus className='h-4 w-4' />
                Add Session
              </Link>
            </Button>
            <CommunityDeleteButton
              communityId={community.id}
              communityName={community.name}
              size='sm'
            />
          </div>
        </div>

        {/* Sessions List */}
        <SessionList sessions={sessions} />
      </div>
    </div>
  );
}
