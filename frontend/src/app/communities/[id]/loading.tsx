export default function CommunityLoading() {
  return (
    <div className='w-full'>
      {/* Banner skeleton */}
      <div className='h-64 w-full animate-pulse bg-muted sm:h-80 lg:h-96' />

      <div className='mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8'>
        <div className='mb-8 space-y-4'>
          <div className='h-10 w-72 animate-pulse rounded bg-muted' />
          <div className='h-5 w-full max-w-lg animate-pulse rounded bg-muted' />
          <div className='flex gap-3 pt-4'>
            <div className='h-9 w-20 animate-pulse rounded bg-muted' />
            <div className='h-9 w-28 animate-pulse rounded bg-muted' />
          </div>
        </div>

        <div className='grid gap-8 sm:grid-cols-2 lg:grid-cols-3'>
          {[...Array(3)].map((_, i) => (
            <div key={i} className='h-72 animate-pulse rounded-xl bg-muted' />
          ))}
        </div>
      </div>
    </div>
  );
}
