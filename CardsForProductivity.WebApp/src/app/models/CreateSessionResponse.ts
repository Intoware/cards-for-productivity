export interface CreateSessionResponse {
    sessionId: string;
    sessionCode: string;
    hostCode: string;
    userId: string;
    authCode: string;
    pointChoices: string[];
}
