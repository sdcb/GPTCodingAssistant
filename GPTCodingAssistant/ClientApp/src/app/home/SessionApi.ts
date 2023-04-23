export class SessionApi {

  constructor(private baseUrl: string) {}

  getSessions(): Promise<SessionSimpleResponse[]> {
    return fetchWrapper<SessionSimpleResponse[]>(`${this.baseUrl}/session`);
  }

  getSessionById(sessionId: number): Promise<SessionResponse> {
    return fetchWrapper<SessionResponse>(`${this.baseUrl}/session/${sessionId}`);
  }

  createSession(): Promise<SessionResponse> {
    return fetchWrapper<SessionResponse>(`${this.baseUrl}/session`, {
      method: "POST",
    });
  }

  async deleteSession(sessionId: number): Promise<void> {
    await fetchWrapper<void>(`${this.baseUrl}/session/${sessionId}`, {
      method: "DELETE",
    });
  }
}

async function fetchWrapper<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(url, options);
  if (!response.ok) {
    throw new Error(`API request failed: ${response.statusText}`);
  }
  return response.json();
}



export interface SessionSimpleResponse {
  sessionId: number;
  title: string;
}

export interface SessionResponse extends SessionSimpleResponse {
  messages: ChatMessageResponse[];
}

export interface ChatMessageResponse {
  chatMessageId: number;
  message: string;
  role: PredefinedRoles;
}

export type PredefinedRoles = 'user' | 'assistant' | 'system';

export class ChatMessage {
  role: PredefinedRoles;
  content: string;

  constructor(role: PredefinedRoles, content: string) {
    this.role = role;
    this.content = content;
  }
}