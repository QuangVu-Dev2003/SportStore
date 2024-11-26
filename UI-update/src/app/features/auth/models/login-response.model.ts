export interface LoginResponse {
    token: string;
    refreshToken: string;
    expiresAt: string;
    userId: string;
}