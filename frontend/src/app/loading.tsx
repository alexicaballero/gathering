export default function Loading() {
  return (
    <div className='mx-auto max-w-7xl px-4 py-12 sm:px-6 sm:py-16 lg:px-8 lg:py-20'>
      <div className='mb-12 space-y-4'>
        <div className='h-4 w-40 animate-pulse rounded bg-muted' />
        <div className='h-10 w-80 animate-pulse rounded bg-muted' />
      </div>
      <div className='grid gap-8 sm:grid-cols-2 lg:grid-cols-3'>
        {[...Array(6)].map((_, i) => (
          <div key={i} className='h-72 animate-pulse rounded-xl bg-muted' />
        ))}
      </div>
    </div>
  );
}
