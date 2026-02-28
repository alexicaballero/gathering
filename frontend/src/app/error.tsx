'use client';

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  return (
    <div className='mx-auto max-w-4xl px-4 py-16 text-center sm:px-6 sm:py-24 lg:px-8'>
      <div className='rounded-lg border border-dashed border-destructive/30 bg-destructive/5 p-12'>
        <h2 className='text-3xl font-bold text-foreground sm:text-4xl'>
          Something went wrong
        </h2>
        <p className='mt-4 text-lg text-muted-foreground'>
          {error.message || 'An unexpected error occurred.'}
        </p>
        <button
          onClick={reset}
          className='mt-6 inline-flex items-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90'
        >
          Try again
        </button>
      </div>
    </div>
  );
}
