// API Configuration
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5236';

export interface FetchOptions extends RequestInit {
  cache?: RequestCache;
  revalidate?: number | false;
}

async function executeRequest<T>(url: string, config: RequestInit): Promise<T> {
  try {
    const response = await fetch(url, config);

    if (!response.ok) {
      // Try to get error message from response body
      let errorMessage = `API Error: ${response.status} ${response.statusText}`;

      try {
        const errorBody = await response.text();
        if (errorBody) {
          const errorData = JSON.parse(errorBody);
          if (errorData.error) {
            errorMessage = errorData.error;
          }
        }
      } catch {
        // If parsing fails, use default message
      }

      throw new Error(errorMessage);
    }

    const text = await response.text();

    if (!text || response.status === 204) {
      return undefined as unknown as T;
    }

    try {
      return JSON.parse(text) as T;
    } catch {
      return undefined as unknown as T;
    }
  } catch (error) {
    if (error instanceof Error) {
      throw error; // Re-throw with original message
    }
    throw new Error(`Failed to fetch from ${url}`);
  }
}

/**
 * Generic API request handler with error handling
 */
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

  return executeRequest<T>(url, config);
}

async function formRequest<T>(
  endpoint: string,
  options?: FetchOptions,
): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;

  const config: RequestInit = {
    ...options,
    headers: {
      Accept: 'application/json',
      ...options?.headers,
    },
  };

  return executeRequest<T>(url, config);
}

/**
 * GET request
 */
export async function get<T>(
  endpoint: string,
  options?: FetchOptions,
): Promise<T> {
  return request<T>(endpoint, { ...options, method: 'GET' });
}

/**
 * POST request
 */
export async function post<T>(
  endpoint: string,
  data?: unknown,
  options?: FetchOptions,
): Promise<T> {
  return request<T>(endpoint, {
    ...options,
    method: 'POST',
    body: data ? JSON.stringify(data) : undefined,
  });
}

/**
 * PUT request
 */
export async function put<T>(
  endpoint: string,
  data?: unknown,
  options?: FetchOptions,
): Promise<T> {
  return request<T>(endpoint, {
    ...options,
    method: 'PUT',
    body: data ? JSON.stringify(data) : undefined,
  });
}

/**
 * DELETE request
 */
export async function del<T>(
  endpoint: string,
  options?: FetchOptions,
): Promise<T> {
  return request<T>(endpoint, { ...options, method: 'DELETE' });
}

export async function postFormData<T>(
  endpoint: string,
  data: FormData,
  options?: FetchOptions,
): Promise<T> {
  return formRequest<T>(endpoint, {
    ...options,
    method: 'POST',
    body: data,
  });
}

export async function putFormData<T>(
  endpoint: string,
  data: FormData,
  options?: FetchOptions,
): Promise<T> {
  return formRequest<T>(endpoint, {
    ...options,
    method: 'PUT',
    body: data,
  });
}
