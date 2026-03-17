export enum SessionStatus {
  Scheduled = 0,
  Completed = 1,
  Canceled = 2,
}

export enum SessionResourceType {
  Video = 0,
  Notes = 1,
  ExternalLink = 2,
}

export interface SessionResponse {
  id: string;
  title: string;
  description: string;
  speaker: string;
  scheduledAt: string;
  status: string;
}

export interface Community {
  id: string;
  name: string;
  description: string;
  image: string | null;
  sessions: SessionResponse[];
}

export interface Session {
  id: string;
  communityId: string;
  title: string;
  description: string;
  image: string | null;
  speaker: string;
  scheduledAt: string;
  status: SessionStatus;
}

export interface SessionResource {
  id: string;
  sessionId: string;
  type: SessionResourceType;
  title: string | null;
  url: string | null;
  notes: string | null;
  createdAt: string;
  updatedAt: string | null;
}
