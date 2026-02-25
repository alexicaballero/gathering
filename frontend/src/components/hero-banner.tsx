import Image from 'next/image';

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
      <div className='absolute inset-0 bg-foreground/75' />
      <div className='relative mx-auto max-w-7xl px-4 py-12 sm:px-6 sm:py-20 lg:px-8 lg:py-28'>
        <div className='max-w-3xl'>
          <p className='text-sm font-semibold uppercase tracking-widest text-primary-foreground/80'>
            Practice Communities
          </p>
          <h1 className='mt-4 text-balance text-5xl font-bold tracking-tight text-primary-foreground sm:text-6xl lg:text-7xl'>
            Learn, share and grow in community
          </h1>
          <p className='mt-6 max-w-2xl text-lg leading-relaxed text-primary-foreground/90'>
            Explore our Frontend, Python and Cloud communities. Collaborative
            sessions, resources and content to enhance your skills.
          </p>
        </div>
      </div>
    </section>
  );
}
