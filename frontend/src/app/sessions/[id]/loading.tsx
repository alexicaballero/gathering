export default function SessionLoading() {
  return (
    <div className='mx-auto max-w-6xl space-y-8 px-4 py-8 sm:px-6 lg:space-y-12 lg:px-8'>
      {/* Back button skeleton */}
      <div className='h-8 w-36 animate-pulse rounded bg-muted' />

      {/* Session card skeleton */}
      <div className='overflow-hidden rounded-3xl border border-border'>
        <div className='h-72 w-full animate-pulse bg-muted sm:h-96' />
        <div className='space-y-6 p-8 sm:p-10'>
          <div className='h-10 w-3/4 animate-pulse rounded bg-muted' />
          <div className='h-5 w-full animate-pulse rounded bg-muted' />
          <div className='h-5 w-1/2 animate-pulse rounded bg-muted' />
        </div>
      </div>

      {/* Resources skeleton */}
      <div className='space-y-4'>
        <div className='h-6 w-40 animate-pulse rounded bg-muted' />
        <div className='grid gap-6 lg:grid-cols-2'>
          {[...Array(2)].map((_, i) => (
            <div key={i} className='h-32 animate-pulse rounded-xl bg-muted' />
          ))}
        </div>
      </div>
    </div>
  );
}
