'use client';

import { FormEvent, useState } from 'react';
import { useRouter } from 'next/navigation';
import { Loader2 } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { SessionResource, SessionResourceType } from '@/lib/types';
import {
  createSessionResource,
  updateSessionResource,
} from '@/lib/actions/session-actions';

interface SessionResourceFormDialogProps {
  sessionId: string;
  mode: 'create' | 'edit';
  initialData?: SessionResource;
  isOpen: boolean;
  onClose: () => void;
}

const TYPE_LABELS: Record<SessionResourceType, string> = {
  [SessionResourceType.Video]: 'Video',
  [SessionResourceType.Notes]: 'Notes',
  [SessionResourceType.ExternalLink]: 'External Link',
};

export function SessionResourceFormDialog({
  sessionId,
  mode,
  initialData,
  isOpen,
  onClose,
}: SessionResourceFormDialogProps) {
  const router = useRouter();

  const [type, setType] = useState<SessionResourceType>(
    initialData?.type ?? SessionResourceType.Video,
  );
  const [title, setTitle] = useState(initialData?.title ?? '');
  const [url, setUrl] = useState(initialData?.url ?? '');
  const [notes, setNotes] = useState(initialData?.notes ?? '');
  const [isSaving, setIsSaving] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  if (!isOpen) return null;

  const needsUrl =
    type === SessionResourceType.Video ||
    type === SessionResourceType.ExternalLink;
  const needsNotes = type === SessionResourceType.Notes;

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsSaving(true);
    setErrorMessage(null);

    try {
      if (mode === 'create') {
        await createSessionResource(sessionId, {
          type,
          title: title.trim() || null,
          url: needsUrl ? url.trim() || null : null,
          notes: needsNotes ? notes.trim() || null : null,
        });
      } else {
        await updateSessionResource(sessionId, initialData!.id, {
          title: title.trim() || null,
          url: needsUrl ? url.trim() || null : null,
          notes: needsNotes ? notes.trim() || null : null,
        });
      }
      router.refresh();
      onClose();
    } catch (error) {
      setErrorMessage(
        error instanceof Error
          ? error.message
          : 'An error occurred. Please try again.',
      );
    } finally {
      setIsSaving(false);
    }
  };

  const handleBackdropClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget && !isSaving) onClose();
  };

  return (
    <div
      className='fixed inset-0 z-50 flex items-center justify-center bg-black/50'
      onClick={handleBackdropClick}
    >
      <dialog
        open
        className='relative w-full max-w-md rounded-lg border border-border bg-background p-6 shadow-lg'
      >
        <h2 className='mb-4 text-lg font-semibold'>
          {mode === 'create' ? 'Add Resource' : 'Edit Resource'}
        </h2>

        {errorMessage && (
          <div className='mb-4 rounded border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800'>
            {errorMessage}
          </div>
        )}

        <form onSubmit={handleSubmit} className='space-y-4'>
          {/* Type */}
          {mode === 'create' ? (
            <div>
              <label className='block text-sm font-medium'>
                Type *
                <select
                  value={type}
                  onChange={(e) =>
                    setType(Number(e.target.value) as SessionResourceType)
                  }
                  className='mt-1 w-full rounded border border-input bg-background px-3 py-2 text-sm'
                  required
                >
                  <option value={SessionResourceType.Video}>Video</option>
                  <option value={SessionResourceType.Notes}>Notes</option>
                  <option value={SessionResourceType.ExternalLink}>
                    External Link
                  </option>
                </select>
              </label>
            </div>
          ) : (
            <div>
              <p className='text-sm font-medium'>Type</p>
              <p className='mt-1 rounded border border-input bg-muted px-3 py-2 text-sm text-muted-foreground'>
                {TYPE_LABELS[initialData!.type]}
              </p>
            </div>
          )}

          {/* Title (optional for all types) */}
          <div>
            <label className='block text-sm font-medium'>
              Title
              <input
                type='text'
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder='Enter a title (optional)'
                maxLength={200}
                className='mt-1 w-full rounded border border-input bg-background px-3 py-2 text-sm placeholder-muted-foreground'
              />
            </label>
            <p className='mt-0.5 text-xs text-muted-foreground'>
              {title.length}/200
            </p>
          </div>

          {/* URL — Video or ExternalLink */}
          {needsUrl && (
            <div>
              <label className='block text-sm font-medium'>
                URL *
                <input
                  type='url'
                  value={url}
                  onChange={(e) => setUrl(e.target.value)}
                  placeholder={
                    type === SessionResourceType.Video
                      ? 'https://youtube.com/watch?v=...'
                      : 'https://example.com'
                  }
                  maxLength={2000}
                  required
                  className='mt-1 w-full rounded border border-input bg-background px-3 py-2 text-sm placeholder-muted-foreground'
                />
              </label>
            </div>
          )}

          {/* Notes — Notes type */}
          {needsNotes && (
            <div>
              <label className='block text-sm font-medium'>
                Notes *
                <textarea
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                  placeholder='Enter notes content...'
                  rows={5}
                  maxLength={4000}
                  required
                  className='mt-1 w-full rounded border border-input bg-background px-3 py-2 text-sm placeholder-muted-foreground'
                />
              </label>
              <p className='mt-0.5 text-xs text-muted-foreground'>
                {notes.length}/4000
              </p>
            </div>
          )}

          <div className='flex justify-end gap-3 pt-2'>
            <Button
              type='button'
              variant='outline'
              onClick={onClose}
              disabled={isSaving}
            >
              Cancel
            </Button>
            <Button type='submit' disabled={isSaving} className='gap-2'>
              {isSaving && <Loader2 className='h-4 w-4 animate-spin' />}
              {mode === 'create' ? 'Add Resource' : 'Save Changes'}
            </Button>
          </div>
        </form>
      </dialog>
    </div>
  );
}
