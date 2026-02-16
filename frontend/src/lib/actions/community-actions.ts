'use server';

import { Community } from '../types';
import * as api from '../api-client';

/**
 * Get all communities from the API
 * Uses Next.js cache by default - revalidates every 60 seconds
 */
export async function getCommunities(): Promise<Community[]> {
  return api.get<Community[]>('/api/v1/communities', {
    next: { revalidate: 60 }, // Cache for 60 seconds
  });
}

/**
 * Get a single community by ID
 * Uses Next.js cache by default - revalidates every 60 seconds
 */
export async function getCommunity(id: string): Promise<Community> {
  return api.get<Community>(`/api/v1/communities/${id}`, {
    next: { revalidate: 60 }, // Cache for 60 seconds
  });
}

/**
 * Get communities without cache (always fetch fresh data)
 */
export async function getCommunitiesNoCache(): Promise<Community[]> {
  return api.get<Community[]>('/api/v1/communities', {
    cache: 'no-store',
  });
}

/**
 * Create a new community
 */
export async function createCommunity(
  data: Omit<Community, 'id' | 'created_at' | 'updated_at'>,
): Promise<Community> {
  return api.post<Community>('/api/v1/communities', data);
}

/**
 * Update an existing community
 */
export async function updateCommunity(
  id: string,
  data: Partial<Community>,
): Promise<Community> {
  return api.put<Community>(`/api/v1/communities/${id}`, data);
}

/**
 * Delete a community
 */
export async function deleteCommunity(id: string): Promise<void> {
  return api.del<void>(`/api/v1/communities/${id}`);
}
