'use client';

import { ChangeEvent, FormEvent, useEffect, useRef, useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { Upload, Loader2 } from 'lucide-react';
import Image from 'next/image';

import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Session, SessionState, Community } from '@/lib/types';
import {
  createSessionWithFormData,
  updateSessionWithFormData,
} from '@/lib/actions/session-actions';

interface SessionFormProps {
  mode: 'create' | 'edit';
  initialData?: Omit<Session, 'communityId'> & { communityId?: string };
  communityData?: Community; // For create mode
}

export default function SessionForm({
  initialData,
  mode,
  communityData,
}: SessionFormProps) {
  if (mode === 'edit' && !initialData) {
    throw new Error('Initial data is required in edit mode');
  }

  if (mode === 'create' && !communityData) {
    throw new Error('Community data is required in create mode');
  }

  const router = useRouter();
  const searchParams = useSearchParams();
  const fileInputRef = useRef<HTMLInputElement>(null);

  const communityId =
    initialData?.communityId ||
    communityData?.id ||
    searchParams?.get('communityId') ||
    '';
  const [title, setTitle] = useState(initialData?.title ?? '');
  const [description, setDescription] = useState(
    initialData?.description ?? '',
  );
  const [speaker, setSpeaker] = useState(initialData?.speaker ?? '');
  const [schedule, setSchedule] = useState(
    initialData?.schedule
      ? new Date(initialData.schedule).toISOString().slice(0, 16)
      : '',
  );
  const [state, setState] = useState<SessionState>(
    initialData?.state ?? SessionState.Scheduled,
  );
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [preview, setPreview] = useState(initialData?.image ?? null);
  const [isSaving, setIsSaving] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const objectUrlRef = useRef<string | null>(null);

  useEffect(() => {
    return () => {
      if (objectUrlRef.current) {
        URL.revokeObjectURL(objectUrlRef.current);
      }
    };
  }, []);

  const handleImageChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.currentTarget.files?.[0] ?? null;
    setImageFile(file);

    if (objectUrlRef.current) {
      URL.revokeObjectURL(objectUrlRef.current);
      objectUrlRef.current = null;
    }

    if (file) {
      const url = URL.createObjectURL(file);
      objectUrlRef.current = url;
      setPreview(url);
    } else {
      setPreview(initialData?.image ?? null);
    }
  };

  const handleDragOver = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault();
    event.stopPropagation();
  };

  const handleDrop = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault();
    event.stopPropagation();
    const file = event.dataTransfer.files?.[0];
    if (file?.type.startsWith('image/')) {
      setImageFile(file);
      if (objectUrlRef.current) {
        URL.revokeObjectURL(objectUrlRef.current);
      }
      const url = URL.createObjectURL(file);
      objectUrlRef.current = url;
      setPreview(url);
    }
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSaving(true);
    setErrorMessage(null);

    try {
      if (!communityId && mode === 'create') {
        throw new Error('Community ID is required');
      }

      if (!schedule) {
        throw new Error('Schedule is required');
      }

      const formData = new FormData();
      formData.append('communityId', communityId);
      formData.append('title', title);
      formData.append('description', description);
      formData.append('speaker', speaker);
      formData.append('schedule', new Date(schedule).toISOString());
      if (mode === 'edit') {
        formData.append('state', state.toString());
      }
      if (imageFile) {
        formData.append('image', imageFile);
      }

      if (mode === 'create') {
        await createSessionWithFormData(formData);
        router.push(`/communities/${communityId}`);
      } else {
        await updateSessionWithFormData(initialData!.id, formData);
        router.push(`/sessions/${initialData!.id}`);
      }
    } catch (error) {
      setErrorMessage(
        error instanceof Error ? error.message : 'An error occurred',
      );
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className='space-y-6'>
      {errorMessage && (
        <Card className='border-red-200 bg-red-50 p-4 text-red-800'>
          {errorMessage}
        </Card>
      )}

      {/* Title */}
      <Card className='p-6'>
        <label className='block text-sm font-medium'>
          Session Title *
          <input
            type='text'
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder='Enter session title'
            required
            maxLength={200}
            className='mt-2 w-full rounded border px-3 py-2 placeholder-muted-foreground'
          />
        </label>
        <p className='mt-1 text-xs text-muted-foreground'>
          {title.length}/200 characters
        </p>
      </Card>

      {/* Description */}
      <Card className='p-6'>
        <label className='block text-sm font-medium'>
          Description *
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder='Enter session description'
            required
            maxLength={1000}
            rows={4}
            className='mt-2 w-full rounded border px-3 py-2 placeholder-muted-foreground'
          />
        </label>
        <p className='mt-1 text-xs text-muted-foreground'>
          {description.length}/1000 characters
        </p>
      </Card>

      {/* Speaker */}
      <Card className='p-6'>
        <label className='block text-sm font-medium'>
          Speaker Name *
          <input
            type='text'
            value={speaker}
            onChange={(e) => setSpeaker(e.target.value)}
            placeholder='Enter speaker name'
            required
            className='mt-2 w-full rounded border px-3 py-2 placeholder-muted-foreground'
          />
        </label>
      </Card>

      {/* Schedule */}
      <Card className='p-6'>
        <label className='block text-sm font-medium'>
          Schedule *
          <input
            type='datetime-local'
            value={schedule}
            onChange={(e) => setSchedule(e.target.value)}
            required
            className='mt-2 w-full rounded border px-3 py-2'
          />
        </label>
      </Card>

      {/* State (Edit Only) */}
      {mode === 'edit' && (
        <Card className='p-6'>
          <label className='block text-sm font-medium'>
            State
            <select
              value={state}
              onChange={(e) => setState(Number(e.target.value) as SessionState)}
              className='mt-2 w-full rounded border px-3 py-2'
            >
              <option value={SessionState.Scheduled}>Scheduled</option>
              <option value={SessionState.Completed}>Completed</option>
              <option value={SessionState.Canceled}>Canceled</option>
            </select>
          </label>
        </Card>
      )}

      {/* Image Upload */}
      <Card className='p-6'>
        <label className='block text-sm font-medium mb-4'>Session Image</label>
        <div
          onDragOver={handleDragOver}
          onDrop={handleDrop}
          className='rounded-lg border-2 border-dashed border-muted-foreground/25 p-8 text-center transition-colors hover:border-muted-foreground/50'
        >
          {preview ? (
            <div className='space-y-4'>
              <Image
                src={preview}
                alt='Preview'
                width={400}
                height={256}
                className='mx-auto max-h-64 rounded object-cover'
              />
              <button
                type='button'
                onClick={() => {
                  setImageFile(null);
                  setPreview(null);
                  if (fileInputRef.current) {
                    fileInputRef.current.value = '';
                  }
                }}
                className='text-sm text-blue-600 hover:underline'
              >
                Remove image
              </button>
            </div>
          ) : (
            <div className='space-y-2'>
              <Upload className='mx-auto h-8 w-8 text-muted-foreground' />
              <div>
                <button
                  type='button'
                  onClick={() => fileInputRef.current?.click()}
                  className='text-sm font-medium text-blue-600 hover:underline'
                >
                  Click to upload
                </button>
                <span className='text-sm text-muted-foreground'>
                  {' '}
                  or drag and drop
                </span>
              </div>
              <p className='text-xs text-muted-foreground'>
                PNG, JPG, GIF up to 10MB
              </p>
            </div>
          )}
        </div>
        <input
          ref={fileInputRef}
          type='file'
          accept='image/*'
          onChange={handleImageChange}
          className='hidden'
        />
      </Card>

      {/* Actions */}
      <div className='flex gap-3'>
        <Button
          type='button'
          variant='outline'
          onClick={() => router.back()}
          disabled={isSaving}
        >
          Cancel
        </Button>
        <Button type='submit' disabled={isSaving} className='gap-2'>
          {isSaving && <Loader2 className='h-4 w-4 animate-spin' />}
          {mode === 'create' ? 'Create Session' : 'Update Session'}
        </Button>
      </div>
    </form>
  );
}
