'use client';

import { ReactNode } from 'react';
import { AlertCircle, X } from 'lucide-react';
import { Button } from './button';

interface AlertDialogProps {
  isOpen: boolean;
  title: string;
  description: string;
  variant?: 'error' | 'warning' | 'info' | 'success';
  onClose: () => void;
  children?: ReactNode;
}

export function AlertDialog({
  isOpen,
  title,
  description,
  variant = 'error',
  onClose,
  children,
}: AlertDialogProps) {
  if (!isOpen) return null;

  const colorMap = {
    error: 'text-destructive',
    warning: 'text-yellow-600 dark:text-yellow-500',
    info: 'text-blue-600 dark:text-blue-500',
    success: 'text-green-600 dark:text-green-500',
  };

  return (
    <div className='fixed inset-0 z-50 flex items-center justify-center bg-black/50'>
      <dialog
        open={isOpen}
        className='relative w-full max-w-sm rounded-lg border border-border bg-background p-6 shadow-lg'
        onClick={(e) => {
          if (e.target === e.currentTarget) onClose();
        }}
      >
        <button
          onClick={onClose}
          className='absolute right-4 top-4 rounded-md p-1 hover:bg-accent'
        >
          <X className='h-4 w-4' />
        </button>

        <div className='flex gap-3'>
          <AlertCircle
            className={`h-5 w-5 flex-shrink-0 ${colorMap[variant]}`}
          />
          <div className='flex-1'>
            <h2 className='text-lg font-semibold'>{title}</h2>
            <p className='mt-2 text-sm text-muted-foreground'>{description}</p>
            {children && <div className='mt-4'>{children}</div>}
          </div>
        </div>

        <div className='mt-6 flex justify-end'>
          <Button onClick={onClose}>Close</Button>
        </div>
      </dialog>
    </div>
  );
}
