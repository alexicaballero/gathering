'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Edit2, Trash2 } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { ConfirmDialog } from '@/components/ui/confirm-dialog';
import { AlertDialog } from '@/components/ui/alert-dialog';
import { SessionResource } from '@/lib/types';
import { deleteSessionResource } from '@/lib/actions/session-actions';
import { SessionResourceFormDialog } from './session-resource-form-dialog';

interface SessionResourceActionsProps {
  sessionId: string;
  resource: SessionResource;
}

export default function SessionResourceActions({
  sessionId,
  resource,
}: SessionResourceActionsProps) {
  const router = useRouter();
  const [isEditOpen, setIsEditOpen] = useState(false);
  const [isDeleteOpen, setIsDeleteOpen] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleConfirmDelete = async () => {
    try {
      setIsDeleting(true);
      await deleteSessionResource(sessionId, resource.id);
      router.refresh();
    } catch (error) {
      console.error('Failed to delete resource:', error);
      setErrorMessage('Failed to delete resource. Please try again.');
    } finally {
      setIsDeleting(false);
      setIsDeleteOpen(false);
    }
  };

  const resourceLabel = resource.title || 'this resource';

  return (
    <>
      <div className='flex gap-1'>
        <Button
          variant='outline'
          size='icon-sm'
          onClick={() => setIsEditOpen(true)}
          title='Edit resource'
        >
          <Edit2 className='h-3.5 w-3.5' />
        </Button>
        <Button
          variant='destructive'
          size='icon-sm'
          onClick={() => setIsDeleteOpen(true)}
          title='Delete resource'
        >
          <Trash2 className='h-3.5 w-3.5' />
        </Button>
      </div>

      <SessionResourceFormDialog
        sessionId={sessionId}
        mode='edit'
        initialData={resource}
        isOpen={isEditOpen}
        onClose={() => setIsEditOpen(false)}
      />

      <ConfirmDialog
        isOpen={isDeleteOpen}
        isDestructive
        isLoading={isDeleting}
        title='Delete Resource'
        description={`Are you sure you want to delete "${resourceLabel}"? This action cannot be undone.`}
        confirmText='Delete Resource'
        cancelText='Cancel'
        onConfirm={handleConfirmDelete}
        onCancel={() => setIsDeleteOpen(false)}
      />

      <AlertDialog
        isOpen={!!errorMessage}
        title='Error'
        description={errorMessage || ''}
        variant='error'
        onClose={() => setErrorMessage(null)}
      />
    </>
  );
}
