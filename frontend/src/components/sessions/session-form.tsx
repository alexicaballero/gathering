'use client';

import { ChangeEvent, FormEvent, useEffect, useRef, useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { Upload, Loader2 } from 'lucide-react';
import Image from 'next/image';

import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Select } from '@/components/ui/select';
import { Label } from '@/components/ui/label';
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
    <form
      onSubmit={handleSubmit}
      className='space-y-6 rounded-3xl border border-border bg-card/70 p-6 shadow-lg'
    >
      <div>
        <p className='text-sm font-semibold uppercase tracking-[0.3em] text-muted-foreground/80'>
          Session details
        </p>
        <h1 className='mt-2 text-3xl font-semibold text-foreground'>
          {mode === 'create' ? 'Create a session' : 'Edit session'}
        </h1>
      </div>

      {errorMessage && (
        <div className='rounded-lg border border-destructive/50 bg-destructive/10 p-4 text-sm text-destructive'>
          {errorMessage}
        </div>
      )}

      {/* Title */}
      <div className='space-y-2'>
        <Label htmlFor='session-title'>Title</Label>
        <Input
          id='session-title'
          type='text'
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder='Enter session title'
          required
          maxLength={200}
        />
        <p className='text-xs text-muted-foreground'>
          {title.length}/200 characters
        </p>
      </div>

      {/* Description */}
      <div className='space-y-2'>
        <Label htmlFor='session-description'>Description</Label>
        <Textarea
          id='session-description'
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          placeholder='Enter session description'
          required
          maxLength={1000}
          rows={4}
        />
        <p className='text-xs text-muted-foreground'>
          {description.length}/1000 characters
        </p>
      </div>

      {/* Speaker */}
      <div className='space-y-2'>
        <Label htmlFor='session-speaker'>Speaker Name</Label>
        <Input
          id='session-speaker'
          type='text'
          value={speaker}
          onChange={(e) => setSpeaker(e.target.value)}
          placeholder='Enter speaker name'
          required
        />
      </div>

      {/* Schedule */}
      <div className='space-y-2'>
        <Label htmlFor='session-schedule'>Schedule</Label>
        <Input
          id='session-schedule'
          type='datetime-local'
          value={schedule}
          onChange={(e) => setSchedule(e.target.value)}
          required
        />
      </div>

      {/* State (Edit Only) */}
      {mode === 'edit' && (
        <div className='space-y-2'>
          <Label htmlFor='session-state'>Status</Label>
          <Select
            id='session-state'
            value={state}
            onChange={(e) => setState(Number(e.target.value) as SessionState)}
          >
            <option value={SessionState.Scheduled}>Scheduled</option>
            <option value={SessionState.Completed}>Completed</option>
            <option value={SessionState.Canceled}>Canceled</option>
          </Select>
        </div>
      )}

      {/* Image Upload */}
      <div className='space-y-3'>
        <div>
          <p className='text-sm font-medium text-foreground'>Session image</p>
          <p className='text-sm text-muted-foreground'>
            Upload a file and the backend will store it for you.
          </p>
        </div>

        <input
          ref={fileInputRef}
          id='session-image'
          type='file'
          accept='image/*'
          className='hidden'
          onChange={handleImageChange}
        />

        <Card
          className='relative border-2 border-dashed border-border hover:border-primary hover:bg-accent/50 transition-all cursor-pointer'
          onDragOver={handleDragOver}
          onDrop={handleDrop}
          onClick={() => fileInputRef.current?.click()}
        >
          <div className='p-8'>
            {preview ? (
              <div className='space-y-4'>
                <div
                  className='h-40 w-full rounded-lg overflow-hidden bg-cover bg-center'
                  style={{
                    backgroundImage: `linear-gradient(180deg, rgba(15,23,42,0.15), rgba(15,23,42,0.6)), url(${preview})`,
                  }}
                />
                <div className='text-center'>
                  <p className='text-sm font-medium text-foreground'>
                    ✓ Image selected
                  </p>
                  <p className='text-xs text-muted-foreground mt-1'>
                    Click or drag to change image
                  </p>
                </div>
              </div>
            ) : (
              <div className='flex flex-col items-center justify-center gap-3 py-4'>
                <div className='rounded-full bg-primary/10 p-3'>
                  <Upload className='w-6 h-6 text-primary' />
                </div>
                <div className='text-center'>
                  <p className='text-sm font-medium text-foreground'>
                    Drag and drop your image
                  </p>
                  <p className='text-xs text-muted-foreground mt-1'>
                    or click to browse
                  </p>
                </div>
              </div>
            )}
          </div>
        </Card>
      </div>

      {/* Actions */}
      <div className='flex justify-end gap-3'>
        <Button
          type='button'
          variant='outline'
          size='lg'
          onClick={() => router.back()}
          disabled={isSaving}
        >
          Cancel
        </Button>
        <Button type='submit' size='lg' className='px-6' disabled={isSaving}>
          {isSaving
            ? 'Saving…'
            : mode === 'create'
              ? 'Create session'
              : 'Update session'}
        </Button>
      </div>
    </form>
  );
}
