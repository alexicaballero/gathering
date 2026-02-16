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
      className='mx-auto mt-10 max-w-6xl space-y-8 px-4 pb-16 sm:mt-16 sm:px-6 lg:space-y-12 lg:px-8'
    >
      <div className='flex flex-col gap-3 rounded-3xl bg-muted/50 p-6 text-muted-foreground shadow-inner sm:flex-row sm:items-end sm:justify-between sm:gap-4'>
        <div>
          <p className='text-xs font-semibold uppercase tracking-[0.3em] text-muted-foreground/70'>
            Community of Practices
          </p>
          <h2 className='mt-2 text-3xl font-semibold text-foreground sm:text-4xl'>
            Sessions that inspire action
          </h2>
        </div>
        <Link href='/communities/new' className='self-start sm:self-center'>
          <Button variant='outline' size='sm'>
            <Plus /> Add
          </Button>
        </Link>
      </div>

      <div className='grid gap-6 sm:grid-cols-2 lg:grid-cols-3'>
        {communities.map((community) => (
          <CommunityCard key={community.id} community={community} />
        ))}
      </div>
    </section>
  );
}
