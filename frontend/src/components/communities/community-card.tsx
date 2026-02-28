import Link from 'next/link';

import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import { Community } from '@/lib/types';
import { Edit2 } from 'lucide-react';

interface CommunityCardProps {
  community: Community;
}

export default function CommunityCard({ community }: CommunityCardProps) {
  const isActive = (community.sessions?.length ?? 0) > 0;

  return (
    <Card className='group relative overflow-hidden transition-all hover:shadow-xl hover:scale-105'>
      {/* Badge */}
      <Badge
        variant={isActive ? 'success' : 'inactive'}
        className='absolute top-4 right-4 z-10'
      >
        {isActive ? 'Active' : 'Inactive'}
      </Badge>

      <div className='border-b border-border'>
        {community.image ? (
          <div
            className='h-48 w-full bg-cover bg-center'
            style={{
              backgroundImage: `linear-gradient(180deg, rgba(15,23,42,0.1), rgba(15,23,42,0.35)), url(${community.image})`,
            }}
          />
        ) : (
          <div className='flex h-48 w-full items-center justify-center bg-linear-to-br from-primary/20 to-primary/10'>
            <span className='text-5xl font-bold text-primary/20'>
              {community.name.charAt(0).toUpperCase()}
            </span>
          </div>
        )}
      </div>
      <CardHeader className='gap-2 px-6 pt-6 pb-3'>
        <CardTitle className='text-xl'>{community.name}</CardTitle>
        <CardDescription className='line-clamp-2'>
          {community.description}
        </CardDescription>
      </CardHeader>
      <CardContent className='px-6 pb-3'>
        <p className='text-sm font-medium text-muted-foreground'>
          {community.sessions?.length ?? 0}{' '}
          {(community.sessions?.length ?? 0) === 1 ? 'session' : 'sessions'}
        </p>
      </CardContent>
      <CardFooter className='flex gap-2 px-6 pb-6 pt-3'>
        <Button asChild variant='default' className='flex-1'>
          <Link
            href={`/communities/${community.id}`}
            className='text-sm font-medium text-primary-foreground'
          >
            Explore
          </Link>
        </Button>
        <Button asChild variant='outline' size='icon' className='h-10 w-10'>
          <Link
            href={`/communities/${community.id}/edit`}
            title='Edit community'
          >
            <Edit2 className='h-4 w-4' />
          </Link>
        </Button>
      </CardFooter>
    </Card>
  );
}
