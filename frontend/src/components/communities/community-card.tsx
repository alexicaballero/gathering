import Link from 'next/link';

import { Button } from '@/components/ui/button';
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import { Community } from '@/lib/types';

interface CommunityCardProps {
  community: Community;
}

export default function CommunityCard({ community }: CommunityCardProps) {
  return (
    <Card className='overflow-hidden'>
      <div className='border-b border-border'>
        {community.image ? (
          <div
            className='h-40 w-full bg-cover bg-center'
            style={{
              backgroundImage: `linear-gradient(180deg, rgba(15,23,42,0.1), rgba(15,23,42,0.35)), url(${community.image})`,
            }}
          />
        ) : (
          <div className='flex h-40 w-full items-center justify-center bg-muted'>
            <span className='text-4xl font-bold text-muted-foreground'>
              {community.name.charAt(0)}
            </span>
          </div>
        )}
      </div>
      <CardHeader className='gap-1 px-6 pt-6'>
        <CardTitle>{community.name}</CardTitle>
        <CardDescription>{community.description}</CardDescription>
      </CardHeader>
      <CardContent className='px-6'>
        <p className='text-sm text-muted-foreground'>
          {community.sessions?.length ?? 0} active sessions
        </p>
      </CardContent>
      <CardFooter className='px-6 pb-6 pt-3'>
        <Button asChild variant='default'>
          <Link
            href={`/communities/${community.id}`}
            className='text-sm font-medium text-primary-foreground'
          >
            Go
          </Link>
        </Button>
      </CardFooter>
    </Card>
  );
}
