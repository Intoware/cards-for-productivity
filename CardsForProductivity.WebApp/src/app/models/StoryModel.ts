export interface StoryModel {
    storyId?: string;
    sessionId?: string;
    title: string;
    description: string;
    points?: string;
    isSelected?: boolean;
    isDuplicate?: boolean;
    isDeleted?: boolean;
    userPoints?: Record<string, string>;
    revealed?: boolean;
}
