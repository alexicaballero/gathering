import Link from 'next/link';
import Image from 'next/image';
import { Button } from '@/components/ui/button';
import { ArrowRight, Plus } from 'lucide-react';

export function HeroBanner() {
  return (
    <section className='relative overflow-hidden'>
      <Image
        src='/images/banner.png'
        alt='Practice communities banner'
        width={1920}
        height={600}
        className='absolute inset-0 h-full w-full object-cover'
        priority
      />
      <div className='absolute inset-0 bg-foreground/70' />
      <div className='relative mx-auto max-w-7xl px-4 py-24 sm:px-6 sm:py-32 lg:px-8 lg:py-40'>
        <div className='max-w-2xl'>
          <p className='text-sm font-semibold uppercase tracking-widest text-primary-foreground/80'>
            Practice Communities
          </p>
          <h1 className='mt-3 text-balance text-4xl font-bold tracking-tight text-primary-foreground sm:text-5xl lg:text-6xl'>
            Learn, share and grow in community
          </h1>
          <p className='mt-5 max-w-lg text-lg leading-relaxed text-primary-foreground/80'>
            Explore our Frontend, Python and Cloud communities. Collaborative
            sessions, resources and content to enhance your skills.
          </p>
          <div className='mt-8 flex flex-wrap gap-3'>
            <Link href='#communities'>
              <Button size='lg' className='gap-2'>
                Explore Communities
                <ArrowRight className='h-4 w-4' />
              </Button>
            </Link>
            <Link href='/communities/new'>
              <Button
                size='lg'
                variant='outline'
                className='flex items-center gap-2 border-primary-foreground/30 bg-transparent text-primary-foreground hover:bg-primary-foreground/10 hover:text-primary-foreground'
              >
                <Plus className='h-4 w-4' />
                Create Community
              </Button>
            </Link>
          </div>
        </div>
      </div>
    </section>
  );
}
