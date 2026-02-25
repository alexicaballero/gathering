'use server';
import { revalidatePath } from 'next/cache';
import * as api from '../api-client';
import { Session, SessionResource, SessionResourceType } from '@/lib/types';

/**
 * Get sessions by community ID
 * Uses Next.js cache by default - revalidates every 60 seconds
 */
export async function getSessionsByCommunity(id: string): Promise<Session[]> {
  return api.get<Session[]>(`/api/v1/sessions/community/${id}`, {
    next: { revalidate: 60 }, // Cache for 60 seconds
  });
}

/**
 * Get a single session by ID
 * Uses Next.js cache by default - revalidates every 60 seconds
 */
export async function getSessionById(id: string): Promise<Session> {
  return api.get<Session>(`/api/v1/sessions/${id}`, {
    next: { revalidate: 60 }, // Cache for 60 seconds
  });
}

/**
 * Get sessions by community without cache (always fetch fresh data)
 */
export async function getSessionsByCommunityNoCache(
  id: string,
): Promise<Session[]> {
  return api.get<Session[]>(`/api/v1/sessions/community/${id}`, {
    cache: 'no-store',
  });
}

/**
 * Create a new session using FormData (for file uploads)
 */
export async function createSessionWithFormData(
  formData: FormData,
): Promise<void> {
  await api.postFormData<void>('/api/v1/sessions', formData);

  // Revalidate all paths that might show sessions
  revalidatePath('/communities');
  revalidatePath('/sessions');
}

/**
 * Update a session using FormData (for file uploads)
 */
export async function updateSessionWithFormData(
  id: string,
  formData: FormData,
): Promise<void> {
  await api.putFormData<void>(`/api/v1/sessions/${id}`, formData);

  // Revalidate all paths that might show sessions
  revalidatePath('/communities');
  revalidatePath('/sessions');
}

/**
 * Create a new session
 */
export async function createSession(
  data: Omit<Session, 'id' | 'created_at' | 'updated_at'>,
): Promise<Session> {
  return api.post<Session>('/api/v1/sessions', data);
}

/**
 * Update an existing session
 */
export async function updateSession(
  id: string,
  data: Partial<Session>,
): Promise<Session> {
  return api.put<Session>(`/api/v1/sessions/${id}`, data);
}

/**
 * Delete a session
 */
export async function deleteSession(id: string): Promise<void> {
  await api.del<void>(`/api/v1/sessions/${id}`);

  // Revalidate all paths that might show sessions
  revalidatePath('/communities');
  revalidatePath('/sessions');
}

/**
 * Get all resources for a session
 * Uses Next.js cache by default - revalidates every 60 seconds
 */
export async function getSessionResources(
  sessionId: string,
): Promise<SessionResource[]> {
  return api.get<SessionResource[]>(`/api/v1/sessions/${sessionId}/resources`, {
    next: { revalidate: 60 },
  });
}

/**
 * Create a new session resource
 */
export async function createSessionResource(
  sessionId: string,
  data: {
    type: SessionResourceType;
    url?: string | null;
    notes?: string | null;
    title?: string | null;
  },
): Promise<{ id: string }> {
  const result = await api.post<{ id: string }>(
    `/api/v1/sessions/${sessionId}/resources`,
    data,
  );
  revalidatePath(`/sessions/${sessionId}`);
  return result;
}

/**
 * Update an existing session resource
 */
export async function updateSessionResource(
  sessionId: string,
  resourceId: string,
  data: {
    url?: string | null;
    notes?: string | null;
    title?: string | null;
  },
): Promise<void> {
  await api.put<void>(
    `/api/v1/sessions/${sessionId}/resources/${resourceId}`,
    data,
  );
  revalidatePath(`/sessions/${sessionId}`);
}

/**
 * Delete a session resource
 */
export async function deleteSessionResource(
  sessionId: string,
  resourceId: string,
): Promise<void> {
  await api.del<void>(`/api/v1/sessions/${sessionId}/resources/${resourceId}`);
  revalidatePath(`/sessions/${sessionId}`);
}
