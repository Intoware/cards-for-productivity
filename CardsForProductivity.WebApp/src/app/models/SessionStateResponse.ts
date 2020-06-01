import { StoryModel } from './StoryModel';
import { UserModel } from './UserModel';

export interface SessionStateResponse {
    sessionId: string;
    sessionCode: string;
    userId: string;
    isHost: boolean;
    stories: StoryModel[];
    users: UserModel[];
    pointChoices: string[];
    hasStarted: boolean;
    hasFinished: boolean;
    currentStoryId: string;
}
