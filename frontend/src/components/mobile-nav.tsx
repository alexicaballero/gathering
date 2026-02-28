'use client';

import { useState } from 'react';
import Link from 'next/link';
import { Menu, X, Home } from 'lucide-react';
import { Button } from './ui/button';

interface CommunityLink {
  id: string;
  name: string;
}

interface MobileNavProps {
  communities: CommunityLink[];
}

export default function MobileNav({ communities }: MobileNavProps) {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div className='md:hidden'>
      <Button
        variant='ghost'
        size='icon'
        onClick={() => setIsOpen(!isOpen)}
        aria-label={isOpen ? 'Close menu' : 'Open menu'}
        aria-expanded={isOpen}
      >
        {isOpen ? <X className='h-5 w-5' /> : <Menu className='h-5 w-5' />}
      </Button>

      {isOpen && (
        <nav className='absolute left-0 right-0 top-16 z-50 border-b border-border bg-background/95 backdrop-blur-md'>
          <div className='mx-auto max-w-7xl space-y-1 px-4 py-4 sm:px-6'>
            <Link
              href='/'
              onClick={() => setIsOpen(false)}
              className='flex items-center gap-2 rounded-md px-3 py-2 text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground'
            >
              <Home className='h-4 w-4' />
              Home
            </Link>

            {communities.length > 0 && (
              <>
                <p className='px-3 pt-3 pb-1 text-xs font-semibold uppercase tracking-wider text-muted-foreground/60'>
                  Communities
                </p>
                {communities.map((c) => (
                  <Link
                    key={c.id}
                    href={`/communities/${c.id}`}
                    onClick={() => setIsOpen(false)}
                    className='block rounded-md px-3 py-2 text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground'
                  >
                    {c.name}
                  </Link>
                ))}
              </>
            )}
          </div>
        </nav>
      )}
    </div>
  );
}
