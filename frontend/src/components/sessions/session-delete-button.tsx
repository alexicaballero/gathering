'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Trash2 } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { ConfirmDialog } from '@/components/ui/confirm-dialog';
import { AlertDialog } from '@/components/ui/alert-dialog';
import { deleteSession } from '@/lib/actions/session-actions';

interface SessionDeleteButtonProps {
  sessionId: string;
  communityId: string;
  size?: 'sm' | 'default';
  className?: string;
}

export default function SessionDeleteButton({
  sessionId,
  communityId,
  size = 'default',
  className = '',
}: SessionDeleteButtonProps) {
  const router = useRouter();
  const [isOpen, setIsOpen] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleConfirmDelete = async () => {
    try {
      setIsDeleting(true);
      await deleteSession(sessionId);
      router.push(`/communities/${communityId}`);
    } catch (error) {
      console.error('Failed to delete session:', error);
      setErrorMessage('Failed to delete session. Please try again.');
    } finally {
      setIsDeleting(false);
      setIsOpen(false);
    }
  };

  return (
    <>
      <Button
        variant='destructive'
        size={size}
        onClick={() => setIsOpen(true)}
        disabled={isDeleting}
        className={`gap-1 ${className}`}
      >
        <Trash2 className='h-3 w-3' />
        Delete
      </Button>

      <ConfirmDialog
        isOpen={isOpen}
        isDestructive
        isLoading={isDeleting}
        title='Delete Session'
        description='Are you sure you want to delete this session? This action cannot be undone.'
        confirmText='Delete Session'
        cancelText='Cancel'
        onConfirm={handleConfirmDelete}
        onCancel={() => setIsOpen(false)}
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
