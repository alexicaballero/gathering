'use client';

import { ReactNode } from 'react';
import { AlertCircle, Loader2 } from 'lucide-react';
import { Button } from './button';

interface ConfirmDialogProps {
  isOpen: boolean;
  title: string;
  description: string;
  confirmText?: string;
  cancelText?: string;
  isDestructive?: boolean;
  isLoading?: boolean;
  onConfirm: () => void | Promise<void>;
  onCancel: () => void;
  children?: ReactNode;
}

export function ConfirmDialog({
  isOpen,
  title,
  description,
  confirmText = 'Confirm',
  cancelText = 'Cancel',
  isDestructive = false,
  isLoading = false,
  onConfirm,
  onCancel,
  children,
}: ConfirmDialogProps) {
  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 z-50 flex items-center justify-center bg-black/50'>
      <dialog
        open={isOpen}
        className='relative w-full max-w-sm rounded-lg border border-border bg-background p-6 shadow-lg'
        onClick={(e) => {
          if (e.target === e.currentTarget) onCancel();
        }}
      >
        <div className='flex gap-3'>
          {isDestructive && (
            <AlertCircle className='h-5 w-5 shrink-0 text-destructive' />
          )}
          <div className='flex-1'>
            <h2 className='text-lg font-semibold'>{title}</h2>
            <p className='mt-2 text-sm text-muted-foreground'>{description}</p>
            {children && <div className='mt-4'>{children}</div>}
          </div>
        </div>

        <div className='mt-6 flex justify-end gap-3'>
          <Button variant='outline' onClick={onCancel} disabled={isLoading}>
            {cancelText}
          </Button>
          <Button
            variant={isDestructive ? 'destructive' : 'default'}
            onClick={onConfirm}
            disabled={isLoading}
          >
            {isLoading && <Loader2 className='h-4 w-4 animate-spin' />}
            {confirmText}
          </Button>
        </div>
      </dialog>
    </div>
  );
}
