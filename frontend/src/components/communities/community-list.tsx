import Link from 'next/link';

import { Button } from '@/components/ui/button';
import { Plus } from 'lucide-react';

import CommunityCard from './community-card';
import { Community } from '@/lib/types';

interface CommunityListProps {
  communities: Community[];
}

export default function CommunityList({ communities }: CommunityListProps) {
  return (
    <section
      id='communities'
      className='mx-auto max-w-7xl px-4 py-12 sm:px-6 sm:py-16 lg:px-8 lg:py-20'
    >
      <div className='mb-12 space-y-4'>
        <div>
          <p className='text-xs font-semibold uppercase tracking-[0.3em] text-muted-foreground/70'>
            Community of Practices
          </p>
          <h2 className='mt-3 text-4xl font-bold tracking-tight text-foreground sm:text-5xl'>
            Sessions that inspire action
          </h2>
        </div>
        <div className='flex justify-end'>
          <Link href='/communities/new'>
            <Button size='lg' className='gap-2'>
              <Plus className='h-5 w-5' />
              Create Community
            </Button>
          </Link>
        </div>
      </div>

      <div className='grid gap-8 sm:grid-cols-2 lg:grid-cols-3'>
        {communities.length > 0 ? (
          communities.map((community) => (
            <CommunityCard key={community.id} community={community} />
          ))
        ) : (
          <div className='col-span-full rounded-lg border border-dashed border-border bg-muted/30 p-12 text-center'>
            <h3 className='text-xl font-semibold text-foreground'>
              No communities yet
            </h3>
            <p className='mt-2 text-sm text-muted-foreground'>
              Create your first community of practice to get started.
            </p>
          </div>
        )}
      </div>
    </section>
  );
}
