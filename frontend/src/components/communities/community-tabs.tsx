'use client';

import { Session } from '@/lib/types';
import SessionList from '@/components/sessions/session-list';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs';

interface CommunityTabsProps {
  sessions: Session[];
}

export default function CommunityTabs({ sessions }: CommunityTabsProps) {
  return (
    <Tabs defaultValue='sessions' className='w-full'>
      <TabsList className='grid w-full grid-cols-4'>
        <TabsTrigger value='sessions'>Sessions</TabsTrigger>
        <TabsTrigger value='about'>About</TabsTrigger>
        <TabsTrigger value='members'>Members</TabsTrigger>
        <TabsTrigger value='resources'>Resources</TabsTrigger>
      </TabsList>

      <TabsContent value='sessions' className='mt-8'>
        <SessionList sessions={sessions} />
      </TabsContent>

      <TabsContent value='about' className='mt-8'>
        <div className='rounded-lg border border-border p-6'>
          <h3 className='text-lg font-semibold mb-4'>About this community</h3>
          <p className='text-muted-foreground'>
            Community information and details will be displayed here.
          </p>
        </div>
      </TabsContent>

      <TabsContent value='members' className='mt-8'>
        <div className='rounded-lg border border-border p-6'>
          <h3 className='text-lg font-semibold mb-4'>Members</h3>
          <p className='text-muted-foreground'>
            Members list will be displayed here.
          </p>
        </div>
      </TabsContent>

      <TabsContent value='resources' className='mt-8'>
        <div className='rounded-lg border border-border p-6'>
          <h3 className='text-lg font-semibold mb-4'>Resources</h3>
          <p className='text-muted-foreground'>
            Community resources will be displayed here.
          </p>
        </div>
      </TabsContent>
    </Tabs>
  );
}
