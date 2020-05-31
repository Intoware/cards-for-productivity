export interface UserModel {
    userId: string;
    sessionId: string;
    nickname: string;
    isHost: boolean;
    isOnline?: boolean;
}
