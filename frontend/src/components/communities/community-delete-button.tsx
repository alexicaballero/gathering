'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Trash2 } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { ConfirmDialog } from '@/components/ui/confirm-dialog';
import { AlertDialog } from '@/components/ui/alert-dialog';
import { deleteCommunity } from '@/lib/actions/community-actions';

interface CommunityDeleteButtonProps {
  communityId: string;
  communityName: string;
  size?: 'sm' | 'default';
  className?: string;
}

export default function CommunityDeleteButton({
  communityId,
  communityName,
  size = 'default',
  className = '',
}: CommunityDeleteButtonProps) {
  const router = useRouter();
  const [isOpen, setIsOpen] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleConfirmDelete = async () => {
    try {
      setIsDeleting(true);
      await deleteCommunity(communityId);
      router.refresh();
      router.push('/');
    } catch (error) {
      console.error('Failed to delete community:', error);
      setErrorMessage('Failed to delete community. Please try again.');
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
        title='Delete Community'
        description={`Are you sure you want to delete "${communityName}"? This action cannot be undone and will remove all associated sessions.`}
        confirmText='Delete Community'
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
