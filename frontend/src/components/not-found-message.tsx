interface NotFoundMessageProps {
  title?: string;
  description?: string;
}

export function NotFoundMessage({
  title = 'Content not found',
  description = 'Sorry, we could not find what you were looking for.',
}: NotFoundMessageProps) {
  return (
    <div className='container mx-auto py-10'>
      <h1 className='text-3xl font-bold mb-4'>{title}</h1>
      <p className='text-lg text-muted-foreground'>{description}</p>
    </div>
  );
}
