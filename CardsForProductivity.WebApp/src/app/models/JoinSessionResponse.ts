import { StoryModel } from './StoryModel';
import { UserModel } from './UserModel';

export interface JoinSessionResponse {
    sessionId: string;
    userId: string;
    authCode: string;
    rejoinCode: string;
    stories: StoryModel[];
    users: UserModel[];
    pointChoices: string[];
    hasStarted: boolean;
}
