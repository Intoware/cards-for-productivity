import { StoryModel } from './StoryModel';

export interface CreateSessionRequest {
    stories: StoryModel[];
    nickname: string;
    pointChoices: string[];
    userId: string;
    hostCode: string;
    sessionId: string;
}
