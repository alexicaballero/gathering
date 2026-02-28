import Link from 'next/link';

export default function NotFound() {
  return (
    <div className='mx-auto max-w-4xl px-4 py-16 text-center sm:px-6 sm:py-24 lg:px-8'>
      <div className='rounded-lg border border-dashed border-border bg-muted/30 p-12'>
        <p className='text-6xl font-bold text-primary'>404</p>
        <h1 className='mt-4 text-3xl font-bold text-foreground sm:text-4xl'>
          Page not found
        </h1>
        <p className='mt-4 text-lg text-muted-foreground'>
          Sorry, we could not find the page you were looking for.
        </p>
        <Link
          href='/'
          className='mt-6 inline-flex items-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90'
        >
          Go home
        </Link>
      </div>
    </div>
  );
}
