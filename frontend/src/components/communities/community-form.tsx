'use client';

import { ChangeEvent, FormEvent, useEffect, useRef, useState } from 'react';
import { useRouter } from 'next/navigation';
import { Upload } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Community } from '@/lib/types';
import { postFormData, putFormData } from '@/lib/api-client';

interface CommunityFormProps {
  mode: 'create' | 'edit';
  initialData?: Pick<Community, 'id' | 'name' | 'description' | 'image'>;
}

export default function CommunityForm({
  initialData,
  mode,
}: CommunityFormProps) {
  if (mode === 'edit' && !initialData) {
    throw new Error('Initial data is required in edit mode');
  }

  const router = useRouter();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [name, setName] = useState(initialData?.name ?? '');
  const [description, setDescription] = useState(
    initialData?.description ?? '',
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
    setErrorMessage(null);

    if (!name.trim()) {
      setErrorMessage('Community name is required.');
      return;
    }

    setIsSaving(true);

    const payload = new FormData();
    payload.append('name', name.trim());
    payload.append('description', description);
    if (imageFile) {
      payload.append('image', imageFile);
    }

    try {
      const endpoint =
        mode === 'create'
          ? '/api/v1/communities'
          : `/api/v1/communities/${initialData?.id}`;

      await (mode === 'create'
        ? postFormData<Community>(endpoint, payload)
        : putFormData<Community>(endpoint, payload));

      setIsSaving(false);
      router.push(`/`);
      router.refresh();
    } catch (error) {
      const message =
        error instanceof Error ? error.message : 'Unable to save community.';
      setErrorMessage(message);
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
          Community details
        </p>
        <h1 className='mt-2 text-3xl font-semibold text-foreground'>
          {mode === 'create' ? 'Create a community' : 'Edit community'}
        </h1>
      </div>

      <div className='space-y-2'>
        <label
          className='text-sm font-medium text-foreground'
          htmlFor='community-name'
        >
          Name
        </label>
        <input
          id='community-name'
          type='text'
          className='w-full rounded-xl border border-border bg-background px-4 py-3 text-base focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary'
          placeholder='Community name'
          value={name}
          onChange={(event) => setName(event.target.value)}
        />
      </div>

      <div className='space-y-2'>
        <label
          className='text-sm font-medium text-foreground'
          htmlFor='community-description'
        >
          Description
        </label>
        <textarea
          id='community-description'
          rows={4}
          className='w-full resize-none rounded-xl border border-border bg-background px-4 py-3 text-base focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary'
          placeholder='Add a short explanation about this community'
          value={description}
          onChange={(event) => setDescription(event.target.value)}
        />
      </div>

      <div className='space-y-3'>
        <div>
          <p className='text-sm font-medium text-foreground'>Cover image</p>
          <p className='text-sm text-muted-foreground'>
            Upload a file and the backend will store it for you.
          </p>
        </div>

        <input
          ref={fileInputRef}
          id='community-image'
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
                    ✓ {imageFile?.name}
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

      {errorMessage && (
        <p className='text-sm font-medium text-destructive' role='alert'>
          {errorMessage}
        </p>
      )}

      <div className='flex justify-end'>
        <Button type='submit' size='lg' className='px-6' disabled={isSaving}>
          {isSaving
            ? 'Saving…'
            : mode === 'create'
              ? 'Create community'
              : 'Update community'}
        </Button>
      </div>
    </form>
  );
}
