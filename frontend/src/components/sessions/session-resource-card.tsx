import { SessionResource, SessionResourceType } from '@/lib/types';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import { FileText, Link as LinkIcon, Video } from 'lucide-react';
import SessionResourceActions from './session-resource-actions';

interface SessionResourceCardProps {
  resource: SessionResource;
  sessionId: string;
}

function getYouTubeEmbedUrl(url: string): string | null {
  try {
    const urlObj = new URL(url);

    if (urlObj.hostname === 'youtu.be') {
      const videoId = urlObj.pathname.slice(1);
      return `https://www.youtube.com/embed/${videoId}`;
    }

    if (urlObj.hostname.includes('youtube.com')) {
      const videoId = urlObj.searchParams.get('v');
      if (videoId) {
        return `https://www.youtube.com/embed/${videoId}`;
      }
    }

    return null;
  } catch {
    return null;
  }
}

function ResourceIcon({ type }: { type: SessionResourceType }) {
  switch (type) {
    case SessionResourceType.Video:
      return <Video className='h-5 w-5 text-blue-500' />;
    case SessionResourceType.Notes:
      return <FileText className='h-5 w-5 text-green-500' />;
    case SessionResourceType.ExternalLink:
      return <LinkIcon className='h-5 w-5 text-purple-500' />;
    default:
      return <FileText className='h-5 w-5 text-muted-foreground' />;
  }
}

function ResourceTypeLabel({ type }: { type: SessionResourceType }) {
  switch (type) {
    case SessionResourceType.Video:
      return (
        <span className='text-xs font-medium uppercase tracking-wide text-blue-500'>
          Video
        </span>
      );
    case SessionResourceType.Notes:
      return (
        <span className='text-xs font-medium uppercase tracking-wide text-green-500'>
          Notes
        </span>
      );
    case SessionResourceType.ExternalLink:
      return (
        <span className='text-xs font-medium uppercase tracking-wide text-purple-500'>
          External Link
        </span>
      );
    default:
      return null;
  }
}

export default function SessionResourceCard({
  resource,
  sessionId,
}: SessionResourceCardProps) {
  const embedUrl =
    resource.type === SessionResourceType.Video && resource.url
      ? getYouTubeEmbedUrl(resource.url)
      : null;

  return (
    <Card className='overflow-hidden transition-shadow hover:shadow-md'>
      {/* Video embed */}
      {embedUrl && (
        <div className='aspect-video w-full border-b border-border'>
          <iframe
            src={embedUrl}
            title={resource.title || 'Video'}
            allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture'
            allowFullScreen
            className='h-full w-full'
          />
        </div>
      )}

      <CardHeader className='gap-2'>
        <div className='flex items-center justify-between gap-2'>
          <div className='flex items-center gap-2'>
            <ResourceIcon type={resource.type} />
            <ResourceTypeLabel type={resource.type} />
          </div>
          <SessionResourceActions sessionId={sessionId} resource={resource} />
        </div>
        {resource.title && (
          <CardTitle className='text-lg'>{resource.title}</CardTitle>
        )}
      </CardHeader>

      {(resource.notes || resource.url) && (
        <CardContent className='space-y-3'>
          {resource.notes && (
            <CardDescription className='whitespace-pre-wrap leading-relaxed'>
              {resource.notes}
            </CardDescription>
          )}
          {resource.url &&
            resource.type !== SessionResourceType.Video &&
            !embedUrl && (
              <a
                href={resource.url}
                target='_blank'
                rel='noopener noreferrer'
                className='inline-flex items-center gap-2 text-sm font-medium text-primary hover:underline'
              >
                <LinkIcon className='h-4 w-4' />
                Open Link
              </a>
            )}
        </CardContent>
      )}
    </Card>
  );
}
