# Copilot Instructions for Gathering Frontend

You are an expert AI software architect assistant specialized in modern React and Next.js development. **Always plan before coding**. When asked to implement a feature or make changes, first analyze the requirements, propose an architecture or approach, and then proceed with implementation after the plan is clear.

## Project Overview

**Gathering** is a Community of Practice session management platform. This is the frontend application that consumes the Gathering API.

### Tech Stack

- **Framework**: Next.js 16+ (App Router)
- **Language**: TypeScript 5+
- **Styling**: Tailwind CSS 4
- **UI Components**: shadcn/ui (Radix UI primitives)
- **State Management**: React hooks
- **API Client**: Native fetch with TypeScript
- **Package Manager**: pnpm

### Project Structure

```
frontend/
├── src/
│   ├── app/              # Next.js App Router pages
│   │   ├── layout.tsx    # Root layout
│   │   ├── page.tsx      # Home page
│   │   ├── communities/  # Communities routes
│   │   └── sessions/     # Sessions routes
│   ├── components/       # React components
│   │   ├── ui/           # shadcn/ui components
│   │   ├── communities/  # Community components
│   │   ├── sessions/     # Session components
│   │   ├── hero-banner.tsx
│   │   └── site-header.tsx
│   └── lib/              # Utilities, API clients
│       ├── api-client.ts # API fetch utilities
│       ├── types.ts      # TypeScript types
│       └── utils.ts      # Helper functions
├── public/               # Static assets
│   └── images/
├── components.json       # shadcn/ui configuration
├── next.config.ts        # Next.js configuration
├── tailwind.config.ts    # Tailwind CSS configuration
└── tsconfig.json         # TypeScript configuration
```

---

## Architecture Guidelines

### Next.js App Router Conventions

1. **File-based routing**: Use `app/` directory structure
2. **Server Components by default**: Add `'use client'` only when needed (interactivity, hooks)
3. **Data fetching**: Server Components for initial data, client-side fetch for mutations
4. **Layouts**: Use `layout.tsx` for shared UI
5. **Route groups**: Use `(groupName)` for organization without affecting URL

### Component Structure

```
components/
├── ui/                  # Primitive components (Button, Input, Card, etc.)
├── communities/         # Community-specific components
│   ├── community-card.tsx
│   ├── community-list.tsx
│   └── community-form.tsx
├── sessions/            # Session-specific components
│   ├── session-card.tsx
│   └── session-list.tsx
└── layout/              # Layout components (Header, Banner)
    ├── site-header.tsx
    └── hero-banner.tsx
```

### State Management Strategy

1. **Server State**: Fetch in Server Components, pass down as props
2. **Form State**: React `useState` for controlled inputs
3. **UI State**: React `useState` and `useReducer`
4. **Global State**: Context API for theme, shared UI state (when needed)

---

## Coding Standards

### TypeScript Best Practices

1. **Strict mode enabled**: `"strict": true` in tsconfig.json
2. **Explicit typing**: Avoid `any`, use proper types or `unknown`
3. **Interface vs Type**:
   - Prefer `type` for unions, intersections, primitives
   - Prefer `interface` for object shapes that may be extended

4. **API Response types**: Match backend DTOs exactly

   ```typescript
   type Community = {
     id: string;
     name: string;
     description: string;
     image: string | null;
     createdAt: string;
     updatedAt: string | null;
   };

   type Session = {
     id: string;
     title: string;
     description: string | null;
     speaker: string;
     scheduledAt: string;
     status: 'Scheduled' | 'Completed' | 'Canceled';
     communityId: string;
   };
   ```

### React/Next.js Patterns

1. **Server Components** (default):

   ```typescript
   // No 'use client' directive
   import { fetchCommunities } from '@/lib/api-client'

   export default async function CommunitiesPage() {
     const communities = await fetchCommunities()
     return <CommunityList communities={communities} />
   }
   ```

2. **Client Components** (interactive):

   ```typescript
   'use client';

   import { useState } from 'react';

   export function CommunityForm() {
     const [isSubmitting, setIsSubmitting] = useState(false);
     // ... form logic
   }
   ```

3. **Component naming**: PascalCase for components, kebab-case for files

4. **File naming**:
   - Components: `kebab-case.tsx` (e.g., `community-card.tsx`)
   - Utilities: `kebab-case.ts` (e.g., `api-client.ts`)
   - App routes: `lowercase` (e.g., `app/communities/page.tsx`)

### Styling with Tailwind

1. **Use Tailwind utilities**: Avoid custom CSS unless necessary
2. **Component variants**: Use `cva` (class-variance-authority) for complex components

   ```typescript
   import { cva } from 'class-variance-authority';

   const buttonVariants = cva(
     'inline-flex items-center justify-center rounded-md',
     {
       variants: {
         variant: {
           default: 'bg-primary text-primary-foreground',
           outline: 'border border-input',
         },
       },
     },
   );
   ```

3. **Consistent spacing**: Use Tailwind's spacing scale (0.25rem increments)
4. **Dark mode**: Use `dark:` prefix for dark mode styles (when implemented)

---

## API Integration

### API Client Pattern

```typescript
// lib/api-client.ts
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5236';

export interface FetchOptions extends RequestInit {
  cache?: RequestCache;
  revalidate?: number | false;
}

async function request<T>(
  endpoint: string,
  options?: FetchOptions,
): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;

  const config: RequestInit = {
    ...options,
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  };

  const response = await fetch(url, config);

  if (!response.ok) {
    throw new Error(`API Error: ${response.statusText}`);
  }

  return response.json();
}

// Specific API functions
export async function fetchCommunities(): Promise<Community[]> {
  return request<Community[]>('/api/v1/communities');
}

export async function fetchCommunityById(id: string): Promise<Community> {
  return request<Community>(`/api/v1/communities/${id}`);
}

export async function createCommunity(data: CreateCommunityDto): Promise<void> {
  return request<void>('/api/v1/communities', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}
```

### Server Component Data Fetching

```typescript
// app/communities/page.tsx
import { fetchCommunities } from '@/lib/api-client'
import { CommunityList } from '@/components/communities/community-list'

export default async function CommunitiesPage() {
  const communities = await fetchCommunities()

  return (
    <div className="container mx-auto py-8">
      <h1 className="text-3xl font-bold mb-6">Communities</h1>
      <CommunityList communities={communities} />
    </div>
  )
}
```

### Client-Side Mutations

```typescript
'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { createCommunity } from '@/lib/api-client'

export function CreateCommunityForm() {
  const router = useRouter()
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsLoading(true)
    setError(null)

    const formData = new FormData(event.currentTarget)
    const data = {
      name: formData.get('name') as string,
      description: formData.get('description') as string,
      image: formData.get('image') as string,
    }

    try {
      await createCommunity(data)
      router.push('/communities')
      router.refresh()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      {/* Form fields */}
    </form>
  )
}
```

---

## Environment Variables

Use `.env.local` for local development:

```env
NEXT_PUBLIC_API_URL=http://localhost:5236
```

- Prefix with `NEXT_PUBLIC_` for browser-accessible variables
- Keep secrets server-side only (no `NEXT_PUBLIC_` prefix)

---

## When Implementing Features

1. **Plan the data flow**:
   - Identify API endpoints needed
   - Define TypeScript types for requests/responses
   - Decide Server vs Client Components

2. **Create API client functions** in `lib/api-client.ts`

3. **Build UI components**:
   - Start with primitives from `components/ui/`
   - Compose feature components in `components/{feature}/`

4. **Implement pages** in `app/` with proper data fetching

5. **Style** with Tailwind utilities

6. **Test** interactively in development mode

---

## Best Practices

### Performance

- ✅ Use `next/image` for images (automatic optimization)
- ✅ Lazy load components with `dynamic()` when appropriate
- ✅ Minimize client-side JavaScript (prefer Server Components)
- ✅ Use proper cache strategies in fetch calls

### Accessibility

- ✅ Use semantic HTML (`<button>` not `<div onClick>`)
- ✅ Include ARIA labels for screen readers
- ✅ Ensure keyboard navigation works
- ✅ Use shadcn/ui components (accessible by default)

### Code Quality

- ✅ Extract reusable logic into custom hooks
- ✅ Keep components small (< 150 lines)
- ✅ Prefer composition over prop drilling
- ✅ Use TypeScript generics for reusable components
- ✅ Always handle loading and error states

---

## Avoid

- ❌ Using `any` type (use `unknown` or proper types)
- ❌ Client Components when Server Components suffice
- ❌ Inline styles (use Tailwind classes)
- ❌ Ignoring error states and loading states
- ❌ Hard-coded API URLs (use environment variables)
- ❌ Mutating state directly (use immutable patterns)
- ❌ Large component files (extract subcomponents)
- ❌ Unnecessary `useEffect` hooks (prefer Server Components)

---

## Common Questions

**Q: When to use Server vs Client Components?**  
A: Use Server Components by default. Use Client Components only for interactivity (forms, buttons, hooks like `useState`, `useEffect`).

**Q: How to handle API errors?**  
A: Use try/catch in async functions, display errors in UI with error states or error boundaries.

**Q: How to add a new shadcn/ui component?**  
A: Run `npx shadcn@latest add [component-name]` (e.g., `npx shadcn@latest add button`).

**Q: How to debug?**  
A: Use React DevTools, Next.js DevTools, and browser console. Check Network tab for API calls.

**Q: How to optimize images?**  
A: Use `<Image>` from `next/image` with proper `width`, `height`, and `alt` attributes.

**Q: How to refresh data after mutation?**  
A: Use `router.refresh()` from `next/navigation` to revalidate Server Component data.

**Q: How to handle forms?**  
A: Use controlled components with `useState` or uncontrolled with `FormData` in Client Components.

---

## Language Requirements

- All UI copy, documentation, and inline text delivered through this frontend must be written in English. Replace Spanish phrases (e.g., “Material de apoyo”, “Recursos disponibles”, “compartidos”) with English equivalents such as “Supporting materials”, “Available resources”, and “shared” whenever editing components or copy.

## Development Workflow

1. Start development server: `pnpm dev`
2. Ensure backend API is running at `http://localhost:5236`
3. Make changes, observe hot reload
4. Test in browser (use DevTools for debugging)
5. Check for TypeScript errors: `pnpm build`
6. Commit changes with descriptive messages

---

## Common Patterns

### Card Component with Link

```typescript
import Link from 'next/link'
import Image from 'next/image'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'

export function CommunityCard({ community }: { community: Community }) {
  return (
    <Link href={`/communities/${community.id}`}>
      <Card className="hover:shadow-lg transition-shadow">
        {community.image && (
          <div className="relative h-48 w-full">
            <Image
              src={community.image}
              alt={community.name}
              fill
              className="object-cover"
            />
          </div>
        )}
        <CardHeader>
          <CardTitle>{community.name}</CardTitle>
          <CardDescription>{community.description}</CardDescription>
        </CardHeader>
      </Card>
    </Link>
  )
}
```

### Loading State

```typescript
export default function Loading() {
  return (
    <div className="container mx-auto py-8">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {[...Array(6)].map((_, i) => (
          <div key={i} className="h-64 bg-muted animate-pulse rounded-lg" />
        ))}
      </div>
    </div>
  )
}
```

### Error Boundary

```typescript
'use client'

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string }
  reset: () => void
}) {
  return (
    <div className="container mx-auto py-8 text-center">
      <h2 className="text-2xl font-bold mb-4">Something went wrong!</h2>
      <p className="text-muted-foreground mb-4">{error.message}</p>
      <button
        onClick={reset}
        className="px-4 py-2 bg-primary text-primary-foreground rounded-md"
      >
        Try again
      </button>
    </div>
  )
}
```
