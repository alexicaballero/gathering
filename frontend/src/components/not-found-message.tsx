interface NotFoundMessageProps {
  title?: string;
  description?: string;
}

export function NotFoundMessage({
  title = 'Content not found',
  description = 'Sorry, we could not find what you were looking for.',
}: NotFoundMessageProps) {
  return (
    <div className='mx-auto max-w-4xl px-4 py-16 sm:px-6 sm:py-24 lg:px-8'>
      <div className='rounded-lg border border-dashed border-border bg-muted/30 p-12 text-center'>
        <h1 className='text-3xl font-bold text-foreground sm:text-4xl'>
          {title}
        </h1>
        <p className='mt-4 text-lg text-muted-foreground'>{description}</p>
      </div>
    </div>
  );
}
