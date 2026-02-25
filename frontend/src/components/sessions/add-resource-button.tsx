'use client';

import { useState } from 'react';
import { Plus } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { SessionResourceFormDialog } from './session-resource-form-dialog';

interface AddResourceButtonProps {
  sessionId: string;
}

export default function AddResourceButton({
  sessionId,
}: AddResourceButtonProps) {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <Button
        variant='outline'
        size='sm'
        onClick={() => setIsOpen(true)}
        className='gap-2'
      >
        <Plus className='h-4 w-4' />
        Add Resource
      </Button>

      <SessionResourceFormDialog
        sessionId={sessionId}
        mode='create'
        isOpen={isOpen}
        onClose={() => setIsOpen(false)}
      />
    </>
  );
}
