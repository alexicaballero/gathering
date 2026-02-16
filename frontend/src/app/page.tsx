import CommunityList from '@/components/communities/community-list';
import { HeroBanner } from '@/components/hero-banner';
import { getCommunities } from '@/lib/actions/community-actions';

export default async function Home() {
  const communities = await getCommunities();

  return (
    <>
      <HeroBanner />
      <CommunityList communities={communities} />
    </>
  );
}
