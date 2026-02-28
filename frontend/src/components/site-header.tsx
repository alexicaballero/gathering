import Link from 'next/link';
import Image from 'next/image';
import { Button } from './ui/button';
import { getCommunities } from '@/lib/actions/community-actions';
import { Home, ChevronDown } from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from './ui/dropdown-menu';
import MobileNav from './mobile-nav';

export default async function SiteHeader() {
  const communities = await getCommunities();
  const showDropdown = communities.length > 3;

  return (
    <header className='sticky top-0 z-50 w-full border-b border-border bg-background/80 backdrop-blur-md'>
      <div className='mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8'>
        <Link href='/' className='flex items-center gap-2.5'>
          <div className='flex h-9 w-9 items-center justify-center rounded-lg'>
            <Image
              src='/images/fountain.svg'
              alt='Gathering'
              width={35}
              height={35}
              className='h-9 w-9'
            />
          </div>
          <span className='text-lg font-semibold tracking-tight text-foreground'>
            Gathering
          </span>
        </Link>

        {/* Mobile nav */}
        <MobileNav
          communities={communities.map((c) => ({ id: c.id, name: c.name }))}
        />

        {/* Desktop nav */}
        <nav className='hidden items-center gap-1 md:flex'>
          <Link href='/'>
            <Button
              variant='ghost'
              size='sm'
              className='text-muted-foreground hover:text-foreground'
            >
              <Home /> Home
            </Button>
          </Link>

          {showDropdown ? (
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button
                  variant='ghost'
                  size='sm'
                  className='text-muted-foreground hover:text-foreground'
                >
                  Communities
                  <ChevronDown className='w-4 h-4 ml-1' />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align='start' className='w-48'>
                {communities.map((c) => (
                  <DropdownMenuItem key={c.id} asChild>
                    <Link
                      href={`/communities/${c.id}`}
                      className='cursor-pointer'
                    >
                      {c.name}
                    </Link>
                  </DropdownMenuItem>
                ))}
              </DropdownMenuContent>
            </DropdownMenu>
          ) : (
            communities.map((c) => (
              <Link key={c.id} href={`/communities/${c.id}`}>
                <Button
                  variant='ghost'
                  size='sm'
                  className='text-muted-foreground hover:text-foreground'
                >
                  {c.name}
                </Button>
              </Link>
            ))
          )}
        </nav>
      </div>
    </header>
  );
}
