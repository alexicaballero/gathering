'use client';

import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs';
import SessionResourceList from './session-resource-list';

interface SessionTabsProps {
  resources: any[];
  sessionId: string;
}

export default function SessionTabs({
  resources,
  sessionId,
}: SessionTabsProps) {
  return (
    <Tabs defaultValue='details' className='w-full'>
      <TabsList className='grid w-full grid-cols-3'>
        <TabsTrigger value='details'>Details</TabsTrigger>
        <TabsTrigger value='discussion'>Discussion</TabsTrigger>
        <TabsTrigger value='resources'>Resources</TabsTrigger>
      </TabsList>

      <TabsContent value='details' className='mt-8'>
        <div className='rounded-lg border border-border p-6'>
          <h3 className='text-lg font-semibold mb-4'>Session Details</h3>
          <p className='text-muted-foreground'>
            Detailed information about this session will be displayed here.
          </p>
        </div>
      </TabsContent>

      <TabsContent value='discussion' className='mt-8'>
        <div className='rounded-lg border border-border p-6'>
          <h3 className='text-lg font-semibold mb-4'>Discussion</h3>
          <p className='text-muted-foreground'>
            Participant discussion and comments will be displayed here.
          </p>
        </div>
      </TabsContent>

      <TabsContent value='resources' className='mt-8'>
        <SessionResourceList resources={resources} sessionId={sessionId} />
      </TabsContent>
    </Tabs>
  );
}
